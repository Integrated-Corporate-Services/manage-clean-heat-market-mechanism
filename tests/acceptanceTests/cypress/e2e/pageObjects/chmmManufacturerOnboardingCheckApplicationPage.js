require('@cypress/xpath');

class CHMMManufacturerOnboardingCheckApplicationPage {

   
    /****************** Methods *****************/

    /**
     * Function asserting values for the keys on the page
     * @param {*} datatable 
     */

    CheckValueForOnBehalfOfGroupOfOrganisation(datatable)
    {
        cy.wait(2000);
        datatable.hashes().forEach((element) => {
        cy.xpath("//dt[contains(text(), '"+element.text+"')]/following-sibling::dd").should('contain', element.value);
       })
       
    }

    CheckValueForOnBehalfOfGroupOfOrganisationInsidecard(cardName,datatable)
    {
        cy.wait(2000);
        datatable.hashes().forEach((element) => {
      cy.xpath("//h2[contains(text(), '"+cardName+"')]/ancestor::div[@class='govuk-summary-card']//dt[contains(text(),'"+element.text+"')]/following-sibling::dd").should('contain',element.value); 
      
      })
       
    }
    
 /**
  * Click on change link for different section to edit 
  * @param {*} link 
  */

     ClickOnChangeLinkforManufacturer(link)
  {
   
        cy.xpath("//dt[contains(text(), '"+link+"')]/following-sibling::dd/a").should('be.visible').click();

  }

  ClickOnChangeLinkforManufacturerCard(link)
  {
   
        cy.xpath("//h2[contains(text(), '"+link+"')]/following-sibling::ul/li/a").should('be.visible').click();

  }

 

}

export default CHMMManufacturerOnboardingCheckApplicationPage;