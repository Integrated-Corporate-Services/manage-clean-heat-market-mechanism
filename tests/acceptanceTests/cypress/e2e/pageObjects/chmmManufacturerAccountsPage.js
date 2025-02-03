require('@cypress/xpath');

class CHMMManufacturerAccountsPage {

    /****************** page objects *****************/

    elements = {
        maufacturerAccountsTableBody: () => cy.xpath("//manufacturers-table//tbody", { timeout: 10000 }).should('be.visible'),
        showArchivedCheckbox: () => cy.xpath("//input[@id='showArchived']", { timeout: 10000 }).should('be.enabled'),
        searchTextfield: () => cy.xpath("//input[@id='search']", { timeout: 10000 }).should('be.visible'),
        manufacturerAccountsTableRowOne: () => cy.xpath("//table/tbody/tr[1]", { timeout: 10000 }).should('be.visible')
    }

    /****************** methods *****************/

    /**
     * Get the row element from the manufacturer accounts table using organisation name
     * @param {*} orgName 
     * @returns 
     */
    getManufacturerAccountsTableRow(orgName) {
        return this.elements.maufacturerAccountsTableBody().contains(orgName).parents('tr');
    }

    /**
     * Get the Manufacturer accounts table body
     * @returns 
     */
    getManufacturerAccountsTableBody() {
        return this.elements.maufacturerAccountsTableBody();
    }

    /**
     * Check the show retired checkbox
     */
    checkShowArchivedCheckbox() {
        this.elements.showArchivedCheckbox().check();
    }

    /**
     * UnCheck the show retired checkbox
     */
    uncheckShowArchivedCheckbox() {
        this.elements.showArchivedCheckbox().uncheck();
    }

    /**
     * Search for the given organisation name
     * @param {*} orgName 
     */
    searchForOrganisation(orgName) {
        this.elements.searchTextfield().clear();
        this.elements.searchTextfield().type(orgName);
    }

    /**
     * Get first row of the manufacturer accounts table
     * @returns 
     */
    getManufacturerAccountsTableRowOne() {
        return this.elements.manufacturerAccountsTableRowOne();
    }

    /**
    * Click on manufacturer organisation link for the created organisation
    **/
    clickonManufacturerOrganisationLink(orgName) {
        if (orgName.toLowerCase() === "random" || orgName.toLowerCase() === "newrandom") {
            orgName = localStorage.getItem("OrganisationName")
        }
        this.searchForOrganisation(orgName);
        var locator = "//a[contains(text(), '" + orgName + "')]"
        cy.xpath(locator).should('be.visible').click();
    }

    /**
     * Click on the given link
     * @param {*} linkText 
     */
    clickOnLink(linkText) {
        cy.xpath("//a[contains(text(), '" + linkText + "')]", { timeout: 10000 }).should('be.visible').click();
    }


}

export default CHMMManufacturerAccountsPage;