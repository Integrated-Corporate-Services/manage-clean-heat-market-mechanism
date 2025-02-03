/// <reference types="Cypress" />
const uuid = require('uuid');
const chaiUuid = require('chai-uuid');

import { createAnnualBoilerSalesAPIRequestJson } from "../../fixtures/chmmManufacturerApplicationAPIJsonData.js"
import CHMMApiPOSTRequestHelper from "../helperClasses/chmmAPIPOSTRequestHelper.js"
import CHMMApiGETRequestHelper from "../helperClasses/chmmAPIGETRequestHelper.js";
import CHMMApiTestDataHandlerHelper from "../helperClasses/chmmAPITestDataHandlerHelper.js";


/******************** Properties *****************/

let schemeYearId = null;
// let requestJsonBody = null;

/******************** Step defs ************************/

When("the user send a POST request to {string} to upload {string} file for {string} with authentication token", function (endpointUrl, fileNames, parameterName) {
    // Read all files when an array of file names are passed
    // Create a function to handle file uploads
    const uploadFiles = (fileNames) => {
        const formData = new FormData();
        var fileNamesList = fileNames.split(';');
        return cy.wrap(fileNamesList).each((fileName) => {
        cy.readFile(`cypress/fixtures/${fileName}`, 'binary').then((fileContent) => {
            const blob = Cypress.Blob.binaryStringToBlob(fileContent, 'text/plain');
            formData.append(parameterName, blob, fileName);
        });
        }).then(() => {
        // Return the FormData to be used in the next 'then' block
        return formData;
        });
    };

    uploadFiles(fileNames).then((formData) => {
        if (endpointUrl.includes('organisationId')) {
            cy.get('@organisationId').then((organisationId) => { 
                endpointUrl = endpointUrl.replace('organisationId', organisationId);
                var postRequestHelper = new CHMMApiPOSTRequestHelper();
                postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'multipart/form-data;', formData, false);
            }) 
        } else {
            var postRequestHelper = new CHMMApiPOSTRequestHelper();
            postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'multipart/form-data;', formData, false);
        }               
    })    
})

When("the user send a POST request to {string} to upload {string} file for {string} without authentication token", function (endpointUrl, fileNames, parameterName) {
    // Read all files when an array of file names are passed
    // Create a function to handle file uploads
    const uploadFiles = (fileNames) => {
        const formData = new FormData();
        var fileNamesList = fileNames.split(';');
        return cy.wrap(fileNamesList).each((fileName) => {
        cy.readFile(`cypress/fixtures/${fileName}`, 'binary').then((fileContent) => {
            const blob = Cypress.Blob.binaryStringToBlob(fileContent, 'text/plain');
            formData.append(parameterName, blob, fileName);
        });
        }).then(() => {
        // Return the FormData to be used in the next 'then' block
        return formData;
        });
    };

    uploadFiles(fileNames).then((formData) => {
        if (endpointUrl.includes('organisationId')) {
            cy.get('@organisationId').then((organisationId) => { 
                endpointUrl = endpointUrl.replace('organisationId', organisationId);
                var postRequestHelper = new CHMMApiPOSTRequestHelper();
                postRequestHelper.sendPOSTRequestWithoutAuthenticationToken(endpointUrl, 'multipart/form-data;', formData, false);
            }) 
        } else {
            var postRequestHelper = new CHMMApiPOSTRequestHelper();
            postRequestHelper.sendPOSTRequestWithoutAuthenticationToken(endpointUrl, 'multipart/form-data;', formData, false);
        }               
    })       
})

When("the user send a POST request to {string} to delete the uploaded {string} file with authentication token",
    function (endpointUrl, fileName) {
      // Function to create FormData with the filename
      const createFormData = (fileName) => {
        const formData = new FormData();
        formData.append('file', fileName); // Replace 'file' with the field name expected by your API
        return formData;
      };
  
      // Create FormData with the filename
      const formData = createFormData(fileName);
  
      // Handle dynamic endpointUrl replacement and send the request
      if (endpointUrl.includes('organisationId')) {
        cy.get('@organisationId').then((organisationId) => {
          endpointUrl = endpointUrl.replace('organisationId', organisationId);
  
          // Use the helper to send the request
          const postRequestHelper = new CHMMApiPOSTRequestHelper();
          postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, null, formData, false);
        });
      } else {
        // Directly send the request
        const postRequestHelper = new CHMMApiPOSTRequestHelper();
        postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, null, formData, false);
      }
    }
  );

