require('@cypress/xpath');

class chmmCreditTransferPage {

    /****************** page objects *****************/


    elements = {
        creditTransferButton: () => cy.xpath("//button[contains(text(), ' Transfer credits ')]", { timeout: 10000 }).should('be.visible'),
       numberOfCreditsTextField: () => cy.xpath("//input[@id='noOfCredits']", { timeout: 10000 }).should('be.visible'),
       amendCreditBalanceButton: () => cy.xpath("//button[contains(text(), ' Amend credit balance ')]", { timeout: 10000 }).should('be.visible'),
       addingCreditsRadioButton: () => cy.xpath("//input[@id='adding']", { timeout: 10000 }),
       removingCreditsRadioButton: () => cy.xpath("//input[@id='removing']", { timeout: 10000 }),
       numberOfCreditsTextFieldOnAmendCreditpage: () => cy.xpath("//input[@id='amount']", { timeout: 10000 }).should('be.visible'),
       confirmAndCompleteTransferButton: () => cy.xpath("//button[contains(text(), ' Confirm and complete transfer ')]", { timeout: 10000 }),
       
    }


    /**************** methods  ***********/
/**
 * Navigate to credit transfer page 
 */

    navigateToCreditTransferPage(date)
        {
            cy.wait(5000);
            cy.on('uncaught:exception', (err, runnable) => {
                return false
            })
            cy.url().then((url) => {
                cy.log('Current URL is: ' + url)
                cy.visit(Cypress.env("URL")+date)
              
                cy.wait(5000);
              })
           
      }

/**
 * click on credit transfer button
 */
     clickOnCreditTransferButton()
     {
        this.elements.creditTransferButton().click();
     }


     /**
      * select organisation from drop down
      * @param {*} org 
      */
     selectOrganisation(org) {
       
        cy.get('select[id="organisation"]').select(org)

    }

/**
 * select newly created organisation
 * @param {*} org 
 */
    selectNewlyCreatedOrganisation(org) {
    
        if (org.toLowerCase() === "random" || org.toLowerCase() === "newrandom") {
            org = localStorage.getItem("OrganisationName")
        }
        cy.get('select[id="organisation"]').select(org)
    }


    /**
     * enter number of credits 
     * @param {*} value 
     */
    enterNumberOfCredits(value){
        this.elements.numberOfCreditsTextField().clear();
        this.elements.numberOfCreditsTextField().type(value);     
    }

    /**
     * click on amned credit balance button
     */
    clickOnAmendCreditBalance()

    {
        this.elements.amendCreditBalanceButton().click();
    }

    /**
     * click on adding radio button
     */
    clickOnAddingCreditRadioButton()
    {
        this.elements.addingCreditsRadioButton().click();
    }

    /**
     * click on removing radio button
     */
    clickOnRemovingCreditRadioButton()
    {
        this.elements.removingCreditsRadioButton().click();
    }

    /**
     * enter number of credits on amend credit page 
     * @param {*} value 
     */
    enterNumberOfCreditsOnAmendCreditPage(value){
        this.elements.numberOfCreditsTextFieldOnAmendCreditpage().clear();
        this.elements.numberOfCreditsTextFieldOnAmendCreditpage().type(value);     
    }

    /**
     * click on confirm and complete transfer button
     */
    clickOnConfirmAndCOmpleteTransferButton()
    {
        this.elements.confirmAndCompleteTransferButton().click();
    }

    /**
     * click on change links on check your answers page 
     * @param {*} link 
     */
    ClickOnChangeLinkforCheckYourAnswer(link)
    {
     
          cy.xpath("//dt[contains(text(), '"+link+"')]/following-sibling::dd/a").should('be.visible').click();
  
    }
  
    /**
     * check values on credit balance calculation section
     * @param {*} datatable 
     */
    CheckValueOnCreditBalanceCalculationSection(datatable)
    {
        cy.wait(2000);
        datatable.hashes().forEach((element) => {
        cy.xpath("//th[contains(text(),'"+element.text+"')]/following-sibling::td").should('contain', element.value);
       })
       
    }
  }




export default chmmCreditTransferPage;