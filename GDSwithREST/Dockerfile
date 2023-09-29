#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 58810

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GDSwithREST/GDSwithREST.csproj", "GDSwithREST/"]
RUN dotnet restore "GDSwithREST/GDSwithREST.csproj"
COPY . .
WORKDIR "/src/GDSwithREST"
RUN dotnet build "GDSwithREST.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GDSwithREST.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GDSwithREST.dll"]