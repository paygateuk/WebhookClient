using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebhookClient.Models
{
    /// <summary>
    /// Payload sent to a customer's callback URI endpoint.
    /// </summary>
    public class WebhookPayload
    {
        /// <summary>
        /// Submission ID.
        /// </summary>
        public Guid SubmissionId { get; set; }

        /// <summary>
        /// Would be "Approve" following a manual Sign action.
        /// Would be "Send" following a manual Approve action.
        /// </summary>
        public string NextAction { get; set; }

        /// <summary>
        /// Date payload sent to customer endpoint.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Currently always set to 1.
        /// This version doesn't track whether the customer has responded to the callback but this may be implemented in a future version.
        /// </summary>
        public int Attempt { get; set; }
    }
}
