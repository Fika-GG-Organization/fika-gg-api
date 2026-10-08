# Fika GG web

![Place any banner here]()

This repository houses the *FIKA GG* API to Postgress database among other backend infrastructure. 

## Table of Contents

- [Background](#Background)
	- [See also](#See_also)
- [Development](#Development)
	- [Dependencies](#Dependencies)
	- [CLI](#CLI)
- [Contributing](#Contributing)
- [License](#License)

## Background

***FIKA GG*** is an calendar-based gaming hub where GAMERS can connect with fellow GAMERS for participating or planning online gaming events. 

Our main target demographic are Swedish students and young adults with an interest in gaming.

### See also

- [fika-gg-web](https://github.com/Fika-GG-Organization/fika-gg-web): The web client application.

## Development

Monolith API, deployed to Azure Container Apps, and ran locally with Aspire
Code is organized by feature (Vertical slice), PostgresDb. Feature-based organization with controller service architecture

Project structure

- `FikaGG.AppHost/` Aspire
- `FikaGG.ServiceDefaults/` telemetry, health checks, logging... etc
- `FikaGG.Api/` ASP.NET Core web api
- `FikaGG.Tests/` xUnit tests

`FikaGG.Api` folder structure

- `Features/` controllers, services, dtos, validators, etc all grouped by feature, for example features/events
- `Common/` error handling, other common use functions/interfaces?
- `Integrations/` external services
- `Entities/` database entities
- `Data/` EF Core dbcontext
- `Migrations/` db migrations

### Dependencies

The repository uses the following 3rd party dependencies.

- Aspire
- Scalar

### CLI

DevOps info here

## Contributing

Visit [Github Issues](https://github.com/orgs/Fika-GG-Organization/projects/1) for tasks to do.

Do note that only persons from [Fika GG Organization](https://github.com/Fika-GG-Organization) are allowed to contribute to this repository.

### Collaborators

<a href="https://github.com/iskall94"><img width="50px" alt="iskall94" src="https://github.com/iskall94.png"/></a> <a href="https://github.com/GitUser4179"><img width="50px" alt="GitUser4179" src="https://github.com/GitUser4179.png"/></a> <a href="https://github.com/krixtin"><img width="50px" alt="krixtin" src="https://github.com/krixtin.png"/></a> <a href="https://github.com/GaKa00"><img width="50px" alt="GaKa00" src="https://github.com/GaKa00.png"/></a> <a href="https://github.com/blubeatbee"><img width="50px" alt="blubeatbee" src="https://github.com/blubeatbee.png"/></a> <a href="https://github.com/FrusTrick"><img width="50px" alt="FrusTrick" src="https://github.com/FrusTrick.png"/></a>

## License

[GNU General Public License v3.0](./LICENSE) &copy; Fika GG, 2026
