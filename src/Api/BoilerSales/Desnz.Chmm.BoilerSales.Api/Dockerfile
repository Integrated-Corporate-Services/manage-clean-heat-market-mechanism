FROM public.ecr.aws/lambda/dotnet:8 AS base
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /src
COPY ["Api/BoilerSales/Desnz.Chmm.BoilerSales.Api/Desnz.Chmm.BoilerSales.Api.csproj", "Api/BoilerSales/Desnz.Chmm.BoilerSales.Api/"]
COPY ["Api/BoilerSales/Desnz.Chmm.BoilerSales.ApiTests/Desnz.Chmm.BoilerSales.ApiTests.csproj", "Api/BoilerSales/Desnz.Chmm.BoilerSales.ApiTests/"]
COPY ["Api/BoilerSales/Desnz.Chmm.BoilerSales.Common/Desnz.Chmm.BoilerSales.Common.csproj", "Api/BoilerSales/Desnz.Chmm.BoilerSales.Common/"]
COPY ["Api/BoilerSales/Desnz.Chmm.BoilerSales.UnitTests/Desnz.Chmm.BoilerSales.UnitTests.csproj", "Api/BoilerSales/Desnz.Chmm.BoilerSales.UnitTests/"]
COPY ["Common/Desnz.Chmm.ApiClients/Desnz.Chmm.ApiClients.csproj", "Common/Desnz.Chmm.ApiClients/"]
COPY ["Common/Desnz.Chmm.Common/Desnz.Chmm.Common.csproj", "Common/Desnz.Chmm.Common/"]
COPY ["Common/Desnz.Chmm.Common.Tests/Desnz.Chmm.Common.Tests.csproj", "Common/Desnz.Chmm.Common.Tests/"]
COPY ["Common/Desnz.Chmm.Testing.Common/Desnz.Chmm.Testing.Common.csproj", "Common/Desnz.Chmm.Testing.Common/"]
COPY . .
WORKDIR /src/Api/BoilerSales/Desnz.Chmm.BoilerSales.Api
RUN dotnet build -c Release -o /app/build

FROM build AS test
WORKDIR /src/Api/BoilerSales/Desnz.Chmm.BoilerSales.UnitTests/
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