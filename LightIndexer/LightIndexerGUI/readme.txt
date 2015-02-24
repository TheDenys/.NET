Features:
	- Indexing of plaintext files.
    - Index zip file content.
	- Search in path and file content. You can use wildcards in path filter.
	- Content filter allows to search whole word/phrase or piece of phrase.
	- Long path support.
	  Used library from MS Experimental (http://bcl.codeplex.com/wikipage?title=Long%20Path&referringTitle=Home)
	  More about the problem: http://blogs.msdn.com/b/bclteam/archive/2007/02/13/long-paths-in-net-part-1-of-3-kim-hamilton.aspx
	- Built-in editor with color syntax highlighting. (ICSharp.Editor)
	- Drag'n'Drop.
	- Long paths supported.
	- Delete indexed entries.

Hot keys:
 F3 Built-in Viewer
 F4 Editor (by default notepad)
 Esc closes window
 Ctrl+Tab selects next window

3rd party components used:
 - Lucene.net (3.0.3 RC2)
 - Microsoft Reactive Extensions
 - Microsoft.Experimental.IO
 - ICSharpCode.Editor
 - SharpZipLib

Note:
There is AfterBuild event for Release configuration that does ILMerge against referenced dlls.

TODO:
- Add IGNORED_FOLDERS (for .svn, .ReSharper, etc.)
- Use http://sevenziplib.codeplex.com/ for archives.
- Unpack archive to temp file/memory depending on setting.
- Add unittests
- Find ZIP msbuild task to add to deployment
- Custom config section
- Refactor the code in UI
- Refactor code in BL
- Synchronize index access
- Pass index folder path or settings file in command line

Known bugs:
- Crashes when delete documents and many search windows open