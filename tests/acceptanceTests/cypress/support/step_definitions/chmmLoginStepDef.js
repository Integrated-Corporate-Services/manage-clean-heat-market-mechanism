/// <reference types="Cypress" />
import LoginPage from "../../e2e/pageObjects/chmmLoginPage";
import LoginHelper from "../helperClasses/chmmLoginHelper";
import MenuPage from "../../e2e/pageObjects/chmmMenuPage";
/**************** Constants ************************/

const adminPassword = '$Asomeday1';


When("the admin user log in to CHMM system with {string} email address", function (emailId) {    
    const loginHelper = new LoginHelper();   
    const loginPage = new LoginPage();
    loginHelper.loginasAdmin(emailId);
    loginPage.navigateToSignIn();
});

Given("the manufacturer user log in to chmm system with {string} email address", function (emailId) {
  const loginHelper = new LoginHelper();   
  const loginPage = new LoginPage();

  loginHelper.loginasAdmin(emailId);
  loginPage.navigateToRegistration();
})

Given("the manufacturer user log in to chmm system with {string}", function (emailId) {
  const loginHelper = new LoginHelper();   
  const loginPage = new LoginPage();

  loginHelper.loginasAdmin(emailId);
})

Given("The user signs out of the account", function(){
const menuPage = new MenuPage();
menuPage.UserSignOutButton();
})



  
