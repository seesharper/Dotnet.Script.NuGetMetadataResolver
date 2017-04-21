### Syntax

I did not have much to go after with regards to the syntax other than this issue

[Add NuGet support in scripting APIs #6900](https://github.com/dotnet/roslyn/issues/6900)

Simply adopted the suggested syntax with the assumption that this might be implemented in a future release of Roslyn.

I do however agree that we should align with F# and adopt the same syntax

```
#r "nuget:AutoMapper,6.0.0"
```



### Resolver - How does it work

~~The NuGetMetadataReferenceResolver looks for #r references that starts with the word "nuget" and then parses the package name and version from that reference.~~

~~It then looks for the package in the global nuget cache and from here two things can happen.~~

~~Either the package is found in the cache and we reference the package from the global cache.~~

~~If the package is not found in the global cache, we install the package into a temporary folder which gets deleted after installing. This causes the package also to be installed into the global cache and we can again reference the package from the global cache.~~

~~The process of actually installing the package is offloaded to the NuGet command line application as NuGet does not offer a .Net Core version of [NuGet.PackageManagement](https://www.nuget.org/packages/NuGet.PackageManagement)~~

~~After the package is resolved from the global cache we will look for the nearest matching framework from that package. This is done using the *FrameworkReducer* class provided by the NuGet API.~~ 

~~The target framework, meaning the context for which to find the nearest matching framework is either inferred from the host process or it can be specified when creating the resolver.~~

~~It is important to notice that the resolver does not make any attempts to resolve the package dependencies. Since there is no lock file in play here, it made more sense to explicitly (#r)eference all dependencies including transients.~~

~~It that sense you could say that the script is also the lock file.~~

~~The resolver uses the *PackageSourceProvider* (also provided by the NuGet API) to figure out the feeds to be used. This for instance means that we support a local NuGet.Config.~~ 

### How does it works - take 2

The purpose of this library is to resolve NuGet metadata references in C# script files (csx files).



* Resolve references when executing scripts 
* In OmniSharp to provide intellisense for NuGet references

OmniSharp looks for a project.json file and uses that file to obtain a runtime context that is used to provide the metadata references needed for intellisense light up.

The idea behind this library is to generate a project.json file automatically based on one or more script files.

The best way to explain this is by giving an example.

Consider this script file located at C:\demo\foo.xsx

```csharp
#r "nuget:AutoMapper,5.1.1"

using AutoMapper;
Console.WriteLine(typeof(MapperConfiguration));
```

Using this script file we can now generate a project file 

```
var pathToProjectJson = scriptProjectProvider.CreateProject(@"C:\demo", "net46")	
```

```json
{
    "dependencies": {
        "AutoMapper": "5.1.1"
    },
    "frameworks": {
        "net46": {}
    }
}
```

The *ScriptProjectProvider* will look for script files in the target folder including subfolder and resolve all references to NuGet packages.  Note that the target framework is set to "net46" in this example. In OmniSharp we will infer the target framework from the host prosess (OmniSharp) which is the full desktop framework.

It is however possible to specify the target framework using the #! directive

```
#! "netcoreapp1.0"
#r "nuget:AutoMapper,5.1.1"

using AutoMapper;
Console.WriteLine(typeof(MapperConfiguration));
```

If we specify the target framework using the #! directive, the default target framework passed to the *CreateProject* method will be ignored. 

For OmniSharp this means that everything will continue to work as before in a non-breaking manner, but we now have an opt-in approach for referencing NuGet packages and optionally specifying the target framework for which to resolve metadata references.







































###   

#  