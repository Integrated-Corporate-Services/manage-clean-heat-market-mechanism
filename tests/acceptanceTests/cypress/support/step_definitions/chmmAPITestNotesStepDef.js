/// <reference types="Cypress" />

import CHMMApiPOSTRequestHelper from "../helperClasses/chmmAPIPOSTRequestHelper.js";
import CHMMApiGETRequestHelper from "../helperClasses/chmmAPIGETRequestHelper.js";

/******************** Step defs ************************/

When("the user send a POST request to {string} to create a note with the following data and with authentication token:", function (endpointUrl, datatable) {
    let notesJsonData = {};

    cy.get('@organisationId').then((organisationId) => {
        cy.log('organisationId = ' + organisationId);
        datatable.hashes().forEach((row) => {
            if (row.key.toLowerCase() === 'organisationid' && (row.value.toLowerCase() === 'random' || row.value.toLowerCase() === 'newrandom')) {
                row.value = organisationId;
            }            
            notesJsonData[row.key] = row.value;
        })
        cy.log('notesJsonData = ' + JSON.stringify(notesJsonData));
        var postRequestHelper = new CHMMApiPOSTRequestHelper();
        postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'application/json', JSON.stringify(notesJsonData), false);
    })
    cy.get('@requestResponse').then((response) => {
        const uuidPattern = /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/;

        if (uuidPattern.test(response.body['id'])) {
            cy.wrap(JSON.stringify(response.body['id']).slice(1, -1)).as('noteId');
            cy.log('Note Id = ' + JSON.stringify(response.body['id']).slice(1, -1));
        }                
    }) 
})

When("the user send a POST request to {string} to create a note with the following data and without authentication token:", function (endpointUrl, datatable) {
    let notesJsonData = {};

    cy.get('@organisationId').then((organisationId) => {
        cy.log('organisationId = ' + organisationId);
        datatable.hashes().forEach((row) => {
            if (row.key.toLowerCase() === 'organisationid' && (row.value.toLowerCase() === 'random' || row.value.toLowerCase() === 'newrandom')) {
                row.value = organisationId;
            }            
            notesJsonData[row.key] = row.value;
        })
        cy.log('notesJsonData = ' + JSON.stringify(notesJsonData));
        var postRequestHelper = new CHMMApiPOSTRequestHelper();
        postRequestHelper.sendPOSTRequestWithoutAuthenticationToken(endpointUrl, 'application/json', JSON.stringify(notesJsonData), false);
    }) 
})

When("the user send a POST request to {string} with authentication token", function (endpointUrl) {
    if (endpointUrl.includes('organisationId')) {
        cy.get('@organisationId').then((organisationId) => {
            endpointUrl = endpointUrl.replace('organisationId', organisationId);
            var postRequestHelper = new CHMMApiPOSTRequestHelper();
            postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, '', '', false);
        })
    } else {
        var postRequestHelper = new CHMMApiPOSTRequestHelper();
        postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, '', '', false);
    } 
})

When("the user send a POST request to {string} without authentication token", function (endpointUrl) {
    if (endpointUrl.includes('organisationId')) {
        cy.get('@organisationId').then((organisationId) => {
            endpointUrl = endpointUrl.replace('organisationId', organisationId);
            var postRequestHelper = new CHMMApiPOSTRequestHelper();
            postRequestHelper.sendPOSTRequestWithoutAuthenticationToken(endpointUrl, '', '', false);
        })
    } else {
        var postRequestHelper = new CHMMApiPOSTRequestHelper();
        postRequestHelper.sendPOSTRequestWithoutAuthenticationToken(endpointUrl, '', '', false);
    } 
})

Then("the response body array for get notes for {string} organisation should contain the following data:", function (orgId, datatable) {
    cy.get('@organisationId').then((organisationId) => {
        if (orgId.toLowerCase() === 'newrandom' || orgId.toLowerCase() === 'random') {
            orgId = organisationId;
        }

        cy.get('@requestResponse').then((response) => {
            const targetObject = response.body.find(obj => obj['organisationId'] === orgId);        
            expect(targetObject).to.exist;

            datatable.hashes().forEach((row) => {
                if (row.key === 'creationDate') {
                    if(row.creationDate === 'today') {
                        creationDate = generateDate();
                    } 
                    expect(targetObject[row.key]).to.include(creationDate);
                } else if (row.key.toLowerCase() === 'organisationid') {
                    expect(targetObject[row.key].toString()).to.equal(orgId);
                } else {
                    expect(targetObject[row.key].toString()).to.equal(row.value.toString());
                }
            })
        })
    })    
})

When("the user send a GET request to {string} to fetch/download notes files with authentication token", function (endpointUrl) {
    var getRequestHelper = new CHMMApiGETRequestHelper();
    if (endpointUrl.toLowerCase().includes('organisationid') && endpointUrl.toLowerCase().includes('noteid')) {        
        cy.get('@organisationId').then((organisationId) => {  
            endpointUrl = endpointUrl.replace('organisationId', organisationId)
            cy.get('@noteId').then((noteId) => {
                endpointUrl = endpointUrl.replace('noteId', noteId)
                getRequestHelper.sendGETRequestWithAuthenticationToken(endpointUrl, false);
            })                  
        })
    } else if (endpointUrl.toLowerCase().includes('organisationid')) {
        cy.get('@organisationId').then((organisationId) => {  
            getRequestHelper.sendGETRequestWithAuthenticationToken(endpointUrl.replace('organisationId', organisationId), false);      
        })
    } else if (endpointUrl.toLowerCase().includes('noteid')) {
        cy.get('@noteId').then((noteId) => {
            endpointUrl = endpointUrl.replace('noteId', noteId)
            getRequestHelper.sendGETRequestWithAuthenticationToken(endpointUrl, false);
        })
    } else {
        getRequestHelper.sendGETRequestWithAuthenticationToken(endpointUrl, false);
    } 
})

When("the user send a GET request to {string} to fetch/download notes files without authentication token", function (endpointUrl) {
    var getRequestHelper = new CHMMApiGETRequestHelper();
    if (endpointUrl.toLowerCase().includes('organisationid') && endpointUrl.toLowerCase().includes('noteid')) {        
        cy.get('@organisationId').then((organisationId) => {  
            endpointUrl = endpointUrl.replace('organisationId', organisationId)
            cy.get('@noteId').then((noteId) => {
                endpointUrl = endpointUrl.replace('noteId', noteId)
                getRequestHelper.sendGETRequestWithoutAuthenticationToken(endpointUrl, false);
            })                  
        })
    } else if (endpointUrl.toLowerCase().includes('organisationid')) {
        cy.get('@organisationId').then((organisationId) => {  
            getRequestHelper.sendGETRequestWithoutAuthenticationToken(endpointUrl.replace('organisationId', organisationId), false);      
        })
    } else if (endpointUrl.toLowerCase().includes('noteid')) {
        cy.get('@noteId').then((noteId) => {
            endpointUrl = endpointUrl.replace('noteId', noteId)
            getRequestHelper.sendGETRequestWithoutAuthenticationToken(endpointUrl, false);
        })
    } else {
        getRequestHelper.sendGETRequestWithoutAuthenticationToken(endpointUrl, false);
    } 
})

/*************** functions ****************/

/**
 * Function to generate date in "2024-01-24" format
 * @returns 
 */
function generateDate() {
    const currentDate = new Date();
    const dateString = currentDate.toISOString().split('T')[0];
    return dateString;
}