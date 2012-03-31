
# VPatch.net

VPatch.net is a port of Koen van de Sande's differential patching program
"VPatch" to the .NET framework. The original program is provided by the author
at [his site][VPatchHome] as well as is bundled with the
[Nullsoft Scriptable Install System][NSISHome] for making small delta patches.
It allows C# (and other .NET code, as far as I'm aware) to make use of the
algorithm both as a patch generator and as a patch applier.

[VPatchHome]: http://www.tibed.net/vpatch/
[NSISHome]: http://nsis.sourceforge.net/

## Rationale

I needed a way to do delta patches when experimenting with my own patcher GUI,
as the existing GUIs for creating deployable patches are either expensive with
draconic licensing or underdeveloped. Remembering VPatch from reading NSIS
documentation, and that the C# version of bsdiff depended on GPL code, I
decided to port the BSD-licensed VPatch algorithm to .NET and release it under
a similar license.

This .NET port aims to make the creation and application of delta patches
trivial, which should be useful for applying application updates or potentially
synchronizing complex game states that have run amok.

## Building

While VPatch.net is developed with SharpDevelop, the project files _should_
be openable in Visual Studio. You may, additionally, drop the source code files
in to a project in an existing solution and build it; there are no special
requirements other than patch creation code depends on `System.LINQ`.

Patch application code does not have this requirement so that component
*should* be safe to use on .NET 2.0 which is pre-installed on all Windows
Vista and 7 machines, and updated XP machines.

## Compatibilities

* VPatch.net files created with the `Data.Formatter.PatFormatter` class are
compatible with the original vpatch, and in turn are compatible with NSIS
scripts.

## Shortcomings

* Takes approximately twice as much RAM as the original C version, from some
simple tests.
* Not compatible with VPatch's .dat format if they contain more than one
file, only the first file is read.
* Filetimes are recorded when creating patches but are ignored when
applying them. If maintaining file time stamps becomes important I'll look
in to fixing that.
* May throw exceptions here and there. Those will be stamped out as use
cases can be formed around them.
