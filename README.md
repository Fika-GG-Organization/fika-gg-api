# Fika GG API

![FIKA.GG logo. It depicts a minimalistic white tea or coffee cup on a dark background. There's controller pads and two circular buttons printed on the cup.](https://raw.githubusercontent.com/Fika-GG-Organization/fika-gg-web/refs/heads/main/public/fikaggbg.png)

[![.NET CI](https://github.com/Fika-GG-Organization/fika-gg-api/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Fika-GG-Organization/fika-gg-api/actions/workflows/dotnet.yml)
[![Build and deploy ASP.Net Core app to Azure Web App - fika-gg-api](https://github.com/Fika-GG-Organization/fika-gg-api/actions/workflows/main_fika-gg-api.yml/badge.svg)](https://github.com/Fika-GG-Organization/fika-gg-api/actions/workflows/main_fika-gg-api.yml)

This repository houses the *FIKA GG* API to Postgress database among other backend infrastructure. 

## Table of Contents

- [Background](#background)
	- [See also](#see-also)
- [Development](#development)
- [Dependencies](#dependencies)
	- [Secrets](#secrets)
- [Contributing](#contributing)
- [License](#license)

## Background

***FIKA GG*** is an calendar-based gaming hub where GAMERS can connect with fellow GAMERS for participating or planning online gaming events. 

Our main target demographic are Swedish students and young adults with an interest in gaming.

### See also

- [fika-gg-web🔗‍️](https://github.com/Fika-GG-Organization/fika-gg-web): The web client application.

## Install

How to install for local development:

1. Open CLI/terminal.
1. Clone the server and the web client repositories to a local folder named `fika-gg/`.

    ```sh
    git clone https://github.com/Fika-GG-Organization/fika-gg-api.git fika-gg/api
    git clone https://github.com/Fika-GG-Organization/fika-gg-web.git fika-gg/web
    ```

If you want to fork these repositories, follow this [guide🔗‍️](https://docs.github.com/en/pull-requests/how-tos/work-with-forks/fork-a-repo?tool=webui).

---

### Dependencies

These dependencies are required to be manually installed or configured. See most dependencies that are automatically included at [dependencies](docs/development/dependencies.md).

- **Cloudinary**

    Allows the application to store and retrieve profile pictures. 
    
    On startup, **Aspire** will prompt for cloud name, API key and API secret. These can be acquired by creating a free account at [Cloudinary🔗‍️](https://cloudinary.com/documentation/developer_onboarding_faq_find_credentials).

- **RAWG API**

    The application uses **RAWG** to search for game data. 
    
    Get a free RAWG API key at [rawg.io/apidocs🔗‍️](https://rawg.io/apidocs), then run the following command from the server root folder:

    ```sh
    dotnet user-secrets set "Parameters:rawg-api-key" "<your-key>" --project src/FikaGg.AppHost
    ```
    
    The key is stored locally on your machine (never in the repository) and passed to the API by Aspire on startup.

    Alternatively, you can input the API key on **Aspire** startup.

- **Docker Desktop**

    Required for server startup. Download at [docker.com🔗‍️](https://docs.docker.com/get-started/get-docker/)

- **Node.js‍️**

    Required for developing the web client repository. Download at [nodejs.org🔗‍️](https://nodejs.org/en/download).

## Development

See [docs](docs/README.md) for info about project structure, tutorials, references, etc.

## Dependencies

The repository uses the following 3rd party dependencies.

- Aspire
- Scalar
- [RAWG API🔗‍️](https://rawg.io/apidocs) — game data
- Cloudinary — for user avatars/profile picture
- XUnit.v3 — Backend tests
- NSubstitute — Testing helper

### Secrets

The API needs a RAWG API key to search for games. 
Get a free key at [rawg.io/apidocs🔗‍️](https://rawg.io/apidocs), then run this from the repository root:

    dotnet user-secrets set "Parameters:rawg-api-key" "<your-key>" --project src/FikaGg.AppHost

The key is stored locally on your machine (never in the repository) and passed to the API by Aspire on startup.

---

Add cloudinary here

## Contributing

Visit [Github Issues](https://github.com/orgs/Fika-GG-Organization/projects/1) for tasks to do.

Do note that only persons from [Fika GG Organization](https://github.com/Fika-GG-Organization) are allowed to contribute to this repository.

### Collaborators

<a href="https://github.com/iskall94"><img width="50px" alt="iskall94" src="https://github.com/iskall94.png"/></a> <a href="https://github.com/GitUser4179"><img width="50px" alt="GitUser4179" src="https://github.com/GitUser4179.png"/></a> <a href="https://github.com/krixtin"><img width="50px" alt="krixtin" src="https://github.com/krixtin.png"/></a> <a href="https://github.com/GaKa00"><img width="50px" alt="GaKa00" src="https://github.com/GaKa00.png"/></a> <a href="https://github.com/blubeatbee"><img width="50px" alt="blubeatbee" src="https://github.com/blubeatbee.png"/></a> <a href="https://github.com/FrusTrick"><img width="50px" alt="FrusTrick" src="https://github.com/FrusTrick.png"/></a>

## License

[GNU General Public License v3.0](./LICENSE) &copy; Fika GG, 2026
