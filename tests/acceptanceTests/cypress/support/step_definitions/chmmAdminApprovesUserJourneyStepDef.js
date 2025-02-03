/// <reference types="Cypress" />

import AdminViewOrganisationdetailsPage from "../../e2e/pageObjects/chmmAdminViewOrganisationdetailsPage";
import ConfirmManufacturerAccountApprovalPage from "../../e2e/pageObjects/chmmConfirmManufacturerAccountApprovalPage";
import AreYouSurePage from "../../e2e/pageObjects/chmmAreYouSurePage";
import MenuPage from "../../e2e/pageObjects/chmmMenuPage";

When("the admin user clicks on approve button", function() {
    const adminViewOrganisationdetailsPage = new AdminViewOrganisationdetailsPage();
    adminViewOrganisationdetailsPage.clickOnViewOrganisationApproveButton();
})


When("the admin adds additional comments {string}", function(comments){
    const confirmManufacturerAccountApprovalPage = new ConfirmManufacturerAccountApprovalPage();
    confirmManufacturerAccountApprovalPage.addAdditionalComments(comments);

})
When("the admin clicks yes approve on Confirm manufacturer account approval page", function(){
    const confirmManufacturerAccountApprovalPage = new ConfirmManufacturerAccountApprovalPage();
    confirmManufacturerAccountApprovalPage.clickApproveOnManufacturerAccountApprovalPage();

})


When("the admin clicks cancel on confirm manufacturer account approval page", function(){
    const confirmManufacturerAccountApprovalPage = new ConfirmManufacturerAccountApprovalPage();
    confirmManufacturerAccountApprovalPage.clickCancelOnManufacturerAccountApprovalPage();


})

When("the admin clicks yes approve on are you sure page", function(){
const areYouSurePage = new AreYouSurePage();
areYouSurePage.clickOnYesApproveButton();
cy.wait(5000);

})

When("the admin clicks cancel on are you sure page", function(){
    const areYouSurePage = new AreYouSurePage();
    areYouSurePage.clickOnYesCancelButton();
    
})

When("the admin user clicks on back link", function(){
    const menuPage = new MenuPage();
    menuPage.clickOnBackButton();

})
