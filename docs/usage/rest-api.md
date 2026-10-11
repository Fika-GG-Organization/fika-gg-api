# REST API

## Table of Contents

- [Definitions](#definitions)
    - [Safe Methods](#safe-methods)
    - [Idempotent Methods](#idempotent-methods)
- [HTTP Methods](#http-methods)
    - [GET](#get)
    - [HEAD](#head)
    - [POST](#post)
    - [PUT](#put)
    - [DELETE](#delete)
    - [CONNECT](#connect)
    - [OPTIONS](#options)
    - [TRACE](#put)
    - [PATCH](#patch)

## Definitions

Here are important concepts to know when designing **REST APIs**.

### Safe Methods

WIP

The following HTTP methods are safe: `GET`, `HEAD`, `OPTIONS`, and `TRACE`.

### Idempotent Methods

A HTTP request with an idempotent method will always produce the same result regardless of 
how many times it is executed.

The following HTTP request methods should be **idempotent**: `GET`, `HEAD`, `PUT`, `DELETE`, 
and `OPTIONS`, and `TRACE`.

- `GET` requests should always result in the same table row resource being retrieved from 
database.
- `PUT` requests should always result in the same resource being created or replaced on the 
specified table row.
- `DELETE` requests should always result in the same specified database row having its 
resource deleted.

Client **should not** be able to automatically retry a request with a non-idempotent method.

## HTTP Methods

A HTTP method lets the server know what the client intends to do with the requested resource. 

These HTTP methods are defined by [RFC 9110 Section 9🔗‍️](https://www.rfc-editor.org/rfc/rfc9110.html#section-9).

### [`GET`🔗‍️](https://www.rfc-editor.org/rfc/rfc9110.html#section-9.3.1)

Reads one or more existing resources.

- `GET` methods **should not** send with a body.
	- Search criteria for the requested resources should be sent in the URI instead.
- If successful, server should send a `200 (OK)` with the requested content.
- `GET` methods should be **idempotent**.
- `GET` methods can be cached.

### [`HEAD`🔗‍️](https://www.rfc-editor.org/rfc/rfc9110.html#section-9.3.2)

Identical to `GET` methods except the server **must not** send content in the response.

- Used for testing links or finding recent modifications.

### [`POST`🔗‍️](https://www.rfc-editor.org/rfc/rfc9110.html#section-9.3.3)

Create one or more resources. Can also be used to send form data, post messages or append data to a resource's existing representation(s).

- If resources has been created, the server **should** send a `201 (Created)` containing a Location header field to the created resource.

### [`PUT`🔗‍️](https://www.rfc-editor.org/rfc/rfc9110.html#section-9.3.4)

`PUT` methods completely replaces an an existing resource with a copy of said resource,
even if only one column was updated. 

- This allows `PUT` methods to be **idempotent**.

`PUT` expects the **whole** resource to be updated:
- If `PUT` tries to update a partial resource, it will still try to replace the unspecified values with empty values, often resulting in data loss.
	- If only partial data is to be updated, use `PATCH` instead.
- The server **must** send either a `200 (OK)` or a `204 (No Content)` on a successful resource update.
- If the new resource is targeting a non-existent id, then that resource should be created with that id.
	- This allows `PUT` methods to also behave like `POST` methods.
	- The server **must** send a `201 (Created)` on a successful resource creation.

### [`DELETE`](https://www.rfc-editor.org/rfc/rfc9110.html#section-9.3.5)

Deletes one or more existing resources.

- `DELETE` methods should be **idempotent**.
- `DELETE` methods **should not** create resources. 
- The server should send either a
	- `202 (Accepted)` if the deletion will be applied, but at a later date.
	- `204 (No Content)` on a successful deletion and no further info will be supplied.
	- `200 (OK)` on a successful deletion and the response message includes a status.

### [`CONNECT`🔗‍️](https://www.rfc-editor.org/rfc/rfc9110.html#section-9.3.6)

Requests that the recipient of a `Connect` request establish a tunnel to the destination origin server identified by the request target and, if successful, thereafter restrict its behaviour to blind forwarding of data, in both directions, until the tunnel is closed.

- Tunnels are commonly used to create an end-to-end virtual connection, through one or more proxies, which can then be secured using TLS.
- Uses an unique form of request target whereby the client **must** send the host and port number separated by a colon.

```http
CONNECT server.example.com:80 HTTP/1.1
Host: server.example.com
```

- A server must/should send
	- a `400 (Bad Request)` if request target has an invalid port number.
	- any `2xx (Successful)`.

### [`OPTIONS`🔗‍️](https://www.rfc-editor.org/rfc/rfc9110.html#section-9.3.7)

### [`TRACE`🔗‍️](https://www.rfc-editor.org/rfc/rfc9110.html#section-9.3.8)

### [`PATCH`🔗‍️](https://www.rfc-editor.org/info/rfc5789/#section-2)

Released in 2010 as an additional **Update** method.

- The standard `PUT` method needs the whole resource to be updated.
	- This may also result in the creation of a new resource.
	- Requires the full data post which can be slow if we only want to update a part of a resource.
		- Can be mitigated with DTOs.
- The `PATCH` method allows for partial update of a resource.
	- Will not accidentally create new resources.
	- This makes `PATCH` to not always be **idempotent**.
	- This will require more logic onto the server, so use `PATCH` sparingly.
