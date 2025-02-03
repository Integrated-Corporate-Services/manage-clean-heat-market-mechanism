/// <reference types="Cypress" />
import omit from "lodash/omit";
import isEqual from "lodash/isEqual"; 
var authToken;


Given("I send a GET request to {string}", function(endpointUrl) {
      // cy.request('GET','https://postman-echo.com/get?foo1=bar1&foo2=bar2');
  cy.request('GET',Cypress.env("APIURL")+endpointUrl).as('requestResponse');
      })

Then  ("I check response", function(dataTable){
            cy.get('@requestResponse').then((response)=>{
              expect(response.status).to.eq(200);
              expect(response.body.args.foo1).to.eq(dataTable.rawTable[1][1])
               expect(response.body.args.foo2).to.eq(dataTable.rawTable[2][1])
            })
        }) 



Given("I have authentication token", ()=> {
   cy.request({ method:'POST', url: Cypress.env('url2')+'/api-clients/', headers:{'Content-type': 'application/json'},
         body:{
            clientName: 'Triadgroup', 
            clientEmail: Math.random().toString(5).substring(2)+"@gmail.com"
         }
      }).then((response)=>{
         authToken=response.body.accessToken;
             })
             cy.log("authToken = " + authToken)
    });

When("I send POST request to {string} endpoint:", function(endpointUrl,dataString){
   cy.log("Body ="+dataString)   
   cy.request({
         method:'POST',
         url: Cypress.env('url2')+endpointUrl,
         headers: {
                  'Content-type': 'application/json',
                  'Authorization': 'Bearer '+authToken
         },
            body:dataString
        }).as('requestResponse'); 
       })
  
Then ('I check post response', function(dataString){
    cy.get('@requestResponse').then((response)=>{
           var result= isEqual(omit(response.body, 'orderId'), omit(JSON.parse(dataString), 'orderId'))
              cy.log(result)
              })
        }) 




Given ('I send POST request',function (dataString){
   cy.request({ method:'POST', url: Cypress.env('url3'),
   body:dataString
}).as('requestResponse');
    })

Then ("I check post request response", function(){
   cy.get('@requestResponse').then((response)=>{
     expect(response.status).to.eq(201);
    })
   })

When ('I send PUT request to {string}', function(endpointUrl) {
      cy.fixture('putSample').then((data)=>
      {
         const requestBody=data;
        cy.request({ method:'PUT', url: Cypress.env('url3')+endpointUrl,
         body:requestBody
      })
      }).as('requestResponse')
   })

Then ("I check put request response", function(dataString){
      cy.get('@requestResponse').then((response)=>{
       expect(response.status).to.eq(200);
       var result= isEqual((response.body), (JSON.parse(dataString)))
          cy.log(result)
        
      })
      })



Given ("I send DELETE request {string}", function(endpointUrl) {
        cy.request({ 
        method:'DELETE', 
        url: Cypress.env('url3')+endpointUrl,
      })
    .as('requestResponse');
      })
  
  
Then ("I check delete response", function(){
        cy.get('@requestResponse').then((response)=>{
        expect(response.status).to.eq(200);
        })
      })






