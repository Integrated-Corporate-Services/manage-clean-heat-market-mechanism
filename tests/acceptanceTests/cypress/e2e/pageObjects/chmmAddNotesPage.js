require('@cypress/xpath');

import MenuPage from "../../e2e/pageObjects/chmmMenuPage";

class CHMMAddNotesPage {
    /**************** Class static variables  ******************/


    /****************** page objects *****************/

    elements = {

        addNotesButton: () => cy.xpath("//a[contains(text(), 'Add note')]", { timeout: 10000 }).should('be.visible'),
        
        addDetails: () => cy.xpath("//textarea[@id='details']", { timeout: 10000 }).should('be.visible'),

        addNotesButtonOnAddNotesPage: () => cy.xpath("//button[contains(text(), 'Add note')]", { timeout: 10000 }).should('be.visible'),

    }

    /******************** methods *******************/

    
    /**
     * Click on Save and send notification button
     */
    clickOnAddNotesButton() {
        this.elements.addNotesButton().click();
    }

    /**
     * Add details for notes 
     * @param {*} details 
     */
    addDetailsForNotes(details){
        this.elements.addDetails().clear();
        this.elements.addDetails().type(details);     
    }

    /**
     * click on add notes button on add notes page 
     */

    clickOnAddNotesButtonOnAddNotesPage() {
        this.elements.addNotesButtonOnAddNotesPage().click();
    }

    /**
         * upload verification statement 
         */
 uploadFiles(files)
 {
     cy.get("input[id='files']")
     .click()
     .attachFile(files);
 }
}

export default CHMMAddNotesPage;