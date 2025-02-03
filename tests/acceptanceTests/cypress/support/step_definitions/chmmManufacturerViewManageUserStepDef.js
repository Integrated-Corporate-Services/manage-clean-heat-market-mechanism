/// <reference types="Cypress" />
import LoginPage from "../../e2e/pageObjects/chmmLoginPage";
import LoginHelper from "../helperClasses/chmmLoginHelper";
import MenuPage from "../../e2e/pageObjects/chmmMenuPage";
import ManufacturerManageUserPage from "../../e2e/pageObjects/chmmManufacturerViewManageUserPage"


/**************** Step defs ************************/

Given("the manufacturer user log in to chmm view manage user page with {string} email address", function (emailId) {
    const loginHelper = new LoginHelper();   
    const loginPage = new LoginPage();
    const menuPage = new MenuPage();
  
    loginHelper.loginasAdmin(emailId);
    loginPage.navigateToSignIn();
    menuPage.navigateToManufacturerManageUsersPage();    
  })

  Then("the user should see the following user with {string} email has details in the manage users table:", function (email, datatable) {
    const manufacturerManageUserPage = new ManufacturerManageUserPage();
    var manufacturerManageUserTableRow = manufacturerManageUserPage.getManufacturerManageUserRow(email);
    datatable.hashes().forEach((element) => {
      manufacturerManageUserTableRow.should('contain.text', element.text);
    }) 
  })
