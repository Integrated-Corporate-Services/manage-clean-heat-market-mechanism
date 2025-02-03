require('@cypress/xpath');

class CHMMAdminAccountsPage {

    /****************** page objects *****************/

    elements = {
        adminAccountsTable: () => cy.xpath("//admin-users-table//tbody", { timeout: 10000 }).should('be.visible')


    }

    /****************** methods *****************/

    /**
     * Get Admin accounts table row element with given email id
     * @param {*} email 
     * @returns 
     */
    getAdminAccountsRow(email) {
        cy.wait(3000);
        return this.elements.adminAccountsTable().contains(email).parent();        
    }

    /**
     * Navigate to the given admin user details page
     * @param {*} userName 
     * @param {*} email 
     */
    navigateToAdminUserDetailsPage(userName, email) {
        this.getAdminAccountsRow(email).contains(userName).click();
    }


}

export default CHMMAdminAccountsPage;