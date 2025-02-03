require('@cypress/xpath');

class CHMMMenuPage {
    /****************** page objects *****************/

    elements = {
        administratorAccountsLink: () => cy.xpath("//a[contains(text(), 'Administrators')]", { timeout: 10000 }).should('be.visible'),
        manufacturersAccountsLink: () => cy.xpath("//a[contains(text(), 'Manufacturers')]", { timeout: 10000 }).should('be.visible'),
        backToadministratorAccountsLink: () => cy.xpath("//a[contains(text(), 'Back to admin accounts')]", { timeout: 10000 }).should('be.visible'),
        manufacturerUsersLink: () => cy.xpath("//a[contains(text(), 'Users')]", { timeout: 10000 }).should('be.visible'),
        BackButton: () => cy.xpath("//a[contains(text(), 'Back')]", { timeout: 10000 }).should('be.visible'),
        SignOutButton: () => cy.xpath("//a[contains(text(),' Sign out ')]", { timeout: 10000 }).should('be.visible'),
        boilerSalesLink: () => cy.xpath("//a[contains(text(),' Boiler sales ')]", { timeout: 10000 }).should('be.visible'),
        addNotes: () => cy.xpath("//a[contains(text(),' Notes ')]", { timeout: 10000 }).should('be.visible'),
        summaryLink: () => cy.xpath("//a[contains(text(), 'Summary')]", { timeout: 10000 }).should('be.visible'),
        schemeYear2024RadioButton: () => cy.xpath("//input[@id='d525e380-4aee-40e9-a7f0-1784d8cb49d9']", { timeout: 10000 }).should('be.enabled'),
        schemeYear2025RadioButton: () => cy.xpath("//input[@id='6aa95a01-5283-4975-a360-84a515b17360']", { timeout: 10000 }).should('be.enabled'),
        goButton: () => cy.xpath("//button[contains(text(),' Go ')]", { timeout: 10000 }).should('be.visible'),
        
    }

    /******************** methods *******************/

    /**
     * Nevigate to Admin accounts page
     */
    navigateToAdministratorAccountsPage() {
        this.elements.administratorAccountsLink().click().wait(2000);
        cy.reload();        
    }

    /**
     * Navigate to Admin accounts page
     */
    navigateBackToAdministratorAccountsPage() {
        this.elements.backToadministratorAccountsLink().click().wait(2000);
        cy.reload();
    }

    /**
     * Navigate to Manufacturer users page
     */
    navigateToManufacturerManageUsersPage() {
        this.elements.manufacturerUsersLink().click().wait(2000);
        cy.reload();
    }

    /**
     * Navigate to Manufacturer accounts page
     */
    navigateToManufacturerAccountsPage() {
        this.elements.manufacturersAccountsLink().click().wait(2000);
        cy.reload();
    }

    clickOnSummaryLink() {
        this.elements.summaryLink().click().wait(2000);
        cy.reload();
    }
    

    /**
     * Click on back link
     **/
    clickOnBackButton(){
        this.elements.BackButton().click().wait(2000);
    }

    /**
     * User sign out button 
     */
    UserSignOutButton(){
        this.elements.SignOutButton().click().wait(2000);
    }

    clickOnBoilerSalesLink(){
        this.elements.boilerSalesLink().click().wait(2000);
        cy.reload();
    }

    /**
     * click on notes link
     */
    clickOnNotesLink(){
        this.elements.addNotes().click().wait(2000);
        cy.reload();
    }
/**
 * select 2024 radio button
 */
    Schemeyearselection2024(){
        this.elements.schemeYear2024RadioButton().click();
    }

    /**
 * select 2025 radio button
 */
    Schemeyearselection2025(){
        this.elements.schemeYear2025RadioButton().click();
    }

/**
 * click on go button 
 */

 clickOnGoButton(){
        this.elements.goButton().click();
    }
  
}

export default CHMMMenuPage;