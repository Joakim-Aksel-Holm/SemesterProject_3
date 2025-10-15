# =======================
# 1) BUILD STAGE (SDK)
# =======================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies (cache-friendly)
COPY csharp/csharp.csproj csharp/
RUN dotnet restore csharp/csharp.csproj

# Copy the rest of the source and publish
COPY csharp/ csharp/
WORKDIR /src/csharp
RUN dotnet publish -c Release -o /out

# =======================
# 2) RUNTIME STAGE
# =======================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Bring the published output from the build stage
COPY --from=build /out ./

# Listen on 8080 inside the container
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

# (Optional) enable only if you add /health in Program.cs
# HEALTHCHECK --interval=30s --timeout=3s --start-period=20s \
#   CMD wget -qO- http://localhost:8080/health || exit 1

# Run your app (your DLL really is csharp.dll per your screenshot)
ENTRYPOINT ["dotnet", "csharp.dll"]
