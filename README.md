# InvoicePdfGenerationQueue.Processor.Function
Built an Event-Driven Invoice PDF Generation System using Azure Functions (.NET 8), Service Bus &amp; Blob Storage.
README.md 
# Invoice PDF Generation using Azure Function

## Overview

This project demonstrates an event-driven architecture using:

- Azure Service Bus
- Azure Function (Queue Trigger)
- Azure Blob Storage

## Scenario

When an order is placed:
1. Order service pushes invoice data to Service Bus queue
2. Azure Function listens to queue
3. Uploads PDF to Azure Blob Storage

## Technologies Used

- .NET 8
- Azure Functions (Isolated Worker)
- Azure Service Bus
- Azure Blob Storage
- QuestPDF

## How to Run

1. Configure Service Bus
2. Configure Blob Storage
3. Update local.settings.json
4. Run: func start

## Architecture

Order → Service Bus → Azure Function → Blob Storage

