# PolicyTracker - Public Repository

## Introduction
Policy Tracker was developed to help departments track receipts of employee acknowledgement of city/department policies and training materials.

## Objectives
* Reduce time and efforts spent in distributing, collecting, and verifying policy acknowledgements
* Allow administrators to easily track employees who have or have not acknowledged a policy
* Provide employees a more convenient way to review and acknowledge policies
* Save financial resources by reducing paper documents and storages 

## Configuration
* .NET Framework 4.5, VB.Net
* SQL Server database
* IIS Web Server with Windows/Forms Authentication
* Batch jobs can run on Windows Scheduled Tasks

## Getting Started
1. Database Setup
  * Create a SQL Server database instance and run scripts under `Database` folder to create tables and views
  * Import `*.csv` data files under `Database` folder using SQL Server Import and Export Wizard
2. IIS Setup
  There are different publish profiles under `My Project` - `PublishProfiles` for 3 different environments, Demo/Production/Test, and 2 different authentication types, Windows/Forms. 
  * Under the IIS web site, create 2 applications, `PolicyTracker` and `PolicyTracker.Kiosk`. 
  * Set `PolicyTracker` to use Windows Authentication. This is for employees who have AD account.
  * Set `PolicyTracker.Kiosk` to use Forms Authentication. This is for employees who do not have AD account. They can login with their 2F5L employee ID and PIN.
  * Publish `PolicyTracker` project using Visual Studio's Publish function to each target location on the IIS server
  
