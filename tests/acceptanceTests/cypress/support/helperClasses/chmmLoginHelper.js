import LoginPage from "../../e2e/pageObjects/chmmLoginPage";

const adminPassword = '$Asomeday1';

class CHMMloginHelper {

    /**
     * Helper method to log in to CHMM application using One Login
     * @param {*} emailId 
     */
    loginasAdmin(emailId) {
        // Remove the below code block once exception error is fixed in One Login
        cy.on('uncaught:exception', (err, runnable) => {
            return false
        })
        cy.visit(Cypress.env("URL"));
        const loginPage = new LoginPage();
        loginPage.navigateToSignIn();
        cy.wait(5000);
        loginPage.clickOneLoginSigninButton();
        loginPage.submitLoginEmailAddress(emailId);
        loginPage.submitLoginPassword(adminPassword);
        loginPage.clickOnContinue();
    }    

}

export default CHMMloginHelper;