/// <reference types="Cypress" />
import CHMMApiPOSTRequestHelper from "../helperClasses/chmmAPIPOSTRequestHelper.js"
import CHMMApiTestDataHandlerHelper from "../helperClasses/chmmAPITestDataHandlerHelper.js";


/******************** Step defs ************************/

When("the user send a POST request to {string} to create a licence holder with authentication token and following data:", function (endpointUrl, datatable) {
    postToCreateLicenceHolders(endpointUrl, datatable);    
    cy.get('@requestResponse').then((response) => {
        const responseBody = response.body;
        var licenceHolderId;
        if (typeof responseBody === 'object') {
            const jsonString = JSON.stringify(responseBody);
            try {
                const parsedJson = JSON.parse(jsonString);                
                cy.log('Parsed JSON: ', parsedJson)
                licenceHolderId = parsedJson.id;
            } catch (error) {
                cy.log('Non-JSON string:', jsonString);
                licenceHolderId = jsonString.slice(1, -1);
            }
        }
        cy.wrap(licenceHolderId).as('licenceHolderId');
        cy.log('Licence Holder Id = ' + licenceHolderId);
    })
})

When("the user send a POST request to {string} to create a licence holder without authentication token and following data:", function (endpointUrl, datatable) {
    let licenceHoldersJson;
    if (endpointUrl.includes('batch')) {
        licenceHoldersJson = createLicenceHolderBatchJsonObject(datatable);
    } else {
        licenceHoldersJson = createLicenceHolderJsonObject(datatable);
    }     
    cy.log('Licence Holder Json = ' + JSON.stringify(licenceHoldersJson));

    var postRequestHelper = new CHMMApiPOSTRequestHelper();
    postRequestHelper.sendPOSTRequestWithoutAuthenticationToken(endpointUrl, 'application/json', JSON.stringify(licenceHoldersJson), false);    
})

When("the user save {string} licence holder Id", function (licenceHolderId) {
    cy.wrap(licenceHolderId).as('licenceHolderId');
})

When("the user send a POST request to {string} to unlink licence holder to organisation with authentication token:", function (endpointUrl, dataString) {    
    cy.log("Request body = " + dataString);
    if (endpointUrl.includes('licenceHolderId')) {
        cy.get('@licenceHolderId').then((licenceHolderId) => {
            cy.get('@organisationId').then((organisationId) => {
                endpointUrl = endpointUrl.replace('organisationId', organisationId);
                endpointUrl = endpointUrl.replace('licenceHolderId', licenceHolderId);
                var postRequestHelper = new CHMMApiPOSTRequestHelper();
                postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'application/json', dataString, false);
            })
        })
    } else if (endpointUrl.includes('organisationId')) {
        cy.get('@organisationId').then((organisationId) => {
            endpointUrl = endpointUrl.replace('organisationId', organisationId);
            var postRequestHelper = new CHMMApiPOSTRequestHelper();
            postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'application/json', dataString, false);
        })
    } else {
        var postRequestHelper = new CHMMApiPOSTRequestHelper();
        postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'application/json', dataString, false);
    }    
})

When("the user send a POST request to {string} to link licence holder to organisation with authentication token:", function (endpointUrl, dataString) {
    cy.log("Request body = " + dataString);
    if (endpointUrl.includes('licenceHolderId')) {
        cy.get('@licenceHolderId').then((licenceHolderId) => {
            cy.get('@organisationId').then((organisationId) => {
                endpointUrl = endpointUrl.replace('organisationId', organisationId).replace('licenceHolderId', licenceHolderId);
                var postRequestHelper = new CHMMApiPOSTRequestHelper();
                postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'application/json', dataString, false);
            })
        })
    } else if (endpointUrl.includes('organisationId')) {
        cy.get('@organisationId').then((organisationId) => {
            endpointUrl = endpointUrl.replace('organisationId', organisationId);
            var postRequestHelper = new CHMMApiPOSTRequestHelper();
            postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'application/json', dataString, false);
        })
    } else {
        var postRequestHelper = new CHMMApiPOSTRequestHelper();
        postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'application/json', dataString, false);
    }
})

