require('@cypress/xpath');

import MenuPage from "../../e2e/pageObjects/chmmMenuPage";

class CHMMAddNewUserPage {
    /**************** Class static variables  ******************/


    /****************** page objects *****************/

    elements = {

        addUserButton: () => cy.xpath("//a[contains(text(), 'Add user')]", { timeout: 10000 }).should('be.visible'),
        nameTextField: () => cy.xpath("//input[@id='name']", { timeout: 10000 }).should('be.visible'),
        emailAddressTextField: () => cy.xpath("//input[@id='email']", { timeout: 10000 }).should('be.visible'),
        permissionLevelDropdown: () => cy.xpath("//select[@id='permission']", { timeout: 10000 }).should('be.visible'),
        continueButton: () => cy.xpath("//button[contains(text(), 'Continue')]", { timeout: 10000 }).should('be.visible'),
        cancelButton: () => cy.xpath("//button[contains(text(), 'Cancel')]", { timeout: 10000 }).should('be.visible'),
        ChecktheuserdetailsbeforeaddingtheuseraddUserButton: () => cy.xpath("//button[contains(text(), ' Add user ')]", { timeout: 10000 }).should('be.visible')

    }

    /******************** methods *******************/

    /**
     * Navigate to Add new user page
     */
    navigateToAddNewUserPage() {
        const menuPage = new MenuPage();
        menuPage.navigateToAdministratorAccountsPage();
        this.elements.addUserButton().click();
    }

    /**
     * Add new user
     * @param {*} datatable 
     */
    addNewUser(datatable) {
        datatable.hashes().forEach((element) => {
            if (element.email.toLowerCase() === 'random') {
                localStorage.setItem("EmailId", `QAAutomation_${Date.now()}@example.com`);
            } else {
                localStorage.setItem("EmailId", element.email);
            }
            this.elements.nameTextField().clear();
            this.elements.nameTextField().type(element.name);
            this.elements.emailAddressTextField().clear();
            this.elements.emailAddressTextField().type(localStorage.getItem("EmailId"));
            //this.elements.permissionLevelDropdown().select(element.permission);
            this.elements.continueButton().click();
        })
    }

    /**
     * Click on Save and send notification button
     */
    clickOnSaveAndSendNotificationButton() {
        this.elements.ChecktheuserdetailsbeforeaddingtheuseraddUserButton().click();
    }


}

export default CHMMAddNewUserPage;