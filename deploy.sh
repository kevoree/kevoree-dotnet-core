#!/bin/bash
rm -rf  *.nupkg
nuget pack *.nuspec
nuget push *.nupkg -Source https://www.nuget.org/api/v2/package
