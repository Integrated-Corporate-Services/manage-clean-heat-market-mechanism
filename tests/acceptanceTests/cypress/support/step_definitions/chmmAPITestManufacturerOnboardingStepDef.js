/// <reference types="Cypress" />
chai.use(require('chai-uuid'));

import omit from "lodash/omit";
import isMatch from "lodash/isMatch"; 

import { createManufacturerOnboardingAPIJson } from "../../fixtures/chmmManufacturerApplicationAPIJsonData.js"
import { findValueByKey } from "../../fixtures/chmmManufacturerApplicationAPIJsonData.js"
import CHMMApiPOSTRequestHelper from "../helperClasses/chmmAPIPOSTRequestHelper.js"
import CHMMApiGETRequestHelper from "../helperClasses/chmmAPIGETRequestHelper.js";
import CHMMApiPUTRequestHelper from "../helperClasses/chmmAPIPUTRequestHelper.js";


/******************** Properties *****************/

let requestJsonBody = null;

/******************** Step defs ************************/

When("the user send a GET request to {string} with authentication token", function (endpointUrl) {
    var getRequestHelper = new CHMMApiGETRequestHelper();
    if (endpointUrl.toLowerCase().includes('organisationid')) {
        cy.get('@organisationId').then((organisationId) => {  
            getRequestHelper.sendGETRequestWithAuthenticationToken(endpointUrl.replace('organisationId', organisationId), false);      
        })
    } else {
        getRequestHelper.sendGETRequestWithAuthenticationToken(endpointUrl, false);
    } 

    if (endpointUrl.toLowerCase().includes('obligation') && endpointUrl.toLowerCase().includes('summary')) {
        cy.get('@requestResponse').then((response) => {
            cy.wrap(response.body).as('obligationSummaryResponseBody');
        })
    }    
})

When("the user send a GET request to {string} without authentication token", function (endpointUrl) {
    var getRequestHelper = new CHMMApiGETRequestHelper();
    if (endpointUrl.includes('organisationId')) {
        cy.get('@organisationId').then((organisationId) => {  
            getRequestHelper.sendGETRequestWithoutAuthenticationToken(endpointUrl.replace('organisationId', organisationId), false);      
        })
    } else {
        getRequestHelper.sendGETRequestWithoutAuthenticationToken(endpointUrl, false);
    } 
})

When("the user send a GET request to {string}", function (endpointUrl) {
    var getRequestHelper = new CHMMApiGETRequestHelper();
    getRequestHelper.sendGETRequestWithoutAuthenticationToken(endpointUrl,false);      
})

Then("the response should contain the following data:", function (dataString) {
    dataString = JSON.stringify(JSON.parse(dataString));
    cy.get('@requestResponse').then((response) => {
        expect(JSON.stringify(response.body)).contains(dataString)
    })
})

Then("the response status code should be {int}", function (expStatusCode) {
    cy.get('@requestResponse').then((response) => {
        expect(response.status).to.eq(expStatusCode);
    })
})

Then("the response body array should contain {int} items", function (expItemCount) {
    cy.get('@requestResponse').then((response) => {
        expect(response.body).to.be.an('array');
        expect(response.body).to.have.lengthOf(expItemCount);
    })
})

Then("the response body array should contain the following items:", function (datatable) {
    cy.get('@requestResponse').then((response) => {
        datatable.hashes().forEach((element) => {            
            var fileNamesList = element.item.split(';');
            // expect(response.body).to.deep.equal(fileNamesList);

            const jsonResponse = (arr) => JSON.stringify(arr.sort(), null, 2).replace(/\n/g, '');
            const responseJson = jsonResponse(response.body);
            const expectedJson = jsonResponse(fileNamesList);
            expect(responseJson).to.equal(expectedJson);
        })
    })
})

Then("the response body array should not contain the following items:", function (datatable) {
    cy.get('@requestResponse').then((response) => {
        datatable.hashes().forEach((element) => {
            var fileNamesList = element.item.split(';');
            fileNamesList.forEach((item) => {
                expect(response.body).to.not.include(item);
            })
        })
    })
})

Then("the response body should contain user id in the form of uuid", function () {
    cy.get('@organisationId').then((response) => {        
        expect(response).to.be.a.uuid();
    })
})

