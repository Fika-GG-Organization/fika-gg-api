# Endpoint Reference

A reference list of all server endpoint URLs, and what their request 
body should contain, and what the server response body will contain.

Each model/DTO are sent as JSON format.

## Table of Contents

- [Contribute](#contribute)
- [ASPNET Identity endpoints](#aspnet-identity-endpoints)
    - [Register new user](#register-new-user)
    - [Log in user](#log-in-user)
    - [Refresh login](#refresh-login)
    - [Forgot password](#forgot-password)
    - [Reset password](#reset-password)
    - [Check if email confirmed](#check-if-email-confirmed)
    - [Change password while logged in](#change-password-while-logged-in)
- [Game endpoints](#game-endpoints)
    - [Search games](#search-games)
- [X endpoints](#x-endpoints)

## Contribute

Please update this file with new endpoints and their respective JSON bodies!

Here's the syntax for adding a new endpoint reference.

```markdown
### [SHORT DESCRIPTION]

[LONGER DESCRIPTION]

#### Request

[HTTP METHOD] `[ENDPOINT PATH]`

- Route Parameter
- Query Parameter
- Body

#### Response

- [STATUS CODE]

    ```json
    {
        "": 
    }
    ```
```

---

## ASPNET Identity endpoints

### Register new user

Register and create new user.

#### Request

POST `api/auth/register`

- Body

    ```json
    {
      "email": "string",
      "password": "string"
    }
    ```

#### Response

- 200 OK

    No Body

- 400 Bad Request
    
    ```json
    {
      "type": null | "",
      "title": null | "",
      "status": null | "" | 1,
      "detail": null | "",
      "instance": null | "",
      "errors": {
        "additionalProperty": [
          "string"
        ]
      }
    }
    ```

### Log in user

Client send credentials to login.

#### Request

POST `api/auth/login`

- Query Parameters
  - `useCookies` boolean 
  - `useSesstionCookies` boolean

- Body

    ```json
    {
      "email": "",
      "password": "",
      "twoFactorCode": null | "",
      "twoFactorRecoveryCode": null | ""
    }
    ```

#### Response

- 200 OK

    ```json
    {
      "tokenType": null | "",
      "accessToken": "",
      "expiresIn": 1,
      "refreshToken": ""
    }
    ```

### Refresh login

Logged in client request new `accessToken` after their old one expired.

#### Request

POST `api/auth/refresh`

- Body

    ```json
    {
        "refreshToken": ""
    }
    ```

#### Response

- 200 OK

    ```json
    {
      "tokenType": null | "",
      "accessToken": "",
      "expiresIn": 1,
      "refreshToken": ""
    }
    ```

### Forgot password

Not logged in client asks to get new password via email.

#### Request

POST `api/auth/forgotPassword`

- Body

    ```json
    {
      "email": ""
    }
    ```

#### Response

- 200 OK

    No Body

- 400 Bad Request

    ```json
    {
      "type": null | "",
      "title": null | "",
      "status": null | "" | 1,
      "detail": null | "",
      "instance": null | "",
      "errors": {
        "additionalProperty": [
          ""
        ]
      }
    }
    ```

### Reset password

Not logged in client resets password.

#### Request

POST `api/auth/resetPassword`

- Body

    ```json
    {
      "email": "",
      "newPassword": "",
      "resetCode": ""
    }
    ```

#### Response

- 200 OK

    No Body

- 400 Bad Request

    ```json
    {
      "type": null | "",
      "title": null | "",
      "status": null | "" | 1,
      "detail": null | "",
      "instance": null | "",
      "errors": {
        "additionalProperty": [
          ""
        ]
      }
    }
    ```

### Check if email confirmed

Logged in client checks email and if the email is confirmed.

#### Request

GET `api/auth/manage/info`

No Body

#### Response

- 200 OK

    ```json
    {
      "email": "string",
      "isEmailConfirmed": true
    }
    ```

- 400 Bad Request

    ```json
    {
      "type": null | "",
      "title": null | "",
      "status": null | "" | 1,
      "detail": null | "",
      "instance": null | "",
      "errors": {
        "additionalProperty": [
          ""
        ]
      }
    }
    ```

- 404 Not Found

    No Body

### Change password while logged in

Logged in client changes password.

#### Request

POST `api/auth/manage/info`

- Body

    ```json
    {
      "newEmail": "" | null,
      "newPassword": "" | null,
      "oldPassword": "" | null
    }
    ```

#### Response

- 200 OK

    ```json
    {
        "email": "",
        "isEmailConfirmed": boolean
    }
    ```

- 400 Bad Request

    ```json
    {
      "type": null | "",
      "title": null | "",
      "status": null | "" | 1,
      "detail": null | "",
      "instance": null | "",
      "errors": {
        "additionalProperty": [
          ""
        ]
      }
    }
    ```

- 404 Not Found

    No Body

## Game endpoints

### Search games

Logged in client searches for games via RAWG. Returns max 10 results. 
Queries shorter than 2 characters return an empty list without calling RAWG.

#### Request

GET `api/games/search`

- Query Parameter
    - `query` string, required, max 100 characters

#### Response

- 200 OK

    ```json
    [
      {
        "rawgId": 1,
        "name": "",
        "coverUrl": null | ""
      }
    ]
    ```

- 400 Bad Request

    `query` is missing or longer than 100 characters.

- 401 Unauthorized

    No Body

## X endpoints
