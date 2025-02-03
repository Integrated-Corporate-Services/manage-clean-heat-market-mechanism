FROM public.ecr.aws/lambda/dotnet:8 AS base
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /src
COPY ["Api/Notes/Desnz.Chmm.Notes.Api/Desnz.Chmm.Notes.Api.csproj", "Api/Notes/Desnz.Chmm.Notes.Api/"]
COPY ["Api/Notes/Desnz.Chmm.Notes.ApiTests/Desnz.Chmm.Notes.ApiTests.csproj", "Api/Notes/Desnz.Chmm.Notes.ApiTests/"]
COPY ["Api/Notes/Desnz.Chmm.Notes.Common/Desnz.Chmm.Notes.Common.csproj", "Api/Notes/Desnz.Chmm.Notes.Common/"]
COPY ["Api/Notes/Desnz.Chmm.Notes.UnitTests/Desnz.Chmm.Notes.UnitTests.csproj", "Api/Notes/Desnz.Chmm.Notes.UnitTests/"]
COPY ["Common/Desnz.Chmm.ApiClients/Desnz.Chmm.ApiClients.csproj", "Common/Desnz.Chmm.ApiClients/"]
COPY ["Common/Desnz.Chmm.Common/Desnz.Chmm.Common.csproj", "Common/Desnz.Chmm.Common/"]
COPY ["Common/Desnz.Chmm.Common.Tests/Desnz.Chmm.Common.Tests.csproj", "Common/Desnz.Chmm.Common.Tests/"]
COPY ["Common/Desnz.Chmm.Testing.Common/Desnz.Chmm.Testing.Common.csproj", "Common/Desnz.Chmm.Testing.Common/"]
COPY . .
WORKDIR /src/Api/Notes/Desnz.Chmm.Notes.Api
RUN dotnet build -c Release -o /app/build

FROM build AS test
WORKDIR /src/Api/Notes/Desnz.Chmm.Notes.UnitTests/
RUN dotnet test -v=normal
WORKDIR /src/Common/Desnz.Chmm.Common.Tests/
RUN dotnet test -v=normal

FROM build AS publish
RUN dotnet publish \
    --configuration Release \ 
    --runtime linux-x64 \
    --self-contained false \ 
    --output /app/publish \
    -p:PublishReadyToRun=true  

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .