/// <reference types="Cypress" />
import CHMMApiPOSTRequestHelper from "../helperClasses/chmmAPIPOSTRequestHelper.js"
import CHMMApiTestDataHandlerHelper from "../helperClasses/chmmAPITestDataHandlerHelper.js";


/******************** Step defs ************************/

When("the user send a POST request to {string} to amend/submit an obligation with authentication token and following data:", function (endpointUrl, datatable) {
    postToAmendObligations(endpointUrl, datatable);

    cy.get('@requestResponse').then((response) => {
        const responseBody = response.body;
        var obligationAdjustmentId;
        if (typeof responseBody === 'object') {
            const jsonString = JSON.stringify(responseBody);
            try {
                const parsedJson = JSON.parse(jsonString);                
                cy.log('Parsed JSON: ', parsedJson)
                obligationAdjustmentId = parsedJson.id;
            } catch (error) {
                cy.log('Non-JSON string:', jsonString);
                licenceHolderId = jsonString.slice(1, -1);
            }
        }
        cy.wrap(obligationAdjustmentId).as('obligationAdjustmentId');
        cy.log('Obligation adjustment Id = ' + obligationAdjustmentId);
    })
})

When("the user send a POST request to {string} to amend/submit an obligation without authentication token and following data:", function (endpointUrl, datatable) {
    let amendObligationJson = {};
    datatable.hashes().forEach((row) => {
        amendObligationJson[row.key] = row.value;
    })
    var postRequestHelper = new CHMMApiPOSTRequestHelper();
    postRequestHelper.sendPOSTRequestWithoutAuthenticationToken(endpointUrl, 'application/json', JSON.stringify(amendObligationJson), false);
})

Then("the response json body should contain {string} element with the following data:", function (jsonElement, datatable) {
    let responseKeyPath = null;
    cy.get('@requestResponse').then((response) => {
        datatable.hashes().forEach((row) => {
            
            if (jsonElement.toLowerCase() === 'obligationadjustmentid') {
                cy.get('@obligationAdjustmentId').then((adjustmentId) => { 
                    responseKeyPath = response.body[adjustmentId];
                    expect(responseKeyPath[row.key].toString()).to.eq(row.value);
                })                 
            } else if (row.key.toLowerCase() === 'dateoftransaction' && (row.value.toLowerCase() === 'currentdate' || row.value.toLowerCase() === 'today')) {
                expect(responseKeyPath[row.key].toString()).include(generateDate());
            } else {
                responseKeyPath = response.body[jsonElement];
                expect(responseKeyPath[row.key].toString()).to.eq(row.value);
            }
        })                
    })
})


/*************** functions ****************/

/**
 * Function to send a POST request to amend obligations
 * @param {*} endpointUrl 
 * @param {*} datatable 
 */
function postToAmendObligations(endpointUrl, datatable) {
    let amendObligationJson = {};
    datatable.hashes().forEach((row) => {
        if (row.key.toLowerCase() === 'transactiondate') {
            row.value = generateDateTime();
        }
        amendObligationJson[row.key] = row.value;
    })    

    

    if (amendObligationJson['organisationId'].toLowerCase() === 'random' || amendObligationJson['organisationId'].toLowerCase() === 'newrandom') {
        cy.get('@organisationId').then((organisationId) => {
            cy.log('organisationId = ' + organisationId);
            amendObligationJson['organisationId'] = organisationId;
            cy.log('amendObligationJson = ' + JSON.stringify(amendObligationJson));
            var postRequestHelper = new CHMMApiPOSTRequestHelper();
            postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'application/json', JSON.stringify(amendObligationJson), false);
        }) 
    }  else {
        cy.log('amendObligationJson = ' + JSON.stringify(amendObligationJson));
        var postRequestHelper = new CHMMApiPOSTRequestHelper();
        postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'application/json', JSON.stringify(amendObligationJson), false);
    }
}

/**
 * Function to generate date in "2024-01-24" format
 * @returns 
 */
function generateDate() {
    const currentDate = new Date();
    const dateString = currentDate.toISOString().split('T')[0];
    return dateString;
}

/**
 * Function to generate date time in "2024-01-24T12:12:22.941Z" format
 * @returns 
 */
function generateDateTime() {
    const currentDate = new Date();
    const dateTimeString = currentDate.toISOString();
  
    return dateTimeString;
  }