When("the user send a POST request to {string} to link/unlink licence holder to organisation without authentication token", function (endpointUrl) {    
    if (endpointUrl.includes('licenceHolderId')) {
        cy.get('@licenceHolderId').then((licenceHolderId) => {
            cy.get('@organisationId').then((organisationId) => {
                endpointUrl = endpointUrl.replace('organisationId', organisationId);
                endpointUrl = endpointUrl.replace('licenceHolderId', licenceHolderId);
                var postRequestHelper = new CHMMApiPOSTRequestHelper();
                postRequestHelper.sendPOSTRequestWithoutAuthenticationToken(endpointUrl, '', '', false);
            })
        })
    } else {
        var postRequestHelper = new CHMMApiPOSTRequestHelper();
        postRequestHelper.sendPOSTRequestWithoutAuthenticationToken(endpointUrl, '', '', false);
    } 
})

Then("the response body array for linked/unlinked licence holder with name {string} should contain the following data:", function (name, datatable) {
    var testDataHandler = new CHMMApiTestDataHandlerHelper();

    if (name.toLowerCase() === 'newrandom' || name.toLowerCase() === 'random') {
        name = testDataHandler.getMcsManufacturerName();
    }
    cy.get('@requestResponse').then((response) => {
        const targetObject = response.body.find(obj => obj['name'] === name || obj['licenceHolderName'] === name);
        
        expect(targetObject).to.exist;
        
        datatable.hashes().forEach((element) => {
            switch (element.key) {
                case 'id':
                    expect(targetObject).to.have.property('id').that.matches(/^[a-f\d]{8}(-[a-f\d]{4}){4}[a-f\d]{8}$/i);
                    break;
                case 'licenceHolderId':
                    if (element.value.toLowerCase() === 'random' || element.value.toLowerCase() === 'newrandom') {
                        cy.get('@licenceHolderId').then((licenceHolderId) => {
                            expect(targetObject.licenceHolderId.toString()).to.equal(licenceHolderId);
                        })                        
                    } else {
                        expect(targetObject.licenceHolderId.toString()).to.equal(element.value.toString());
                    }
                    break;
                case 'licenceHolderName':
                    if (element.value.toLowerCase() === 'random' || element.value.toLowerCase() === 'newrandom') {
                        var testDataHandler = new CHMMApiTestDataHandlerHelper();
                        expect(targetObject.licenceHolderName).to.equal(testDataHandler.getMcsManufacturerName());
                    } else {
                        expect(targetObject.licenceHolderName).to.equal(element.value);
                    }
                    break;
                case 'organisationId':
                    if (element.value.toLowerCase() === 'newrandom' || element.value.toLowerCase() === 'random') {
                        cy.get('@organisationId').then((organisationId) => {
                            expect(targetObject.organisationId).to.equal(organisationId);
                        })
                    } else {
                        expect(targetObject.organisationId).to.equal(element.value);
                    }
                    break;
                case 'organisationName':
                    if (element.value.toLowerCase() === 'newrandom' || element.value.toLowerCase() === 'random') {
                        expect(targetObject.organisationName).to.equal(localStorage.getItem('OrganisationName'));                        
                    } else {
                        expect(targetObject.organisationName).to.equal(organisationName);
                    }
                    break;
                case 'mcsManufacturerId':
                    if (element.value.toLowerCase() === 'newrandom' || element.value.toLowerCase() === 'random') {
                        var testDataHandler = new CHMMApiTestDataHandlerHelper();
                        expect(targetObject.mcsManufacturerId).to.equal(testDataHandler.getMcsManufacturerId());                        
                    } else {
                        expect(targetObject.mcsManufacturerId).to.equal(testDataHandler.getMcsManufacturerId());
                    }
                    break;
                case 'name':
                    if (element.value.toLowerCase() === 'newrandom' || element.value.toLowerCase() === 'random') {
                        var testDataHandler = new CHMMApiTestDataHandlerHelper();
                        expect(targetObject.name).to.equal(testDataHandler.getMcsManufacturerName());                        
                    } else {
                        expect(targetObject.name).to.equal(testDataHandler.getMcsManufacturerName());
                    }
                    break;

                default:
                    if (element.value === 'null') {
                        expect(targetObject[element.key]).to.null;
                    } else {
                        expect(targetObject[element.key].toString()).to.equal(element.value);
                    }
                    
            }            
        })
    })
})



