
class CHMMApiPOSTRequestHelper {

/**
 * Send a POST request to the given endpont with authentication token and request body
 * @param {*} endpointUrl 
 *  @param {*} contentType
 * @param {*} requestBody 
 * @param {*} failOnStatusCode 
 */
sendPOSTRequestWithAuthenticationToken(endpointUrl, contentType, requestBody, failOnStatusCode) {    
    cy.request({
        method:'POST',
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
 * Send a POST request to the given endpont with request body and without authentication token
 * @param {*} endpointUrl 
 *  @param {*} contentType
 * @param {*} requestBody 
 * @param {*} failOnStatusCode 
 */
sendPOSTRequestWithoutAuthenticationToken(endpointUrl, contentType, requestBody, failOnStatusCode) {    
    cy.request({
        method:'POST',
        url: Cypress.env('APIURL') + endpointUrl,
        headers: {
                 'Content-type': contentType
        },
        body: requestBody,
        failOnStatusCode: failOnStatusCode
    }).as('requestResponse'); 
}

/**
 * Send a POST request to the given endpont with authentication token and request body
 * @param {*} endpointUrl 
 *  @param {*} contentType
 * @param {*} requestBody 
 * @param {*} failOnStatusCode 
 * @param {*} dateTimeOverride
 */
sendPOSTRequestWithAuthenticationTokenAndDateTimeOverride(endpointUrl, contentType, dateTimeOverride, requestBody, failOnStatusCode) {    
    cy.request({
        method:'POST',
        url: Cypress.env('APIURL') + endpointUrl,
        headers: {
            'Content-type': contentType,
            'Authorization': 'Bearer ' + localStorage.getItem("AuthToken"),
            'date-time-override': dateTimeOverride
        },
        body: requestBody,
        failOnStatusCode: failOnStatusCode
    }).as('requestResponse'); 
}

/**
 * Send a POST request to the given endpont without authentication token and request body
 * @param {*} endpointUrl 
 *  @param {*} contentType
 * @param {*} requestBody 
 * @param {*} failOnStatusCode 
 * @param {*} dateTimeOverride
 */
sendPOSTRequestWithoutAuthenticationTokenAndDateTimeOverride(endpointUrl, contentType, dateTimeOverride, requestBody, failOnStatusCode) {    
    cy.request({
        method:'POST',
        url: Cypress.env('APIURL') + endpointUrl,
        headers: {
            'Content-type': contentType,
            'date-time-override': dateTimeOverride
        },
        body: requestBody,
        failOnStatusCode: failOnStatusCode
    }).as('requestResponse'); 
}

/**
 * Send a POST request to the given endpont with API key and request body
 * @param {*} endpointUrl 
 * @param {*} contentType
 * @param {*} requestBody 
 * @param {*} failOnStatusCode 
 */
sendPOSTRequestWithAuthenticationTokenAndAPIKey(endpointUrl, contentType, requestBody, failOnStatusCode) {    
    // debugger;
    cy.request({
        method:'POST',
        url: Cypress.env('APIURL') + endpointUrl,
        headers: {
            'Content-type': contentType,
            // 'Authorization': 'Bearer ' + localStorage.getItem("AuthToken"),
            'chmm-api-key': 'iT35bgUWjy4DowR19kOZmXzhPj88BIfa'
        },
        body: requestBody,
        failOnStatusCode: failOnStatusCode
    }).as('requestResponse'); 
}

}

export default CHMMApiPOSTRequestHelper;