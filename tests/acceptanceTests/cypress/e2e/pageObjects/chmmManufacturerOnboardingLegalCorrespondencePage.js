require('@cypress/xpath');

class CHMMManufacturerOnboardingLegalCorrespondencePage {

    /****************** Page objects *****************/

    elements = {
        yesRadioButton: () => cy.xpath("//input[@id='yes']", { timeout: 10000 }).should('be.enabled'),
        noRadioButton: () => cy.xpath("//input[@id='no']", { timeout: 10000 }).should('be.enabled'),
        continueButton: () => cy.xpath("//button[contains(text(), 'Continue')]", { timeout: 10000 }).should('be.enabled')
    }

    /****************** Methods *****************/

    /**
     * Select Yes radio button
     */
    selectYes() {
        this.elements.yesRadioButton().click();
    }

    /**
     * Select No radio button
     */
    selectNo() {
        this.elements.noRadioButton().click();
    }

    /**
     * Click on Continue button
     */
    clickContinue() {
        this.elements.continueButton().click();
    }

}

export default CHMMManufacturerOnboardingLegalCorrespondencePage;