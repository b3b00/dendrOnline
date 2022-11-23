FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./TreeMe.sln .
COPY ./BackEnd/BackEnd.csproj ./src/BackEnd/
COPY ./GitHubOAuthMiddleWare/GitHubOAuthMiddleWare.csproj ./GitHubOAuthMiddleWare/
COPY ./TreeMeX/TreeMeX.csproj ./TreeMeX/TreeMeX.csproj
COPY ./Tests/Tests.csproj ./Tests/Tests.csproj
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish --no-restore -c Release -o out ./src/Treeme

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["./StreamBadgerLogin"]
