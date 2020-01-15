# PolicyTracker - Public Repository

## Introduction
Policy Tracker was developed to help departments distribute city/department policies and training materials to employees using email, and track receipts of employee acknowledgement of them.

## Objectives
* Reduce time and efforts spent in distributing packets, collecting and verifying policy acknowledgements
* Allow administrators to easily track employees who have or have not acknowledged a policy
* Provide employees a more convenient way to review and acknowledge policies
* Save financial resources by reducing paper documents and storages 

## Configuration
* .NET Framework 4.5, VB.Net
* SQL Server database
* IIS Web Server with Windows/Forms Authentication
* Batch jobs can run on Windows Scheduled Tasks

## Getting Started

### Database Setup
  * Create a SQL Server database instance and run scripts under `Database` folder to create tables and views
  * Import `*.csv` data files under `Database` folder using SQL Server Import and Export Wizard

### IIS Setup
   There are different publish profiles under `My Project\PublishProfiles` for combination of 3 different environments, Demo/Production/Test, and 2 different authentication types, Windows/Forms. 
  * Under the IIS web site, create 2 applications, `PolicyTracker` and `PolicyTracker.Kiosk`. 
  * Set `PolicyTracker` to use Windows Authentication. This is for employees who have AD account.
  * Set `PolicyTracker.Kiosk` to use Forms Authentication. This is for employees who do not have AD account. They can login with their 2F5L employee ID and PIN.
  * Publish `PolicyTracker` project using Visual Studio's Publish function to each target location on the IIS server
  
### Visual Studio
Open solution file `PolicyTracker.sln`. It includes 3 projects.

#### **PolicyTracker.vbproj** - Web application
Update `web.config` and additional config files in `connectionStrings`, `appSettings`, and `mailSettings` for your environment. This can be published to both `PolicyTracker` and `PolicyTracker.Kiosk` web applications.

#### **PolicyTracker.Batch.vbproj** - Console application
Update `app.config` and additional config files in `connectionStrings`, `appSettings`, and `mailSettings` for your environment. This is used to import/update HR data, and to create releases/packets/notifications data and send email messages. You can register different scheduled tasks to run this with different options.

#### **PolicyTracker.Lib.vbproj** - Libraries to support both of the projects listed above
