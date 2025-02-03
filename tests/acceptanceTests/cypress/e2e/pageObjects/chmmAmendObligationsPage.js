require('@cypress/xpath');

class CHMMAmendObligationsPage {

    /****************** page objects *****************/

    elements = {

        amendObligationButton: () => cy.xpath("//button[contains(text(), ' Amend obligation ')]", { timeout: 10000 }).should('be.visible'),
        amendedObligationAmount: () => cy.xpath("//th[contains(text(), 'Manually amended by administrator')]/following-sibling::td", { timeout: 10000 }).should('be.visible'),
        addingRadioButton: () => cy.xpath("//input[@id='adding']", { timeout: 10000 }),
        removingRadioButton: () => cy.xpath("//input[@id='removing']", { timeout: 10000 }),
        amountTextField: () => cy.xpath("//input[@id='amount']", { timeout: 10000 }).should('be.visible'),
        continueButton: () => cy.xpath("//button[contains(text(), 'Continue')]", { timeout: 10000 }).should('be.visible'),
        cancelButton: () => cy.xpath("//button[contains(text(), 'Cancel')]", { timeout: 10000 }).should('be.visible')
    }

    /********* Methods *****/

    /**
     * Click on Amend Obligation button
     */
    clickOnAmendObligationButton() {
        this.elements.amendObligationButton().click();
    }

    /**
     * Check Adding radio button
     */
    checkAddingRadioButton() {
        this.elements.addingRadioButton().check();
    }

    /**
     * Check Removing radio button
     */
    checkRemovingRadioButton() {
        this.elements.removingRadioButton().check();
    }

    /**
     * Type given amount into the amount field
     * @param {*} amount 
     */
    enterObligationAmount(amount) {
        this.elements.amountTextField().clear();
        this.elements.amountTextField().type(amount);
    }

    /**
     * Click on Continue button
     */
    clickOnContinueButton() {
        this.elements.continueButton().click();
    }

    /**
     * Click on Cancel button
     */
    clickOnCancelButton() {
        this.elements.cancelButton().click();
    }

    /**
     * Check that the amended obligation is displayed correctly
     * @param {*} expectedtext 
     */
    checkAmendedObligationAmount(expectedtext) {
        this.elements.amendedObligationAmount().invoke('text').then((actualText)=> {
            expect(actualText.toLowerCase()).to.include(expectedtext.toLowerCase());
        })
    }

}

export default CHMMAmendObligationsPage;