rem @echo off
..\..\NuGet\NuGet.exe pack Miracle.Diagnostics.csproj -prop Configuration=release
echo "run ..\..\NuGet\NuGet.exe push Miracle.Diagnostics.x.x.x.x.nupkg to publish"
pause