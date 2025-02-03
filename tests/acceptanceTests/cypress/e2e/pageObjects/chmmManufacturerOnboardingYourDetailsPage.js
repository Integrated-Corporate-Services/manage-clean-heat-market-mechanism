require('@cypress/xpath');

class CHMMManufacturerOnboardingYourDetailsPage {

    /****************** Page objects *****************/

    elements = {
        fullNameTextField: () => cy.xpath("//input[@id='fullName']", { timeout: 10000 }).should('be.visible'),
        jobTitleTextField: () => cy.xpath("//input[@id='jobTitle']", { timeout: 10000 }).should('be.visible'),
        emailAddressTextField: () => cy.xpath("//input[@id='emailAddress']", { timeout: 10000 }).should('be.visible'),
        telephoneNumberTextField: () => cy.xpath("//input[@id='telephoneNumber']", { timeout: 10000 }).should('be.visible'),
        confirmCheckbox: () => cy.xpath("//input[@id='confirmation']", { timeout: 10000 }).should('be.enabled'),
        continueButton: () => cy.xpath("//button[contains(text(), 'Continue')]", { timeout: 10000 })
    }

    /************* Methods ***************/

    /**
     * Enter your details in the fields
     * @param {*} datatable 
     */
    enterYourDetails(datatable) {
        datatable.hashes().forEach((element) => {   
            if (element.emailAddress.toLowerCase() === 'random') {
                localStorage.setItem("ManufacturerEmailId", `QAAutoMfr_${Date.now()}@example.com`);
            } else {
                localStorage.setItem("ManufacturerEmailId", element.emailAddress);
            }         
            this.elements.fullNameTextField().clear();
            this.elements.fullNameTextField().type(element.fullName);     
           this.elements.jobTitleTextField().clear();
            this.elements.jobTitleTextField().type(element.jobTitle);
            this.elements.emailAddressTextField().clear();
            this.elements.emailAddressTextField().type(localStorage.getItem("ManufacturerEmailId"));   
            this.elements.telephoneNumberTextField().clear();
            this.elements.telephoneNumberTextField().type(element.telephoneNumber);     
        }) 
    }

    /**
     * Edit your details except email 
     * @param {*} datatable 
     */

    editYourDetails(datatable) {
        datatable.hashes().forEach((element) => {      
            this.elements.fullNameTextField().clear();
            this.elements.fullNameTextField().type(element.fullName);     
           this.elements.jobTitleTextField().clear();
            this.elements.jobTitleTextField().type(element.jobTitle); 
            this.elements.telephoneNumberTextField().clear();
            this.elements.telephoneNumberTextField().type(element.telephoneNumber);     
        })
    }
    

    /**
     * Check the confirmation checkbox
     */
    checkConfirmationCheckbox() {
        this.elements.confirmCheckbox().then(($elm) => {
            if ($elm.hasClass('ng-invalid')) {
                this.elements.confirmCheckbox().click();
            }
        })        
    }

    /**
     * Click on Continue button
     */
    clickContinue() {
        this.elements.continueButton().should('be.enabled').click();
    }

}

export default CHMMManufacturerOnboardingYourDetailsPage;