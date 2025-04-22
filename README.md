# RefAPI

RefAPI is a .NET API project that explores different ways to build APIs based on my development style. It includes
examples of architectural patterns (like CQRS, traditional request handling, and the Result Pattern), with multiple
implementation approaches for each. While the patterns are reusable, the project is intended as a practical reference
rather than a boilerplate template. It also features a working LLM integration, designed to work with the
[azure-openai-search-custom](https://github.com/ivanrdvc/azure-openai-search-custom) implementation.

## Prerequisites

To run this project, you'll need:

- An OpenAI or Azure OpenAI API key
- .NET 9 SDK
- (Optional) Entra ID configuration for authentication

## Running the Project

1. Database Setup:
   ```bash
   dotnet ef database update
   ```

2. Run the Application:
   Choose one of these methods:
   ```bash
   # Using dotnet CLI
   dotnet run

   # Using Docker Compose
   docker compose up
   ```

## Project Overview

The repository contains two implementations of the same chat API:

1. `RefApi`: A modern implementation using Minimal APIs and newer libraries, focusing on simplicity and practicality
2. `RefApi.Controllers`: This project showcases a traditional Controller-based API structure

Most of the implementation logic resides in `RefApi`, while `RefApi.Controllers` references and reuses this core
functionality.

Key Features:

- Chat completion endpoints with streaming support
- Conversation management and persistence
