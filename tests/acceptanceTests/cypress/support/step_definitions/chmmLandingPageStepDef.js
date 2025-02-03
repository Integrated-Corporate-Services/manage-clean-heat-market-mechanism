/// <reference types="Cypress" />
require('@cypress/xpath');
import LoginPage from "../../e2e/pageObjects/chmmLoginPage";


Given("the user navigates to the CHMM landing page", function () {
    // Remove the below code block once exception error is fixed in One Login
    cy.on('uncaught:exception', (err, runnable) => {
        return false
    })

    cy.visit(Cypress.env("URL"));
});

Then("the user should see {string} message on the page", function (expText) {
    Cypress.config('defaultCommandTimeout', 7000);
    const loginPage = new LoginPage();
    loginPage.elements.PageBody().should('contain.text', expText);
});

Then("the user should not see {string} message on the page", function (expText) {
    Cypress.config('defaultCommandTimeout', 7000);
    const loginPage = new LoginPage();
    loginPage.elements.PageBody().should('not.contain.text', expText);
});

Then("the user should see the following text on the page:", function (datatable) {
    datatable.hashes().forEach(element => {
        const loginPage = new LoginPage();
        loginPage.elements.PageBody().should('contain.text', element.text);
    });
});
