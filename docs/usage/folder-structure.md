# Folder Structure

Learn how we structure the `fika-gg-api` repository.

## Overview

`fika-gg-api` have the following folders.

- `📁fika-gg-api/` root folder
    - `📁github/` ci/cd (.yml files for devops)
    - `📁docs/` documentation files
    - `📁src/`
        - [`📁FikaGg.Api/`](#fikagg.api) where the server code lives
          - [`#️⃣Program.cs`](../src/fikagg.api/program.cs) entry point (server app code starts here)
        - `📁FikaGg.AppHost/` Aspire files
        - `📁FikaGg.ServiceDefaults/` telemetry, health checks, logging, etc...
    - `📁tests/`
        - [`📁FikaGg.Tests/`](#fikagg.tests) xUnit test files

> Remeber to update [Overview](#overview) whenever folders are added or removed!

---

## `FikaGg.Api/`

This is the main project folder for our controller-based API. 

Most code files in here are organised into "Features" and is structured based on **Vertical Slice Design**. 
See [`Features/`](#features) for more examples.

`FikaGg.Api/` have the following folders.

- [`📁Common/`](#common) shared files such as helper methods, error handling, interfaces, etc
- `📁Data/` EF Core dbcontext class
- [`📁Entities/`](#entities) database entities
- [`📁Features/`](#features) controllers, services, DTOs, validators, etc...
- [`📁Integrations/`](#integrations) external services
- `📁Migrations/` database migration files
- `📁Properties/` Visual Studio settings folder (do not touch)

### `Common/`

Any generic code that is shared for the whole project should be placed in this folder.
For example middlewares, interfaces, helper methods, exception handlers, etc.

If a class, method, constant or variable is used in multiple folders/namespaces, 
then consider moving them into `Common/`.

### `Entities/`

Place all `DbContext` entities in this folder. 

An entity represents a database table.

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

This way of structuring is known as **Vertical Slice Design**. 
For more, see https://milanjovanovic.tech/blog/vertical-slice-architecture.

### `Integrations/`

WIP

## `FikaGg.Tests`

Place unit testing files in this project folder.

WIP
