require('@cypress/xpath');
import EditUserPage from "../../e2e/pageObjects/chmmEditUserPage";
import AdminAccountsPage from "../../e2e/pageObjects/chmmAdminAccountsPage";

class CHMMActivateAndDeactivateAdminUserPage {

    /****************** page objects *****************/

    elements = {

        yesDeactivateThisUserButton: () => cy.xpath("//button[contains(text(), 'Yes, deactivate this user')]", { timeout: 10000 }).should('be.visible'),
        noReturnToListButton: () => cy.xpath("//button[contains(text(), 'No, return to list')]", { timeout: 10000 }).should('be.visible'),
        yesActivateThisUserButton: () => cy.xpath("//button[contains(text(), 'Yes, activate this user')]", {timeout: 10000 }).should('be.visible')
    }

    /******************** methods *******************/

    /**
     * Deactivate an active admin user with given name and email
     * @param {*} name 
     * @param {*} email 
     */
    deactivateAdminUser(name, email) {        
        cy.contains('tr', email).invoke('text').then((text) => {
            if (text.includes('Active')) {                
                const adminAccountsPage = new AdminAccountsPage();
                adminAccountsPage.navigateToAdminUserDetailsPage(name, email);
                cy.wait(6000);
                cy.reload();
                cy.wait(6000);  
                const editUserPage = new EditUserPage();
                editUserPage.clickOnDeactivateButton();              
                this.elements.yesDeactivateThisUserButton().click();
            }
        })        
    }

    /**
     * Click on Yes Deactivate this user button
     */
    clickYesDeactivateThisUserButton() {
        this.elements.yesDeactivateThisUserButton().click();
    }

    /**
     * Click on No Return to List button
     */
    clickNoReturnToListButton() {
        this.elements.noReturnToListButton().click();
    }

    /**
     * Activate an inactive admin user with given name and email
     * @param {*} name 
     * @param {*} email 
     */
    activateAdminUser(name, email) {
        cy.contains('tr', email).invoke('text').then((text) => {
            cy.log("Element text = " + text)
            if (text.includes('Inactive')) {
                const adminAccountsPage = new AdminAccountsPage();
                adminAccountsPage.navigateToAdminUserDetailsPage(name, email);                 
                const editUserPage = new EditUserPage();
                cy.wait(6000);
                cy.reload();
                cy.wait(6000); 
                editUserPage.clickOnActivateButton();              
                this.elements.yesActivateThisUserButton().click();                    
            }
        })
    }

    /**
     * Click on Yes Activate this user button
     */
    clickYesActivateThisUserButton() {
        this.elements.yesActivateThisUserButton().click();
    }


}

export default CHMMActivateAndDeactivateAdminUserPage;