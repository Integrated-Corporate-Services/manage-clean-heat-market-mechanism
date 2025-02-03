require('@cypress/xpath');

class chmmSummaryPage {


    /****************** methods *****************/
 /**
         * Ccheck obligation value above threshold 
         * @param {*} datatable 
         */
 CheckObligationCalculation(datatable)
 {
     cy.wait(2000);
     datatable.hashes().forEach((element) => {
     cy.xpath("//th[contains(text(), '"+element.text+"')]/following-sibling::td[1]/following-sibling::td[1]").should('contain', element.value);
    })
    
 }


 /**
  * check obligation total sales value
  * @param {*} datatable 
  */
 CheckTotalSalesObligationCalculation(datatable)
 {
     cy.wait(2000);
     datatable.hashes().forEach((element) => {
     cy.xpath("//th[contains(text(), '"+element.text+"')]/following-sibling::td[1]").should('contain', element.value);
    })
    
 }
}
export default chmmSummaryPage;