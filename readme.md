# What is it?

This library implements a custom [MetadataReferenceResolver](https://github.com/dotnet/roslyn/blob/master/src/Compilers/Core/Portable/MetadataReference/MetadataReferenceResolver.cs) that makes it possible to resolve references to NuGet packages in C# scripts.

Roslyn scripting has built-in support for referencing other script files and other assemblies, but lacks the ability to reference a NuGet package. There is an [open issue](https://github.com/dotnet/roslyn/issues/6900) on GitHub and the syntax for referencing NuGet packages is based on this issue.

```c#
#r "nuget:AutoMapper/6.0.0"
```



The resolver looks for references that starts with "nuget" and if such a reference is found it will install the package and create a reference to the binaries contained within the package. 









