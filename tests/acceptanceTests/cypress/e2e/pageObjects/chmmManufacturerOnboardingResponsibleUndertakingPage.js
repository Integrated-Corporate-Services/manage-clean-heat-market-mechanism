require('@cypress/xpath');

class CHMMManufacturerOnboardingResponsibleUndertakingPage {

    /****************** Page objects *****************/

    elements = {
        yesRadioButton: () => cy.xpath("//input[@id='yes']", { timeout: 10000 }).should('be.enabled'),
        noRadioButton: () => cy.xpath("//input[@id='no']", { timeout: 10000 }).should('be.enabled'),
        organisationNameTextField: () => cy.xpath("//input[@id='name']", { timeout: 10000 }).should('be.visible'),
        companiesHouseNumberTextField: () => cy.xpath("//input[@id='companiesHouseNumber']", { timeout: 10000 }).should('be.visible'),
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
     * Generate a random org name and enter into the organisation name field 
     * @param {*} name 
     */
    enterOrganisationName(name) {
        if (name.toLowerCase() === 'random') {
            localStorage.setItem("OrganisationName", `QAAutomationOrg_${Date.now()}`);
        } else {
            localStorage.setItem("OrganisationName", name);
        }
        this.elements.organisationNameTextField().clear();
        this.elements.organisationNameTextField().type(localStorage.getItem("OrganisationName"));
        cy.log("OrganisationName="+localStorage.getItem("OrganisationName"))
    }

    /**
     * Generate a random company number and enter into the Companies house number field
     * @param {*} number 
     */
    enterCompaniesHouseNumber(number) {
        if (number) {
            if (number.toLowerCase() === 'random') {
                localStorage.setItem("CompaniesHouseNumber", `QAAutomationCompanyNumber_${Date.now()}`);
            } else {
                localStorage.setItem("CompaniesHouseNumber", number);
            }
            this.elements.companiesHouseNumberTextField().clear();
            this.elements.companiesHouseNumberTextField().type(localStorage.getItem("CompaniesHouseNumber"));
        }        
    }

    /**
     * Click on Continue button
     */
    clickContinue() {
        this.elements.continueButton().click();
    }

}

export default CHMMManufacturerOnboardingResponsibleUndertakingPage;