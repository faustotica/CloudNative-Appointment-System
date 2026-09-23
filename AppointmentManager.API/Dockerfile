FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["AppointmentManager.API/AppointmentManager.API.csproj", "AppointmentManager.API/"]
COPY ["AppointmentManager.Application/AppointmentManager.Application.csproj", "AppointmentManager.Application/"]
COPY ["AppointmentManager.Core/AppointmentManager.Core.csproj", "AppointmentManager.Core/"]
COPY ["AppointmentManager.Infrastructure/AppointmentManager.Infrastructure.csproj", "AppointmentManager.Infrastructure/"]
COPY ["AppointmentManager.Services/AppointmentManager.Services.csproj", "AppointmentManager.Services/"]
RUN dotnet restore "AppointmentManager.API/AppointmentManager.API.csproj"
COPY . .
WORKDIR "/src/AppointmentManager.API"
RUN dotnet build "AppointmentManager.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppointmentManager.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppointmentManager.API.dll"]
