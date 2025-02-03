/// <reference types="Cypress" />

import ActivateDeactivatePage from "../../e2e/pageObjects/chmmActivateAndDeactivateAdminUserPage";
import MenuPage from "../../e2e/pageObjects/chmmMenuPage";
import AdminAccountsPage from "../../e2e/pageObjects/chmmAdminAccountsPage";
import EditUserPage from "../../e2e/pageObjects/chmmEditUserPage";

Given("the admin user {string} with {string} email is in active state", function(name, email) {
    if (email === "Random" || email === "random") {
        email = localStorage.getItem("EmailId");
    }
    const menuPage = new MenuPage();
    menuPage.navigateToAdministratorAccountsPage();
    
    const activateDeactivatePage = new ActivateDeactivatePage();
    activateDeactivatePage.activateAdminUser(name, email);
})

When("the logged in admin user deactivates another admin user {string} with {string} email", function(name, email) {
    if (email === "Random" || email === "random") {
        email = localStorage.getItem("EmailId");
    }

    const menuPage = new MenuPage();
    menuPage.navigateToAdministratorAccountsPage();

    const activateDeactivatePage = new ActivateDeactivatePage();
    activateDeactivatePage.deactivateAdminUser(name, email);
})

When("the logged in admin user start deactivating another admin user {string} with {string} email", function(name, email) {
    if (email === "Random" || email === "random") {
        email = localStorage.getItem("EmailId");
    }

    const menuPage = new MenuPage();
    menuPage.navigateToAdministratorAccountsPage();

    const activateDeactivatePage = new ActivateDeactivatePage();
    activateDeactivatePage.deactivateAdminUser(name, email);
})

When("the admin user cancel the deactivate process by clicking on No return to list button", function() {
    const activateDeactivatePage = new ActivateDeactivatePage();
    activateDeactivatePage.clickNoReturnToListButton();
})

When("the admin user cancel the activate process by clicking on No return to list button", function () {
    const activateDeactivatePage = new ActivateDeactivatePage();
    activateDeactivatePage.clickNoReturnToListButton();
})

Given("the logged in admin user navigates to user details page for user {string} with {string} email", function(name, email) {
    if (email === "Random" || email === "random") {
        email = localStorage.getItem("EmailId");
    }
    const adminAccountsPage = new AdminAccountsPage();
    adminAccountsPage.navigateToAdminUserDetailsPage(name, email);
})

When("the admin user click on deactivate button", function() {
    const editUserPage = new EditUserPage();
    editUserPage.clickOnDeactivateButton();
})

When("the admin user click on activate button", function () {
    const editUserPage = new EditUserPage();
    editUserPage.clickOnActivateButton();
})

Given("the admin user {string} with {string} email is in inactive state", function (name, email) {
    if (email === "Random" || email === "random") {
        email = localStorage.getItem("EmailId");
    }
    const menuPage = new MenuPage();
    menuPage.navigateToAdministratorAccountsPage();
    
    const activateDeactivatePage = new ActivateDeactivatePage();
    activateDeactivatePage.deactivateAdminUser(name, email);
})

When("the logged in admin user activates another admin user {string} with {string} email", function (name, email) {
    if (email === "Random" || email === "random") {
        email = localStorage.getItem("EmailId");
    }

    const menuPage = new MenuPage();
    menuPage.navigateToAdministratorAccountsPage();

    const activateDeactivatePage = new ActivateDeactivatePage();
    activateDeactivatePage.activateAdminUser(name, email);
    cy.wait(5000);
})