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

    1. For server deployment, when prompted select server computer option in the dropdown, standalone MySql server, config type server computer (no need to change the parameters unless necessary), use recommended.
    2. when prompted for a password, input the password you wish the root owner of the database to use.
    
2. Download the [ASP .Net Core Runtime Hosting Bundle 3.0](https://download.visualstudio.microsoft.com/download/pr/32b71802-0b4d-4064-a7e6-083b5155d3b1/080cf60a5c06be4ed27e2eac6c693f2f/dotnet-hosting-3.0.1-win.exe)

3. Open `Turn windows features on or off` and select the folder in the list named `Web Server(IIS)` / `Internet Information Services`    

4. Download and extract (Remember to unblock the archive before extracting!) the [newest version](https://github.com/UtbOvertorneaPM/EquipmentManagementSystem/releases) to the folder on the computer/server where you wish to host the application.

5. Create a new MySql database using either MySql CLI(Command Line Interface) or workbench.

    a. Login to the CLI using the root user you setup during the MySql installation. 
    To create a database schema using the CLI, type in `create database YOURDATABASENAMEHERE;`
  
    b. Using workbench press the add schema and fill in the form.
  
6. Create a table called `users`

    a. Using the CLI, first select the database by using the command `use YOURDATABASENAMEHERE;`, then paste the commands in the box below
    ```
    create table users (
    id int not null auto_increment,
    name varchar(255) not null,
    password varchar(1500) not null,
    primary key(id));
    ```
  
    b. Using workbench to design the table as follows 
  
    Column_name | Datatype       | PK | NN | UQ | BIN | UN | ZF | AI
    ----------- | -------------- | -- | -- | -- | --- | -- | -- | --
    id          | int            | X  | X  |    |     |    |    | X 
    name        | varchar(255)   |    | X  |    |     |    |    |    
    password    | varchar(1500)  |    | X  |    |     |    |    |

7. Edit the `prodSettings.json` file in the folder you extracted to using a text editor, to match your settings

    User and password should be the MySql user that has access to the database, if this is the only database you will use on the computer you can use the `root` user you created during the MySql setup.
    
    Server is the IP to the server or `localhost` if database is local.
    
    DbName is the database name created during step 5
    
   prodsettings.json format:
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
 
     1. [Create a self signed certificate](https://aboutssl.org/how-to-create-a-self-signed-certificate-in-iis/) or obtain a certificate though other means
     2. Set the name of the site, choose your Applicationpool: use either DefaultAppPool or a new one
     3. Set the physical path to point at the folder of the extracted files.
     3. Set the binding to HTTPS and sign it with a certificate
 
 9. Edit folder permission of the folder wherein the extracted files are and add the IIS user or `IIS AppPool\DefaultAppPool` in folder permissions so it has full control over the application folder.
 
 10. Using the most up-to-date [PasswordHasher](https://github.com/UtbOvertorneaPM/PasswordHasher/releases/download/v1.1/PasswordHasher.7z) application, add any users that will have access to the equipment management system by following the application instructions.


