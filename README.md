# RealTimeChatBot

RealTimeChatBot is a .NET-based chat application that enables real-time communication with SignalR and ASP.NET Web API. It allows users to register, log in, and exchange messages in real-time, with offline message handling and queueing.

## Features

- **User Registration & Login**: Basic authentication flow for secure login.
- **Real-time Messaging**: Uses SignalR to enable instant communication.
- **Offline Message Queue**: Messages sent to offline users are queued and delivered when they return online.
- **CI/CD with Azure DevOps**: Automated build and deployment to Azure with GitHub integration.
- **Storage**: Uses a lightweight SQLite database for data persistence.

## Technology Stack

- **Backend**: ASP.NET Web API (.NET 4.8)
- **Frontend**: ASP.NET WebForms
- **Real-time Communication**: SignalR
- **Background Processing**: Hangfire for offline message processing
- **Database**: SQLite (or other lightweight database)
- **Deployment**: Azure DevOps CI/CD pipeline

## Getting Started

### Prerequisites

- Visual Studio
- SQL Server (for local development)
- Azure DevOps and GitHub accounts for CI/CD setup

### Setup and Run

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   ```

2. **Database Configuration**:
   Update your connection string in `Web.config`:
   ```xml
   <connectionStrings>
       <add name="ChatBot" connectionString="Your_Connection_String_Here" providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

3. **Build and Run**:
   - Open the solution in Visual Studio.
   - Build and run the project.

4. **Configure Azure DevOps**:
   - Connect your GitHub repository to Azure DevOps.
   - Set up CI/CD pipelines to automate deployment.

### Usage

1. Register a new user and log in.
2. Select another user to chat in real-time.
3. If a user is offline, messages are queued and will be available upon their next login.
