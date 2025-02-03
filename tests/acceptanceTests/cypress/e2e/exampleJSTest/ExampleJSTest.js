/// <reference types="Cypress" />
import HomePage from "../pageObjects/HomePage"

describe('Example to Verify One login error message', function() {
    it('One login page', function() {
        const homePage = new HomePage()

        cy.visit(Cypress.env("URL"))

        Cypress.config('defaultCommandTimeout', 5000)

        homePage.getPrivacyPolicyDeclarationCheckBox().click()
        homePage.getPrivacyPolicyDeclarationContinueButton().click()
        cy.get('#sign-in-button').click()
        cy.get('#email').type('Test1@testmail.com')
        cy.get('form > .govuk-button').click()
        cy.get('.govuk-heading-l').should("have.text", "No GOV.UK One Login found")
        //cy.get('form > :nth-child(2)').should("have.text", "\n    There is no GOV.UK One Login for \n    test1@testmail.com\n")
        cy.get('form > :nth-child(2)').then(function(element)
        {
            const actualText = element.text()
            expect(actualText.includes("There is no GOV.UK One Login for")).to.be.true
            expect(actualText.includes("test1@testmail.com")).to.be.true
        })
    })
})