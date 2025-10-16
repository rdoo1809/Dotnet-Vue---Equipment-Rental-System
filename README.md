# Book Library Management API

This project is an academic assignment built with **.NET 8 Web API**.  
It demonstrates clean architecture practices using the **Repository Pattern**, **Unit of Work Pattern**, and **Dependency Injection** to manage Books. This project also makes use of **JWT Authentication**, **role-based-access-control**, and **API versioning**.

---

## 🚀 Tech Stack
- **.NET 8 Web API**
- **Entity Framework Core**
- **SQL Server (localdb)** for persistence
- Tested with **Postman**

---

## 📦 Project Setup

1. Clone the repository:
   ```bash
   git clone <repo-url>
   cd <project-folder>

2. Restore dependencies:
   ```bash
   dotnet restore

3. Apply migrations & seed the database:
   ```bash
   dotnet ef database update

4. Run the project:
   ```bash
   dotnet run


### Roles

•	Admin: Can create, update, and delete books.

•	User: Can only read books.

To get a token, send a POST request to /api/auth/login with one of the seeded credentials.

---

### API Endpoints

Version 1 (/api/v1/books)

•	GET /api/v1/books → List all books (Admin/User)

•	GET /api/v1/books/{id} → Get book by Id (Admin/User)

•	POST /api/v1/books → Create book (Admin only; ignores Genre & PublishedYear)

•	PUT /api/v1/books/{id} → Update book (Admin only; ignores Genre & PublishedYear)

•	DELETE /api/v1/books/{id} → Delete book (Admin only)

Version 2 (/api/v2/books)

•	GET /api/v2/books → List all books (Admin/User) including Genre & PublishedYear

•	GET /api/v2/books/{id} → Get book by Id (Admin/User) including Genre & PublishedYear

•	POST /api/v2/books → Create book (Admin only; can set Genre & PublishedYear)

•	PUT /api/v2/books/{id} → Update book (Admin only; can update Genre & PublishedYear)

•	DELETE /api/v2/books/{id} → Delete book (Admin only)

---

### Testing

You can test the API endpoints using Postman or any other REST client.
All endpoints return proper JSON responses and HTTP status codes.

---

### Notes
•	All endpoints require a valid JWT token.
•	v1 endpoints hide Genre and PublishedYear, v2 endpoints include all properties.