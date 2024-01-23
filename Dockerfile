FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR ./

# Copy csproj and restore as distinct layers ....
COPY TreeMe.sln .
COPY BackEnd/BackEnd.csproj ./BackEnd/BackEnd.csproj
COPY GitHubOAuthMiddleWare/GitHubOAuthMiddleWare.csproj ./GitHubOAuthMiddleWare/GitHubOAuthMiddleWare.csproj
COPY dendrOnlineSPA/dendrOnlineSPA.csproj ./dendrOnlineSPA/dendrOnlineSPA.csproj
COPY Tests/Tests.csproj ./Tests/Tests.csproj
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR dendrOnlineSPA
RUN apt-get install -y nodejs
RUN npm install
RUN npm run build
WORKDIR ../
RUN dotnet publish --no-restore -c Release -o out ./dendrOnlineSPA

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /
COPY --from=build-env /out .
ENTRYPOINT ["./dendrOnlineSPA"]