When("the user send a GET request to {string} to fetch files with authentication token", function (endpointUrl) {
    if (endpointUrl.includes('organisationId')) {
        cy.get('@organisationId').then((organisationId) => {
            endpointUrl = endpointUrl.replace('organisationId', organisationId);
            var getRequestHelper = new CHMMApiGETRequestHelper();
            getRequestHelper.sendGETRequestWithAuthenticationToken(endpointUrl, false);
        })        
    } else {
        var getRequestHelper = new CHMMApiGETRequestHelper();
        getRequestHelper.sendGETRequestWithAuthenticationToken(endpointUrl, false);
    }        
})

When("the user send a GET request to {string} to fetch files without authentication token", function (endpointUrl) {
    if (endpointUrl.includes('organisationId')) {
        cy.get('@organisationId').then((organisationId) => {
            endpointUrl = endpointUrl.replace('organisationId', organisationId);
            var getRequestHelper = new CHMMApiGETRequestHelper();
            getRequestHelper.sendGETRequestWithoutAuthenticationToken(endpointUrl, false);
        })        
    } else {
        var getRequestHelper = new CHMMApiGETRequestHelper();
        getRequestHelper.sendGETRequestWithoutAuthenticationToken(endpointUrl, false);
    } 
})

When("the user send a POST request to {string} to delete the following file with authentication token:", function (endpointUrl, dataString) {
    dataString = JSON.stringify(JSON.parse(dataString));
    if (endpointUrl.includes('organisationId')) {
        cy.get('@organisationId').then((organisationId) => {
            endpointUrl = endpointUrl.replace('organisationId', organisationId);
            var postRequestHelper = new CHMMApiPOSTRequestHelper();
            postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'application/json-patch+json', dataString, false);
        })        
    } else {
        var postRequestHelper = new CHMMApiPOSTRequestHelper();
        postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'application/json-patch+json', dataString, false);
    }      
})

When("the user send a POST request to {string} to delete the following file without authentication token:", function (endpointUrl, dataString) {
    dataString = JSON.stringify(JSON.parse(dataString));
    if (endpointUrl.includes('organisationId')) {
        cy.get('@organisationId').then((organisationId) => {
            endpointUrl = endpointUrl.replace('organisationId', organisationId);
            var postRequestHelper = new CHMMApiPOSTRequestHelper();
            postRequestHelper.sendPOSTRequestWithoutAuthenticationToken(endpointUrl, 'application/json-patch+json', dataString, false);
        })        
    } else {
        var postRequestHelper = new CHMMApiPOSTRequestHelper();
        postRequestHelper.sendPOSTRequestWithoutAuthenticationToken(endpointUrl, 'application/json-patch+json', dataString, false);
    }
})

When("the user send a GET request to {string} to download files with authentication token", function (endpointUrl) {
    if (endpointUrl.includes('organisationId')) {
        cy.get('@organisationId').then((organisationId) => {
            endpointUrl = endpointUrl.replace('organisationId', organisationId);
            var getRequestHelper = new CHMMApiGETRequestHelper();
            getRequestHelper.sendGETRequestWithAuthenticationToken(endpointUrl, false);
        })        
    } else {
        var getRequestHelper = new CHMMApiGETRequestHelper();
        getRequestHelper.sendGETRequestWithAuthenticationToken(endpointUrl, false);
    } 
})

When("the user send a GET request to {string} to download files without authentication token", function (endpointUrl) {
    if (endpointUrl.includes('organisationId')) {
        cy.get('@organisationId').then((organisationId) => {
            endpointUrl = endpointUrl.replace('organisationId', organisationId);
            var getRequestHelper = new CHMMApiGETRequestHelper();
            getRequestHelper.sendGETRequestWithoutAuthenticationToken(endpointUrl, false);
        })        
    } else {
        var getRequestHelper = new CHMMApiGETRequestHelper();
        getRequestHelper.sendGETRequestWithoutAuthenticationToken(endpointUrl, false);
    }
})