Then("the response body should contain {string}", function (expString) {
    cy.get('@requestResponse').then((response) => {
        expect(response.body.toString()).to.contain(expString);
    })
})

Then("the response body should contain uuid", function () {
    cy.get('@requestResponse').then((response) => {
        expect(response.body).to.have.property('id').that.matches(/^[a-f\d]{8}(-[a-f\d]{4}){4}[a-f\d]{8}$/i);
    })
})

Then("the response json body should contain the following data:", function (datatable) {   
    let responseKeyPath = null;
    cy.get('@requestResponse').then((response) => {
        cy.log("Response body = " + JSON.stringify(response.body));
        datatable.hashes().forEach((element) => {
            if(element.key.includes('.') || element.key.includes('[')){
                responseKeyPath = element.key.replace('[', '.').replace(']', '').split(".")
                if (responseKeyPath.length === 3) {
                    responseKeyPath = response.body[responseKeyPath[0]][responseKeyPath[1]][responseKeyPath[2]];
                } else {
                    responseKeyPath = response.body[responseKeyPath[0]][responseKeyPath[1]];
                } 
            } else {
                responseKeyPath = response.body[element.key];
            }
            
            if (element.value === 'true' || element.value === 'false') {
                expect(responseKeyPath).to.eq(element.value == 'true');
            } else if (element.key === 'detail' && element.value.includes('licenceHolderId')) {
                cy.get('@licenceHolderId').then((id) => {
                    element.value = element.value.replace('licenceHolderId', id);
                    expect(responseKeyPath.toString()).to.eq(element.value);
                })
            } else if (element.value === 'null') {
                expect(responseKeyPath).to.be.null;
            } else {                
                expect(responseKeyPath.toString()).to.eq(element.value);
            }
        })        
    })
})

Then("the response json body should contain {string} element array with the following data:", function (responseKeyPath, datatable) {
    cy.get('@requestResponse').then((response) => {
        var responseBodyElement = response.body[responseKeyPath];
        datatable.hashes().forEach((element) => {
            const foundObject = responseBodyElement.find((obj) => obj.hasOwnProperty(element.key));
            expect(foundObject[element.key].toString()).to.eq(element.value);
        })        
    })
})

Then("the response body array should contain the following data:", function (datatable) {
    cy.get('@requestResponse').then((response) => {
        datatable.hashes().forEach((element) => {
            const foundObject = response.body.find((obj) => obj.hasOwnProperty(element.key));
            expect(foundObject[element.key].toString()).to.eq(element.value);
        })        
    })
})

Then("the response body for quarterly boiler sales should contain the following data:", function (datatable) {
    cy.get('@requestResponse').then((response) => {
        datatable.hashes().forEach((element) => {
            const foundObject = response.body.find((obj) => obj['schemeYearQuarterId'] === element.schemeYearQuarterId);
            expect(foundObject[element.key].toString()).to.eq(element.value);
        })        
    })
})

When("the user send a POST request to {string} with authentication token and following data:", function (endpointUrl, dataString) {
    // dataString = JSON.stringify(JSON.stringify(dataString));
    cy.log("dataString = " +dataString);
    var postRequestHelper = new CHMMApiPOSTRequestHelper(); 
    postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'application/json', dataString, false);
})

Given("the user have a manufacturer onboarding application request body json with the following data:", function (datatable) {
    requestJsonBody = createManufacturerOnboardingAPIJson(datatable);
})

Given("the user has the manufacturer onboarding application json for the new organisation", function () {
    // Extract the new organisation id from the response body of the previous request that submited the application
    // Get the organisation details using the organisation Id
    getOrganisationDetails();
})

When("the admin user send a PUT request to {string} to approve the manufacturer onboarding application with authentication token with following comment:", function (endpointUrl, dataString) {    
    sendPUTrequestToApproveOnboardingApplication(endpointUrl, dataString);
})

When("the user send a PUT request to {string} to edit the organisation details with authentication token", function (endpointUrl,dataString) {    
    sendPUTrequestToEditManufacturerUser(endpointUrl,dataString);
})


