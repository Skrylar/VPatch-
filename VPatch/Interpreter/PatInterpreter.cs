/*
 * ---------------------------------------------------------------------------
 *                            -=* VPatch *=-
 * ---------------------------------------------------------------------------
 *  Copyright (C) 2001-2005 Koen van de Sande / Van de Sande Productions
 * ---------------------------------------------------------------------------
 *  Website: http://www.tibed.net/vpatch
 * 
 *  This software is provided 'as-is', without any express or implied
 *  warranty.  In no event will the authors be held liable for any damages
 *  arising from the use of this software.
 * 
 *  Permission is granted to anyone to use this software for any purpose,
 *  including commercial applications, and to alter it and redistribute it
 *  freely, subject to the following restrictions:
 * 
 *  1. The origin of this software must not be misrepresented; you must not
 *     claim that you wrote the original software. If you use this software
 *     in a product, an acknowledgment in the product documentation would be
 *     appreciated but is not required.
 *  2. Altered source versions must be plainly marked as such, and must not be
 *     misrepresented as being the original software.
 *  3. This notice may not be removed or altered from any source distribution.
 * ---------------------------------------------------------------------------
 * Ported to C# 2012 Joshua Cearley
 */
using System;
using System.IO;
using VPatch.Checksum;
using VPatch.Internal;

namespace VPatch.Interpreter
{
	public class PatInterpreter : IPatchInterpreter
	{
		PatchFileInformation mPatFileInfo;
		
		/// <summary>
		/// The number of patches stored in the patch file.
		/// </summary>
		public int PatchesInFile { get; private set; }
		
		// Magic marker to check if this is actually a patch file.
		const uint MAGIC = 0x54415056;
		
		public void Clear()
		{
			mPatFileInfo = new PatchFileInformation();
		}
		
		public bool Analyze(Stream patchStream)
		{
			if (patchStream == null) throw new ArgumentNullException();
			if (patchStream.CanSeek == false) throw new ArgumentException();
			
			Clear();

			var br = new BinaryReader(patchStream);
			// Make sure the magic word is here.
			uint seed = br.ReadUInt32();
			if (seed != MAGIC) {
				return false;
			}
			
			// Read file count field (note: contains funky bits.)
			uint fileCount = br.ReadUInt32();
			bool md5Mode = ((fileCount & 0x80000000) != 0);
			
			// Clear the weird bits from the file count.
			fileCount &= 0x00FFFFFF;
			
			// We only support newer MD5 versions of files.
			if (!md5Mode) return false;
			
			mPatFileInfo.BlockCount = br.ReadUInt32();
			mPatFileInfo.SourceChecksum = br.ReadBytes(16);
			mPatFileInfo.TargetChecksum = br.ReadBytes(16);
			mPatFileInfo.BodySize = br.ReadUInt32();
				
			return true;
		}
		
		public PatchApplyResponse Apply(Stream oldVersion, Stream patchStream, Stream output, IPatchProgress prog)
		{
			// Checksum our input to make sure things are okay
			byte[] oldVersionHash = MD5.Check(oldVersion);
			oldVersion.Seek(0, SeekOrigin.Begin);
			
			// Check if our signature is the same as the output
			bool isRequired = false;
			for (int i = 0; i < 16; i++) {
				if (oldVersionHash[i] != mPatFileInfo.TargetChecksum[i]) {
					isRequired = true;
					break;
				}
			}
			
			if (!isRequired) {
				return PatchApplyResponse.NotRequired;
			}
			
			// Make sure our file signatures match up
			for (int i = 0; i < 16; i++) {
				if (oldVersionHash[i] != mPatFileInfo.SourceChecksum[i]) return PatchApplyResponse.WrongFile;
			}
			
			byte[] copyBuffer = new byte[4096];
			
			BinaryReader br = new BinaryReader(patchStream);
			for (int currentBlock = 0; currentBlock < (int)mPatFileInfo.BlockCount; currentBlock++) {
				ulong blockSize = 0;
				long derp = patchStream.Position;
				byte blockType = br.ReadByte();
				switch (blockType) {
					// Identical blocks
					// ================
					// Copy an amount of data from the original file in to the new one.
					case 1:
					case 2:
					case 3:
						// Decode the block length
						switch (blockType) {
							case 1:
								blockSize = (ulong)br.ReadByte();
								break;
							case 2:
								blockSize = (ulong)br.ReadUInt16();
								break;
							case 3:
								blockSize = (ulong)br.ReadUInt32();
								break;
						}
						
						long sourceOffset = br.ReadUInt32();
						oldVersion.Seek(sourceOffset, SeekOrigin.Begin);
						
						// If we have a derpyblock or couldn't read it, count it as a failure.
						if (blockSize < 1) {
							return PatchApplyResponse.Failed;
						}
						
						// Copy from the source to the output.
						while (blockSize > 0) {
							int read = oldVersion.Read(copyBuffer, 0, (int)Math.Min(4096, blockSize));
							if (read <= 0) {
								throw new IOException();
							}
							output.Write(copyBuffer, 0, read);
							blockSize -= (ulong)read;
						}
						break;
					
					// Payload delivery blocks
					// =======================
					// Copy an amount of data from our patch file in to the new one.
					case 5:
					case 6:
					case 7:
						switch (blockType) {
							case 5:
								blockSize = (ulong)br.ReadByte();
								break;
							case 6:
								blockSize = (ulong)br.ReadUInt16();
								break;
							case 7:
								blockSize = (ulong)br.ReadUInt32();
								break;
						}
					
						while (blockSize > 0) {
							int read = br.Read(copyBuffer, 0, (int)Math.Min(4096, blockSize));
							if (read <= 0) {
								throw new IOException();
							}
							output.Write(copyBuffer, 0, read);
							blockSize -= (ulong)read;
						}
					
						break;
					
					// Its the end of the taco stand, taco taco stand.
					case 255:
						// TODO: Should we really care about the timestamp?
						br.ReadInt64();
						break;
						
					default:
						return PatchApplyResponse.Failed;
				}
				// Issue any progress updates before moving to the next block
				if (prog != null) {
					prog.OnPatchProgress(currentBlock, mPatFileInfo.BlockCount);
				}
			}
			
			// Make sure we applied the patch correctly
			output.Seek(0, SeekOrigin.Begin);
			byte[] patchedFileChecksum = MD5.Check(output);
			for (int i = 0; i < 16; i++) {
				if (patchedFileChecksum[i] != mPatFileInfo.TargetChecksum[i]) return PatchApplyResponse.Failed;
			}
			
			// We're done!
			return PatchApplyResponse.Ok;
		}
	}
}
