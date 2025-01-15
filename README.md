# frodx-task-julija

# Order Processing Worker Service
## Overview
This application is a Worker Service that fetches and processes mock service order data periodically. It stores the processed orders in a database and logs the results in console. The service runs on a 15-minute schedule using Quartz.NET to orchestrate the order fetching and processing.

## Architecture
The application is designed using Clean Architecture principles and is split into three main layers:

1. EFCore Layer
Defines the Order entity as Model.
Defines ApiOrder entity as Model.
Defines OrderDbContxt as Data.
2. Infrastructure Layer
Implements the IApiOrderService that fetch data from mock service.
Implements the IDbOrderService interfaces that adds data to database.
3. Worker Service Layer
Uses Quartz.NET to schedule a recurring job that fetches orders from the ShedulerJob every 15 minutes.
The orders are saved to the MSSQL.
Logs success and failure of each order processing attempt in console.

## How the Application Works
The Worker Service runs every 15 minutes, scheduled by Quartz.NET.
The worker fetches orders from the IApiOrderService.
The orders are stored in MSSQL.
Logging is used to capture the status of each job execution (success or failure).

## Assumptions & Constraints

OrderDbContext.cs can be in other part of project.
OrderDate and Status are hardcoded.
Logging is done in console, and it could be done in file.
For the bonus tasks, the application can fetch orders from an external mock API.
# frodx-task-julija
