# Folder Structure

Learn the folder structure of `fika-gg-api` repository.

## Overview

`fika-gg-api` have the following folders.

- `📁fika-gg-api/` root folder
    - `📁github/` ci/cd (.yml files for devops)
    - `📁docs/` documentation files
    - `📁src/`
        - [`📁FikaGg.Api/`](#fikagg.api) where most of server code lives
          - [`#️⃣Program.cs`](../src/fikagg.api/program.cs) entry point (server app code starts here)
        - `📁FikaGg.AppHost/` Aspire files
        - `📁FikaGg.ServiceDefaults/` telemetry, health checks, logging, etc...
    - `📁tests/`
        - [`📁FikaGg.Tests/`](#fikagg.tests) xUnit test files

> Remeber to update [Overview](#overview) whenever folders are added or removed!

---

## `FikaGg.Api/`

This is the main project folder for our controller-based API. 

Most code files are organised into "Features" and structured based on **Feature Sliced Design**. 
See [`Features/`](#features) for examples.

`FikaGg.Api/` have the following folders.

- [`📁Common/`](#common) shared files such as helper methods, error handling, interfaces, etc
- `📁Data/` EF Core dbcontext class
- [`📁Entities/`](#entities) database entities
- [`📁Features/`](#features) controllers, services, DTOs, validators, etc...
- [`📁Integrations/`](#integrations) external services
- `📁Migrations/` database migration files added by `Add-Migration`
- `📁Properties/` Visual Studio settings folder (do not touch)

### `Common/`

Any generic code that is shared for the whole project should be placed in this folder.
For example middlewares, interfaces, helper methods, exception handlers, etc.

If a class, method, constant or variable will be used in multiple folders/namespaces, 
then consider moving them into `Common/`.

Files in the `Common/` folder should be grouped by their type, for example:

- Enum files should be inside an `Enums/` folder
- Interface files should be inside an `Interfaces/` folder
- Middleware files should be inside an `Middlewares/` folder
- Files with helper/utility methods should be inside an `Utils/` folder 
- etc...

If a folder doesn't exist, please do add it. 

### `Entities/`

Place all `DbContext` entities in this folder. 

An entity is a class that represents a database table.

### `Features/`

Place DTOs, controllers, services, validators in this folder.

Do *NOT* group code by their type or functionality.

```
// ❌ THIS IS WRONG
📁Features/
├📁Controllers/
│ ├#️⃣EventController.cs
│ └#️⃣GameController.cs
├📁DTO/
│ └#️⃣EventDTO.cs
└📁Services/
  ├#️⃣EventService.cs
  └#️⃣GameService.cs
```

Group them instead by their relationship with other code.

```
// ✅ THIS IS CORRECT
📁Features/
├📁Event/
│ ├#️⃣EventController.cs
│ ├#️⃣EventDTO.cs
│ └#️⃣EventService.cs
└📁Game/
  ├#️⃣GameController.cs
  └#️⃣GameService.cs
```

In the example above, we know that `EventDTO`, `EventService` and `EventController` classes mostly interact with each other; 
we can therefor group them together as a single Event "Feature"! 

Grouping based on features is known as **Feature Sliced Design**.

### `Integrations/`

WIP

## `FikaGg.Tests`

Place unit testing files in this project folder.

WIP
