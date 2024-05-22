FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY admin_fc_ms/*.csproj ./admin_fc_ms/
COPY test_admin_fc_ms/*.csproj ./test_admin_fc_ms/
RUN dotnet restore admin_fc_ms/admin_fc_ms.csproj

COPY admin_fc_ms/. ./admin_fc_ms/
COPY test_admin_fc_ms/. ./test_admin_fc_ms/

WORKDIR /app/test_admin_fc_ms
RUN dotnet test

WORKDIR /app/admin_fc_ms

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/admin_fc_ms/out ./

ENTRYPOINT ["dotnet", "admin_fc_ms.dll"]