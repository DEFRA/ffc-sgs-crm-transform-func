# ffc-sgs-crm-transform-func
Manages Integrations to/from the SGS Technical Spike PoC Dynamics 365 instance using listener functions within the Azure Function, comprising:

## Sending notifications to GOV.UK Notify

Listener created for Service Bus message from Dynamics that is sent to queue: **ffc-sgs-notification-queue-request**

Payload constructed and sent to Notification Queue: **ffc-sgs-notification-queue**

## Sending requests to Eligibility Check part of Agreement Calculator microservice

Listener created for Service Bus message from Dynamics that is sent to queue: **ffc-sfi-eligibility-check-request**

Payload constructed and sent to Eligibility Request Topic: **ffc-sfi-eligibility-check**

## Receiving responses from Eligibility Check part of Agreement Calculator microservice

Listener created for response from Microservice that is sent to queue: **ffc-sfi-eligibility-check-response**
