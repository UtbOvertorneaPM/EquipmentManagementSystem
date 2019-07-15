# EquipmentManagementSystem


To setup the Equipment Management System

Requirements

  Oracle Mysql
  
  IIS

  ASP

  <a href="https://dotnet.microsoft.com/download/dotnet-core/2.2" target="_blank">.Net Core Hosting Bundle 2.2</a>

-------------------------------------------------------------------------------------------------------------------------------

1. Download the latest version of Oracle MySQL Server (https://dev.mysql.com/downloads/mysql/)
  
    1a. Optionally include MySQL Workbench if you prefer a GUI see step 2b.

2. Install MySQL
  
    2a. <a href="http://dev.mysql.com/doc/refman/5.6/en/linux-installation.html" target="_blank">Installing MySQL on Linux</a>

    2b. <a href="http://dev.mysql.com/doc/refman/5.6/en/windows-installation.html" target="_blank">Installing MySQL on Microsoft Windows</a>

3. Create a database with a schema named equipmentmanagementsystem
  
    3a. Open the MySQL Command Line Client and log in with the credentials created during MySQL Server install.
      Input `Create Database equipmentmanagementsystem` in the console.
  
    3b. Open the MySql WorkBench, then click on the instance you wish to add the database to, Input your credentials.
      Then click on the icon of a column with a plus on(create a new schema), name the new schema equipmentmanagementsystem.
      
4.  Create a file called prodSettings.json in the root folder of the extracted folder, using this format
    ```
    {
      "Credentials": {
        "User": "",
        "Password": "",
        "Server": "",
        "DbName": "equipmentmanagementsystem",
        "Domain": "",
        "DebugDomain": ""
      }
    }
    ```
    
    ```
    User is your MySQL username
    Password is your MySQL password
    
    Server is the IP of the server, if it's on the local computer set it to localhost
    
    DbName is the name of the database (step 3)
    
    Domain is the domain and/or users that will have access to the application page, 
    domain and users must be delimited by \\ ( DomainName\\Username )
    
    DebugDomain is the same as domain, but only in use for debugging with VS
    ```
    
Example prodSettings.json:
    
    ```
    {
      "Credentials": {
        "User": "JohnS",
        "Password": "SecureTestingPlatform3452",
        "Server": "localhost",
        "DbName": "equipmentmanagementsystem",
        "Domain": "Work\\JohnS",
        "DebugDomain": "Work\\JohnS"
      }
    }
    ```
