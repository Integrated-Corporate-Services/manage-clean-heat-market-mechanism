// ***********************************************************
// This example support/e2e.js is processed and
// loaded automatically before your test files.
//
// This is a great place to put global configuration and
// behavior that modifies Cypress.
//
// You can change the location of this file or turn off
// automatically serving support files with the
// 'supportFile' configuration option.
//
// You can read more here:
// https://on.cypress.io/configuration
// ***********************************************************

// Import commands.js using ES2015 syntax:
import './commands'

//Import cypress axe comand
import 'cypress-axe'

import CHMMApiPOSTRequestHelper from "./helperClasses/chmmAPIPOSTRequestHelper.js"


// Alternatively you can use CommonJS syntax:
// require('./commands')


// Support to attach the screenshots and videos to the spec reports
import addContext from 'mochawesome/addContext'

const titleToFileName = (title) =>
    title.replace(/[:\/]/g, '')

Cypress.on('test:after:run', (test, runnable) => {
    if (test && test.tags && testHasTags(test, ['@SmokeTest', '@RegressionTest'])) { 
        const filename = getParentTitles(runnable) + titleToFileName(test.title) + ' (failed).png';
        addScreenshotOnFailure(test, filename);
    }   
    
    if (test && test.tags && testHasTags(test, ['@UnlinkLicenceHolder'])) { 
        unlinkLicenceHolder();
    }
})

beforeEach('Authenticate One Login integration before each scenario', () => {
    cy.visit('https://integration-user:winter2021@signin.integration.account.gov.uk/', { failOnStatusCode: false });
})


const unlinkLicenceHolder = () => {
    var endpointUrl = "/identity/licenceholders/licenceHolderId/unlink/organisationId";
    cy.get('@licenceHolderId').then((licenceHolderId) => {
        cy.get('@organisationId').then((organisationId) => {
            endpointUrl = endpointUrl.replace('organisationId', organisationId).replace('licenceHolderId', licenceHolderId);
            var postRequestHelper = new CHMMApiPOSTRequestHelper();
            postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, '', '', false);
        })
    })
}

export const addScreenshotOnFailure = (test, filename) => {
  if (test.state === 'failed') {
    addContext({ test }, `../screenshots/${Cypress.spec.name}/${filename}`);
  }
}; 

function getParentTitles(runnable) {
    let parent = runnable.parent;
    let filename = '';
    while (parent && parent.title) {
      filename = `${titleToFileName(parent.title)} -- ${filename}`;
      parent = parent.parent;
    }
    return filename;
}

function testHasTags(test, tags) {
    return tags.some(tag => test.tags && test.tags.includes(tag));
}