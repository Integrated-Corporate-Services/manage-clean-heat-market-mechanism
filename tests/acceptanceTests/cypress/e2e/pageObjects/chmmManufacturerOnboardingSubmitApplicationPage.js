require('@cypress/xpath');

class CHMMManufacturerOnboardingSubmitApplicationPage {

    /****************** Page objects *****************/

    elements = {
       
        acceptAndSendButton: () => cy.xpath("//button[contains(text(), 'Accept and send')]", { timeout: 10000 }),
        
    }

    /**
     * Click on Continue button
     */
    clickAcceptAndSend() {
        this.elements.acceptAndSendButton().click();
        cy.wait(2000);
    }
}

    export default CHMMManufacturerOnboardingSubmitApplicationPage;
