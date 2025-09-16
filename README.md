StudentManagementApi 🎓

A .NET 9 Web API project that demonstrates JWT Authentication & Authorization with Access Tokens and Refresh Tokens, built for managing students, teachers, and admins.

This project is designed for learning secure API development with role-based access control in ASP.NET Core.


✨ Features

🔐 JWT Authentication (Access + Refresh Tokens)

👥 Role-based Authorization (Admin, Teacher, Student)

👨‍🎓 Manage students (create, update, delete, view)

📊 Add and view student marks

👨‍🏫 Teachers can add marks for students

👨‍💼 Admins can update roles and manage users

📘 Secure endpoints using [Authorize]

📄 API Documentation with Scalar


🚀 Endpoints Overview

🔑 Auth

POST /api/Auth/register → Register new user

POST /api/Auth/login → Login (returns Access + Refresh token)

POST /api/Auth/refresh-token → Get new Access token

GET /api/Auth/me → Get logged-in user profile

GET /api/Auth/admin-only → Test admin-only access


🎓 Students

GET /api/Students/me → Get your profile (Student only)

GET /api/Students → Get all students (Admin only)

POST /api/Students/{studentId}/marks → Add marks (Teacher only)

GET /api/Students/{studentId}/marks → Get marks (Student, Teacher, or Admin)

PUT /api/Students/{id}/role → Update role (Admin only)

DELETE /api/Students/{id} → Delete a user (Admin only)


🛠️ Tech Stack

.NET 9 (ASP.NET Core Web API)

Entity Framework Core (SQL Server)

JWT (System.IdentityModel.Tokens.Jwt)

Scalar API (for OpenAPI docs)


▶️ How to Run

Clone repo:

git clone https://github.com/yourusername/StudentManagementApi.git

cd StudentManagementApi


Apply migrations & create DB:

dotnet ef database update


Run API:

dotnet run


Open docs in browser:

https://localhost:7099/scalar/v1