/*********************** Functions ******************/

/**
 * Create a JSON object for create licence holder POST request body
 * @param {*} datatable 
 * @returns 
 */
function createLicenceHolderJsonObject(datatable) {
    let licenceHoldersJson = {};
    var testDataHandler = new CHMMApiTestDataHandlerHelper();

    datatable.hashes().forEach((element) => {
        if (element.key.toLowerCase() === 'mcsmanufacturerid' && (element.value.toLowerCase() === 'random' || element.value.toLowerCase() === 'newrandom')) {
            // Generate an random number between 10 to 10000 for mcsManufacturerId
            testDataHandler.setMcsManufacturerId(Math.floor(Math.random() * (100000 - 10 + 1)) + 10);
            element.value = testDataHandler.getMcsManufacturerId();
        }

        if (element.key.toLowerCase() === 'mcsmanufacturername' && (element.value.toLowerCase() === 'random' || element.value.toLowerCase() === 'newrandom')) {
            testDataHandler.setMcsManufacturerName(`licenceHolderName_${testDataHandler.getMcsManufacturerId()}`);
            element.value = testDataHandler.getMcsManufacturerName();
        }

        licenceHoldersJson[element.key] = element.value;
    })

    return licenceHoldersJson;
}

/**
 * Function to create the API request json with 'licenceHolders' array to create Licence holders batch
 * @param {*} datatable 
 * @returns 
 */
function createLicenceHolderBatchJsonObject(datatable) {
    var testDataHandler = new CHMMApiTestDataHandlerHelper();
    let licenceHoldersJson;
    const rows = datatable.hashes();
    const groupedRows = groupBy(rows, 'id');
    // Construct API request body with 'licenceHolders' array
    licenceHoldersJson = {
        licenceHolders: Object.values(groupedRows).map((group) => {
            const groupObject = {};
            group.forEach((row) => {
                if (row.key.toLowerCase() === 'mcsmanufacturerid') {
                    if (row.value.toLowerCase() === 'same' || row.value.toLowerCase() === 'duplicate') {
                        row.value = testDataHandler.getMcsManufacturerId();
                    } else if (row.value.toLowerCase() === 'random' || row.value.toLowerCase() === 'newrandom') {
                        // Generate an random number between 10 to 10000 for mcsManufacturerId
                        testDataHandler.setMcsManufacturerId(Math.floor(Math.random() * (100000 - 10 + 1)) + 10);
                        row.value = testDataHandler.getMcsManufacturerId();
                    }
                }

                if (row.key.toLowerCase() === 'mcsmanufacturername') {
                    if (row.value.toLowerCase() === 'same' || row.value.toLowerCase() === 'duplicate') {
                        row.value = testDataHandler.getMcsManufacturerName();
                    } 
                    if (row.value.toLowerCase() === 'random' || row.value.toLowerCase() === 'newrandom') {
                        testDataHandler.setMcsManufacturerName(`licenceHolderName_${testDataHandler.getMcsManufacturerId()}`);
                        row.value = testDataHandler.getMcsManufacturerName();
                    }
                }

                groupObject[row.key] = row.value;
            });
            return groupObject;
        }),
    };
    return licenceHoldersJson;
}

/**
 * Function to POST to create licence holders one/batch
 * @param {*} endpointUrl 
 * @param {*} datatable 
 */
function postToCreateLicenceHolders(endpointUrl, datatable) {
    let licenceHoldersJson;
    if (endpointUrl.includes('batch')) {
        licenceHoldersJson = createLicenceHolderBatchJsonObject(datatable);
    } else {
        licenceHoldersJson = createLicenceHolderJsonObject(datatable);
    }     
    cy.log('Licence Holder Json = ' + JSON.stringify(licenceHoldersJson));

    var postRequestHelper = new CHMMApiPOSTRequestHelper();
    postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'application/json', JSON.stringify(licenceHoldersJson), false);
}

/**
 * Helper function to group an array of objects by a specified key
 * @param {*} array 
 * @param {*} key 
 * @returns 
 */
function groupBy(array, key) {
    return array.reduce((result, obj) => {
        const groupKey = obj[key];
        if (!result[groupKey]) {
            result[groupKey] = [];
        }

        result[groupKey].push(obj);
        return result;
    }, {});
}