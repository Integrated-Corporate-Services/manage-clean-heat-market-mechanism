/// <reference types="Cypress" />

import CHMMApiGETRequestHelper from "../helperClasses/chmmAPIGETRequestHelper.js";
import CHMMApiPUTRequestHelper from "../helperClasses/chmmAPIPUTRequestHelper.js";

/******************** Step defs ************************/

When("the user send a PUT request to {string} to activate/deactivate with {string} email", function (endpointUrl, email) {
    var getRequestHelper = new CHMMApiGETRequestHelper();
    getRequestHelper.sendGETRequestWithAuthenticationToken("/identity/users/admin", false);
    cy.get('@requestResponse').then((response) => {  

        const extractedJsonObject = response.body.find((obj) => obj.email === email);
        const activateAdminUserJson = {
            "id": extractedJsonObject['id']
        };

        var putRequestHelper = new CHMMApiPUTRequestHelper();  
        putRequestHelper.sendPUTRequestWithAuthenticationToken(endpointUrl, 'application/json-patch+json', activateAdminUserJson, false);
    })
})
