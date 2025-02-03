/// <reference types="Cypress" />

import LoginPage from "../../e2e/pageObjects/chmmLoginPage";
import AddOrgStructurePage from "../../e2e/pageObjects/chmmManufacturerOnboardingAddOrganisationStructurePage";
import ResponsibleUndertakingPage from "../../e2e/pageObjects/chmmManufacturerOnboardingResponsibleUndertakingPage";
import RegisteredOfficeAddressPage from "../../e2e/pageObjects/chmmManufacturerOnboardingRegisteredOfficeAddressPage";
import LegalCorrespondencePage from "../../e2e/pageObjects/chmmManufacturerOnboardingLegalCorrespondencePage";
import RelevantFossilFuelBoilersPage from "../../e2e/pageObjects/chmmManufacturerOnboardingRelevantFossilFuelBoilersPage";
import HeatPumpBrandsPage from "../../e2e/pageObjects/chmmManufacturerOnboardingHeatPumpBrandsPage";
import YourDetailsPage from "../../e2e/pageObjects/chmmManufacturerOnboardingYourDetailsPage";
import SeniorResponsibleOfficerPage from "../../e2e/pageObjects/chmmManufacturerOnboardingSeniorResponsibleOfficerDetailsPage";
import OptinForCreditTransfersPage from "../../e2e/pageObjects/chmmManufacturerOnboardingOptinForCreditTransfersPage";
import SubmitApplicationPage from "../../e2e/pageObjects/chmmManufacturerOnboardingSubmitApplicationPage";
import MenuPage from "../../e2e/pageObjects/chmmMenuPage";
import ManufacturerOnboardingCheckApplicationPage from "../../e2e/pageObjects/chmmManufacturerOnboardingCheckApplicationPage";
import CHMMManufacturerAccountsPage from "../../e2e/pageObjects/chmmManufacturerAccountsPage";

Given("the user navigates to add organisation structure page", function () {
    const loginPage = new LoginPage();
    loginPage.navigateToRegistration();
})

When("the user select {string} for group of organisations and continue", function (option) {
    const addOrgStructurePage = new AddOrgStructurePage();
    switch (option.toLowerCase()) {
        case "yes":
            addOrgStructurePage.selectYes();
            break;
        case "no":
            addOrgStructurePage.selectNo();
            break;
    }
    addOrgStructurePage.clickContinue();
})

When("the user select {string} for group of organisations with uploading files {string} and continue", function (option, files) {
    const addOrgStructurePage = new AddOrgStructurePage();
    switch (option.toLowerCase()) {
        case "yes":
            addOrgStructurePage.selectYes();
            break;
        case "no":
            addOrgStructurePage.selectNo();
            break;
    }
    addOrgStructurePage.uploadFiles(files);
    cy.wait(3000);
    addOrgStructurePage.clickContinue();
})




When("the user select {string} for responsible undertaking and continue by entering the following data:", function (option, datatable) {
    const responsibleUndertakingPage = new ResponsibleUndertakingPage();

    datatable.hashes().forEach((element) => {
        responsibleUndertakingPage.enterOrganisationName(element.organisationName);
        switch (option.toLowerCase()) {
            case "yes":
                responsibleUndertakingPage.selectYes();
                responsibleUndertakingPage.enterCompaniesHouseNumber(element.companyNumber);
                break;
            case "no":
                responsibleUndertakingPage.selectNo();
                break;
        }
    })
    responsibleUndertakingPage.clickContinue();
})

When("the user submit registered office address with following data:", function (datatable) {
    const registeredOfficeAddressPage = new RegisteredOfficeAddressPage();
    registeredOfficeAddressPage.enterRegisteredOfficeAddress(datatable);
})

When("the user select Yes, use this address for legal correspondence and continue", function () {
    const legalCorrespondencePage = new LegalCorrespondencePage();
    legalCorrespondencePage.selectYes();
    legalCorrespondencePage.clickContinue();
})

When("the user select No, I will provide another address for legal correspondence and submit with following data:", function (datatable) {
    const legalCorrespondencePage = new LegalCorrespondencePage();
    legalCorrespondencePage.selectNo();

    const registeredOfficeAddressPage = new RegisteredOfficeAddressPage();
    registeredOfficeAddressPage.enterRegisteredOfficeAddress(datatable);

})

When("the user select Yes for relevant fossil fuel boilers and continue", function () {
    const relevantFossilFuelBoilersPage = new RelevantFossilFuelBoilersPage();
    relevantFossilFuelBoilersPage.selectYes();
    relevantFossilFuelBoilersPage.clickContinue();
})

When("the user select No for relevant fossil fuel boilers and continue", function () {
    const relevantFossilFuelBoilersPage = new RelevantFossilFuelBoilersPage();
    relevantFossilFuelBoilersPage.selectNo();
    relevantFossilFuelBoilersPage.clickContinue();
    cy.wait(2000);
})

