const { defineConfig } = require('cypress')
const cypressSplit = require('cypress-split')
const cucumber = require('cypress-cucumber-preprocessor').default

// All the plugin information goes into this function
async function setupNodeEvents(on, config) {  
  
  require('cypress-on-fix')(on);
  cypressSplit(on, config);
  on("file:preprocessor", cucumber());
    
  // Make sure to retun the config object as it might have been modified by the plugin.
  return config;
}

module.exports = defineConfig({
  chromeWebSecurity: false,
  viewportHeight: 1080,
  viewportWidth: 1920,
  pageLoadTimeout: 30000,
  defaultCommandTimeout: 10000,

  // https://github.com/adamgruber/mochawesome
  reporter: 'mochawesome',
  reporterOptions: {    
    reportDir: 'cypress/results',
    reportPageTitle: 'Cypress e2e test report',
    reportFilename: "[status]_[datetime]-[name]-report",
    timestamp: "longDate",
    embeddedScreenshots: true,
    useInlineDiffs: true,
    saveAllAttempts: true,
    json: true,
    html: true,
    overwrite: false
  },

  env: {    
    URL: "https://chmm-test.triad.co.uk",
    APIURL:"https://qj1y1xz5p0.execute-api.eu-west-2.amazonaws.com/api",
    TAGS: "@SmokeTest" 
  },

  retries: {
    runMode: 1,
    openMode: 1
  },

  e2e: {
  // baseUrl: "https://postman-echo.com/",
   //run multiple folders in e2e
   // specPattern: ['cypress/e2e/uitests/*.feature','cypress/e2e/accessibilityTests/*.feature','cypress/e2e/integrationTests/*.feature'],
    specPattern: "**/*.feature",
    retries: 1,

    setupNodeEvents    
  },
   
});
