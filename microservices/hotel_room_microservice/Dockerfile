FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY hotel_fc_ms/*.csproj ./hotel_fc_ms/
COPY test_hotel_fc_ms/*.csproj ./test_hotel_fc_ms/
RUN dotnet restore hotel_fc_ms/hotel_fc_ms.csproj

COPY hotel_fc_ms/. ./hotel_fc_ms/
COPY test_hotel_fc_ms/. ./test_hotel_fc_ms/

WORKDIR /app/test_hotel_fc_ms
RUN dotnet test

WORKDIR /app/hotel_fc_ms

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/hotel_fc_ms/out ./

ENTRYPOINT ["dotnet", "hotel_fc_ms.dll"]