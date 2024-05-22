FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY options_fc_ms/*.csproj ./options_fc_ms/
COPY test_options_fc_ms/*.csproj ./test_options_fc_ms/
RUN dotnet restore options_fc_ms/options_fc_ms.csproj

COPY options_fc_ms/. ./options_fc_ms/
COPY test_options_fc_ms/. ./test_options_fc_ms/

WORKDIR /app/test_options_fc_ms
RUN dotnet test

WORKDIR /app/options_fc_ms

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/options_fc_ms/out ./

ENTRYPOINT ["dotnet", "options_fc_ms.dll"]