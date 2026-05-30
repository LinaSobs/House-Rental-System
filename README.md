# House Rental System

📌 Project Overview  
House Rental System is a web-based application developed using C# and ASP.NET Core MVC.  
The system allows house owners to list rental properties while tenants can browse available houses, submit rental applications, and make secure online payments.

The application uses SQL Server for database management and integrates Stripe Checkout for handling online payments securely. The system is designed to simplify the process of property listing, rental applications, and tenant management.

---

## 🎯 What the Project Does

### House Owners:
* Register and log into the system  
* Add new rental properties  
* Upload property images  
* Manage listed houses  
* View rental applications submitted by tenants  
* Approve or reject applications  

### Tenants:
* Browse available rental properties  
* View house details and features  
* Submit rental applications  
* Track application status  
* Make online payments through Stripe Checkout  

### General Features:
* User authentication and authorization  
* Property availability management  
* Secure payment processing  
* Form validation and error handling  
* Responsive and user-friendly interface  

---

## 🛠️ Technologies Used
* C#  
* ASP.NET Core MVC  
* .NET 9  
* SQL Server  
* Entity Framework Core  
* ASP.NET Identity  
* Stripe Payment Gateway  
* HTML  
* CSS  
* JavaScript  

---

## 💡 Why This Project Is Useful
* Demonstrates full-stack web development using ASP.NET Core MVC  
* Shows implementation of authentication and authorization  
* Demonstrates database integration using Entity Framework Core  
* Provides experience with payment gateway integration using Stripe  
* Helps understand CRUD operations in a real-world application  
* Demonstrates file upload functionality for property images  
* Useful for learning enterprise-level web application development  

---

## 🚀 Possible Future Improvements
* Add property search and filtering  
* Implement messaging between tenants and owners  
* Add property reviews and ratings  
* Include map and location integration  
* Add admin dashboard and reporting  
* Improve UI/UX design and responsiveness  
* Add email notifications for application updates  
* Implement lease agreement generation  

---

## ▶️ Installation and Setup Instructions

### Requirements
Before running the project, make sure you have the following installed:
* Visual Studio 2022  
* .NET 9 SDK  
* SQL Server  
* SQL Server Management Studio (SSMS)  

### Steps to Run the Application
1. Download or clone the project repository.  
2. Open the solution file in Visual Studio 2022.  
3. Open SQL Server and create the project database.  
4. Update the database connection string in the `appsettings.json` file.  
5. Open the Package Manager Console in Visual Studio.  
6. Run the following command to apply database migrations:  
   ```bash
   Update-Database
   ```  
7. Build the solution.  
8. Run the application using IIS Express or the Start button in Visual Studio.  
9. Open the browser and access the system using the generated localhost URL.  

---

## 🔑 Stripe Key Setup Instructions

To enable secure payments, you need to configure your **Stripe API key**.  
⚠️ Never commit real keys to source control — use environment variables instead.

### 1. Set Environment Variable
#### Windows (PowerShell):
```powershell
setx STRIPE_API_KEY "your_real_stripe_key_here"
```
#### macOS/Linux (bash/zsh):
```bash
export STRIPE_API_KEY="your_real_stripe_key_here"
```

### 2. Update `appsettings.json`
Keep only a placeholder:
```json
{
  "Stripe": {
    "ApiKey": "ENV:STRIPE_API_KEY"
  }
}
```

### 3. Load Environment Variables in .NET
In `Program.cs`:
```csharp
builder.Configuration.AddEnvironmentVariables();
var stripeApiKey = builder.Configuration["STRIPE_API_KEY"];
StripeConfiguration.ApiKey = stripeApiKey;
```

---

## 📚 Educational Value
This project is ideal for:
* Application Development students  
* Beginners learning ASP.NET Core MVC  
* Understanding database-driven web applications  
* Learning authentication and authorization  
* Practicing CRUD operations and MVC architecture  
* Understanding payment gateway integration  
* Learning modern web development practices  
* Following best practices for **secret management**  

---

Author: Linamandla Sobikela  
Project Type: Academic / Learning Project  
