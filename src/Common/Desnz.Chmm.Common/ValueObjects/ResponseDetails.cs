namespace Desnz.Chmm.Common.ValueObjects
{

    /// <summary>
    /// Summarises the details of the response for auditing
    /// </summary>
    public class ResponseDetails
    {
        /// <summary>
        /// Builds the response details
        /// </summary>
        /// <param name="success">Was the request successful?</param>
        /// <param name="message">What is the sumary message?</param>
        public ResponseDetails(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        /// <summary>
        /// Returns a "default" ResponseDetails
        /// </summary>
        public static ResponseDetails Default { get { return new ResponseDetails(true, ""); } }

        /// <summary>
        /// Was the request successful?
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Details about the response
        /// </summary>
        public string Message { get; private set; }
    }
}
