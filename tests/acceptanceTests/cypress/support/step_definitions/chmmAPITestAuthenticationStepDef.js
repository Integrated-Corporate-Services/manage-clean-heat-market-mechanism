/// <reference types="Cypress" />
import omit from "lodash/omit";
import isEqual from "lodash/isEqual"; 
import { testStore } from "../commands";

/****************** Properties *************/

let authenticationUri = '/identity/token';
let authToken;

/*************************** Step defs ******************/

Given("the user has authentication token for {string} email id", function (emailId) {    
    if (emailId.toLowerCase() === 'newrandom' || emailId.toLowerCase() === 'random') {
        emailId = localStorage.getItem('mfrUserEmailId');
    }
    cy.wrap(null).then(() => {
        return cy.authenticationApiRequest(emailId);            
        }).then((responseBody) => {        
            cy.log("authToken = " + authToken);
            localStorage.setItem("AuthToken", authToken);
    })
})

Given("the user has authentication token for {string} manufacturer user email id", function (emailId) {
    if (emailId.toLowerCase() === 'newrandom' || emailId.toLowerCase() === 'random') {
        emailId = localStorage.getItem('mfrUserEmailId');
    }
    cy.wrap(null).then(() => {
        return cy.authenticationApiRequest(emailId);            
        }).then((responseBody) => {        
            cy.log("authToken = " + authToken);
            localStorage.setItem("AuthToken", authToken);
    })   
})

Cypress.Commands.add('authenticationApiRequest', (emailId) => {
    return cy
        .request({
            method: 'POST',
            url: Cypress.env('APIURL') + authenticationUri,
            body: {                    
                "IdToken": "something",
                "AccessToken": "something",
                "Email": emailId                    
            },  
        })
        .then((response) => {
            authToken = response.body;
            return response.body;
        })
})
