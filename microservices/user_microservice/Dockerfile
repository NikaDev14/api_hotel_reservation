FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY UserFcApi/*.csproj ./UserFcApi/
COPY TestUserFcAppi/*.csproj ./TestUserFcAppi/
RUN dotnet restore UserFcApi/UserFcApi.csproj

COPY UserFcApi/. ./UserFcApi/
COPY TestUserFcAppi/. ./TestUserFcAppi/

WORKDIR /app/TestUserFcAppi
RUN dotnet test

WORKDIR /app/UserFcApi

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/UserFcApi/out ./

ENTRYPOINT ["dotnet", "UserFcApi.dll"]