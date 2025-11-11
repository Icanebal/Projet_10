FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["MediLabo.sln", "./"]
COPY ["MediLabo.Web/MediLabo.Web.csproj", "MediLabo.Web/"]
COPY ["MediLabo.Common/MediLabo.Common.csproj", "MediLabo.Common/"]

RUN dotnet restore "MediLabo.Web/MediLabo.Web.csproj"

COPY . .

WORKDIR "/src/MediLabo.Web"
RUN dotnet build "MediLabo.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MediLabo.Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "MediLabo.Web.dll"]