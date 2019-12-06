# Equipment Management System
![.Net Core 3.0](https://img.shields.io/badge/.Net%20Core-3.0-success)

Searchable database with web GUI for managing IT equipment and user information

## Getting Started

# Prerequisites

> [7-Zip](https://www.7-zip.org/a/7z1900-x64.exe)

> [ASP Net Core Runtime Hosting Bundle 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0)

> [MySql Installer Community](https://dev.mysql.com/downloads/installer/)

> [Install and Configure IIS](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-3.0#iis-configuration)

# Installing

1. Download and install the Pre-requisites

2. Download and Extract the [newest version](https://github.com/UtbOvertorneaPM/EquipmentManagementSystem/releases) to the folder on the computer/server where you wish to host the application.

3. Create a new MySql db using either MySql CLI or workbench.

    a. Login to the CLI using either root or a user you've setup. 
    To create a database schema using the CLI, type in `create database DATABASENAMEHERE;`
  
    b. Using workbench press the add schema and fill in the form.
  
4. Create a table called `users`

    a. Using the CLI, first select the database by using the command `use DATABASENAMEHERE;`, then input the command
    ```
    create table users (
    id int not null auto_increment,
    name varchar(255) not null,
    password varchar(1500) not null,
    primary key(id));
    ```
  
    b. Using workbench design the table as follows, 
  
    Column_name | Datatype       | PK | NN | UQ | BIN | UN | ZF | AI
    ----------- | -------------- | -- | -- | -- | --- | -- | -- | --
    id          | int            | X  | X  |    |     |    |    | X 
    name        | varchar(255)   |    | X  |    |     |    |    |    
    password    | varchar(1500)  |    | X  |    |     |    |    |

5. Edit `prodSettings.json` file in the root, with the neccessary data:
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
   User and password should be the MySql user that has access to the database, if this is the only database you will use on the computer you can use the root user you created during the MySql setup.

 6. Add a new site in IIS manager, make sure to set binding as https and sign it with a certificate.
 
 7. Edit folder permission so that the IIS user or default AppPool(IIS AppPool\DefaultAppPool)has full control over the application folder.
 
 8. Using the most up-to-date [PasswordHasher](https://github.com/UtbOvertorneaPM/PasswordHasher/releases) application add users that will have access to the equipment management system.


