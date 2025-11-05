# Electricity Cutting Down Management System

## System Overview
- Console App Worker (Data Processing)
- REST API (Data Generation)
- MVC Portal (Dashboard & Monitoring)
- SQL Database (Data Storage)

## Components

### 1. Database
- Tables: Cutting_Down_Header, Cutting_Down_Detail, Cutting_Down_Ignored, Network_Element, etc.
- Stored Procedures: SP_Create, SP_Close, SP_BuildHierarchy
- Functions: FN_CalculateImpactedCustomers
- Views: Dashboard views

### 2. Console Application
- Monitors incident data
- Calls API to generate test data
- Executes stored procedures
- Runs in cycles every 10 seconds

### 3. REST API
- Endpoints for data generation (Source A & B)
- Health check endpoints
- Port: localhost:5032

### 4. MVC Portal
- Dashboard with real-time statistics
- Incident tracking
- Charts and visualizations
- Port: localhost:5000

## Installation & Configuration

### Prerequisites
- SQL Server 2019+
- .NET 6.0 SDK
- Visual Studio 2022

### Step 1: Database Setup
1. Open SQL Server Management Studio
2. Execute scripts in order:
   - 00_Create_Tables.sql
   - 01_Create_Functions.sql
   - 02_Create_StoredProcedures.sql
   - 03_Sample_Data.sql

### Step 2: API Setup
1. cd API
2. dotnet restore
3. Update appsettings.json with connection string
4. dotnet run

### Step 3: Console App Setup
1. cd ConsoleApp
2. dotnet restore
3. Update connection string
4. dotnet run

### Step 4: Portal Setup
1. cd Portal
2. dotnet restore
3. Update connection string
4. dotnet run
5. Open http://localhost:5000

## Running the System
1. Start SQL Server
2. Start API (Terminal 1)
3. Start Console App (Terminal 2)
4. Open Portal in browser

## Testing
- API Health Check: GET http://localhost:5032/api/CuttingDownA/health
- Portal Dashboard: http://localhost:5000/Dashboard
- Database Queries: Check tables in SSMS
