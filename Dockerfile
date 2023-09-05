FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /src
EXPOSE 3355
ENV ASPNETCORE_URLS=http://*:3355

COPY /src/*.csproj .
RUN dotnet restore
COPY /src .
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /publish
COPY --from=build-env /publish .
ENTRYPOINT ["dotnet", "WebApplication.dll"]
