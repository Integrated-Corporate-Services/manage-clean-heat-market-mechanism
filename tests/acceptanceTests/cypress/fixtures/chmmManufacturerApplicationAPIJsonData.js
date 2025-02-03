/******************* Json ***************/

/**
 * Manufacturer onboarding application json
 */
let manufacturerOnboardingAPIData = {
	isOnBehalfOfGroup: null,
	responsibleUndertaking: {
		name: null,
		companiesHouseNumber: null,
	},
	addresses: [
		{
			lineOne: "YD Address1",
			lineTwo: "YD Address2",
			city: "YD Town1",
			county: "YD County1",
			postcode: "YD11 3AA",
			isUsedAsLegalCorrespondence: false
		},
		{
			lineOne: "LC Address1",
			lineTwo: "LC address2",
			city: "LC town",
			county: "LC county",
			postcode: "LC11 3AA",
			isUsedAsLegalCorrespondence: true
		}
	],
	isFossilFuelBoilerSeller: null,
	isNonSchemeParticipant: true,
	legalAddressType: "Use Specified Address",
	heatPumpBrands: [
		"Brand1",
		"Brand2"
	],
	users: [],
	creditContactDetails: null,
}

/**
 * Annual boiler sales api json data
 */
let annualBoilerSalesAPIData = {
	"organisationId": null,
	"schemeYearId": null,
	"oil": 0,
	"gas": 0
}

/***************** Functions **************/

/**
 * Set the value of isOnBehalfOfGroup
 * @param {*} isOnBehalfOfGroup 
 * @returns 
 */
function isOnBehalfOfGroup(isOnBehalfOfGroup) {
	if (isOnBehalfOfGroup == 'true' || isOnBehalfOfGroup == 'false') {
		return isOnBehalfOfGroup == 'true';
	} else {
		return isOnBehalfOfGroup;
	}	
}

/**
 * Set the value of isFossilFuelBoilerSeller
 * @param {*} isFossilFuelBoilerSeller 
 * @returns 
 */
function isFossilFuelBoilerSeller(isFossilFuelBoilerSeller) {
	if (isFossilFuelBoilerSeller == 'true' || isFossilFuelBoilerSeller == 'false') {
		return isFossilFuelBoilerSeller == 'true';
	} else {
		return isFossilFuelBoilerSeller;
	}
}

/**
 * Set the value of isNonSchemeParticipant
 * @param {*} isNonSchemeParticipant 
 * @returns 
 */
function isNonSchemeParticipant(isNonSchemeParticipant) {
	if (isNonSchemeParticipant == 'true' || isNonSchemeParticipant == 'false') {
		return isNonSchemeParticipant == 'true';
	} else {
		return isNonSchemeParticipant;
	}
}

/**
 * Set random organisation name
 * @param {*} name 
 * @returns 
 */
function organisationName(name) {
	if (name.toLowerCase() === 'random' || name.toLowerCase() === 'newrandom') {
		return `APIMfrOrg_${Date.now()}`;
	} else {
		return name;
	}
}

/**
 * Set random company name
 * @param {*} number 
 * @returns 
 */
function companiesHouseNumber(number) {
	if (number.toLowerCase() === 'random' || number.toLowerCase() === 'newrandom') {
		return `APICompanyNumber_${Date.now()}`;
	} else {
		return number;
	}
}

/**
 * Generate a random email id for Manufacturer user to be used in API call
 * @param {*} emailId 
 * @returns 
 */
function manufacturerUserEmailId(emailId) {
	if (emailId != null && (emailId.toLowerCase() === 'random' || emailId.toLowerCase() === 'newrandom')) {
		return `APIMfrUser_${Date.now()}@example.com`;
	} else {
		return emailId;
	}
}

/**
 *  Generate a random email id for manufacturer SRO user to be used in API call
 * @param {*} emailId 
 * @returns 
 */
function manufacturerSROUserEmailId(emailId) {
	if (emailId != null && (emailId.toLowerCase() === 'random' || emailId.toLowerCase() === 'newrandom')) {
		return `APIMfrSROUser_${Date.now()}@example.com`;
	} else {
		return emailId;
	}
}

/**
 * Set manufacturer user details depending in if it is same user as SRO or different user
 * @param {*} isSRO 
 * @param {*} mfrUserEmailId 
 * @param {*} sroUserId 
 * @returns 
 */
function responsibleOfficerUser(isSRO, mfrUserEmailId, sroUserId) {
	let userDetails = null;
	let isSroOfficer = null;
	if (isSRO === 'true' || (isSRO === 'false')) {
		isSroOfficer = (isSRO === "true");
	} else {
		isSroOfficer = isSRO;
	}
	 
	if (isSRO == 'true') {
		userDetails = [{
			"name": "API Test user",
			"jobTitle": "API Test Job",
			"responsibleOfficerOrganisationName": null,
			"email": mfrUserEmailId,
			"telephoneNumber": "123123123",
			"isResponsibleOfficer": isSroOfficer
		}]
	} else {
		userDetails = [{
			"name": "API Test user",
			"jobTitle": "API Test Job",
			"responsibleOfficerOrganisationName": null,
			"email": mfrUserEmailId,
			"telephoneNumber": "123123123",
			"isResponsibleOfficer": isSroOfficer
		},
		{
			"name": "API Test SRO FN",
			"jobTitle": "API Test SRO Job",
			"organisation": "API Test Org",
			"responsibleOfficerOrganisationName": "API Test SRO Org",
			"email": sroUserId,
			"telephoneNumber": "123123123",
			"isResponsibleOfficer": true
		}]

	}

	return userDetails;
}

/**
 * Generate a random email for manufacturer credit transfer contact details
 * @param {*} emailId 
 * @returns 
 */
