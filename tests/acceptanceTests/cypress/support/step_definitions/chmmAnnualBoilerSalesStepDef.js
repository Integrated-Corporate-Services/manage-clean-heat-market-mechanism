/// <reference types="Cypress" />
import MenuPage from "../../e2e/pageObjects/chmmMenuPage";
import AnnualBoilerSalesSummaryPage from "../../e2e/pageObjects/chmmAnnualBoilerSalesSummaryPage";

/************************ Step defs ******************/

When("the user navigates to annual boiler sales summary page", function () {
    const annualBoilerSalesSummaryPage = new AnnualBoilerSalesSummaryPage();
    annualBoilerSalesSummaryPage.navigateToAnnualBoilerSalesPage();
    cy.wait(5000);
})

Given("the user is on boiler sales summary screen with status {string}", function (status) {
    const annualBoilerSalesSummaryPage = new AnnualBoilerSalesSummaryPage();
    annualBoilerSalesSummaryPage.navigateToAnnualBoilerSalesSummaryWithStatus(status);
    cy.wait(5000);
})

When("the user clicks on submit now button on annual sales page", function () {
    const annualBoilerSalesSummaryPage = new AnnualBoilerSalesSummaryPage();
    annualBoilerSalesSummaryPage.clickOnAnnualBolierSalesSubmitNowButton();
})

When("the user enter annual sales with the following data:", function (datatable) {
    const annualBoilerSalesSummaryPage = new AnnualBoilerSalesSummaryPage();
    annualBoilerSalesSummaryPage.enterAnnualBoilerSalesDetails(datatable);
    cy.wait(2000);
})

When("the user uploads verfication statements to annual boiler sales {string}:", function (verificationStatement) {
    const annualBoilerSalesSummaryPage = new AnnualBoilerSalesSummaryPage();
    annualBoilerSalesSummaryPage.uploadVerificationStatement(verificationStatement);
    cy.wait(5000);
})

When("the user uploads supporting evidence to annual boiler sales {string}:", function (supportingEvidence) {
    const annualBoilerSalesSummaryPage = new AnnualBoilerSalesSummaryPage();
    annualBoilerSalesSummaryPage.uploadSupportingEvidence(supportingEvidence);
    cy.wait(5000);
})

When("the user selects checkbox to confirm the details", function () {
    const annualBoilerSalesSummaryPage = new AnnualBoilerSalesSummaryPage();
    annualBoilerSalesSummaryPage.clickOnConfirmDetialsCheckBox();
})

When("the user click on submit button", function () {
    const annualBoilerSalesSummaryPage = new AnnualBoilerSalesSummaryPage();
    annualBoilerSalesSummaryPage.clickOnSubmitButton();
})

When("the user click on cancel button", function () {
    const annualBoilerSalesSummaryPage = new AnnualBoilerSalesSummaryPage();
    annualBoilerSalesSummaryPage.clickOnCancelButton();
})

Then("the user should see below details on check your annual boiler sales page", function (datatable) {
    const annualBoilerSalesSummaryPage = new AnnualBoilerSalesSummaryPage();
    annualBoilerSalesSummaryPage.CheckValueAfterChangingData(datatable);
})

Then("the user should see {string} for annual {string} sales", function (expectedValue, boilerType) {
    const annualBoilerSalesSummaryPage = new AnnualBoilerSalesSummaryPage();
    switch (boilerType.toLowerCase()) {
        case 'gas':
            annualBoilerSalesSummaryPage.checkAnnualGasBoilerSalesValues(expectedValue);
            break;
        case 'oil':
            annualBoilerSalesSummaryPage.checkAnnualOilBoilerSalesValues(expectedValue);
            break;
    }    
})
