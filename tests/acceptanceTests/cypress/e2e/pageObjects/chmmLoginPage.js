require('@cypress/xpath');

class CHMMLoginPage
{
    /****************** page objects *****************/

    elements = {

        registerforAnAccountButton : () => cy.xpath("//a[contains(text(), 'Continue to register')]", { timeout: 10000 }).should('be.visible'),
        chmmSigninButton : () => cy.xpath("//a[contains(text(), 'Sign in')]", { timeout: 10000 }).should('be.visible'),
        oneLoginSigninButton : () => cy.xpath("//button[contains(text(), 'Sign in')]", { timeout: 10000 }).should('be.visible'),
        startNowButton : () => cy.xpath("//a[contains(text(), 'Start now')][1]", { timeout: 10000 }).should('be.visible'),
        emailAddressField : () => cy.xpath("//input[@id='email']", { timeout: 10000 }).should('be.visible'),
        continueButton : () => cy.xpath("//button[@type='Submit']", { timeout: 10000 }).should('be.visible'),
        passwordField : () => cy.xpath("//input[@id='password']", { timeout: 10000 }).should('be.visible'),
        PageBody: () => cy.xpath("//body", { timeout: 10000 }).should('be.visible')

    }

    /******************** methods *******************/

    /**
     * Navigate to sign in page
     */
    navigateToSignIn()
    {
        this.elements.startNowButton().click();
    }

    /**
     * Navigate to registration page
     */
    navigateToRegistration()
    {
        this.elements.registerforAnAccountButton().click();
    }

    /**
     * Click on One Login button
     */
    clickOneLoginSigninButton()
    {
        this.elements.oneLoginSigninButton().click();
    }

    /**
     * Submit One login email id
     * @param {*} emailId 
     */
    submitLoginEmailAddress(emailId) 
    {
       this.elements.emailAddressField().type(emailId);
       this.elements.continueButton().click();         
    }

    /**
     * Submit One login password
     * @param {*} password 
     */
    submitLoginPassword(password)
    {
        this.elements.passwordField().type(password);
        /*this.elements.continueButton().click();*/
    }  

    clickOnContinue()
    {
        this.elements.continueButton().click();
    }

}

export default CHMMLoginPage;