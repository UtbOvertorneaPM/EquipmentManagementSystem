# Equipment Management System

An ASP Core MVC project for managing IT equipment and user information

## Getting Started

# Prerequisites

> [Net Core Hosting Bundle 2.2](https://dotnet.microsoft.com/download/dotnet-core/2.2)

> [MySQL](https://dev.mysql.com/downloads/installer/)

> [Install and Configure IIS](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-3.0#iis-configuration)

# Installing

1. Create a MySQL db, with two tables(Equipment & Owners).

2. Create a `prodSettings.json` file in the root, with the format:
  ```
  {
  "Credentials": {
    "User": "DB_USERNAME",
    "Password": "PASSWORD",
    "Server": "SERVER_IP or LOCALHOST",
    "DbName": "DATABASENAME",
    "Domain": "DOMAINNAME\\USERNAME",
    "DebugDomain": "DEBUG_USERNAME"
  }
}
  ```

 3. Create add a new site in IIS manager, make sure to set binding as https and sign it with a certificate.
 
 4. Edit folder permission so that IIS has full control over application folder.


