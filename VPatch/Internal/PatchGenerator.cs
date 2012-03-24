//---------------------------------------------------------------------------
//                           -=* VPatch *=-
//---------------------------------------------------------------------------
// Copyright (C) 2001-2005 Koen van de Sande / Van de Sande Productions
//---------------------------------------------------------------------------
// Website: http://www.tibed.net/vpatch
//
// This software is provided 'as-is', without any express or implied
// warranty.  In no event will the authors be held liable for any damages
// arising from the use of this software.
//
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
//
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.
//---------------------------------------------------------------------------
// Ported to C# 2012 Joshua Cearley
using System;
using System.Collections.Generic;
using System.IO;

namespace VPatch.Internal
{
	public sealed class PatchGenerator
	{
		#region Operating Streams
		Stream mSource;
		long mSourceSize;
		Stream mTarget;
		long mTargetSize;
		#endregion
		
		long mBlockSize = DefaultBlockSize;
		
		byte[] mTargetCData;
		long mTargetCDataBaseOffset;
		long mTargetCDataSize;
		
		#region Constants
		public const long TargetBufferSize = 65536;
		public const long TargetLookaheadSize = 4096;
		public const long DefaultBlockSize = 64;
		public const long MaxBlockSize = 16384;
		public const long DefaultMaxMatches = 500;
		#endregion
		
		public PatchGenerator(Stream source, long sourceSize,
		                        Stream target, long targetSize)
		{
			if (source == null)
				throw new ArgumentNullException();
			if (target == null)
				throw new ArgumentNullException();
			
			mSource = source;
			mSourceSize = sourceSize;
			
			mTarget = target;
			mTargetSize = targetSize;
			
			mTargetCData = new byte[TargetBufferSize];
			
			Verbose = false;
			MaximumMatches = DefaultMaxMatches;
			mBlockSize = DefaultBlockSize;
		}
		
		/// <param name="sameBlocks">
		/// This list will store blocks that have been found to have remained
		/// the same between files.
		/// </param>
		public void Execute(IList<SameBlock> sameBlocks)
		{
			if (sameBlocks == null)
				throw new ArgumentNullException();
			
			ChunkedFile sourceTree = new ChunkedFile(mSource, mSourceSize, mBlockSize);
			
			// the vector needs an 'empty' first block so checking for overlap with the 'previous' block never fails.
			sameBlocks.Add(new SameBlock());
			
			mTargetCDataBaseOffset = 0;
			mTargetCDataSize = 0;
			bool firstRun = true;
			
			// currentOffset is in the target file
			for (long currentOffset = 0; currentOffset < mTargetSize;) {
				bool reloadTargetCData = true;
				
				if ((currentOffset >= mTargetCDataBaseOffset) &&
				    (currentOffset + TargetLookaheadSize < mTargetCDataBaseOffset + TargetBufferSize))
				{
					if (firstRun) {
						firstRun = false;
					} else {
						reloadTargetCData = false;
					}
				}
				
				if (reloadTargetCData) {
					// at least support looking back blockSize, if possible (findBlock relies on this!)
					mTargetCDataBaseOffset = currentOffset - mBlockSize;
					// handle start of file correctly
					if (currentOffset < BlockSize) mTargetCDataBaseOffset = 0;
					
					mTargetCDataSize = TargetBufferSize;
					
					// check if this does not extend beyond EOF
					if (mTargetCDataBaseOffset + mTargetCDataSize > mTargetSize) {
						mTargetCDataSize = mTargetSize - mTargetCDataBaseOffset;
					}
					
					// we need to update the memory cache of target
					// TODO: Emit debug info here, if verbose is enabled.
					// cout << "[CacheReload] File position = " << static_cast<unsigned long>(targetCDataBaseOffset) << "\n";
					
					Console.WriteLine("[CacheReload] File position = {0}", mTargetCDataBaseOffset);
					
					mTarget.Seek(mTargetCDataBaseOffset, SeekOrigin.Begin);
					mTarget.Read(mTargetCData, 0, (int)mTargetCDataSize);
				}
				
				SameBlock currentSameBlock = FindBlock(sourceTree, currentOffset);
				if (currentSameBlock != null) {
					// We have a match.
					SameBlock previousBlock = sameBlocks[sameBlocks.Count-1];
					if (previousBlock.TargetOffset + previousBlock.Size > currentSameBlock.TargetOffset) {
						// There is overlap, resolve it.
						long difference = previousBlock.TargetOffset + previousBlock.Size - currentSameBlock.TargetOffset;
						currentSameBlock.SourceOffset += difference;
						currentSameBlock.TargetOffset += difference;
						currentSameBlock.Size -= difference;
					}
					Console.WriteLine(currentSameBlock.ToString());
					sameBlocks.Add(currentSameBlock);
					
					// TODO: Emit debug info here, if verbose is enabled.
					
					currentOffset = currentSameBlock.TargetOffset + currentSameBlock.Size;
				} else {
					// No match, advance to the next byte.
					currentOffset++;
				}
			}
			
			// Add a block at the end to prevent bounds checking hassles.
			SameBlock lastBlock = new SameBlock();
			lastBlock.SourceOffset = 0;
			lastBlock.TargetOffset = mTargetSize;
			lastBlock.Size = 0;
			sameBlocks.Add(lastBlock);
		}
		
