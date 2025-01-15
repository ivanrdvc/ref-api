# RefAPI

RefAPI is my personal .NET 9 project that showcases my approach to building modern APIs. I've built this as my reference project, using the coding patterns and practices that work best for my development style. The project includes real-world examples like LLM integration with GPT-4 to demonstrate these patterns in action.

## Prerequisites

To run this project, you'll need:
- An OpenAI API key
- .NET 9 SDK
- (Optional) Entra ID configuration for authentication

## Running the Project

1. **Database Setup**:
   ```bash
   dotnet ef database update
   ```

2. **Run the Application**:
   Choose one of these methods:
   ```bash
   # Using dotnet CLI
   dotnet run

   # Using Docker Compose
   docker compose up
   ```

## Project Overview

The repository contains two implementations of the same chat API:

1. **Minimal API Version** (`RefApi`): A modern, streamlined implementation using Minimal APIs and newer libraries, focusing on simplicity and practicality
2. **Controller Version** (`RefApi.Controllers`): A traditional approach using Controllers and established patterns

Most of the implementation logic resides in `RefApi`, while `RefApi.Controllers` references and reuses this core functionality. 

This API is specifically designed to work with the frontend implementation from [azure-openai-search-custom](https://github.com/ivanrdvc/azure-openai-search-custom).

Key Features:
- Chat completion endpoints with streaming support
- Conversation management and persistence
