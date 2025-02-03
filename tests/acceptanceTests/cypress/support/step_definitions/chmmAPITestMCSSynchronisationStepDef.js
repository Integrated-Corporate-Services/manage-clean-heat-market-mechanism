/// <reference types="Cypress" />

import CHMMApiPOSTRequestHelper from "../helperClasses/chmmAPIPOSTRequestHelper.js"

/******************** Step defs ************************/

When("the user send a POST request to {string} to trigger MCS data with authentication token and following data:", function (endpointUrl, dataString) {
    cy.log("Request body = " + dataString);
    var postRequestHelper = new CHMMApiPOSTRequestHelper(); 
    postRequestHelper.sendPOSTRequestWithAuthenticationTokenAndAPIKey(endpointUrl, 'application/json', dataString, true);
})

When("the user send a POST request to {string} to trigger MCS data without authentication token and following data:", function (endpointUrl, dataString) {
    var postRequestHelper = new CHMMApiPOSTRequestHelper(); 
    postRequestHelper.sendPOSTRequestWithoutAuthenticationToken(endpointUrl, 'application/json', dataString, false);
})