# Equipment Rental Management System - Frontend

A Vue 3 frontend application for the Equipment Rental Management System, built with Vite, Vue Router, Pinia, and Bootstrap.

## Features

- **Authentication**: JWT-based login with role-based access control
- **Dashboard**: Overview of equipment statistics and quick actions
- **Equipment Management**: CRUD operations for equipment (Admin only)
- **Customer Management**: User profile management and admin customer management
- **Rental Management**: Issue, return, and extend equipment rentals
- **Responsive Design**: Mobile-friendly interface using Bootstrap

## Technology Stack

- **Vue 3** - Progressive JavaScript framework
- **Vite** - Fast build tool and development server
- **Vue Router** - Official router for Vue.js
- **Pinia** - State management library
- **Bootstrap 5** - CSS framework for responsive design
- **Axios** - HTTP client for API communication

## Prerequisites

- Node.js (v18 or higher)
- npm or yarn package manager
- Backend API running on `http://localhost:5129`

## Installation

1. Clone the repository and navigate to the frontend directory:
```bash
cd equipment-rental-frontend
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm run dev
```

4. Open your browser and navigate to `http://localhost:3000`

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build

## Project Structure

```
src/
├── components/          # Reusable Vue components
│   ├── Layout.vue      # Main layout with navigation
│   ├── EquipmentModal.vue
│   ├── IssueEquipmentModal.vue
│   ├── ReturnEquipmentModal.vue
│   ├── ExtendRentalModal.vue
│   ├── CustomerModal.vue
│   ├── ConfirmationModal.vue
│   └── Toast.vue
├── views/              # Page components
│   ├── Login.vue
│   ├── Dashboard.vue
│   ├── Equipment.vue
│   ├── EquipmentDetails.vue
│   ├── Customers.vue
│   ├── CustomerDetails.vue
│   ├── Rentals.vue
│   └── RentalDetails.vue
├── services/           # API service layer
│   ├── api.js         # Axios configuration
│   ├── dashboard.js
│   ├── equipment.js
│   ├── customer.js
│   └── rental.js
├── stores/            # Pinia stores
│   └── auth.js        # Authentication store
├── router/            # Vue Router configuration
│   └── index.js
├── utils/             # Utility functions
│   └── date.js
├── assets/            # Static assets
│   └── main.css
└── config/            # Configuration
    └── index.js
```

## API Integration

The frontend communicates with the .NET Web API backend through the following endpoints:

- **Authentication**: `/api/auth/login`
- **Equipment**: `/api/equipment/*`
- **Customers**: `/api/customers/*`
- **Rentals**: `/api/rentals/*`

## Authentication

The application uses JWT tokens stored in localStorage for authentication. The auth store manages:
- User login/logout
- Token storage and retrieval
- User information
- Authentication state

## Role-Based Access Control

- **Admin**: Full access to all features including customer management
- **User**: Limited access to own data and equipment rental functionality

## Development Notes

- All API calls are handled through the centralized API service
- Error handling is implemented with axios interceptors
- Bootstrap modals are used for all CRUD operations
- Date formatting is handled by utility functions
- Toast notifications provide user feedback

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## License

This project is part of the Midterm Assignment for PROG3340.
