CSharpPathResolver
==================
The CSharpPathResolver makes it easy to resolve relative file paths in c# using a single call. Simply pass in the base path and the relative path and the resolver will return the new target path.
Below is an example of the types of path that are supported
* C:\Path1\Path2
* \\\\Server01\Path1\Path2
* //www.somedomain.com/path1
* ftp://ftp.somewhere.com/path1/path2
* http://localhost:23512/path1/path2
* /src/path1/path2

If you are not sure of the format of the path that you are resolving then you would use the following command. This method will automatically detect the path style and resolve it correctly.
```
var resolver = new PathResolver();
var resolvedPath = resolver.ResolvePath("C:\Path1\Path2", "..\Path3");
// resolvedPath == "C:\Path1\Path3"
```
If you know the type of path you are resolving then you would call the specific resolve function.
``` 
PathResolver.ResolveUrlPath("http://bytechaser.com/path1/path2", "../../path3/path4")
// resolvedPath == "http://bytechaser.com/path3/path4"
```
