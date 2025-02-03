require('@cypress/xpath');

import AddNewUserPage from "../../e2e/pageObjects/chmmAddNewUserPage";

class CHMMEditUserPage {

    /****************** page objects *****************/

    elements = {

        deactivateButton: () => cy.xpath("//button[contains(text(), 'Deactivate')]", { timeout: 10000 }).should('be.visible'),
        activateButton: () => cy.xpath("//button[contains(text(), 'Activate')]", { timeout: 10000 }).should('be.visible'),
        nameTextField: () => cy.xpath("//input[@id='name']").should('be.visible'),
        emailField:() => cy.xpath("//input[@id='email']", { timeout: 10000 }).should('be.visible'),
        permissionLevelDropdown: () => cy.xpath("//select[@id='permission']").should('be.visible'),
        saveButton: () => cy.xpath("//button[contains(text(), 'Save')]").should('be.visible'),
        cancelButton: () => cy.xpath("//button[contains(text(), 'Cancel')]").should('be.visible'),
        saveAndSendNotificationButton: () => cy.xpath("//button[contains(text(), 'Save and send notification')]").should('be.visible'),
        adminStatus: () => cy.xpath("//admin-list-status/span").should('be.visible')

    }

    /******************** methods *******************/

    /**
     * Click on Deactivate button
     */
    clickOnDeactivateButton() {   
        this.elements.adminStatus().then(($status) => {
            if ($status.hasClass('govuk-tag--green')) {
                this.elements.deactivateButton().click();
            } else {
                cy.reload();
                this.elements.deactivateButton().click();
            }
        }) 
    }

    /**
     * Click on activate button
     */
    clickOnActivateButton() {
        this.elements.adminStatus().then(($status) => {
            if ($status.hasClass('govuk-tag--green')) {
                this.elements.activateButton().click();
            } else {
                cy.reload();
                this.elements.activateButton().click();
            }
        })           
    }

    /**
     * Click on cancel button
     */
    clickOnCancelButton() {
        this.elements.cancelButton().click();
    }

    /**
     * Edit admin user
     * @param {*} datatable 
     */
    editAdminUser(datatable) {

        datatable.hashes().forEach((element) => {           
            this.elements.nameTextField().clear();
            this.elements.nameTextField().type(element.name);
           // this.elements.permissionLevelDropdown().clear();
            this.elements.permissionLevelDropdown().select(element.permission);
            this.elements.saveButton().click();
        })        
    }

    /**
     * Cancel Edit admin user
     * @param {*} datatable 
     */
    cancelEditAdminUser(datatable) {
        const addNewUserPage = new AddNewUserPage();

        datatable.hashes().forEach((element) => {
           
            this.elements.nameTextField().clear();
            this.elements.nameTextField().type(element.name);
           // this.elements.permissionLevelDropdown().clear();
            this.elements.permissionLevelDropdown().select(element.permission);
            this.elements.cancelButton().click();
        })
    }

    /**
     * Get the text from Admin user status element
     * @returns 
     */
    getAdminUserStatus() {
        var text;
        this.elements.adminStatus().should(($div) => {
            text = $div.text();
        })
        return text;
    }

}

export default CHMMEditUserPage;