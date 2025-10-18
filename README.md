# Equipment Rental Management System

This project is an academic assignment built with **.NET 9.0 Web API**.  
It demonstrates clean architecture practices using the **Repository Pattern**, **Unit of Work Pattern**, and **Dependency Injection** to manage Equipment Rentals. This project also makes use of **JWT Authentication**, **role-based-access-control**, and **Entity Framework Core with SQLite**.

---

## 🚀 Tech Stack
- **.NET 9.0 Web API**
- **Entity Framework Core**
- **SQLite** for persistence
- **JWT Authentication**
- Tested with **Postman**

---

## 📦 Project Setup

1. Clone the repository:
   ```bash
   git clone <repo-url>
   cd <project-folder>
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Apply migrations & seed the database:
   ```bash
   dotnet ef database update
   ```

4. Run the project:
   ```bash
   dotnet run
   ```

---

## 🔐 Authentication & Roles

### Roles
- **Admin**: Full access to all operations (create, read, update, delete)
- **User**: Limited access to own data only

### Getting a Token
To get a token, send a POST request to `/api/auth/login` with one of the seeded credentials:

**Admin Credentials:**
- Username: `AdminOne`
- Password: `password`

**User Credentials:**
- Username: `UserOne`, `UserTwo`, or `UserThree`
- Password: `password`

---

## 🏗️ System Architecture

### Entities
- **Equipment**: Rental items with availability status
- **Customer**: Users who can rent equipment
- **Rental**: Rental transactions with issue/return tracking

### Key Features
- **One Active Rental Rule**: Customers can only have one active rental at a time
- **Equipment Availability**: Equipment must be available to be rented
- **Role-Based Access**: Users can only access their own data (unless Admin)
- **Automatic Availability Updates**: Equipment availability is updated when renting/returning

---

## 📡 API Endpoints

### Authentication
- `POST /api/auth/login` → Login and get JWT token

### Equipment Management
- `GET /api/equipment` → List all equipment (Admin/User)
- `GET /api/equipment/{id}` → Get equipment by ID (Admin/User)
- `POST /api/equipment` → Create equipment (Admin only)
- `PUT /api/equipment/{id}` → Update equipment (Admin only)
- `DELETE /api/equipment/{id}` → Delete equipment (Admin only)

### Customer Management
- `GET /api/customer` → List all customers (Admin only)
- `GET /api/customer/{id}` → Get customer by ID (Admin/User - own data only)
- `POST /api/customer` → Create customer (Admin only)
- `PUT /api/customer/{id}` → Update customer (Admin only)
- `DELETE /api/customer/{id}` → Delete customer (Admin only)

### Rental Management
- `GET /api/rental` → List all rentals (Admin/User - filtered by role)
- `GET /api/rental/active` → List active rentals (Admin/User - filtered by role)
- `GET /api/rental/completed` → List completed rentals (Admin/User - filtered by role)
- `GET /api/rental/overdue` → List overdue rentals (Admin/User - filtered by role)
- `GET /api/rental/{id}` → Get rental by ID (Admin/User - own data only)
- `POST /api/rental/issue` → Issue new rental (Admin/User - own data only)
- `POST /api/rental/return` → Return rental (Admin/User - own data only)
- `PUT /api/rental/{id}` → Update rental due date (Admin only)
- `DELETE /api/rental/{id}` → Delete rental (Admin only)

---

## 🎯 Business Rules

1. **One Active Rental**: Customers can only have one active rental at a time
2. **Equipment Availability**: Equipment must be available to be rented
3. **User Access Control**: Users can only access their own data (unless Admin)
4. **Automatic Updates**: Equipment availability is automatically updated when renting/returning
5. **Due Date Management**: Rentals have a 7-day default due date from issue date

---

## 🧪 Testing

You can test the API endpoints using Postman or any other REST client.
All endpoints return proper JSON responses and HTTP status codes.

### Sample Test Flow
1. Login with admin credentials to get JWT token
2. Create equipment items
3. Create customer accounts
4. Issue rentals for customers
5. Check active/overdue rentals
6. Return rentals

---

## 📝 Notes
- All endpoints require a valid JWT token
- Users can only access their own rental data
- Equipment availability is automatically managed
- All timestamps use UTC time
- The system enforces business rules at the API level