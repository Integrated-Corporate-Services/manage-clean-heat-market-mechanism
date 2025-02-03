/// <reference types="Cypress" />
 import CHMMManufacturerOnboardingCheckApplicationPage from "../../e2e/pageObjects/chmmManufacturerOnboardingCheckApplicationPage";

 When("the user clicks on change link for editing manufacturer {string}", function(link)
{
    const chmmManufacturerOnboardingCheckApplicationPage = new CHMMManufacturerOnboardingCheckApplicationPage();
    chmmManufacturerOnboardingCheckApplicationPage.ClickOnChangeLinkforManufacturer(link);
    cy.wait(2000);
})

Given("the user clicks on change link for editing manufacturer card {string}", function(link)
{
    const chmmManufacturerOnboardingCheckApplicationPage = new CHMMManufacturerOnboardingCheckApplicationPage();
    chmmManufacturerOnboardingCheckApplicationPage.ClickOnChangeLinkforManufacturerCard(link);
    cy.wait(2000);
})