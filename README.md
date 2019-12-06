# Equipment Management System

Searchable database with web GUI for managing IT equipment and user information

## Getting Started

# Prerequisites

> [ASP Net Core Runtime Hosting Bundle 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0)

> [MySql Installer Community](https://dev.mysql.com/downloads/installer/)

> [Install and Configure IIS](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-3.0#iis-configuration)

# Installing

1. Extract the files to the location you wish to host the application on.

2. Create a new MySql db using either MySql CLI or workbench.
  2a. Login to the CLI using either root or a user you've set up. To create a database schema using the CLI, type in `create database DATABASENAMEHERE`
  
  2b. Using workbench press the add schema and fill in the form.
  
3. Create a table called `users`
  3a. Using the CLI, first select the database by using the command `use DATABASENAMEHERE;`, then input the command
  ```
  create table users (
  id int not null auto_increment,
  name varchar(255) not null,
  password varchar(1500) not null,
  primary key(id));
  ```
  
  3b. Using workbench design the table as follows, 
  first row should be called `id` marked not null, set to auto increment and the type should be int.
  second row should be called `name`, marked not null and type is varchar 255.
  third row should be called `password`, marked not null and type is varchar 1500.

4. Edit `prodSettings.json` file in the root, with the neccessary data:
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


