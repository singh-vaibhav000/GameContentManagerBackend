# Game Content Manager

A full-stack web application for managing game characters with role-based access control.

## Features

- User Authentication & Authorization
  - Email/password-based authentication
  - JWT token-based authentication
  - Role-based access control (Admin, Writer, Reader)
  - Password change functionality

- Game Characters Management
  - View character roster in a grid layout
  - Search characters by name or description
  - Create, read, update, and delete characters
  - Character details include:
    - Name
    - Description
    - Rarity (Common/Rare/Epic)
    - Upgrade Levels (1-5)
    - Sort Order
    - Image

- Change Audit
  - Track all changes made to characters
  - Admin access to audit logs

## Technology Stack

- Frontend:
  - Angular
  - HTML/CSS
  - TypeScript

- Backend:
  - .NET Core Web API
  - MongoDB
  - JWT Authentication

## Prerequisites

- Node.js (v14 or later)
- .NET Core SDK (v6.0 or later)
- MongoDB (v4.4 or later)

## Setup Instructions

### Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd GameContentManager.API
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Update the MongoDB connection string in `appsettings.json` if needed.

4. Run the application:
   ```bash
   dotnet run
   ```

### Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd game-content-manager
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Update the API URL in `src/environments/environment.ts` if needed.

4. Run the application:
   ```bash
   ng serve
   ```

## Usage

1. Access the application at `http://localhost:4200`
2. Login with your credentials
3. Navigate through the character list and manage characters based on your role

## API Endpoints

### Authentication
- POST `/api/auth/login` - Login and get JWT token
- POST `/api/auth/change-password` - Change password

### Characters
- GET `/api/gamecharacter` - Get all characters
- GET `/api/gamecharacter/{id}` - Get character by ID
- GET `/api/gamecharacter/search` - Search characters
- POST `/api/gamecharacter` - Create new character
- PUT `/api/gamecharacter/{id}` - Update character
- DELETE `/api/gamecharacter/{id}` - Delete character

## Security

- All API endpoints except login require JWT authentication
- Role-based access control is implemented for all operations
- Passwords are hashed before storage
- JWT tokens expire after 1 hour

## License

This project is licensed under the MIT License. 