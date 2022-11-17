# r4r-api

This is the API for the Resources for Researchers Application.

## Build

**NOTE:** Building this project requires that you add the NuGet source `https://nuget.pkg.github.com/nciocpl/index.json`

1. [Create a personal access token](https://github.com/settings/tokens/new) with `read:packages` scope
1. Add the source with this command, substituting your username and access token.

```
$ dotnet nuget add source https://nuget.pkg.github.com/nciocpl/index.json  --name github --store-password-in-clear-text --username YOURUSERID --password PERSONAL_ACCESS_TOKEN
```



```
cd r4r-api

# install NuGet packages
dotnet restore
# builds all projects (test are dependent on src)
dotnet build
# runs unit tests (only in tests folder)
dotnet test test/**
# Run code coverage
# (../../lcov is because this will run from the test project...)
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=../../lcov test/**
# Publish to publish directory
dotnet publish -c Release -o ../../publish src/NCI.OCPL.Api.ResourcesForResearchers
```
