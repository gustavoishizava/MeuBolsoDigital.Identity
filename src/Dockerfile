FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY **/MBD.Identity.API/*.csproj ./
RUN dotnet restore

COPY **/MBD.Identity.API/. ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "MBD.Identity.API.dll"]