		SameBlock FindBlock(ChunkedFile sourceTree, long targetFileStartOffset)
		{
			if (mTargetSize - targetFileStartOffset < BlockSize) return null;
			
			long preDataSize = targetFileStartOffset - mTargetCDataBaseOffset;
			// rea the current data part in to memory
			ChunkChecksum checksum = new ChunkChecksum();
			sourceTree.CalculateChecksum(mTargetCData, preDataSize, BlockSize, checksum);
			
			long foundIndex;
			if (sourceTree.Search(checksum, out foundIndex)) {
				// we found something
				SameBlock bestMatch = new SameBlock();
				bestMatch.SourceOffset = sourceTree.Chunks[foundIndex].Offset;
				bestMatch.TargetOffset = targetFileStartOffset;
				bestMatch.Size = 0; // default to 0. because they can all be mismatches as well
				
				// inreae match size if possible, also check if it is a match at all
				long matchCount = 0;
				while ((sourceTree.Chunks[foundIndex].Checksum == checksum) &&
				       ((MaximumMatches == 0) || (matchCount < MaximumMatches)))
				{
					// check if this one is better than the current match
					SameBlock match = new SameBlock();
					match.SourceOffset = sourceTree.Chunks[foundIndex].Offset;
					match.TargetOffset = targetFileStartOffset;
					match.Size = 0; // default to 0. could be a mismatch with the same key
					ImproveSameBlockMatch(match, bestMatch.Size);
					if (match.Size > bestMatch.Size) {
						bestMatch = match;
					}
					foundIndex++;
					matchCount++;
				}
				
				// TODO: Emit debugging information here if in verbose mode.
				
				if (bestMatch.Size == 0) {
					return null;
				} else {
					return bestMatch;
				}
			} else {
				return null;
			}
		}
		
		public const long ComparisonSize = 2048;
		
		void ImproveSameBlockMatch(SameBlock match, long currentBest)
		{
			// we should now try to make the match longer by reading big chunks of the files to come
			mSource.Seek(match.SourceOffset + match.Size, SeekOrigin.Begin);
			mTarget.Seek(match.TargetOffset + match.Size, SeekOrigin.Begin);
			
			{
				byte[] sourceData = new byte[ComparisonSize];
				byte[] targetData = new byte[ComparisonSize];
				bool deepBreak = false;
				while (true) {
					long startTarget = match.TargetOffset + match.Size;
					long startSource = match.SourceOffset + match.Size;
					long checkSize = ComparisonSize;
					
					if (checkSize > (mTargetSize - startTarget)) {
						checkSize = mTargetSize - startTarget;
						deepBreak = true;
					}
					
					if (checkSize > (mSourceSize - startSource)) {
						checkSize = mSourceSize - startSource;
						deepBreak = true;
					}
					
					mSource.Read(sourceData, 0, (int)checkSize);
					mTarget.Read(targetData, 0, (int)checkSize);
					
					// TODO: Could we optimize this with either an array primitive or unsafe pointers?
					
					long i = 0;
					while ((i < checkSize) && (sourceData[i] == targetData[i]))
					{
						match.Size++;
						i++;
					}
					
					// check if we stopped because we had a mismatch or ran out of input
					if (i < checkSize || deepBreak) break;
					
					//break; // Maybe many breaks will help?
				}
			}
			
			if (match.Size < BlockSize) {
				match.Size = 0;
			} else {
				// try to improve before match if this is useful
				if ((match.Size + BlockSize) <= currentBest) return;
				// do not do if there is no more data in the target...
				if (match.TargetOffset == 0) return;
				
				// we know it is stored in the cache... so we just need the source one
				byte[] sourceData = new byte[MaxBlockSize];
				
				long startSource = match.SourceOffset - BlockSize;
				long checkSize = BlockSize;
				
				if (checkSize > match.SourceOffset) {
					checkSize = match.SourceOffset;
					startSource = 0;
				}
				
				if (checkSize == 0) return;
				
				mSource.Seek(startSource, SeekOrigin.Begin);
				mSource.Read(sourceData, 0, (int)checkSize);
				checkSize--;
				
				while (sourceData[checkSize] == (mTargetCData[match.TargetOffset - mTargetCDataBaseOffset - 1])) {
					match.TargetOffset--;
					match.SourceOffset--;
					match.Size++;
					checkSize--;
					if (checkSize == 0) break;
					if (match.TargetOffset == 0) break;
				}
			}
		}
		
		#region Public Properties
		public long BlockSize
		{
			get {
				return mBlockSize;
			}
			
			set {
				mBlockSize = value;
			}
		}
		public long MaximumMatches;
		public bool Verbose;
		#endregion
	}
}
