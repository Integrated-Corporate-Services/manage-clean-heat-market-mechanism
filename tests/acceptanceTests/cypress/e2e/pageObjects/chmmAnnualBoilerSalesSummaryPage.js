require('@cypress/xpath');

class chmmAnnualBoilerSalesSummaryPage {

    /****************** page objects *****************/

    elements = {

        boilerSalesLink: () => cy.xpath("//a[contains(text(), ' Boiler sales ')]", { timeout: 10000 }).should('be.visible'),
        annualBoilerSalesSubmitNowButton: () => cy.xpath("//a[contains(text(), 'Submit now')]", { timeout: 10000 }).should('be.visible'),
        numberOfGasBolierSalesTextField: () => cy.xpath("//input[@id='gas']", { timeout: 10000 }).should('be.visible'),
        numberOfOilBolierSalesTextField: () => cy.xpath("//input[@id='oil']", { timeout: 10000 }).should('be.visible'),
        confirmDetailsCheckbox: () => cy.xpath("//input[@type='checkbox']", { timeout: 10000 }),
        submitButton: () => cy.xpath("//button[contains(text(), 'Submit')]", { timeout: 10000 }).should('be.visible'),
        cancelButton: () => cy.xpath("//button[contains(text(), 'Cancel')]", { timeout: 10000 }).should('be.visible'),
        annualGasSubmissionValue: () => cy.xpath("//div[@class='govuk-grid-column-two-thirds']//dt[text()='Gas']/following-sibling::dd[1]", { timeout: 1000 }).should('be.visible'),
        annualOilSubmissionValue: () => cy.xpath("//div[@class='govuk-grid-column-two-thirds']//dt[text()='Oil']/following-sibling::dd[1]", { timeout: 1000 }).should('be.visible'),
        annualBoilerSalesApproveButton: () => cy.xpath("//a[contains(text(),' Approve ')]", { timeout: 10000 }).should('be.visible'),
       
    }
   


        /********* methods *****/

        /**
         * Naviagte annual boiler sales page
         */
        navigateToAnnualBoilerSalesPage()
        {
            this.elements.boilerSalesLink().click();
        }


        /**
         * Navigate to annual boiler sales with different status
         * @param {*} status 
         */
        navigateToAnnualBoilerSalesSummaryWithStatus(status)
        {
            cy.wait(5000);
            cy.on('uncaught:exception', (err, runnable) => {
                return false
            })
            cy.url().then((url) => {
                cy.log('Current URL is: ' + url)
                cy.visit(url+status)
              })
           cy.wait(5000);
      }


      /**
       * click on annual boiler sales submit button
       */
        clickOnAnnualBolierSalesSubmitNowButton()

        {
            this.elements.annualBoilerSalesSubmitNowButton().click();
        }

        /**
         * Enter annual boiler sales details 
         * @param {} datatable 
         */

        enterAnnualBoilerSalesDetails(datatable) {
            datatable.hashes().forEach((element) => {            
                this.elements.numberOfGasBolierSalesTextField().clear();
                this.elements.numberOfGasBolierSalesTextField().type(element.numberOfGasBoilerSales);     
               this.elements.numberOfOilBolierSalesTextField().clear();
                this.elements.numberOfOilBolierSalesTextField().type(element.numberOfOilBoilerSales);
                    
            }) 
        }


        /**
         * upload verification statement 
         */
        uploadVerificationStatement(verificationStatement)
        {
            cy.get("input[id='verificationStatement']")
            .click()
            .attachFile(verificationStatement);
        }

        /**
         * upload supporting evidence 
         * @param {} files 
         */
        uploadSupportingEvidence(supportingEvidence)
        {
            cy.get("input[id='supportingEvidence'] ")
            .click()
            .attachFile(supportingEvidence);
        }


        /**
         * click on confirm details checkbox 
         */
        clickOnConfirmDetialsCheckBox()
        
        {
            this.elements.confirmDetailsCheckbox().click();
        }

        /**
         * click on submit button 
         */

        clickOnSubmitButton()
        {
            this.elements.submitButton().click();
            cy.wait(2000);
        }

        /**
         * click on cancel button 
         */
        clickOnCancelButton()
        {
            this.elements.cancelButton().click();
        }


        /**
         * Checked the changed value after editing fields
         * @param {*} datatable 
         */
        CheckValueAfterChangingData(datatable)
        {
            cy.wait(2000);
            datatable.hashes().forEach((element) => {
            cy.xpath("//dt[contains(text(), '"+element.text+"')]/following-sibling::dd[1]").should('contain', element.value);
           })
           
        }

        /**
         * Check that Gas boiler sales values are same as expected
         * @param {*} exceptionValue 
         */
        checkAnnualGasBoilerSalesValues(exceptionValue) {
            this.elements.annualGasSubmissionValue().invoke('text').should('eq', exceptionValue);
        }

        /**
         * Check that Oil boiler slaes valeus are same as expected
         * @param {*} exceptionValue 
         */
        checkAnnualOilBoilerSalesValues(exceptionValue) {
            this.elements.annualOilSubmissionValue().invoke('text').should('eq', exceptionValue);
        }


        /**
         * click on annual boilers sales approve link
         */
        clickOnAnnualBoilerSalesApproveButton()
        {
            this.elements.annualBoilerSalesApproveButton().click();
            cy.wait(5000);
        }


    }


export default chmmAnnualBoilerSalesSummaryPage;