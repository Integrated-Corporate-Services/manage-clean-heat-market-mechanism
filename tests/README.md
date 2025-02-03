Acceptance tests
================

This directory contains e2e acceptahce tests using Cypress, Cucumber and JavaScript.

# Pre-requisites
- Download and setup <a href="https://nodejs.org/en/download" target="_blank">NodeJS</a>
- Donwload and setup <a href="https://code.visualstudio.com/download" target="_blank">Visual Studio Code</a>
- Install <a href="https://marketplace.visualstudio.com/items?itemName=alexkrechik.cucumberautocomplete" target="_blank">Cucumber plugin</a> for Visual Studio Code extensions 
- Familiarise yourself with writing <a href="https://cucumber.io/docs/gherkin" target="_blank">Gherkin Syntax</a> and <a href="https://cucumber.io/docs/cucumber/step-definitions" target="_blank">Step Definitions</a>
- <a href="https://github.com/badeball/cypress-cucumber-preprocessor/tree/master/docs" target="_blank">Cypress Documentation</a>

# Setting up Cypress e2e test project
- Clone <a href="https://github.com/Integrated-Corporate-Services/manage-clean-heat-market-mechanism-ui-tests-private.git" target="_blank">Manage clean heating market mechanism UI tests </a> repository
- Open the project in Visual Studio code 
- Navigate to `acceptanceTest` directory (This directory contains Cypress installation)

# Install Cypress
- Open a new terminal in VS code and cd to `acceptanceTests` directory
- run `npm install cypress` (This will install Cypress)
- run `npx cypress open` (This command will open Cypress runner window)
- Select `E2E Testing` (Once selected it will display the 'Choose a browser' window)
- Select `Chrome` and click `Start E2E Testing in Chrome` button (This will open the a new Chrome browser window with the list of tests)
- You can run the individual tests by clicking in the test name

# Running tests from VS Code terminal
- In VS Code open a new terminal
- Navigate to `acceptanceTests` directory
- run `Npm run cySmokeTest:run` (cySmokeTest:run is a script in package.json that actually executes `npx cypress run --env tags=@SmokeTest --headed --browser chrome`)
- Once test execution is completed it generates `.json` and `.html` report files in `acceptanceTests/cypress/results/`. Open the `.html` file in a browser window to view the HTML report for the latest test run.

# Using axe dev tool with cypress
- Install cypress -axe from npm:npm install --save-dev cypress-axe
- Install peer dependencies:npm install --save-dev cypress axe-core
- update cypress/support/e2e.js file to include the cypress-axe commands by adding:import 'cypress-axe'