When("the admin user send a POST request to {string} to upload approval files:",function (endpointUrl, dataString,filePath)  {   
    sendPOSTrequestToUploadApprovalFiles(endpointUrl, dataString,filePath);
})
       

When("the user send a PUT request to {string} to edit the organisation details with authentication token with following comment:", function (endpointUrl, dataString) {
    cy.get('@requestResponse').then((response) => {
        const data = new FormData();
        data.set("Comment", dataString);
        data.set("OrganisationDetailsJson", JSON.stringify(response.body)); 
        cy.log('OrganisationDetailsJson = ' + JSON.stringify(response.body)); 
        var putRequestHelper = new CHMMApiPUTRequestHelper();  
        putRequestHelper.sendPUTRequestWithAuthenticationToken(endpointUrl, 'multipart/form-data;', data, false); 
    })
})

When("the admin user send a PUT request to {string} to approve the manufacturer onboarding application without authentication token with following comment:", function (endpointUrl, dataString) {
    // Extract user id, SRO user id, address id from the response of GET request
    cy.get('@requestResponse').then((response) => {  
        const data =new FormData();
        data.set("Comment", dataString);
        data.set("OrganisationDetailsJson", JSON.stringify(response.body)); 
        var putRequestHelper = new CHMMApiPUTRequestHelper();  
        putRequestHelper.sendPUTRequestWithoutAuthenticationToken(endpointUrl, 'multipart/form-data;', data, false);   
    })
})

When("the user send a POST request to {string} with json body and authentication token to submit manufacturer onboarding application", function (endpointUrl) {
    sendPOSTRequestToSubmitOnboardingApplication(endpointUrl);    
})

When("the user send a POST request to {string} with json body", function (endpointUrl) {
    const data =new FormData();
    data.set("OrganisationDetailsJson", JSON.stringify(requestJsonBody));
    var postRequestHelper = new CHMMApiPOSTRequestHelper(); 
    postRequestHelper.sendPOSTRequestWithoutAuthenticationToken(endpointUrl, 'multipart/form-data;', data, false);
})

Given("the user edit the manufacturer onboarding application json with the following data:", function (datatable) {    
    // Update the json with the data from the feature file
    updateUserDetailsInManufacturerOnboardingApplicationJson(datatable);
})

Given("the user has an organisation created with the following data:", function (datatable) {
    // Create Json request body 
    requestJsonBody = createManufacturerOnboardingAPIJson(datatable);
    sendPOSTRequestToSubmitOnboardingApplication('/identity/organisations/onboard');    
    getOrganisationDetails();
    sendPUTrequestToApproveOnboardingApplication('/identity/organisations/approve', 'API Test comment admin approve');
})

Given("the admin user creates an organisation with the following data:", function (datatable) {
    requestJsonBody = createManufacturerOnboardingAPIJson(datatable);
    sendPOSTRequestToSubmitOnboardingApplication("/identity/organisations/onboard");
    // getOrganisationDetails();
    sendPUTrequestToApproveOnboardingApplication("/identity/organisations/approve", "Created by admin");
    sendPUTRequestToEditOrganisationSchemeParticipationFlag("/identity/organisations/edit/scheme-participation", false);
})

Given("the user submits a manufacturer onboarding application via API with the following data:", function (datatable) {
    requestJsonBody = createManufacturerOnboardingAPIJson(datatable);
    sendPOSTRequestToSubmitOnboardingApplication("/identity/organisations/onboard");
    getOrganisationDetails();
}) 

Then("the response body array should not be empty", function () {
    cy.get('@requestResponse').then((response) => {
        expect(response.body).to.be.an('array').and.not.to.be.empty;
    })    
})

Then("all the items in the response body array should contain the following data:", function (datatable) {
    cy.get('@requestResponse').then((response) => {
        datatable.hashes().forEach((row) => {
            response.body.forEach((element) => {
                if (row.value === 'null') {
                    expect(element[row.key]).to.be.null;
                } else {
                    expect(element[row.key]).to.equal(row.value);
                }                
            })
        })
    })
})

