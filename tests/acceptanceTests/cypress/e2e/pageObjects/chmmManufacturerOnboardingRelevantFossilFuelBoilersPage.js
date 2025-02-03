require('@cypress/xpath');

class CHMMManufacturerOnboardingRelevantFossilFuelBoilersPage {

    /****************** Page objects *****************/

    elements = {
        yesRadioButton: () => cy.xpath("//input[@id='yes']", { timeout: 10000 }).should('be.enabled'),
        noRadioButton: () => cy.xpath("//input[@id='no']", { timeout: 10000 }).should('be.enabled'),
        continueButton: () => cy.xpath("//button[@type='submit']", { timeout: 10000 }).should('be.enabled')
    }

    /****************** Methods *****************/

    /**
     * Select Yes radio button
     */
    selectYes() {
        this.elements.yesRadioButton().click();
        cy.wait(2000);
    }

    /**
     * Select No radio button
     */
    selectNo() {
        this.elements.noRadioButton().click();
        cy.wait(2000);
    }

    /**
     * Click on Continue button
     */
    clickContinue() {
        this.elements.continueButton().click();
    }

}

export default CHMMManufacturerOnboardingRelevantFossilFuelBoilersPage;