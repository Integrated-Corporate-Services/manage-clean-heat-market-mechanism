require('@cypress/xpath');

class chmmQuarterlyBoilerSalesSummaryPage {

    /****************** page objects *****************/

    elements = {

        quarterlyGasSubmissionValue: () => cy.xpath("//*[@id='main-content']/boiler-sales-summary/table/tbody/tr[1]/td[3]", { timeout: 1000 }).should('be.visible'),
        quarterlyOilSubmissionValue: () => cy.xpath("//*[@id='main-content']/boiler-sales-summary/table/tbody/tr[1]/td[4]", { timeout: 1000 }).should('be.visible')
      
    }

     /**
         * Check that quarterly Gas boiler sales values are same as expected
         * @param {*} exceptionValue 
         */
     checkQuarterlyGasBoilerSalesValues(exceptionValue) {
        this.elements.quarterlyGasSubmissionValue().invoke('text').invoke('trim').as('actualText')
        cy.get('@actualText').then((actText)=> {
            var text = actText.replace(',','');
            cy.wrap(text).should('eq', exceptionValue)
        }) 
    }

    /**
     * Check that quarterly Oil boiler sales values are same as expected
     * @param {*} exceptionValue 
     */
    checkQuarterlyOilBoilerSalesValues(exceptionValue) {
        this.elements.quarterlyOilSubmissionValue().invoke('text').invoke('trim').as('actualText')
        cy.get('@actualText').then((actText)=> {
            var text = actText.replace(',','');
            cy.wrap(text).should('eq', exceptionValue)
        }) 
    }

}

export default chmmQuarterlyBoilerSalesSummaryPage