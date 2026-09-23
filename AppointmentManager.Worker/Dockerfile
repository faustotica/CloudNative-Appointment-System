FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["AppointmentManager.Worker/AppointmentManager.Worker.csproj", "AppointmentManager.Worker/"]
COPY ["AppointmentManager.Core/AppointmentManager.Core.csproj", "AppointmentManager.Core/"]
RUN dotnet restore "AppointmentManager.Worker/AppointmentManager.Worker.csproj"
COPY . .
WORKDIR "/src/AppointmentManager.Worker"
RUN dotnet build "AppointmentManager.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppointmentManager.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppointmentManager.Worker.dll"]
