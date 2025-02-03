/// <reference types="Cypress" />

import LoginPage from "../../e2e/pageObjects/chmmLoginPage";
import LoginHelper from "../helperClasses/chmmLoginHelper";
import MenuPage from "../../e2e/pageObjects/chmmMenuPage";


/**************** Step definitions ************************/

  When("the admin user log in to View Admin user page with {string} email address", function (emailId) {
    const loginPage = new LoginPage();
    const loginHelper = new LoginHelper();
    const menuPage = new MenuPage();
   
    loginHelper.loginasAdmin(emailId);
    //loginPage.navigateToSignIn();
    menuPage.navigateToAdministratorAccountsPage();
    cy.wait(3000);
  });


  

 When("the admin user log in to View Admin user page with {string} email address after creation of manufacturer", function (emailId) {
    const loginPage = new LoginPage();
    const loginHelper = new LoginHelper();
    const menuPage = new MenuPage();
   
    loginHelper.loginasAdmin(emailId);
    menuPage.navigateToAdministratorAccountsPage();
    cy.wait(3000);
  });

  When("the user selects scheme year 2024", function(){
    const menuPage = new MenuPage();

    menuPage.Schemeyearselection2024();
    menuPage.clickOnGoButton();
    cy.wait(3000);

  });

