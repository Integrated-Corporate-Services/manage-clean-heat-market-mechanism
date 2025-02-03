require('@cypress/xpath');

class CHMMManufacturerOnboardingRegistredOfficeAddressPage {

    /****************** Page objects *****************/

    elements = {
        addressLine1TextField: () => cy.xpath("//input[@id='lineOne']", { timeout: 10000 }).should('be.visible'),
        addressLine2TextField: () => cy.xpath("//input[@id='lineTwo']", { timeout: 10000 }).should('be.visible'),
        townTextField: () => cy.xpath("//input[@id='city']", { timeout: 10000 }).should('be.visible'),
        countyTextField: () => cy.xpath("//input[@id='county']", { timeout: 10000 }).should('be.visible'),
        postcodeTextField: () => cy.xpath("//input[@id='postcode']", { timeout: 10000 }).should('be.visible'),
        continueButton: () => cy.xpath("//button[contains(text(), 'Continue')]", { timeout: 10000 }).should('be.enabled')
    }

    /****************** Methods *****************/

    /**
     * Enter the given address in the registered Office address fields
     * @param {*} datatable 
     */
    enterRegisteredOfficeAddress(datatable) {

        datatable.hashes().forEach((element) => {
            this.elements.addressLine1TextField().clear();
            this.elements.addressLine1TextField().type(element.addressLine1);
            if (element.addressLine2) {
                this.elements.addressLine2TextField().clear();
                this.elements.addressLine2TextField().type(element.addressLine2);
            }            
            this.elements.townTextField().clear();
            this.elements.townTextField().type(element.town);
            if (element.county) {
                this.elements.countyTextField().clear();
                this.elements.countyTextField().type(element.county);
            }            
            this.elements.postcodeTextField().clear();
            this.elements.postcodeTextField().type(element.postcode);
        })
        this.elements.continueButton().click();
        
    }



}

export default CHMMManufacturerOnboardingRegistredOfficeAddressPage;