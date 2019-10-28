# Equipment Management System

An ASP Core MVC project for managing IT equipment and user information

## Getting Started

# Prerequisites

> [Net Core Hosting Bundle 2.2](https://dotnet.microsoft.com/download/dotnet-core/2.2)

> [MySQL](https://dev.mysql.com/downloads/installer/)

# Installing

1. Create a MySQL db, with two tables(Equipment & Owners).

2. Create a prodSettings.json file in the root, with the format:
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


