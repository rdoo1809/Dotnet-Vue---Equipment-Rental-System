# Equipment Rental Management System

This project is a full-stack web application built as an academic assignment demonstrating modern web development practices. It consists of a **.NET 9.0 Web API backend** with **Vue.js 3 frontend** that manages equipment rentals with clean architecture, JWT authentication, and role-based access control.

## 🚀 Tech Stack

### Backend
- **.NET 9.0 Web API**
- **Entity Framework Core** with SQLite
- **JWT Authentication** with role-based access control
- **Repository Pattern** and **Unit of Work Pattern**
- **Dependency Injection** throughout

### Frontend
- **Vue.js 3** with Composition API
- **Vite** for fast development and building
- **Pinia** for state management
- **Vue Router** for client-side routing
- **Bootstrap 5** for responsive UI
- **Axios** for API communication

---

## 📦 Project Setup

### Prerequisites
- **.NET 9.0 SDK** (for backend)
- **Node.js v18+** and **npm** (for frontend)
- **SQLite** (included with .NET)

### Backend Setup

1. **Clone and navigate to project:**
   ```bash
   git clone <repo-url>
   cd <project-folder>
   ```

2. **Restore .NET dependencies:**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations:**
   ```bash
   dotnet ef database update
   ```

4. **Run the backend API:**
   ```bash
   dotnet run
   ```
   The API will be available at `http://localhost:5129`

### Frontend Setup

1. **Navigate to frontend directory:**
   ```bash
   cd equipment-rental-frontend
   ```

2. **Install Node.js dependencies:**
   ```bash
   npm install
   ```

3. **Start development server:**
   ```bash
   npm run dev
   ```
   The frontend will be available at `http://localhost:3000`

### Full Application
- **Backend API**: `http://localhost:5129`
- **Frontend App**: `http://localhost:3000`
- Both services need to be running simultaneously

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

## 🎨 User Interface Features

The Vue.js frontend provides a modern, responsive web interface with the following features:

### Dashboard
- **Statistics Overview**: Total equipment, available count, rented count, overdue rentals
- **Quick Actions**: Issue equipment, return equipment, view rentals
- **Role-Based Display**: Admin sees overdue rentals, users see filtered data

### Equipment Management
- **Browse Equipment**: View all equipment with availability status
- **Equipment Details**: Detailed view of individual equipment
- **CRUD Operations**: Admin can create, update, delete equipment
- **Category Filtering**: Filter equipment by categories (Power Tools, Heavy Machinery, Safety, etc.)

### Customer Management
- **User Profiles**: View and manage customer accounts (Admin only)
- **Customer Details**: Individual customer information and rental history

### Rental Management
- **Active Rentals**: View current rentals with due dates
- **Completed Rentals**: History of returned equipment
- **Overdue Rentals**: Highlight rentals past due date (Admin view)
- **Issue Equipment**: Modal-based equipment rental process
- **Return Equipment**: Modal-based equipment return process
- **Extend Rentals**: Admin can extend due dates

### Responsive Design
- **Mobile-Friendly**: Bootstrap-based responsive layout
- **Toast Notifications**: User feedback for actions
- **Modal Dialogs**: Clean interaction patterns for CRUD operations

---

## 🎯 Business Rules

1. **One Active Rental**: Customers can only have one active rental at a time
2. **Equipment Availability**: Equipment must be available to be rented
3. **User Access Control**: Users can only access their own data (unless Admin)
4. **Automatic Updates**: Equipment availability is automatically updated when renting/returning
5. **Due Date Management**: Rentals have a 7-day default due date from issue date

---

## 🧪 Testing & Usage

### API Testing with Postman
You can test the API endpoints using Postman or any other REST client.
All endpoints return proper JSON responses and HTTP status codes.

### Web Application Usage
The full application can be accessed through the Vue.js frontend at `http://localhost:3000`

### Sample User Flow
1. **Login** with admin or user credentials
2. **Dashboard**: View system statistics and quick actions
3. **Browse Equipment**: Check available equipment
4. **Issue Equipment**: Rent equipment through modal interface
5. **View Rentals**: Check active/completed rentals
6. **Return Equipment**: Return rented equipment
7. **Admin Features**: Create equipment, manage customers, view overdue rentals

### Test Credentials
**Admin Account:**
- Username: `AdminOne`
- Password: `password`

**User Accounts:**
- Username: `UserOne`, `UserTwo`, or `UserThree`
- Password: `password`

---

## 📝 Development Notes

### Backend (.NET)
- **JWT Authentication**: All endpoints require valid JWT tokens
- **Role-Based Access**: Users can only access their own data (unless Admin)
- **Clean Architecture**: Repository pattern with Unit of Work
- **Automatic Updates**: Equipment availability managed by business logic
- **UTC Timestamps**: All dates stored in UTC timezone
- **Business Rules**: Enforced at API level with proper HTTP status codes

### Frontend (Vue.js)
- **State Management**: Pinia store for authentication state
- **API Integration**: Centralized Axios service with interceptors
- **Responsive Design**: Bootstrap 5 for mobile-friendly interface
- **Modal-Based UI**: Clean interaction patterns for all operations
- **Toast Notifications**: User feedback for all actions
- **Route Guards**: Client-side authentication protection

### Security
- **JWT Tokens**: Secure authentication with role-based claims
- **Password Storage**: Plain text for demo (production: use hashing)
- **CORS**: Configured for frontend-backend communication
- **Input Validation**: Server-side validation on all endpoints

### Database
- **SQLite**: Lightweight database included with .NET
- **EF Migrations**: Version-controlled schema changes
- **Seed Data**: Pre-populated with test users and equipment
