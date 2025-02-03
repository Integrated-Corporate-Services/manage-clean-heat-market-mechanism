require('@cypress/xpath');

class chmmAdminViewOrganisationdetailsPage {

    /****************** page objects *****************/


    elements = {

            viewOrganisationApproveButton: () => cy.xpath("//button[contains(text(), 'Approve')]", { timeout: 10000 }).should('be.visible'),
        
            }
     /******************** methods *******************/

     /**
     * Click on approve button on view organisation page
     **/

     clickOnViewOrganisationApproveButton() {
        
        this.elements.viewOrganisationApproveButton().click();
    }

}


export default chmmAdminViewOrganisationdetailsPage;