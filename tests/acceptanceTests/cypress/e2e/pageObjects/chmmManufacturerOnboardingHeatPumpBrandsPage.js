require('@cypress/xpath');

class CHMMManufacturerOnboardingHeatPumpBrandsPage {

    /****************** Page objects *****************/

    elements = {
        iDoNotSupplyHeatPumpsRadioButton: () => cy.xpath("//input[@id='no']", { timeout: 10000 }).should('be.enabled'),
        idoSupplyHeatPumpsRadioButton: () => cy.xpath("//input[@id='yes']", { timeout: 10000 }).should('be.enabled'), 
        heatPumpBrandTextField:() => cy.xpath("//input[contains(@class, 'govuk-input ng-untouched')]", { timeout: 10000 }).should('be.visible'), 
        addAnotherButton: () => cy.xpath("//button[contains(text(), 'Add another')]", { timeout: 10000 }).should('be.visible'), 
        removeButton: () => cy.xpath("//button[contains(text(), 'Remove')]", { timeout: 10000 }).should('be.visible'),     
        continueButton: () => cy.xpath("//button[contains(text(), 'Continue')]", { timeout: 10000 }).should('be.enabled')
    }

    /*************** Properties *************/

    /**
     * Select Yes radio button
     */
    selectIDoNotSupplyHeatPumps() {
        this.elements.iDoNotSupplyHeatPumpsRadioButton().click();
        cy.wait(2000);
    }

    /**
     * Select No radio button
     */
    selectIDoSupplyHeatPumps() {
        this.elements.idoSupplyHeatPumpsRadioButton().click();
        cy.wait(2000);
    }

    /**
     * click on Remove button for heat pump brand
     */
    clickRemoveButton() {
        this.elements.removeButton().click();
    }

    /**
     * Click on Add another button for heat pump brand
     */
    clickAddAnotherButton() {
        this.elements.addAnotherButton().click();
    }

    /**
     * Enter thr given text in the heat pump brand field
     * @param {*} datatable 
     */
    enterHeatPumpBrand(datatable) {
        var dataArray = datatable.hashes();
        dataArray.forEach((element, index) => {           
            this.elements.heatPumpBrandTextField().clear();
            this.elements.heatPumpBrandTextField().type(element.heatPumpBrand);   
            if (!!dataArray[index+1]) {
                this.clickAddAnotherButton();
            }         
        })        
    }

    /**
     * Click on Continue button
     */
    clickContinue() {
        this.elements.continueButton().click();
    }

}

export default CHMMManufacturerOnboardingHeatPumpBrandsPage;