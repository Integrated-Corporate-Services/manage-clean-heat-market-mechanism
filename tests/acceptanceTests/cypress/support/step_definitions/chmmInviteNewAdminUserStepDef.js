/// <reference types="Cypress" />

import AddNewUserPage from "../../e2e/pageObjects/chmmAddNewUserPage";
import LoginPage from "../../e2e/pageObjects/chmmLoginPage";
import MenuPage from "../../e2e/pageObjects/chmmMenuPage";
import AdminAccountsPage from "../../e2e/pageObjects/chmmAdminAccountsPage";


When("the admin user invites a new admin user with the following data:", function (datatable) {
    const addNewUserPage = new AddNewUserPage();
    const menuPage = new MenuPage();
    menuPage.navigateToAdministratorAccountsPage();
    addNewUserPage.navigateToAddNewUserPage();
    addNewUserPage.addNewUser(datatable);
    //addNewUserPage.clickOnSaveAndSendNotificationButton();
    cy.wait(15000);
})

When("the admin user adds a new admin user with the following data:", function (datatable) {
    const addNewUserPage = new AddNewUserPage();
    const menuPage = new MenuPage();
    menuPage.navigateToAdministratorAccountsPage();
    addNewUserPage.navigateToAddNewUserPage();
    addNewUserPage.addNewUser(datatable);
})

Then("the user should see the invited {string} email address on the page", function (email) {
    if (email === "Random" || email === "random") {
        email = localStorage.getItem("EmailId");
    }
    const loginPage = new LoginPage();
    loginPage.elements.PageBody().should('contain.text', email);
})

When("the user navigates to the administrator accounts page", function () {
    const menuPage = new MenuPage();
    menuPage.navigateToAdministratorAccountsPage();
    cy.wait(5000);
})

When("the user navigates to the add new user page", function () {
    const addNewUserPage = new AddNewUserPage();
    addNewUserPage.navigateToAddNewUserPage();
})




Then("the user should see the following new {string} email admin user details in the administrator accounts table:", function (email, datatable) {
    cy.log(localStorage.getItem("EmailId"))
    if (email === "Random" || email === "random") {
        email = localStorage.getItem("EmailId").toLowerCase();
    }
    
    const adminAccountsPage = new AdminAccountsPage();
    var adminTableRow = adminAccountsPage.getAdminAccountsRow(email);
    datatable.hashes().forEach((element) => {
        adminTableRow.should('contain.text', element.text);
    })
})