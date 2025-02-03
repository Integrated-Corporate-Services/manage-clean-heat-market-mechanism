/// <reference types="Cypress" />

import MenuPage from "../../e2e/pageObjects/chmmMenuPage";
import AddNotesPage from "../../e2e/pageObjects/chmmAddNotesPage";

When("the user clicks on notes link", function() {
    const menuPage = new MenuPage();
    menuPage.clickOnNotesLink();
    })

    When("the user clicks on add notes button", function() {
        const addNotesPage = new AddNotesPage();
        addNotesPage.clickOnAddNotesButton();
        })

    When("the user add details {string} in add notes", function(details) {
        const addNotesPage = new AddNotesPage();
        addNotesPage.addDetailsForNotes(details);

        })

        When("the user selects and upload files {string}",function(files){
            const addNotesPage = new AddNotesPage();
            addNotesPage.uploadFiles(files);
            cy.wait(3000);

        })


        When("the user clicks on Add note button", function(){
            const addNotesPage = new AddNotesPage();
            addNotesPage.clickOnAddNotesButtonOnAddNotesPage();
        })