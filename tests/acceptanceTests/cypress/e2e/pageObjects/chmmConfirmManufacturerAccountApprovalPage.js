require('@cypress/xpath');

class chmmConfirmManufacturerAccountApprovalPage {

    /****************** page objects *****************/


    elements = {
        addtionalCommentTextfield: () => cy.xpath("//textarea[@id='comments']", { timeout: 10000 }).should('be.visible'),
        additionalCommentsCheckbox: () => cy.xpath("//input[@id='confirmation']", { timeout: 10000 }),
        confirmManufacturerAccountApprovalPageApproveButton: () => cy.xpath("//button[contains(text(), 'Approve')]", { timeout: 10000 }).should('be.visible'),
        confirmManufacturerAccountApprovalPageCancelButton: () => cy.xpath("//a[contains(text(), ' Cancel ')]", { timeout: 10000 }).should('be.visible')
        
         }


    /**************** methods  ***********/

    addAdditionalComments(comments) {
        
        this.elements.addtionalCommentTextfield().type(comments);
        this.elements.additionalCommentsCheckbox().click();  
               
     }

      /**
     * Click on approve button on confirm manufacturer accounr approval page
     **/

     clickApproveOnManufacturerAccountApprovalPage(){

        this.elements.confirmManufacturerAccountApprovalPageApproveButton().click();
     }

 /**
     * Click on cancel button on confirm manufacturer accounr approval page
     **/

     clickCancelOnManufacturerAccountApprovalPage(){

        this.elements.confirmManufacturerAccountApprovalPageCancelButton().click();
     }
}

export default chmmConfirmManufacturerAccountApprovalPage;