Then("the response body array for organisations should contain the following json:", function (jsonString) {
    const expectedJsonObject = JSON.parse(jsonString);
    cy.get('@organisationId').then((orgId) => {
            
        if (expectedJsonObject.id.toLowerCase() === 'newrandom' || expectedJsonObject.id.toLowerCase() === 'random')  {
            expectedJsonObject.id = orgId;
        } 
        if (expectedJsonObject.name.toLowerCase() === 'newrandom' || expectedJsonObject.name.toLowerCase() === 'random')  {
            expectedJsonObject.name = localStorage.getItem("OrganisationName");
        }
        if (expectedJsonObject.email === 'newrandom' || expectedJsonObject.email === 'random' || expectedJsonObject.email === 'newRandom' || expectedJsonObject.email === 'Random')  {
            expectedJsonObject.email = localStorage.getItem("mfrUserEmailId");
        }      

        cy.log("expectedJsonObject = " + JSON.stringify(expectedJsonObject));
        
        cy.get('@requestResponse').then((response) => {

            const isMatch = response.body.some((element) => {
                return Object.entries(expectedJsonObject).every(([key, value]) => {
                    return Cypress._.isEqual(element[key], value);
                  });
            });
            expect(isMatch, 'Expected JSON object not found in the response body array').to.be.true;
        });   
    });
})

Then("the response body array for the manufacturer users should contain the following json:", function (datatable) {
    cy.get('@requestResponse').then((response) => {
        datatable.hashes().forEach((element) => {
            if (element.key.toLowerCase() === 'email' && (element.value.toLowerCase() === 'newrandom' || element.value.toLowerCase() === 'random')) {
                element.value = localStorage.getItem("mfrUserEmailId").toLowerCase();
            }
            const foundObject = response.body.find((obj) => obj.hasOwnProperty(element.key));
            expect(foundObject[element.key].toString()).to.eq(element.value);
        })        
    })
})

Then("the response body array for audit items should contain the following data:", function (datatable) {    
    cy.get('@requestResponse').then((apiResponse) => {
        datatable.rawTable.slice(1).forEach((row) => {
            const property = row[0];
            const expectedValue = row[1];
        
            let found = false;
            apiResponse.body.forEach((responseObj) => {
              let actualValue = responseObj[property];
              if (property.includes("auditItemRows")) {
                const [label, expectedLabelValue] = expectedValue.split(":");
                actualValue = responseObj.auditItemRows.find(
                  (item) => item.label === label.trim()
                ).simpleValue;
                if (actualValue === expectedLabelValue.trim()) {
                  found = true;
                }
              } else {
                if (actualValue === expectedValue) {
                  found = true;
                }
              }
            });
        
            if (!found) {
              throw new Error(`Expected ${property}: ${expectedValue}`);
            }
          });
    });
})

Then("the response body array has json object with key {string} and value {string} should contain the following data:", function (key, value, datatable) {
    let jsonObject = null;
    let responseKeyPath = null;
    cy.get('@requestResponse').then((response) => {
        // cy.get('@licenceHolderId').then((licenceHolderId) => { 
            
            // Iterate over each object in the array to find the one that matches the given key-value pair
            for (let i = 0; i < response.body.length; i++) {
                if (response.body[i][key] === value) {
                    jsonObject = response.body[i];
                    break;
                }
            }

            datatable.hashes().forEach((element) => {
                if(element.key.includes('.') || element.key.includes('[')){
                    responseKeyPath = element.key.replace('[', '.').replace(']', '').split(".")
                    if (responseKeyPath.length === 3) {
                        responseKeyPath = jsonObject[responseKeyPath[0]][responseKeyPath[1]][responseKeyPath[2]];
                    } else {
                        responseKeyPath = jsonObject[responseKeyPath[0]][responseKeyPath[1]];
                    } 
                } else {
                    responseKeyPath = jsonObject[element.key];
                }
                debugger;

                if (element.value === 'null') {
                    expect(responseKeyPath).to.be.null;
                } else if (element.key.includes('simpleValue') && element.value.toLowerCase() === 'licenceholderid') {
                    cy.get('@licenceHolderId').then((id) => {
                        element.value = id;
                        expect(responseKeyPath).to.eq(element.value);
                    })                       
                } else {
                    expect(responseKeyPath).to.eq(element.value);
                }            
            })
        // })
    })
})

