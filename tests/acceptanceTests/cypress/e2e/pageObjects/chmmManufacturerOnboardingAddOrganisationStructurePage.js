require('@cypress/xpath');

class CHMMManufacturerOnboardingAddOrganisationStructurePage {

    /****************** Page objects *****************/

    elements = {
        yesRadioButton: () => cy.xpath("//input[@id='yes']", { timeout: 10000 }).should('be.enabled'),
        noRadioButton: () => cy.xpath("//input[@id='no']", { timeout: 10000 }).should('be.enabled'),
        continueButton: () => cy.xpath("//button[contains(text(), 'Continue')]", { timeout: 10000 }).should('be.enabled'),
        cancelButton: () => cy.xpath("//a[contains(text(), 'Cancel')]", { timeout: 10000 }).should('be.enabled')

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

    /**
     * Click on Cancel button
     */
    clickCancel() {
        this.elements.cancelButton().click();
    }


    /**Click and upload files */
    uploadFiles(files)
        {
            cy.xpath("//input[@id='files']")
            .click()
            .attachFile(files);
        }
}

export default CHMMManufacturerOnboardingAddOrganisationStructurePage;