When("the user select I do not supply heat pumps and continue", function () {
    const heatPumpBrandsPage = new HeatPumpBrandsPage();
    heatPumpBrandsPage.selectIDoNotSupplyHeatPumps();
    cy.wait(3000);
    heatPumpBrandsPage.clickContinue();
})

When("the user select I do supply heat pumps and submit following heat pump brands:", function (datatable) {
    const heatPumpBrandsPage = new HeatPumpBrandsPage();
    heatPumpBrandsPage.selectIDoSupplyHeatPumps();
    heatPumpBrandsPage.enterHeatPumpBrand(datatable);
    heatPumpBrandsPage.clickContinue();
})

When("the user submit user details with the following data:", function (datatable) {
    const yourDetailsPage = new YourDetailsPage();
    yourDetailsPage.enterYourDetails(datatable);
    yourDetailsPage.checkConfirmationCheckbox();
    yourDetailsPage.clickContinue();
})

When("the user edits user details with the following data:", function (datatable) {
    const yourDetailsPage = new YourDetailsPage();
    yourDetailsPage.editYourDetails(datatable);
    yourDetailsPage.checkConfirmationCheckbox();
    yourDetailsPage.clickContinue();
})

When("the user select Yes, I am the Senior Responsible Officer and continue", function () {
    const seniorResponsibleOfficerPage = new SeniorResponsibleOfficerPage();
    seniorResponsibleOfficerPage.selectYesIAmTheSeniorResponsibleOfficer();
    seniorResponsibleOfficerPage.clickContinue();
})

When("the user select No, I will provide their details for Senior Responsible Officer and submit the following data:", function (datatable) {
    const seniorResponsibleOfficerPage = new SeniorResponsibleOfficerPage();
    seniorResponsibleOfficerPage.selectNoIWillProvideTheirDetails();
    seniorResponsibleOfficerPage.enterSeniorResponsibleOfficerDetails(datatable);
    seniorResponsibleOfficerPage.clickContinue();
})

When("the user edits Senior Responsible Officer with the following data:", function (datatable) {
    const seniorResponsibleOfficerPage = new SeniorResponsibleOfficerPage();
    seniorResponsibleOfficerPage.editSeniorResponsibleOfficerDetails(datatable);
    seniorResponsibleOfficerPage.clickContinue();
})

When("the user select Yes, I will provide contact details for credit transfers and submit the following data:", function (datatable) {
    const optinForCreditTransfersPage = new OptinForCreditTransfersPage();
    optinForCreditTransfersPage.selectYesIWillProvideContactDetailsForCreditTransfers();
    cy.wait(2000);
    optinForCreditTransfersPage.enterContactDetailsForCreditTransfers(datatable);
    optinForCreditTransfersPage.clickContinue();

})

When("the user select No, I don't want to opt-in at this time and continue", function () {
    const optinForCreditTransfersPage = new OptinForCreditTransfersPage();
    optinForCreditTransfersPage.selectNoIDontWantToOptinAtThisTime();
    cy.wait(2000);
    optinForCreditTransfersPage.clickContinue();
})


When("the user click on continue button", function () {
    cy.wait(1000);
    const optinForCreditTransfersPage = new OptinForCreditTransfersPage();
    optinForCreditTransfersPage.clickContinue();
})

When("the user submits the application", function () {
    const submitApplicationPage = new SubmitApplicationPage();
    submitApplicationPage.clickAcceptAndSend();
    cy.wait(5000);
})

When("the user navigates to manufacturers account page", function () {
    const menuPage = new MenuPage();
    menuPage.navigateToManufacturerAccountsPage();
})


Then("the user should see below details on check your answers page", function (datatable) {
    const manufacturerOnboardingCheckApplicationPage = new ManufacturerOnboardingCheckApplicationPage();
    manufacturerOnboardingCheckApplicationPage.CheckValueForOnBehalfOfGroupOfOrganisation(datatable);
})


When("the user naviagtes to {string} organisation details page", function (orgName) {
    const chmmManufacturerAccountsPage = new CHMMManufacturerAccountsPage();
    chmmManufacturerAccountsPage.searchForOrganisation(orgName);
    chmmManufacturerAccountsPage.ClickonManufacturerOrganisationLink(orgName);
})

Then("the user should see below details in {string} card on check your answers page",function(cardName,datatable){
    const manufacturerOnboardingCheckApplicationPage = new ManufacturerOnboardingCheckApplicationPage();
    manufacturerOnboardingCheckApplicationPage.CheckValueForOnBehalfOfGroupOfOrganisationInsidecard(cardName,datatable);
})


       