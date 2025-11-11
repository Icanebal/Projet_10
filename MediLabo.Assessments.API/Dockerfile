FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["MediLabo.sln", "./"]
COPY ["MediLabo.Assessments.API/MediLabo.Assessments.API.csproj", "MediLabo.Assessments.API/"]
COPY ["MediLabo.Common/MediLabo.Common.csproj", "MediLabo.Common/"]

RUN dotnet restore "MediLabo.Assessments.API/MediLabo.Assessments.API.csproj"

COPY . .

WORKDIR "/src/MediLabo.Assessments.API"
RUN dotnet build "MediLabo.Assessments.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MediLabo.Assessments.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "MediLabo.Assessments.API.dll"]