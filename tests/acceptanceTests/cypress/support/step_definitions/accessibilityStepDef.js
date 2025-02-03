/// <reference types="Cypress" />

const a11yOptions = {
  runOnly: {
    type: 'tag',
    values: ['wcag2aa']
  }
}

When("the user validate the page for accessibility violation", function () {
  cy.injectAxe();
  cy.configureAxe({
    iframes: true,
    reporter: "v2"
  })

})

Then("there should be no violation", function () {
  cy.checkA11y(null, a11yOptions);

})