function mfrCreditContactEmailId(emailId) {
	if (emailId != null && (emailId.toLowerCase() === 'random' || emailId.toLowerCase() === 'newrandom')) {
		return `APIMfrCreditContact_${Date.now()}@example.com`;
	} else {
		return emailId;
	}
}

/**
 * Set Credit transfer contact details depending on if they has opted in or not
 * @param {*} hasOptedIn 
 * @param {*} creditContactEmailId 
 * @returns 
 */
function creditContactDetails(hasOptedIn, creditContactEmailId) {
	var contactDetails;
	if (hasOptedIn === 'true') {
		contactDetails = {
			"hasOptedIn": "Yes",
			"name": "CT Name",
			"emailAddress": creditContactEmailId,
			"telephoneNumber": "123123123",
			"email": creditContactEmailId
		}
	} else {
		contactDetails = {
			"hasOptedIn": "No",
			"name": null,
			"emailAddress": null,
			"telephoneNumber": null,
			"email": null
		}
	}
	return contactDetails;
}

/**
 * Set address depending on the legalAddressType
 * @param {*} legalAddressType 
 */
function address(legalAddressType) {
	var address;
	if (legalAddressType === 'No Legal Correspondence Address') {
		address = [
			{
				lineOne: "YD Address1",
				lineTwo: "YD Address2",
				city: "YD Town1",
				county: "YD County1",
				postcode: "YD11 3AA",
				isUsedAsLegalCorrespondence: false
			}	
		]	
	} else {
		address = [
			{
				lineOne: "YD Address1",
				lineTwo: "YD Address2",
				city: "YD Town1",
				county: "YD County1",
				postcode: "YD11 3AA",
				isUsedAsLegalCorrespondence: false
			},
			{
				lineOne: "LC Address1",
				lineTwo: "LC address2",
				city: "LC town",
				county: "LC county",
				postcode: "LC11 3AA",
				isUsedAsLegalCorrespondence: true
			}
		]
	}
	return address;
}

/*************** Create Json Helpers ***********/

/**
 * Create and store the Manufacturer Onboarding API Json body
 * @param {*} datatable 
 * @returns 
 */
export function createManufacturerOnboardingAPIJson(datatable) {	
	datatable.hashes().forEach((element) => {
		switch (element.key) {
			case "isOnBehalfOfGroup":
				manufacturerOnboardingAPIData.isOnBehalfOfGroup = isOnBehalfOfGroup(element.value)
				break;
			case "organisationName":
				manufacturerOnboardingAPIData.responsibleUndertaking.name = organisationName(element.value);
				localStorage.setItem('OrganisationName', manufacturerOnboardingAPIData.responsibleUndertaking.name);
				break;
			case "companyNumber":
				manufacturerOnboardingAPIData.responsibleUndertaking.companiesHouseNumber = companiesHouseNumber(element.value);
				break;			
			case "isResponsibleOfficer":				
				let mfrUserEmailId = manufacturerUserEmailId(findValueByKey(datatable, "manufacturerUserEmailId"));
				localStorage.setItem("mfrUserEmailId", mfrUserEmailId);
				let sroUserId = manufacturerSROUserEmailId(findValueByKey(datatable, "manufacturerSROUserEmailId"));
				localStorage.setItem("sroUserEmailId", sroUserId);
				manufacturerOnboardingAPIData.users = responsibleOfficerUser(element.value, mfrUserEmailId, sroUserId);
				break;
			case "creditTransferOptIn":
				let creditContactEmailId = mfrCreditContactEmailId(findValueByKey(datatable, "creditTransferEmailId"));
				manufacturerOnboardingAPIData.creditContactDetails = creditContactDetails(element.value, creditContactEmailId)
				break;
			case "isFossilFuelBoilerSeller":
				manufacturerOnboardingAPIData.isFossilFuelBoilerSeller = isFossilFuelBoilerSeller(element.value);
				break;
			case "isNonSchemeParticipant":
				manufacturerOnboardingAPIData.isNonSchemeParticipant = isNonSchemeParticipant(element.value);
				break;
			case "legalAddressType":
				manufacturerOnboardingAPIData.legalAddressType = element.value;
				manufacturerOnboardingAPIData.addresses = address(element.value);
				break;
		}
	})

	cy.log("Json = " + JSON.stringify(manufacturerOnboardingAPIData));
	return manufacturerOnboardingAPIData;
}

/**
 * Get the value of a key in the given data table
 * @param {*} dataTable 
 * @param {*} keyToFind 
 * @returns 
 */
export function findValueByKey(dataTable, keyToFind) {
	let arrayOfObjects = dataTable.hashes();
	const foundObject = arrayOfObjects.find(obj => obj.key === keyToFind);

	// Check if an object with the specified key was found
	if (foundObject) {
		return foundObject.value;
	} else {
		return null; // or any other default value or indication
	}
}

/**
 * Create and store annual boiler sales api request json body
 * @param {*} datatable 
 * @returns 
 */
export function createAnnualBoilerSalesAPIRequestJson(datatable) {
	datatable.forEach((element) => {
		switch (element.key) {
			case "organisationId":
				annualBoilerSalesAPIData.organisationId = element.value;
				break;
			case "schemeYearId":
				annualBoilerSalesAPIData.schemeYearId = element.value;
				break;
			case "oil":
				annualBoilerSalesAPIData.oil = parseFloat(element.value, 10);
				break;
			case "gas":
				annualBoilerSalesAPIData.gas = parseFloat(element.value, 10);
				break;
		}
	})
	cy.log('Annual boiler sales json = ' + JSON.stringify(annualBoilerSalesAPIData));
	return annualBoilerSalesAPIData;
}