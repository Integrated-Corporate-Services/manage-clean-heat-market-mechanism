/// <reference types="Cypress" />
import CreditTransferPage from "../../e2e/pageObjects/chmmCreditTransferPage";
import MenuPage from "../../e2e/pageObjects/chmmMenuPage";

When("the user is on credit transfer page with {string}", function (date) {
    const creditTransferPage = new CreditTransferPage();
    creditTransferPage.navigateToCreditTransferPage(date);
    cy.wait(5000);
})


When("the user clicks on credit transfer button",function () {
    const creditTransferPage = new CreditTransferPage();
    creditTransferPage.clickOnCreditTransferButton();
    cy.wait(5000);
})

When("the user selects the organisation {string} from the drop down",function (org) {
    const creditTransferPage = new CreditTransferPage();
    creditTransferPage.selectNewlyCreatedOrganisation(org);
    cy.wait(5000);
})

When("the user enter number of credits {string}",function (value) {
    const creditTransferPage = new CreditTransferPage();
    creditTransferPage.enterNumberOfCredits(value);
    cy.wait(5000);
})

When("the admin user clicks on amend credit balance button",function () {
    const creditTransferPage = new CreditTransferPage();
    creditTransferPage.clickOnAmendCreditBalance();
    cy.wait(5000);
})


When("the user clicks on adding credit radio button on amend credit balance page",function () {
    const creditTransferPage = new CreditTransferPage();
    creditTransferPage.clickOnAddingCreditRadioButton();
    cy.wait(5000);
})

When("the user clicks on removing credit radio button on amend credit balance page",function () {
    const creditTransferPage = new CreditTransferPage();
    creditTransferPage.clickOnRemovingCreditRadioButton();
    cy.wait(5000);
})

When("the user enter number of credits  on amend credit balance page {string}",function (value) {
    const creditTransferPage = new CreditTransferPage();
    creditTransferPage.enterNumberOfCreditsOnAmendCreditPage(value);
    cy.wait(5000);
})


When("the user click on amend credit balance button",function () {
    const creditTransferPage = new CreditTransferPage();
    creditTransferPage.clickOnAmendCreditBalance();
    cy.wait(5000);
}) 


When("the user click on confirm and complete transfer button",function () {
    const creditTransferPage = new CreditTransferPage();
    creditTransferPage.clickOnConfirmAndCOmpleteTransferButton();
    cy.wait(3000);
})  


When("the user clicks on change link for editing on check your answer page {string}", function(link)
{
    const creditTransferPage = new CreditTransferPage();
    creditTransferPage.ClickOnChangeLinkforCheckYourAnswer(link);
    cy.wait(2000);
})

When("the user clicks on summary link",function () {
    const menuPage = new MenuPage();
    menuPage.clickOnSummaryLink();
    cy.wait(3000);
})  

Then("the user should see below details on check credit balance calculation section", function (datatable) {
    const creditTransferPage = new CreditTransferPage();
    creditTransferPage.CheckValueOnCreditBalanceCalculationSection(datatable);
})
