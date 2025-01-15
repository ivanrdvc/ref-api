# Authentication Setup Guide

This application uses Microsoft Entra ID for authentication. Follow these steps to configure authentication:

## Azure Portal Setup

1. **Create API Application Registration:**
   - Go to Microsoft Entra ID (Azure AD) → App registrations → New registration
   - Name: [Your API Name]
   - Supported account types: Choose organization scope
   - Click Register

2. **Configure API:**
   - In your API's app registration:
   - Go to "Expose an API"
   - Set Application ID URI (accept default or customize)
   - Add scope:
     - Name: "api.access"
     - Admin consent only
     - Add appropriate display name and description

3. **Create Client Application Registration:**
   - Create another app registration for the frontend
   - Add platform: Single-page application (SPA)
   - Add redirect URI: `http://localhost:3000` (or your app URL)
   - Under API Permissions:
     - Add permission → My APIs → Select your API
     - Select api.access scope
     - Click "Grant admin consent"