xcopy Config "./Bin/Config" /E /Y
cd Server
dotnet publish
pause