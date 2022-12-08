FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build-env
WORKDIR /app



EXPOSE 5001
ENV ASPNETCORE_ENVIRONMENT=inttest
ENV ASPNETCORE_URLS="http://*:5000"
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENTRYPOINT ["dotnet", "watch", "--project", "src/NCI.OCPL.Api.ResourcesForResearchers", "run"]