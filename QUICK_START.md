# 🚀 Quick Start Guide

## Prerequisites Checklist
- [ ] .NET 8 SDK installed
- [ ] Git installed
- [ ] IDE/Editor ready (VS Code, Visual Studio, or Rider)
- [ ] GitHub Copilot extension installed (recommended)

## 30-Second Setup

```bash
# 1. Clone and navigate
git clone https://github.com/MyatHsuMon23/Flights-WorkOrders_BackendAPIs.git
cd Flights-WorkOrders_BackendAPIs

# 2. Restore, build, and run (one command)
dotnet restore && dotnet build && cd Flights_Work_Order_APIs && dotnet run
```

## 🎯 Immediate Testing

Once the app starts (you'll see `Now listening on: http://localhost:5112`):

### 1. Open Swagger UI
**URL**: http://localhost:5112

### 2. Test Basic Endpoints (No Auth Required)
- Click `GET /api/Aircraft` → Try it out → Execute
- Click `GET /api/Flights` → Try it out → Execute  
- Click `GET /api/Technicians` → Try it out → Execute

### 3. Test Authentication
1. **Login**: `POST /api/Auth/login`
   ```json
   {
     "email": "admin",
     "password": "password123"
   }
   ```

2. **Authorize**: Copy token → Click "Authorize" button → Paste token

3. **Test Protected Endpoint**: `POST /api/Aircraft` (create new aircraft)

## 🔧 Development Mode

```bash
# Hot reload (automatically restarts on file changes)
dotnet watch run

# This is perfect for development - any code changes will immediately restart the app
```

## 📊 Sample Data Available

The app automatically includes:
- ✈️ **3 Aircraft** (Boeing 737, Airbus A320, Boeing 777)
- 👨‍🔧 **3 Technicians** (Engine, Avionics, Structural specialists)
- 🛫 **4 Flights** (Various routes and schedules)
- 🔧 **4 Work Orders** (Different priorities and statuses)

## 🚨 Common Issues

### Port Already in Use
```bash
# Use different port
dotnet run --urls "http://localhost:5113"
```

### Database Issues
```bash
# Reset database
rm FlightWorkOrder.db
dotnet ef database update
```

### Package Issues
```bash
# Clear cache and restore
dotnet nuget locals all --clear
dotnet restore
```

## 🎮 Interactive Testing

### Using Swagger UI (Recommended)
1. Navigate to http://localhost:5112
2. Expand any endpoint
3. Click "Try it out"
4. Fill in parameters
5. Click "Execute"
6. View response

### Using cURL
```bash
# Get all aircraft
curl http://localhost:5112/api/Aircraft

# Create new aircraft (after getting auth token)
curl -X POST http://localhost:5112/api/Aircraft \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{"registrationNumber":"N12345","model":"Test Aircraft","manufacturer":"Test Corp","capacity":100,"status":"Active"}'
```

## 🎯 Next Steps

1. **Explore the API**: Use Swagger UI to test all endpoints
2. **Check the Code**: Look at Controllers, Models, and Data folders
3. **Read Full Docs**: Check the main README.md for complete documentation
4. **Use Copilot**: See COPILOT_GUIDE.md for AI-assisted development tips

## 📞 Need Help?

1. Check the **Troubleshooting** section in README.md
2. Verify all **Prerequisites** are installed
3. Ensure you're in the correct directory (`Flights_Work_Order_APIs`)
4. Check that port 5112 is available

---

**🎉 You're ready to go! The API should be running and ready for testing.**
