FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["MediLabo.sln", "./"]
COPY ["MediLabo.Patients.API/MediLabo.Patients.API.csproj", "MediLabo.Patients.API/"]
COPY ["MediLabo.Common/MediLabo.Common.csproj", "MediLabo.Common/"]

RUN dotnet restore "MediLabo.Patients.API/MediLabo.Patients.API.csproj"

COPY . .

WORKDIR "/src/MediLabo.Patients.API"
RUN dotnet build "MediLabo.Patients.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MediLabo.Patients.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "MediLabo.Patients.API.dll"]