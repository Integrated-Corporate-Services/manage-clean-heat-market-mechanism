# Manage Clean Heat Market Mechanism

To create and deploy a service which enables the administrator of the scheme to capture the quantity of:
- Sales of conventional boilers
- Installs of low carbon heat pumps

The service will calculate obligations on the manufacturers of conventional boilers for each sale and issue credits to the manufacturers of low carbon heat pumps for each installation notified via MCS.
The service will support the reflection of any traded credits outside of the system, allow manufacturers to offset their obligation, and enable the administrator to ensure manufacturers compliance with CHMM policy.

## Getting Started

This solution comprises of three main components:
1. Terraform / Terragrunt IaC to deploy the infrastructure onto AWS to host the application
2. A collection of APIs running on the DotNet 8 runtime within Visual Studio
3. An Angular SPA which can be run using npm

Due to the sensitive nature of the Terraform IaC, it has been excluded from this repository.

## Backstory
### In Scope
A digital service is required to enable the administrator of the scheme to capture the quantity of sales of conventional boilers and installs of low carbon heat pumps and to allow manufacturers to manage their target and credit obligation throughout the year.
- Policy applies to any heating appliance manufacturer who sells heating products in the UK market.
- Policy applies to fossil fuel boiler manufacturers above the de minimis threshold (20,000 sales per annum of gas boilers and 1,000 sales per annum of oil boilers).
- Retro-fit installations (e.g., not new builds) of low carbon heat pumps below 45Kwh and conventional boilers below 70Kwh.
- Intention to be UK wide, including imported products but excluding exported boilers.
### Out of Scope
- New build heat pump installs (covered by Future Home Standard Policy).
- Only applies to low carbon heat pumps, e.g., gas heat pumps are out of scope.
- Heat pumps equal to and above 45Kwh.
- Fossil fuel boilers equal to and above 70Kwh.
- Manufacturers below policy threshold.

## Technology
### Infrastructure as code
The solution uses Terraform to implement infrastructure as code and Continuous Integration / Continuous Deployment (CI/CD) pipelines to deploy the application to the appropriate environment within the AWS infrastructure. GitHub Enterprise is used to host the repositories and to manage the code promotion and deployment process. GitHub Actions host the pipelines which are triggered either by merges into the Main branch, or manually through GitHub itself, this pulls the code from GitHub, builds, tests, and deploys to the relevant environment.

Terragrunt is used to manage deployment of the Terraform infrastructure across the multiple environments

### Web application
The user interface has been implemented as a SPA written in Angular using the Government Digital Service (GDS) components to ensure compliance with government standards.

The front-end application interacts with the back-end services via an AWS API gateway. Interaction is by way of passing commands or queries to the server APIs. 

### APIs
Each of the API microservices are written in DotNet 8 and containerised using a Docker file by GitHub actions, pushed to ECR then deployed to AWS Lambda services running dotnet 8. Each microservice is a single dotnet web application providing access to a number of API endpoints all related to the single purpose of the microservice. For example, the Boiler Sales microservice only handles requests relating to boiler sales figures.

## Build and run
The root of the solution contains Dessnz.Chmm.sln, this opens in Visual Studio and runs each of the APIs and the local host for the web front end.

Open /src/Web/Desnz.Chmm.Web/ClientApp in Visual Studio Code.

It is recommended that Docker be used to run an instance of Postgres for database hosting. The following databases should be created:
- chmm-auditing
- chmm-boilersales
- chmm-configuration
- chmm-creditledger
- chmm-identity
- chmm-mcssynchronisation
- chmm-notes
- chmm-obligation
- chmm-systemaudit
- chmm-yearend

Once the database engine is running start the application in Visual Studio. The following projects should be configured as startup projects:
- Desnz.Chmm.BoilerSales.Api
- Desnz.Chmm.Configuration.Api
- Desnz.Chmm.CreditLedger.Api
- Desnz.Chmm.Identity.Api
- Desnz.Chmm.Notes.Api
- Desnz.Chmm.Obligations.Api
- Desnz.Chmm.SystemAudit.Api
- Desnz.Chmm.McsSynchronisation.Api
- Desnz.Chmm.YearEnd.Api

Finally, using `npm run start` within the ClientApp directory will trigger a build and run of the SPA allowing the application to load.