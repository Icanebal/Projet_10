# Étape 1 : Build de l'application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copier le fichier solution et les projets
COPY ["MediLabo.sln", "./"]
COPY ["MediLabo.Assessments.API/MediLabo.Assessments.API.csproj", "MediLabo.Assessments.API/"]
COPY ["MediLabo.Common/MediLabo.Common.csproj", "MediLabo.Common/"]

# Restaurer les dépendances
RUN dotnet restore "MediLabo.Assessments.API/MediLabo.Assessments.API.csproj"

# Copier tout le code source
COPY . .

# Builder le projet
WORKDIR "/src/MediLabo.Assessments.API"
RUN dotnet build "MediLabo.Assessments.API.csproj" -c Release -o /app/build

# Étape 2 : Publication
FROM build AS publish
RUN dotnet publish "MediLabo.Assessments.API.csproj" -c Release -o /app/publish

# Étape 3 : Image finale runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Exposer le port
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "MediLabo.Assessments.API.dll"]