FROM public.ecr.aws/lambda/dotnet:8 AS base
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /src
COPY ["App/YearEnd/Desnz.Chmm.YearEnd.Api/Desnz.Chmm.YearEnd.Api.csproj", "App/YearEnd/Desnz.Chmm.YearEnd.Api/"]
COPY ["App/YearEnd/Desnz.Chmm.YearEnd.ApiTests/Desnz.Chmm.YearEnd.ApiTests.csproj", "App/YearEnd/Desnz.Chmm.YearEnd.ApiTests/"]
COPY ["App/YearEnd/Desnz.Chmm.YearEnd.Common/Desnz.Chmm.YearEnd.Common.csproj", "App/YearEnd/Desnz.Chmm.YearEnd.Common/"]
COPY ["App/YearEnd/Desnz.Chmm.YearEnd.UnitTests/Desnz.Chmm.YearEnd.UnitTests.csproj", "App/YearEnd/Desnz.Chmm.YearEnd.UnitTests/"]
COPY ["Common/Desnz.Chmm.ApiClients/Desnz.Chmm.ApiClients.csproj", "Common/Desnz.Chmm.ApiClients/"]
COPY ["Common/Desnz.Chmm.Common/Desnz.Chmm.Common.csproj", "Common/Desnz.Chmm.Common/"]
COPY ["Common/Desnz.Chmm.Common.Tests/Desnz.Chmm.Common.Tests.csproj", "Common/Desnz.Chmm.Common.Tests/"]
COPY ["Common/Desnz.Chmm.Testing.Common/Desnz.Chmm.Testing.Common.csproj", "Common/Desnz.Chmm.Testing.Common/"]
COPY . .
WORKDIR /src/App/YearEnd/Desnz.Chmm.YearEnd.Api
RUN dotnet build -c Release -o /app/build

FROM build AS test
WORKDIR /src/App/YearEnd/Desnz.Chmm.YearEnd.UnitTests/
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