When("the user send a PUT request to {string} to set scheme participation flag to {string} with authentication token", function (endpointUrl, flag) {
    sendPUTRequestToEditOrganisationSchemeParticipationFlag(endpointUrl, flag);
})


/**************** Methods ***************/

/**
 * Send a POST request to submit manufacturer onboarding application
 * @param {*} endpointUrl 
 */
function sendPOSTRequestToSubmitOnboardingApplication(endpointUrl) {
    const data = new FormData();
    data.set("OrganisationDetailsJson", JSON.stringify(requestJsonBody));
    var postRequestHelper = new CHMMApiPOSTRequestHelper();
    postRequestHelper.sendPOSTRequestWithAuthenticationToken(endpointUrl, 'multipart/form-data;', data, false); 
    cy.get('@requestResponse').then((response) => {
        const bodyString = Cypress.Blob.arrayBufferToBinaryString(response.body);
        cy.wrap(JSON.parse(bodyString)['id']).as('organisationId');
        cy.log('Organisation Id = ' + JSON.parse(bodyString)['id']);        
    })
}

/**
 * Send a PUT request to approve the manufacturer onboarding application
 * @param {*} endpointUrl 
 * @param {*} dataString 
 */
function sendPUTrequestToApproveOnboardingApplication(endpointUrl, dataString) {
    cy.get('@organisationId').then((organisationId) => {
        const data =new FormData();
        data.set("Comment", dataString);
        data.set("OrganisationId", organisationId);  
        var putRequestHelper = new CHMMApiPUTRequestHelper();  
        putRequestHelper.sendPUTRequestWithAuthenticationToken(endpointUrl, 'multipart/form-data;', data, false);
    })
}

function sendPUTrequestToEditManufacturerUser(endpointUrl, dataString) {
    cy.log("dataString")
    cy.get('@organisationId').then((organisationId) => {
// Parse the JSON string into an object

        const jsonData = JSON.parse(dataString.trim());
       // let data = {};


        cy.get('@requestResponse').then((response) => {
            cy.log("Full response body: " + JSON.stringify(response.body));
           
           const firstItem = response.body[0]; // Ensure the array is not empty
           const idFromResponse = firstItem?.id; 

           cy.log("Extracted Id from response: " + idFromResponse);
           
            //data["OrganisationId"] = organisationId;
            //data["id"] = idFromResponse;
            //data["name"]= dataString;
            //data["jobTitle"]= dataString;
            //data["telephoneNumber"]= dataString;
            jsonData.organisationId=organisationId;
            jsonData.id=idFromResponse;

            var putRequestHelper = new CHMMApiPUTRequestHelper();
            putRequestHelper.sendPUTRequestWithAuthenticationToken(endpointUrl, 'application/json', jsonData, false);
        });
    });
}

/*function sendPUTrequestToEditManufacturerUser(endpointUrl, dataString) {
   cy.get('@organisationId').then((organisationId) => {
        
        const data =new FormData();
        data.set("OrganisationId", dataString);
        data.set("id", dataString);
        data.set("name", dataString);
        data.set("jobTitle", dataString);
        data.set("telephoneNumber", dataString);
        cy.get('@requestResponse').then((response) => {
            cy.log("requestJsonBody = " + JSON.stringify(response.body));
            requestJsonBody = JSON.stringify(response.body);

        })
        var putRequestHelper = new CHMMApiPUTRequestHelper();  
        putRequestHelper.sendPUTRequestWithAuthenticationToken(endpointUrl, 'multipart/form-data;', data, false);
    })

}*/


    function sendPOSTrequestToUploadApprovalFiles(endpointUrl, dataString, filePath = null) {
        // Access the organisation ID stored as a Cypress alias
        cy.get('@organisationId').then((organisationId) => {
            // Create a FormData object
            const data = new FormData();
            data.set("Comment", dataString);
            data.set("OrganisationId", organisationId);
    
            // Check if a file path is provided
            if (filePath) {
                // Use Cypress fixture to load the file
                cy.fixture(filePath, 'binary').then((fileContent) => {
                    // Convert the file content to a Blob
                    const blob = Cypress.Blob.binaryStringToBlob(fileContent);
    
                    // Append the file to the FormData
                    data.append("File", blob, filePath); // File key and filename
                });
            }
    
            // After handling the file, send the POST request
            cy.then(() => {
                // Use Cypress `cy.request` to send the POST request
                cy.request({
                    method: 'POST',              // Change to POST method
                    url: endpointUrl,            // API endpoint URL
                    body: data,                  // FormData object
                    headers: {
                        'Content-Type': 'multipart/form-data;', // Set Content-Type for multipart data
                    },
                    failOnStatusCode: false      // Prevent failure on non-2xx status codes (optional)
                }).then((response) => {
                    // Log the response or handle assertions
                    cy.log('Response:', response);
                    expect(response.status).to.be.oneOf([200, 201]); 
                });
            });
        });
    }

