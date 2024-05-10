# Blackbird.io Plunet

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Plunet is a translation business management software solution. Its features include the creation of quotes, orders, assigning resources to jobs and invoicing. This Plunet app allows you to automate many of the labor-intensive tasks.

## Before setting up

Before you can connect you need to make sure that:

- You have a Plunet instance and have sufficient admin rights on this instance.
- You have a Plunet API user created.

## Connecting

1. Navigate to Apps, and identify the **Plunet** app. You can use search to find it.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My Plunet connection'.
4. Fill in the URL to the Plunet instance you want to connect to. You can usually copy this part of the URL when you are logged into your Plunet instance.
5. Fill in the username of your Plunet API user.
6. Fill in the password of your Plunet API user.
7. Click _Connect_.

### Troubleshooting

If you are unable to connect, please check the following:
1. The URL should look like `https://plunet-friend.plunet.com`. Without /PlunetAPI at the end.
2. You must sure that your URL is accessible from the our servers. To check this, you can try to open the URL with '/PlunetAPI?wsdl' at the end in your browser (you should see an XML document that describes the API).
3. Ensure that you don't have firewall rules that block the connection or whitelist of IP addresses that are allowed to connect to your Plunet instance (or if you do, make sure to whitelist the Blackbird IP addresses).

![1701864930160](image/README/1701864930160.png)

## Actions

### Contacts

- **Get customer contacts** Lists all the contacts related to a customer
- **Get contact** returns the contact's details
- **Get contact by external ID** finds the contact given the external ID field
- **Create contact** creates a new customer contact
- **Update contact** updates an existing customer contact

### Customers

- **Search customers** returns a list of customers based on numerous optional search parameters including:
  - Customer type
  - Email
  - Source language
  - Name 1 & Name 2
  - Status
- **Get customer**
- **Delete customer**
- **Create customer**
- **Update customer**

### Documents

- **Upload file** uploads a file to a specific folder of any entity that can have files attached to it
- **Download file** can download a file of any entity that can have files attached to it
- **Download all files in folder**

### Items

- **Search items** returns a list of items based on the following criteria, optionally the returned currency can be set as well:
  - Order or quote iD
  - Item status
  - Document status
- **Get item**
- **Create item**
- **Delete item**
- **Update item**
- **Get item pricelines** returns all the pricelines attached to this item
- **Create item priceline** Creates a new priceline entry for this item
- **Delete item priceline** Deletes a priceline from an item
- **Update item priceline** Updates a priceline on an item

Note that when creating/updating items, the source and target languages can be set (they must either both be provided or not). Blackbird will assign the correct existing language combination to the item or, if the language combination doesn't exist, the language combination will be created first on the order or quote.

### Jobs

- **Get item jobs** returns a list of jobs attached to a certain item
- **Get job**
- **Create job**
- **Delete job**
- **Update job**
- **Get job pricelines**
- **Create job priceline**
- **Delete job priceline**
- **Update job priceline**

### Orders

- **Search orders** returns a list of orders based on numerous optional search parameters including:
  - Date from
  - Date to
  - Source language
  - Target language
  - Status
  - Customer ID
  - Name
  - Description
  - Type
- **Get order**
- **Create order**
- **Delete order**
- **Update order**
- **Add language combination to order**

![1701867033221](image/README/1701867033221.png)

An example of the granularity at which orders (and other entities) can be selected.

### Payables

- **Search payables** returns a list of payables based on numerous optional search parameters including:
- **Get payable**
- **Update payable**

### Quotes

- **Search quotes** returns a list of quotes based on numerous optional search parameters including:
  - Date from
  - Date to
  - Date refers to (invoice date, value date, due date, paid date)
  - Exported
  - Status
  - Currency
  - Resource ID
- **Get quote**
- **Create quote**
- **Delete quote**
- **Update order**
- **Add quote combination to quote**

### Requests

- **Search requests** returns a list of requests based on numerous optional search parameters including:
  - Date from
  - Date to
  - Source language
  - Target language
  - Status
  - Customer type
  - Customer ID
- **Get request**
- **Create request**
- **Delete request**
- **Update request**
- **Add language combination to request**

### Resources

- **Search resource** returns a list of resources based on numerous optional search parameters including:
  - Contact ID
  - Resource type
  - Email
  - Name 1 & Name 2
  - Status
  - Source language
  - Target language
  - Working status
- **Get resource**

### Custom properties

- **Set property**
- **Get property**
- **Set text module**
- **Get text module**

## Events

All webhooks return all data for the entity they were invoked on. All _status changed_ events have an optional input field _new status_ which can be used to differentiate the new status after the event.

### Customers

- **On customer deleted**
- **On customer created**
- **On customer status changed**

### Items

- **On item deleted**
- **On item created**
- **On item status changed**
- **On item delivery date changed**

### Jobs

- **On job deleted**
- **On job created**
- **On job status changed**
- **On job delivery date changed**
- **On job start date changed**

### Orders

- **On order deleted**
- **On order created**
- **On order status changed**

### Quotes

- **On quote deleted**
- **On quote created**
- **On quote status changed**

### Requests

- **On request deleted**
- **On request created**
- **On request status changed**

### Resources

- **On resource deleted**
- **On resource created**
- **On resource status changed**

## Example

![1701866832560](image/README/1701866832560.png)

The following example shows how a bird can be setup that creates a new Wise batch transfer for all payables that are set to ready for payment and where the resources are on the Wise platform. An acompanying bird would update the payable status when the payment has succesfully been made in Wise.

## Missing features

A couple of features are still missing, if you are interested in any of these then let us know!

- Customer address manipulation
- Payable item manipulation
- Resource creation and manipulation
- Job rounds
- CAT analysis imports
- Invoices

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
