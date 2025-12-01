# Infernal Ink & Steel Suite

A comprehensive management solution for tattoo and piercing studios, featuring a robust Desktop Manager and a modern Web Portal.

## Overview

**Infernal Ink & Steel Suite** consists of three main components:
1.  **Desktop Manager**: A WPF application for in-shop management (appointments, clients, point-of-sale).
2.  **Web Portal**: A React-based web application for remote access (artists viewing schedules, clients booking).
3.  **Backend API**: A .NET Core API that serves as the central hub, synchronizing data between Desktop and Web via a shared SQLite database.

## Prerequisites

- **.NET 8.0 SDK**: [Download here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- **Node.js (v18+)**: [Download here](https://nodejs.org/)
- **Visual Studio 2022** (Recommended for Desktop development) or **VS Code**.

## Setup Instructions

### 1. Database Setup
The application uses a shared SQLite database.
1.  Create the directory: `C:\InfernalInkSteelSuite\Data`
2.  The application will automatically create the database file (`infernalinksteel.db`) on first run.

### 2. Backend API
The API handles data synchronization and business logic.
1.  Navigate to `InfernalInkSteelSuite.Api`.
2.  Run the API:
    ```bash
    dotnet run
    ```
    The API will start on `http://localhost:5000`.

### 3. Web Application
The Web Portal is a React SPA.
1.  Navigate to `InfernalInkSteelSuite.Web/ClientApp`.
2.  Install dependencies:
    ```bash
    npm install
    ```
3.  Start the development server:
    ```bash
    npm run dev
    ```
    The Web App will start on `http://localhost:5173` (or similar, check console output).
4.  **Production Build**:
    ```bash
    npm run build
    ```

### 4. Desktop Application
The Desktop Manager is a WPF app.
1.  Open `Infernal-Ink-Steel-Suite.sln` in Visual Studio.
2.  Set `Infernal-Ink-Steel-Suite.manager` as the startup project.
3.  Build and Run (F5).

## Configuration

### Shared Database Path
Both the API and Desktop App are configured to use the same database file:
- **Path**: `C:\InfernalInkSteelSuite\Data\infernalinksteel.db`
- **Configuration**:
    - **API**: `InfernalInkSteelSuite.Api/appsettings.json`
    - **Desktop**: `Infernal-Ink-Steel-Suite.manager/App.xaml.cs`

### Theme
The application uses the "Infernal Neon" theme, characterized by dark backgrounds (`#050510`) and vibrant accents (Purple `#8A2BE2`, Cyan `#00E5FF`).

## Troubleshooting

- **Database Errors**: Ensure the `C:\InfernalInkSteelSuite\Data` directory exists and the user has write permissions.
- **API Connection**: Ensure the API is running on port 5000 before starting the Web App.
- **Build Errors**: Run `dotnet restore` and `npm install` to ensure all packages are downloaded.