// When("the user send a POST request to {string} to submit/approve {string} boiler sales figures with authentication token and following data:", function (endpointUrl, submissionType, datatable) {
When("the user send a POST request to {string} to submit/approve boiler sales figures with date {string}, authentication token and following data:", function (endpointUrl, date, datatable) {
    let boilerSalesJsonData = {};
    datatable.hashes().forEach((element) => {
        boilerSalesJsonData[element.key] = element.value;
    })

    if (boilerSalesJsonData['organisationId'].toLowerCase() === 'random' || boilerSalesJsonData['organisationId'].toLowerCase() === 'newrandom') {
        cy.get('@organisationId').then((organisationId) => {
            cy.log('organisationId = ' + organisationId);
            boilerSalesJsonData['organisationId'] = organisationId;
            endpointUrl = endpointUrl.replace('organisationId', organisationId);
            cy.log('boilerSalesJsonData = ' + JSON.stringify(boilerSalesJsonData));
            var postRequestHelper = new CHMMApiPOSTRequestHelper();
            postRequestHelper.sendPOSTRequestWithAuthenticationTokenAndDateTimeOverride(endpointUrl, 'application/json', date, JSON.stringify(boilerSalesJsonData), false);
        })
    } else {
        cy.log('boilerSalesJsonData = ' + JSON.stringify(boilerSalesJsonData));
        var postRequestHelper = new CHMMApiPOSTRequestHelper();
        postRequestHelper.sendPOSTRequestWithAuthenticationTokenAndDateTimeOverride(endpointUrl, 'application/json', date, JSON.stringify(boilerSalesJsonData), false);
    }
})

When("the user send a POST request to {string} to submit/approve {string} boiler sales figures without authentication token and following data:", function (endpointUrl, submissionType, datatable) {
    let boilerSalesJsonData = {};
    datatable.hashes().forEach((element) => {
        boilerSalesJsonData[element.key] = element.value;
    })

    if (boilerSalesJsonData['organisationId'].toLowerCase() === 'random' || boilerSalesJsonData['organisationId'].toLowerCase() === 'newrandom') {
        cy.get('@organisationId').then((organisationId) => {
            cy.log('organisationId = ' + organisationId);
            boilerSalesJsonData['organisationId'] = organisationId;
            endpointUrl = endpointUrl.replace('organisationId', organisationId);
            cy.log('boilerSalesJsonData = ' + JSON.stringify(boilerSalesJsonData));
            var postRequestHelper = new CHMMApiPOSTRequestHelper();
            postRequestHelper.sendPOSTRequestWithoutAuthenticationToken(endpointUrl, 'application/json', JSON.stringify(boilerSalesJsonData), false);
        })
    } else {
        cy.log('boilerSalesJsonData = ' + JSON.stringify(boilerSalesJsonData));
        var postRequestHelper = new CHMMApiPOSTRequestHelper();
        postRequestHelper.sendPOSTRequestWithoutAuthenticationToken(endpointUrl, 'application/json', JSON.stringify(boilerSalesJsonData), false);
    }
})

When("the user send a GET request to {string} to fetch boiler sales figures with authentication token", function (endpointUrl) {
    cy.get('@organisationId').then((organisationId) => {
        endpointUrl = endpointUrl.replace('organisationId', organisationId);
        var getRequestHelper = new CHMMApiGETRequestHelper();
        getRequestHelper.sendGETRequestWithAuthenticationToken(endpointUrl, false);
    })    
})

Then("the response json body for annual boiler sales should contain the following data:", function (datatable) {
    cy.get('@requestResponse').then((response) => {
        cy.log("Response body = " + JSON.stringify(response.body));        
        datatable.hashes().forEach((element) => {                        
            if (element.key.toLowerCase() === 'organisationid' && (element.value.toLowerCase() === 'newrandom' || element.value.toLowerCase() === 'random')) {
                cy.get('@organisationId').then((organisationId) => {
                    element.value = organisationId;
                    expect(response.body[element.key].toString()).to.eq(element.value);
                })
            }  else {
                expect(response.body[element.key].toString()).to.eq(element.value);
            }            
        })        
    })
})

Then("the response body array for quarterly boiler sales for {string} quarter should contain the following data:", function(schemeYearQuarterId, datatable) {
    cy.get('@requestResponse').then((response) => {
        const responseBody = response.body;
        const targetObject = responseBody.find(obj => obj['schemeYearQuarterId'] === schemeYearQuarterId);

        expect(targetObject).to.exist;
        datatable.hashes().forEach((element) => {
            if (element.key.toLowerCase() === 'organisationid' && (element.value.toLowerCase() === 'newrandom' || element.value.toLowerCase() === 'random')) {
                cy.get('@organisationId').then((organisationId) => {
                    expect(targetObject.organisationId).to.equal(organisationId);
                })
            } else {
                expect(targetObject[element.key].toString()).to.equal(element.value);
            }
            
        })
    })
})