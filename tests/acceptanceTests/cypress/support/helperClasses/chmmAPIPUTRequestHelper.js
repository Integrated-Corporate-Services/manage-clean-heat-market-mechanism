
class CHMMApiPUTRequestHelper {

    /**
 * Send a PUT request to the given endpont with authentication token and request body
 * @param {*} endpointUrl 
 *  @param {*} contentType
 * @param {*} requestBody 
 * @param {*} failOnStatusCode 
 */
sendPUTRequestWithAuthenticationToken(endpointUrl, contentType, requestBody, failOnStatusCode) {    
    cy.request({
        method:'PUT',
        url: Cypress.env('APIURL') + endpointUrl,
        headers: {
                 'Content-type': contentType,
                 'Authorization': 'Bearer ' + localStorage.getItem("AuthToken")
        },
        body: requestBody,
        failOnStatusCode: failOnStatusCode
    }).as('requestResponse'); 
}

/**
 * Send a PUT request to the given endpont with request body and without authentication token
 * @param {*} endpointUrl 
 *  @param {*} contentType
 * @param {*} requestBody 
 * @param {*} failOnStatusCode 
 */
sendPUTRequestWithoutAuthenticationToken(endpointUrl, contentType, requestBody, failOnStatusCode) {    
    cy.request({
        method:'PUT',
        url: Cypress.env('APIURL') + endpointUrl,
        headers: {
                 'Content-type': contentType
        },
        body: requestBody,
        failOnStatusCode: failOnStatusCode
    }).as('requestResponse'); 
}

}

export default CHMMApiPUTRequestHelper;