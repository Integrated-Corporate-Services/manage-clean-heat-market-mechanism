/// <reference types="Cypress" />

import AddNewUserPage from "../../e2e/pageObjects/chmmAddNewUserPage";
import MenuPage from "../../e2e/pageObjects/chmmMenuPage";
import EditUserPage from "../../e2e/pageObjects/chmmEditUserPage";

When("the admin user edits admin user with the following data:", function (datatable) {
    const addNewUserPage = new AddNewUserPage();
    const editUserPage = new EditUserPage();
    // cy.wait(5000);
    editUserPage.editAdminUser(datatable);
    addNewUserPage.clickOnSaveAndSendNotificationButton();
    // cy.wait(15000);
    })

When("the user navigates to the administrator accounts page using back to adminstrator account link", function () {
    const menuPage = new MenuPage();
    menuPage.navigateBackToAdministratorAccountsPage();
    // cy.wait(15000);
    })

When("the user clicks cancel on edits admin page:", function (datatable) {
        const editUserPage = new EditUserPage();
        // cy.wait(5000);
        editUserPage.cancelEditAdminUser(datatable);
        // cy.wait(15000);
    })

When("the user clicks cancel on Check the details before submitting page:", function (datatable) {
        const editUserPage = new EditUserPage();
        // cy.wait(5000);
        editUserPage.editAdminUser(datatable);
        editUserPage.clickOnCancelButton();
        // cy.wait(15000);
    })

    When("the admin user edits admin user page:", function (datatable) {
        const editUserPage = new EditUserPage();
        editUserPage.editAdminUser(datatable); 
        // cy.wait(15000);
    })