/**
 * Send a PUT request to edit the organisation scheme participation flag
 * @param {*} endpointUrl 
 * @param {*} flag 
 */
function sendPUTRequestToEditOrganisationSchemeParticipationFlag(endpointUrl, flag) {
    var requestJsonBody = {
        "organisationId": "orgId",
        "isNonSchemeParticipant": true
      }
    cy.get('@organisationId').then((orgId) => {
        requestJsonBody.organisationId = orgId;
        requestJsonBody.isNonSchemeParticipant = flag;
        cy.log("requestJsonBody = " + requestJsonBody);
        var putRequestHelper = new CHMMApiPUTRequestHelper();  
        putRequestHelper.sendPUTRequestWithAuthenticationToken(endpointUrl, 'application/json-patch+json;', requestJsonBody, false);
    })
}

/**
 * Get the organisation details using the organisation Id from the response body of the POST request for Manuafacturer onboarding apllication 
 */
function getOrganisationDetails() {   
    var getRequestHelper = new CHMMApiGETRequestHelper(); 
    cy.get('@organisationId').then((organisationId) => {   
        getRequestHelper.sendGETRequestWithAuthenticationToken("/identity/organisations/" + organisationId, false);     
    })
    cy.get('@requestResponse').then((response) => {
        cy.log("requestJsonBody = " + JSON.stringify(response.body));
        requestJsonBody = JSON.stringify(response.body);
    })
}

/**
 * Update User details in manufacturer onboarding application Json
 * @param {*} datatable 
 */
function updateUserDetailsInManufacturerOnboardingApplicationJson(datatable) {
    cy.get('@requestResponse').then((response) => {
        var org = response.body;
        delete org.status;
        datatable.hashes().forEach((element) => {
            set(org, element.key, element.value);
        })        
    })
}

/**
 * Helper method to extract the elements of Json object
 * @param {*} obj 
 * @param {*} path 
 * @param {*} value 
 * @returns 
 */
const helper = (obj, path, value) => {
    // get the current and the remaining keys from the path
    let [current, ...rest] =  path;
  
    // if there are more keys
    // add the value as an object or array
    // depending upon the typeof key
    if(rest.length > 0){
        // if there is no key present
        // create a new one
        if(!obj[current]){
          // if the key is numeric
          // add an array
          // else add an object
          const isNumber = `${+rest[0]}` === rest[0];
          obj[current] = isNumber ? [] : {};
        }
            
        // recurisvely update the remaining path
        // if the last path is not of object type
        // but key is then
        // create an object or array based on the key
        // and update the value
        if(typeof obj[current] !== 'object'){
          // determine if the key is string or numeric 
          const isNumber = `${+rest[0]}` === rest[0];
          obj[current] = helper(isNumber ? [] : {}, rest, value)
        }
        // else directly update value
        else{
          obj[current] = helper(obj[current], rest, value);
        }
    }
    // else directly assing the value to the key
    else{
      obj[current] = value;
    }
  
    // return the updated obj
    return obj;
 }

 /**
  * Set values in the given JSON object
  * @param {*} obj 
  * @param {*} path 
  * @param {*} value 
  */
const set = (obj, path, value) => {
   let pathArr = path;
  
   // if path is of string type
   // replace the special characters
   // and split the string on . to get the path keys array
   if(typeof path === 'string'){
     pathArr = path.replace('[', '.').replace(']', '').split(".");
   }
   
   // use the helper function to update
   helper(obj, pathArr, value);
};
