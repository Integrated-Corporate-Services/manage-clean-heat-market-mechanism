
class CHMMApiGETRequestHelper {

/**
 * Send a GET request to the given endpoint with authentication token
 * @param {*} endpointUrl 
 */
sendGETRequestWithAuthenticationToken(endpointUrl, failOnStatusCode) {
    cy.request({
        method:'GET',
        url: Cypress.env('APIURL') + endpointUrl,
        headers: {
            'Content-type': 'application/json',
            'Authorization': 'Bearer '+ localStorage.getItem("AuthToken")
        },
        failOnStatusCode: failOnStatusCode
    }).as('requestResponse');
}

/**
 * Send GET request to the given endpoint without authentication token
 * @param {*} endpointUrl 
 * @param {*} failOnStatusCode 
 */
sendGETRequestWithoutAuthenticationToken(endpointUrl, failOnStatusCode) {
    cy.request({
        method:'GET',
        url: Cypress.env('APIURL') + endpointUrl,
        headers: {
            'Content-type': 'application/json'
        },
        failOnStatusCode: failOnStatusCode
    }).as('requestResponse');
}

/**
 * Send a GET request to the given endpoint with date time override and with authentication token
 * @param {*} endpointUrl 
 */
    sendGETRequestWithAuthenticationTokenAndDateTimeOverride(endpointUrl, dateTimeOverride, failOnStatusCode) {
        cy.request({
            method:'GET',
            url: Cypress.env('APIURL') + endpointUrl,
            headers: {
                'Content-type': 'application/json',
                'date-time-override': dateTimeOverride,
                'Authorization': 'Bearer '+ localStorage.getItem("AuthToken")
            },
            failOnStatusCode: failOnStatusCode
        }).as('requestResponse');
    }

}

export default CHMMApiGETRequestHelper;