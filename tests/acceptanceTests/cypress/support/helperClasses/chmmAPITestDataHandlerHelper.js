
class CHMMApiTestDataHandlerHelper {

    static requestJsonBody;
    static schemeYearId;
    static mcsManufacturerId;
    static mcsManufacturerName;

    /**
 * Setter function for request Json body
 * @param {*} jsonBody 
 */
    setRequestJsonBody(jsonBody) {
        CHMMApiTestDataHandlerHelper.requestJsonBody = jsonBody;
    }

    /**
     * Getter function for requestJsonBody
     * @returns 
     */
    getRequestJsonBody() {
        return CHMMApiTestDataHandlerHelper.requestJsonBody;
    }

    /**
     * Setter for schemeYearId`
     */
    set schemeYearId(id) {
        CHMMApiTestDataHandlerHelper.schemeYearId = id;
    }

    /**
     * Getter for schemeYearId
     */
    get schemeYearId() {
        return CHMMApiTestDataHandlerHelper.schemeYearId;
    }

    /**
     * Setter for mcsManufacturerId
     * @param {*} id 
     */
    setMcsManufacturerId(id) {
        CHMMApiTestDataHandlerHelper.mcsManufacturerId = id;
    }

    /**
     * Getter for mcsManufacturerId
     * @returns 
     */
    getMcsManufacturerId() {
        return CHMMApiTestDataHandlerHelper.mcsManufacturerId;
    }

    /**
     * Setter for mcsManufacturerName
     * @param {*} name 
     */
    setMcsManufacturerName(name) {
        CHMMApiTestDataHandlerHelper.mcsManufacturerName = name;
    }

    /**
     * Getter for mcsManufacturerName
     * @returns 
     */
    getMcsManufacturerName() {
        return CHMMApiTestDataHandlerHelper.mcsManufacturerName;
    }


}

export default CHMMApiTestDataHandlerHelper;