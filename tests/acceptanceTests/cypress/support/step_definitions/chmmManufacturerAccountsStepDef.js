/// <reference types="Cypress" />
import MenuPage from "../../e2e/pageObjects/chmmMenuPage";
import ManufacturerAccountsPage from "../../e2e/pageObjects/chmmManufacturerAccountsPage";


/************************ Step defs ******************/


When("the admin user navigates to manufacturer accounts page", function () {
    const menuPage = new MenuPage();
    menuPage.navigateToManufacturerAccountsPage();
    const mfrAccountsPage = new ManufacturerAccountsPage();
    mfrAccountsPage.getManufacturerAccountsTableBody();
})

Then("the user should see the following data in manufacturer accounts table:", function (datatable) {
    const mfrAccountsPage = new ManufacturerAccountsPage();
    datatable.hashes().forEach((element) => {
        var mfrAccountRow = mfrAccountsPage.getManufacturerAccountsTableRow(element.name);
        mfrAccountRow.should('contain.text', element.status);
    }) 
})

Then("the user should not see retired manufacturer accounts", function (expText) {
    const mfrAccountsPage = new ManufacturerAccountsPage();
    mfrAccountsPage.getManufacturerAccountsTableBody().should('not.contain.text', expText)
})

When("the user check show retired manufacturer accounts", function () {
    const mfrAccountsPage = new ManufacturerAccountsPage();
    mfrAccountsPage.checkShowArchivedCheckbox(); 
})
        
Then("the user should see retired manufacturer accounts", function () {
    const mfrAccountsPage = new ManufacturerAccountsPage();
    mfrAccountsPage.getManufacturerAccountsTableBody().should('contain.text', expText)
})

When("the user search for {string} organisation", function (orgName) {
    const mfrAccountsPage = new ManufacturerAccountsPage();
    mfrAccountsPage.searchForOrganisation(orgName);
})

Then("the user should see the following data in the first row of manufacturer accounts table:", function (datatable) {
    const mfrAccountsPage = new ManufacturerAccountsPage();
    datatable.hashes().forEach((element) => {
        mfrAccountsPage.getManufacturerAccountsTableRowOne().should('contain.text', element.name).and('contain.text', element.status)
    })    
})

When("the user navigates to new {string} organisation created", function(orgName) {
    const mfrAccountsPage = new ManufacturerAccountsPage();
    mfrAccountsPage.clickonManufacturerOrganisationLink(orgName);
})

When("the user navigates to {string} page for new {string} organisation created", function (pageLink, orgName) {
    const mfrAccountsPage = new ManufacturerAccountsPage();
    mfrAccountsPage.clickonManufacturerOrganisationLink(orgName);
    mfrAccountsPage.clickOnLink(pageLink);
})

When("the user navigates to {string} page from the navigation menu", function (pageLink) {
    const mfrAccountsPage = new ManufacturerAccountsPage();
    mfrAccountsPage.clickOnLink(pageLink);
}) 