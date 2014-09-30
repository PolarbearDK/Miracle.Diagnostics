@echo off

MSBUILD.exe Miracle.Diagnostics.csproj /t:BeforeBuild
MSBUILD.exe Miracle.Diagnostics.csproj /t:Build /p:Configuration="Release 3.5"
MSBUILD.exe Miracle.Diagnostics.csproj /t:Build /p:Configuration="Release 4.0"
MSBUILD.exe Miracle.Diagnostics.csproj /t:Build /p:Configuration="Release 4.5"
MSBUILD.exe Miracle.Diagnostics.csproj /t:AfterBuild;Package /p:Configuration="Release 4.5"

echo "run ..\..\NuGet\NuGet.exe push NuGet\Miracle.Diagnostics... to publish"
pause