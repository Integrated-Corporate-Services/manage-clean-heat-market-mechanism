require('@cypress/xpath');

class CHMMManufacturerViewManageUserPage {


    /****************** page objects *****************/

    elements = {
        maufacturerManageUsersTable: () => cy.xpath("//manufacturer-users-table//tbody", { timeout: 10000 }).should('be.visible')

    }

    /****************** methods *****************/

    /**
     * Get Manufacturer manage user table row element with given email id
     * @param {*} email 
     * @returns 
     */
    getManufacturerManageUserRow(email) {
        cy.log(email);
        return this.elements.maufacturerManageUsersTable().contains(email).parent();        
    }
}

export default CHMMManufacturerViewManageUserPage;
