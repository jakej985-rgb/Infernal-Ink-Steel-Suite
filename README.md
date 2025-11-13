# Infernal-Ink-Steel-Suite
# ShopManager

Modern Windows tattoo shop manager for handling appointments, clients, artists, and day-to-day studio operations.

---

## Overview

**ShopManager** is a **Windows desktop application** built with **C++** in **Visual Studio 2022** to help tattoo and piercing studios manage their business in one place. It focuses on a clean, themeable UI and workflows tailored specifically for studios instead of generic POS software.

---

## Features

### 🗓 Appointment Management
- Create, edit, and cancel appointments  
- Link appointments to clients and artists  
- Filter, search, and browse upcoming bookings  
- Double-click or context actions for quick edits

### 👤 Client & User Management
- Store client details and visit history  
- User accounts with admin / non-admin roles  
- Database-backed user management via dedicated `UserDB` / `AppointmentDB` classes  

### 🎨 Theme & UI System
- Centralized theme manager for consistent colors, fonts, and styles  
- Accent color support for branding your studio  
- Optional glow/shadow/highlight effects for important buttons and UI elements  

### 📅 Integrations (Work in Progress)
- Integration layer for online calendars (e.g. Google Calendar, Apple ecosystem)  
- HTTP clients built on **cpr** and **cpp-httplib**  
- Xodo Sign client for handling digital document signing workflows  

### 🔐 Networking & Security
- Uses **curl** and **OpenSSL** to talk to remote APIs over HTTPS  
- **cpr** and **cpp-httplib** for higher-level HTTP(S) requests  
- Designed with secure TLS communication in mind  

---

## Tech Stack

- **Language:** C++ (C++17/20 recommended)  
- **IDE:** Visual Studio 2022  
- **Platform:** Windows 10/11 desktop  
- **Libraries:**
  - [`curl`](https://curl.se/) – low-level HTTP
  - [`OpenSSL`](https://www.openssl.org/) – TLS/crypto
  - [`cpr`](https://github.com/libcpr/cpr) – C++ HTTP client
  - [`cpp-httplib`](https://github.com/yhirose/cpp-httplib) – single-header HTTP library
- **Database:** SQL database (e.g. SQLite) accessed through small DB helper classes (`UserDB`, `AppointmentDB`, etc.)

---

## Building (Visual Studio 2022)

1. Clone the repository:
   ```bash
   git clone https://github.com/<your-user>/<your-repo>.git
   cd <your-repo>
