@echo off
"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\amd64\MSBuild.exe" SimHubHaptics.csproj /property:GenerateFullPaths=true /t:build /p:Configuration=Debug /p:Platform=AnyCPU /consoleloggerparameters:NoSummary
