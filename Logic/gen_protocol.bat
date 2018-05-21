@echo off
set HOME=%~dp0
cd ..\ProtoGen\Bin\netcoreapp2.0\
dotnet ProtoGen.dll %HOME%\protocol.xml %HOME%\Protocols RC.Game.Protocols
cd %HOME%