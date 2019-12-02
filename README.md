# Equipment Management System

Searchable database with web GUI for managing IT equipment and user information

## Getting Started

# Prerequisites

> [Net Core Hosting Bundle 2.2](https://dotnet.microsoft.com/download/dotnet-core/2.2)

> [MySQL](https://dev.mysql.com/downloads/installer/)

> [Install and Configure IIS](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-3.0#iis-configuration)

You can choose to either use the CLI or workbench for managing MySql

# Installing

1. Extract the files to the location you wish to host the application on.

2. Create a new MySql db, with three tables `Equipment`, `Owners` and 'Users'.

3. Edit `prodSettings.json` file in the root, with the neccessary data:
  ```
  {
  "Credentials": {
    "User": "DB_USERNAME",
    "Password": "PASSWORD",
    "Server": "SERVER_IP or LOCALHOST",
    "DbName": "DATABASENAME"
  }
}
  ```
	User and password should be the MySql user that has access to the database, if this is the only database you will use on the computer you can 
	use the root user you created during the MySql setup.

 4. Add a new site in IIS manager, make sure to set binding as https and sign it with a certificate.
 
 5. Edit folder permission so that the IIS user or default AppPool(IIS AppPool\DefaultAppPool)has full control over the application folder.
 
 6. Using the most up-to-date [PasswordHasher](https://github.com/UtbOvertorneaPM/PasswordHasher/releases) application add users that will have access
 to the equipment management system.


