require('@cypress/xpath');

class CHMMLicenceHoldersPage {

    /****************** page objects *****************/

    elements = {

        linkLicenceHolderBurron: () => cy.xpath("//a[contains(text(), 'Link licence holder')]", { timeout: 10000 }).should('be.visible'),
        licenceHoldersDropdown: () => cy.xpath("//select[@id='selectedLicenceHolder']", { timeout: 10000 }).should('be.visible'),
        linkButton: () => cy.xpath("//button[contains(text(), ' Link ')]", { timeout: 10000 }).should('be.visible'),
        cancelButton: () => cy.xpath("//a[contains(text(), 'Cancel')]", { timeout: 10000 }).should('be.visible')
    }

    /******************** methods *******************/

    /**
     * Click on Link licence holder button
     */
    clickOnLinkLicenceHolderButton() {
        this.elements.linkLicenceHolderBurron().click();
    }

    /**
     * Select given option from the licence holders Dropdown
     * @param {*} name 
     */
    selectLicenceHolder(name) {
        cy.get('select').select(name);
    }

    /**
     * Click on Link button
     */
    clickOnLinkButton() {
        this.elements.linkButton().click();
    }

    /**
     * Click on cancel button
     */
    clickOnCancelButton() {
        this.elements.cancelButton().click();
    }

    /**
     * Click on unlink for the given licence holder name
     * @param {*} licenceHolderName 
     */
    unlinkLicenceHolder(licenceHolderName) {
        cy.xpath("//div[contains(text(), '"+licenceHolderName+"')]/following-sibling::button[1]", { timeout: 10000 }).should('be.visible').click();
    }


}

export default CHMMLicenceHoldersPage;