# Fika GG API

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

- [fika-gg-web🔗‍️](https://github.com/Fika-GG-Organization/fika-gg-web): The web client application.

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
