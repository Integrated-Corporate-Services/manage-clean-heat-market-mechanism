require('@cypress/xpath');

class chmmAreYouSurePage {

    /****************** page objects *****************/


    elements = {

        areYouSureApproveButton: () => cy.xpath("//button[contains(text(), ' Yes, approve ')]", { timeout: 10000 }).should('be.visible'),
        areYouSureCancelButton: () => cy.xpath("//a[contains(text(), ' No, return to application ')]", { timeout: 10000 }).should('be.visible'),   
        noReturnToBoilerSalesButton: () => cy.xpath("//a[contains(text(), ' No, return to application ')]", { timeout: 10000 }).should('be.visible'),   
   
      }

     /**************** methods  ***********/

      /**
     * Click on approve button on are you sure page
     **/

     clickOnYesApproveButton()
     {

        this.elements.areYouSureApproveButton().click(); 
     }

      /**
     * Click on cancel button on are you sure page
     **/

     clickOnYesCancelButton()
     {

        this.elements.areYouSureCancelButton().click(); 
     }


     /**
      * click on No return to boiler sales button 
      */
     clickOnNoReturnToBoilerSalesButton()
     {

      this.elements.noReturnToBoilerSalesButton().click(); 
   }
}

export default chmmAreYouSurePage;