FROM node as build-front-env
# copy all content from repository
COPY . .

# build svelte front
WORKDIR /dendrOnlineSPA
RUN ls -alh
RUN pwd
RUN npm install
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-back-env
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

# Copy svelte javascript/CSS assets
COPY --from=build-front-env /dendrOnlineSPA/wwwroot /dendrOnlineSPA/wwwroot

# build aspnet core application
RUN dotnet publish --no-restore -c Release -o out ./dendrOnlineSPA

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /
# copy application
COPY --from=build-back-env /out .
ENTRYPOINT ["./dendrOnlineSPA"]

