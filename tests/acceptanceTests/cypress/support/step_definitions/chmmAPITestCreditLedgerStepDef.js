/// <reference types="Cypress" />

import CHMMApiPOSTRequestHelper from "../helperClasses/chmmAPIPOSTRequestHelper.js";
import CHMMApiGETRequestHelper from "../helperClasses/chmmAPIGETRequestHelper.js";


/******************** Step defs ************************/

When("the user send a POST request to {string} to adjust/transfer credits with the following data and authentication token:", function (endpointUrl, datatable) {
    let creditsJsonData = {};
    
    cy.get('@organisationId').then((organisationId) => {
        cy.log('organisationId = ' + organisationId);
        datatable.hashes().forEach((row) => {
            if (row.key.toLowerCase() === 'organisationid' && (row.value.toLowerCase() === 'random' || row.value.toLowerCase() === 'newrandom')) {
                row.value = organisationId;
            }
            if (row.key.toLowerCase() === 'destinationorganisationid' && (row.value.toLowerCase() === 'random' || row.value.toLowerCase() === 'newrandom')) {
                row.value = organisationId
            }
            creditsJsonData[row.key] = row.value;
        })
        cy.log('creditsJsonData = ' + JSON.stringify(creditsJsonData));
        var postRequestHelper = new CHMMApiPOSTRequestHelper();
        postRequestHelper.sendPOSTRequestWithAuthenticationTokenAndDateTimeOverride(endpointUrl, 'application/json', '2024-10-02', JSON.stringify(creditsJsonData), false);
    })
})

When("the user send a POST request to {string} to adjust/transfer credits with the following data and without authentication token:", function (endpointUrl, datatable) {
    let creditsJsonData = {};
    
    cy.get('@organisationId').then((organisationId) => {
        cy.log('organisationId = ' + organisationId);
        datatable.hashes().forEach((row) => {
            if (row.key.toLowerCase() === 'organisationid' && (row.value.toLowerCase() === 'random' || row.value.toLowerCase() === 'newrandom')) {
                row.value = organisationId;
            }
            if (row.key.toLowerCase() === 'destinationorganisationid' && (row.value.toLowerCase() === 'random' || row.value.toLowerCase() === 'newrandom')) {
                row.value = organisationId
            }
            creditsJsonData[row.key] = row.value;
        })
        cy.log('creditsJsonData = ' + JSON.stringify(creditsJsonData));
        var postRequestHelper = new CHMMApiPOSTRequestHelper();
        postRequestHelper.sendPOSTRequestWithoutAuthenticationTokenAndDateTimeOverride(endpointUrl, 'application/json', '2024-10-02', JSON.stringify(creditsJsonData), false);
    })
})

When("the user send a GET request to {string} with date time override to {string} with authentication token", function (endpointUrl, dateTimeOverride) {
    var getRequestHelper = new CHMMApiGETRequestHelper();
    if (endpointUrl.toLowerCase().includes('organisationid')) {
        cy.get('@organisationId').then((organisationId) => {  
            getRequestHelper.sendGETRequestWithAuthenticationTokenAndDateTimeOverride(endpointUrl.replace('organisationId', organisationId), dateTimeOverride, false);      
        })
    } else {
        getRequestHelper.sendGETRequestWithAuthenticationTokenAndDateTimeOverride(endpointUrl, dateTimeOverride, false);
    } 
})

Then("the response json body for credit transactions should contain {string} element array with the following data:", function (responseKeyPath, datatable) {
    cy.get('@requestResponse').then((response) => {
        const jsonData = response.body[responseKeyPath];
        
        const expectedValues = datatable.hashes().map(row => ({
            dataOfTransaction: new Date(row.dataOfTransaction),
            credits: parseInt(row.credits),
        }));

        const actualValues = jsonData.map(item => ({
            dataOfTransaction: new Date(item.dataOfTransaction),
            credits: item.credits,
        }));
        const sortFunction = (a, b) => a.dateOfTransaction - b.dateOfTransaction;
        expectedValues.sort(sortFunction);
        actualValues.sort(sortFunction);
        
        expect(actualValues).to.deep.equal(expectedValues);            
    })
})

