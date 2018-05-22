@echo off
set HOME=%~dp0
cd ..\ProtoGen\Bin\netcoreapp2.0\
dotnet ProtoGen.dll %HOME%\prop.xml %HOME% RC.Game.Protocol
cd %HOME%