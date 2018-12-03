# Owin.Oas.Generator
Command line tool for the retrieval of OAS from an Owin hosted web api.

[![Build status](https://ci.appveyor.com/api/projects/status/ky01lw6kupic85hh?svg=true)](https://ci.appveyor.com/project/waxtell/owin-oas-generator)

Install the nuget package and add the following MSBuild task (updating the assembly and Startup names to suite):

```csharp
  <Target Name="Oas" AfterTargets="Build">
    <Exec Command="$(OasGenExe) ^
      --assembly $(ProjectDir)$(OutDir)MyOwinAssembly.dll ^
      --startup MyOwinAssebly.Startup ^
      --output $(OutDir)\swagger.json ^
      --referencepaths $(ProjectDir)$(OutDir) ^
      --base $(ProjectDir) ^
      --headers &quot;Authorization:Basic dXNlcm5hbWU6cGFzc3dvcmQ=&quot;" 
    />
  </Target>
  ```
