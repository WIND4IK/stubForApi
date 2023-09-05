FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-env
WORKDIR /WebApplication
EXPOSE 3355
ENV ASPNETCORE_URLS=http://*:3355

COPY /WebApplication/*.csproj .
RUN dotnet restore
COPY /WebApplication .
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime
WORKDIR /publish
COPY --from=build-env /publish .
ENTRYPOINT ["dotnet", "WebApplication.dll"]
