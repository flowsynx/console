<div align="center">
  <h1>FlowSynx Console</h1>
  <p><i>Web-based management console for FlowSynx Workflow Automation</i></p>
</div>

**FlowSynx Console** is a modern, web-based management console for
orchestrating, executing, and monitoring workflows via the **FlowSynx
Workflow Automation API**. Built with **Blazor**, it
provides a feature-rich, interactive, and responsive user interface for
managing enterprise-grade workflow automation at scale.

The console is designed for IT administrators, DevOps engineers,
business analysts, operations teams, and even less-exprience users 
who need a secure, intuitive,and centralized platform to manage automation 
workflows, plugins,runtime execution, and monitoring in distributed environments.

![FlowSynx Console Screenshot](/img/console.jpg)

## Key Capabilities

### Workflow Management

-   Design, configure, and monitor **complex workflows** with support for:
    -   Parallel execution
    -   Conditional branching
    -   Retry and error handling policies
    -   Human-in-the-loop tasks and manual approvals
-   Real-time workflow execution tracking with detailed task-level logs.
-   Visual DAG-based representation of workflows for easy troubleshooting.

### Plugin Management

-   Register, update, and manage plugins through a unified interface.
-   **Versioning support** to enable side-by-side execution of multiple plugin versions.
-   Configurable lifecycle operations including install, update, and decommission.
-   Fine-grained access control for plugin usage and runtime parameters.

### Plugin Configuration

-   Define and manage plugin-specific **connection details** (e.g., databases, APIs, cloud services).
-   Secure handling of **authentication credentials** using industry-standard practices.
-   Runtime parameters configurable per environment (development, staging, production).

### Authentication & Security

-   **OpenID Connect** and **cookie-based authentication** for secure access.
-   Integration with enterprise identity providers (e.g., **Keycloak**, **Azure AD**, **Okta**).
-   Role-based access control (RBAC) for fine-grained permissions.
-   Secure session management and automatic token renewal.

### Real-Time Monitoring

-   **SignalR-powered live updates** for workflow progress and plugin health.
-   Dynamic dashboards for monitoring workflow KPIs, execution timelines, and system health.
-   Proactive error notifications and alerting mechanisms.

### Persistent Storage

-   Local storage for session, preferences, and configuration persistence.
-   Seamless recovery of user sessions across browser refreshes.

### Modern User Experience

-   Built on **MudBlazor** for a consistent, responsive, and accessible UI.
-   Dark and light themes for operator comfort.
-   Rich dialog-driven interactions for workflows, plugins, and configuration wizards.

------------------------------------------------------------------------

## Architecture Overview

The FlowSynx Console is a **client-side Blazor WebAssembly application**
designed for scalability, security, and extensibility.

-   **Front-End:**
    -   Blazor WebAssembly + MudBlazor
    -   Responsive design with enterprise-ready components
    -   Localization and accessibility support
-   **Back-End Integration:**
    -   RESTful communication with **FlowSynx Workflow Automation API**
    -   Real-time communication via **SignalR WebSockets**
    -   Secure authentication flows through **OIDC**
-   **Deployment:**
    -   Self-hosted or containerized deployment (Docker/Kubernetes)
    -   Reverse proxy support (Nginx, Apache, Traefik, Azure App Gateway)
    -   TLS/HTTPS enforced by default

------------------------------------------------------------------------

## Getting Started

### Prerequisites

-   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
-   Access to a running [FlowSynx Workflow Automation API](https://flowsynx.io)

### Build & Run

1.  **Clone the repository:**

    ```bash
    git clone https://github.com/flowsynx/console.git
    cd console/src
    ```

2.  **Restore dependencies:**

    ```bash
    dotnet restore
    ```

3.  **Build the project:**

    ```bash
    dotnet build
    ```

4.  **Run the application:**

    ```bash
    dotnet run
    ```

5.  **Access the console:**
    Open your browser at:

        https://localhost:6264

------------------------------------------------------------------------

## Configuration

### Authentication

-   Configure **OpenID Connect** and cookie authentication in `appsettings.json` or via environment variables.
-   Example snippet:

    ```json
    "Authentication": {
        "Authority": "https://identity.flowsynx.io",
        "ClientId": "flowsynx-console",
        "ClientSecret": "<CLIENT-SECRET>",
        "RedirectUri": "/signin-oidc",
        "RequireHttps": true
    }
    ```

------------------------------------------------------------------------

## Deployment

-   **Docker**: Prebuilt container images available via GitHub Packages or Docker Hub.
-   **Kubernetes**: Helm charts for high-availability deployments.
-   **CI/CD Integration**: Ready-to-use pipelines for GitHub Actions, GitLab CI, or Azure DevOps.
-   **Cloud-Native Support**: Deployable to AWS ECS/EKS, Azure AKS, or Google GKE.

------------------------------------------------------------------------

## Contributing

We welcome community contributions!
- Submit issues or feature requests via [GitHub Issues](https://github.com/flowsynx/console/issues).
- Open pull requests with clear documentation and test coverage.
- Adhere to coding standards and best practices outlined in our [Contribution Guidelines](https://github.com/flowsynx/console/blob/master/CONTRIBUTING.md).

------------------------------------------------------------------------

## Roadmap

Planned enhancements include:
- **Audit logging** and advanced compliance reports.
- **Multi-tenancy support** for enterprise customers.
- **Workflow versioning and rollback** capabilities.
- **Drag-and-drop workflow designer** integrated into the console.

------------------------------------------------------------------------

## License

© FlowSynx. All rights reserved.
This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).

------------------------------------------------------------------------

## Support & Documentation

-   Documentation: [flowsynx.io/docs](https://flowsynx.io/docs/overview)
-   Support: <support@flowsynx.io>