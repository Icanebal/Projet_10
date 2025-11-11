# Étape 1 : Build de l'application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copier le fichier solution et les projets
COPY ["MediLabo.sln", "./"]
COPY ["MediLabo.Patients.API/MediLabo.Patients.API.csproj", "MediLabo.Patients.API/"]
COPY ["MediLabo.Common/MediLabo.Common.csproj", "MediLabo.Common/"]

# Restaurer les dépendances
RUN dotnet restore "MediLabo.Patients.API/MediLabo.Patients.API.csproj"

# Copier tout le code source
COPY . .

# Builder le projet
WORKDIR "/src/MediLabo.Patients.API"
RUN dotnet build "MediLabo.Patients.API.csproj" -c Release -o /app/build

# Étape 2 : Publication
FROM build AS publish
RUN dotnet publish "MediLabo.Patients.API.csproj" -c Release -o /app/publish

# Étape 3 : Image finale runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Exposer le port
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "MediLabo.Patients.API.dll"]