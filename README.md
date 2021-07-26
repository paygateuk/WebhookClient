# WebhookClient
Example of a client handling a paygate API callback where a user manually approves a submission.

After a manual approval, paygate can send a callback to a customer endpoint if one has been registered. 
This would allow the customer application to create the "Send" submission request to the BACS API.

The payload sent to the customer endpoint is signed using the same API key used to create the OAuth 2.0 token used to access the BACS API. 
The signature hash is sent in the headers.
The endpoint can use the API key to verify that the payload has originated from paygate.
