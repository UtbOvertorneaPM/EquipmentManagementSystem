# Equipment Management System
![.Net Core 3.0](https://img.shields.io/badge/.Net%20Core-3.0-success)

Searchable database with web GUI for managing IT equipment and user information

## Getting Started

# Prerequisites

> [7-Zip](https://www.7-zip.org/a/7z1900-x64.exe)

> [ASP Net Core Runtime Hosting Bundle 3.0](https://download.visualstudio.microsoft.com/download/pr/32b71802-0b4d-4064-a7e6-083b5155d3b1/080cf60a5c06be4ed27e2eac6c693f2f/dotnet-hosting-3.0.1-win.exe)

> [MySql Installer Community](https://dev.mysql.com/downloads/installer/)

# Installing

1. Download and install MySql

    a. For server deployment, when prompted select server computer option, standalone MySql server, config type server computer(no need to change the parameters unless necessary), use recommended.
    a2. when prompted for a password, input the password you wish the root owner of the database to use.
    
2. Download the [ASP .Net Core Runtime Hosting Bundle 3.0](https://download.visualstudio.microsoft.com/download/pr/32b71802-0b4d-4064-a7e6-083b5155d3b1/080cf60a5c06be4ed27e2eac6c693f2f/dotnet-hosting-3.0.1-win.exe)

3. Open `Turn on windows features` and select the folder in the list from the table below

    Name                                               |
    -------------------------------------------------- |
    Web Server(IIS) / Internet Information Services    |
    

4. Download and Extract the [newest version](https://github.com/UtbOvertorneaPM/EquipmentManagementSystem/releases) to the folder on the computer/server where you wish to host the application.

5. Create a new MySql database using either MySql CLI(Command Line Interface) or workbench.

    a. Login to the CLI using the root user you setup during the MySql installation. 
    To create a database schema using the CLI, type in `create database DATABASENAMEHERE;`
  
    b. Using workbench press the add schema and fill in the form.
  
6. Create a table called `users`

    a. Using the CLI, first select the database by using the command `use DATABASENAMEHERE;`, then paste the commands in the box below
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

7. Edit the `prodSettings.json` file in the folder you extrated to using a text editor, with the your data

    User and password should be the MySql user that has access to the database, if this is the only database you will use on the computer you can use the `root` user you created during the MySql setup.
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

 8. Start the IIS Manager application, and add a new site in IIS manager to the computer/server
     a. Set the name of the site, point to the folder of the extracted files.
     b. Set the binding to HTTPS and sign it with a certificate
 
 9. Edit folder permission of the folder where the extracted files are in, so that the IIS user or default AppPool(IIS AppPool\DefaultAppPool)has full control over the application folder.
 
 10. Using the most up-to-date [PasswordHasher](https://github.com/UtbOvertorneaPM/PasswordHasher/releases) application add users that will have access to the equipment management system.


