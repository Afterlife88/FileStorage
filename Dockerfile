FROM microsoft/dotnet:latest

# Set env variables
ENV ASPNETCORE_URLS http://*:5000

COPY /src/FileStorage.Web /app/src/FileStorage.Web
COPY /src/FileStorage.Domain /app/src/FileStorage.Domain
COPY /src/FileStorage.Utils /app/src/FileStorage.Utils

# Restore domain
WORKDIR /app/src/FileStorage.Domain
RUN ["dotnet", "restore"]

# Restore token provider 
WORKDIR /app/src/FileStorage.Utils
RUN ["dotnet", "restore"]

WORKDIR /app/src/FileStorage.Web
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
 
# Open port
EXPOSE 5000/tcp
 
ENTRYPOINT ["dotnet", "run"]