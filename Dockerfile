FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["LabProject.sln", "./"]
COPY ["src/LabProject.Domain/LabProject.Domain.csproj", "src/LabProject.Domain/"]
COPY ["src/LabProject.Application/LabProject.Application.csproj", "src/LabProject.Application/"]
COPY ["src/LabProject.Infrastructure/LabProject.Infrastructure.csproj", "src/LabProject.Infrastructure/"]
COPY ["src/LabProject.WebApi/LabProject.WebApi.csproj", "src/LabProject.WebApi/"]

RUN dotnet restore "src/LabProject.WebApi/LabProject.WebApi.csproj"

COPY . .
WORKDIR "/src/src/LabProject.WebApi"
RUN dotnet publish "LabProject.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LabProject.WebApi.dll"]