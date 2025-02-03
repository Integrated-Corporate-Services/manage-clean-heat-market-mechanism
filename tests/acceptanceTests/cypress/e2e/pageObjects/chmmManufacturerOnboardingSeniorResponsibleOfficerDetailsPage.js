require('@cypress/xpath');

class CHMMManufacturerOnboardingSeniorResponsibleOfficerDetailsPage {

    /****************** Page objects *****************/

    elements = {
        yesRadioButton: () => cy.xpath("//input[@id='yes']", { timeout: 10000 }).should('be.enabled'),
        noRadioButton: () => cy.xpath("//input[@id='no']", { timeout: 10000 }).should('be.enabled'),
        continueButton: () => cy.xpath("//button[contains(text(), 'Continue')]", { timeout: 10000 }).should('be.enabled'),
        fullNameTextField: () => cy.xpath("//input[@id='fullName']", { timeout: 10000 }).should('be.visible'),
        jobTitleTextField: () => cy.xpath("//input[@id='jobTitle']", { timeout: 10000 }).should('be.visible'),
        organisationTextField: () => cy.xpath("//input[@id='organisation']", { timeout: 10000 }).should('be.visible'),
        emailAddressTextField: () => cy.xpath("//input[@id='emailAddress']", { timeout: 10000 }).should('be.visible'),
        telephoneNumberTextField: () => cy.xpath("//input[@id='telephoneNumber']", { timeout: 10000 }).should('be.visible')
    }

    /************* Getters and Setters ***************/

    /**
     * Select Yes radio button
     */
    selectYesIAmTheSeniorResponsibleOfficer() {
        this.elements.yesRadioButton().click();
    }

    /**
     * Select No radio button
     */
    selectNoIWillProvideTheirDetails() {
        this.elements.noRadioButton().click();
    }

    /**
     * Click on Continue button
     */
    clickContinue() {
        this.elements.continueButton().click();
    }

    /**
     * Enter the Senior Responsible Officer deatils in the fields
     * @param {*} datatable 
     */
    enterSeniorResponsibleOfficerDetails(datatable) {
        datatable.hashes().forEach((element) => {     
            if (element.emailAddress.toLowerCase() === 'random') {
                localStorage.setItem("ResponsibleOfficerEmailId", `QAAutoSrOfficer_${Date.now()}@example.com`);
            } else {
                localStorage.setItem("ResponsibleOfficerEmailId", element.emailAddress);
            }       
            this.elements.fullNameTextField().clear();
            this.elements.fullNameTextField().type(element.fullName);     
            this.elements.jobTitleTextField().clear();
            this.elements.jobTitleTextField().type(element.jobTitle);
            this.elements.organisationTextField().clear();
            this.elements.organisationTextField().type(element.organisation);
            this.elements.emailAddressTextField().clear();
            this.elements.emailAddressTextField().type(localStorage.getItem("ResponsibleOfficerEmailId"));   
            this.elements.telephoneNumberTextField().clear();
            this.elements.telephoneNumberTextField().type(element.telephoneNumber);     
        })
    }


    /**
     * edit senior responsibility officer details
     * @param {*} datatable 
     */
    editSeniorResponsibleOfficerDetails(datatable) {
        datatable.hashes().forEach((element) => {     
    
            this.elements.fullNameTextField().clear();
            this.elements.fullNameTextField().type(element.fullName);     
            this.elements.jobTitleTextField().clear();
            this.elements.jobTitleTextField().type(element.jobTitle);
            this.elements.organisationTextField().clear();
            this.elements.organisationTextField().type(element.organisation);   
            this.elements.telephoneNumberTextField().clear();
            this.elements.telephoneNumberTextField().type(element.telephoneNumber);     
        })
    }


}

export default CHMMManufacturerOnboardingSeniorResponsibleOfficerDetailsPage;