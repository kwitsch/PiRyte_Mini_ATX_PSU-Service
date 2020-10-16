#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["PiRyte_Mini_ATX_PSU-Service/PiRyte_Mini_ATX_PSU-Service.csproj", "PiRyte_Mini_ATX_PSU-Service/"]
RUN dotnet restore "PiRyte_Mini_ATX_PSU-Service/PiRyte_Mini_ATX_PSU-Service.csproj"
COPY . .
WORKDIR "/src/PiRyte_Mini_ATX_PSU-Service"
RUN dotnet build "PiRyte_Mini_ATX_PSU-Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PiRyte_Mini_ATX_PSU-Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PiRyte_Mini_ATX_PSU-Service.dll"]