require('@cypress/xpath');

class CHMMManufacturerOnboardingOptinForCreditTransfersPage {

    /****************** Page objects *****************/

    elements = {

        yesRadioButton: () => cy.xpath("//input[@id='yes']", { timeout: 10000 }).should('be.enabled'),
        noRadioButton: () => cy.xpath("//input[@id='no']", { timeout: 10000 }).should('be.enabled'),
        continueButton: () => cy.xpath("//button[@type='submit']", { timeout: 10000 }).should('be.enabled'),
        contactNameTextField: () => cy.xpath("//input[@id='name']", { timeout: 10000 }).should('be.visible'),
        emailAddressTextField: () => cy.xpath("//input[@id='emailAddress']", { timeout: 10000 }).should('be.visible'),
        telephoneNumberTextField: () => cy.xpath("//input[@id='telephoneNumber']", { timeout: 10000 }).should('be.visible')
    }

    /****************** Getters and Setters *****************/

    /**
     * Select Yes radio button
     */
    selectYesIWillProvideContactDetailsForCreditTransfers() {
        this.elements.yesRadioButton().click();
    }

    /**
     * Select No radio button
     */
    selectNoIDontWantToOptinAtThisTime() {
        this.elements.noRadioButton().click();
    }

    /**
     * Click on Continue button
     */
    clickContinue() {
        this.elements.continueButton().click();
    }

    /**
     * Enter the contact details for credit transfers
     * @param {*} datatable 
     */
    enterContactDetailsForCreditTransfers(datatable) {
        datatable.hashes().forEach((element) => { 
            if (element.emailAddress.toLowerCase() === 'random') {
                localStorage.setItem("CreditTransfersEmailId", `QAAutoCrdTrf_${Date.now()}@example.com`);
            } else {
                localStorage.setItem("CreditTransfersEmailId", element.emailAddress);
            }            
            this.elements.contactNameTextField().clear();
            this.elements.contactNameTextField().type(element.contactName);                
            this.elements.emailAddressTextField().clear();
            this.elements.emailAddressTextField().type(localStorage.getItem("CreditTransfersEmailId"));   
            this.elements.telephoneNumberTextField().clear();
            this.elements.telephoneNumberTextField().type(element.telephoneNumber);     
        }) 
    }

}

export default CHMMManufacturerOnboardingOptinForCreditTransfersPage;