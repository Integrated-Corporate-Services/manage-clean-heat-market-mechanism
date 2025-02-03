/// <reference types="Cypress" />
import LoginPage from "../../e2e/pageObjects/chmmLoginPage";
import LoginHelper from "../helperClasses/chmmLoginHelper";
/**************** Constants ************************/



When("the admin user log in to CHMM dashboard page with {string} email address", function (emailId) {
  const loginHelper = new LoginHelper();
    loginHelper.loginasAdmin(emailId);
  });


  