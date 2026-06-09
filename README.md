# Apex Order Management System (OMS)

A centralized, enterprise-grade web application engineered to digitalize traditional industrial tracking workflows and automate manual data pipelines at **Apex Footwear Limited**. This system bridges the operational gap between the **Design Lab (DL)** and **Industrial Business (IB)** units by replacing paper-based ledgers with an automated state-machine tracking tracking workflow.

## 🚀 Core Features

- **Multi-Tiered Role-Based Access Control (RBAC):** Enforces strict middleware route filters isolating system domains for SuperAdmins, Design Lab Operators, and Industrial Business Users.
- **Article & BOM Synchronization:** Implements a strict $1:1$ normalization rule linking structural shoe styles and lasts directly to active Bill of Materials (BOM) consumption logs.
- **High-Velocity Milestone Tracking:** Real-time visibility into factory floor progress, monitoring high-priority production checkpoints such as **Knife Status** and **Tech Status**.
- **Embedded Real-Time Analytics:** Seamless integration of interactive **Power BI Dashboards** powered by **DirectQuery** pipelines to aggregate live transactional KPIs for executive-level tracking.
- **Non-Blocking Asynchronous File Operations:** Built with client-side **AJAX** file handling protocols to upload profile pictures and digital documentation instantly without browser postbacks or screen degradation.

## 🛠️ Technical Architecture & Stack

- **Backend Framework:** ASP.NET Core MVC (C#)
- **Database Architecture:** Microsoft SQL Server Management Studio (SSMS) configured to 3rd Normal Form (3NF)
- **Frontend Layer:** HTML5, CSS3, JavaScript (jQuery / AJAX), Bootstrap Responsive Layout Grid
- **Business Intelligence Engine:** Power BI Service APIs via DirectQuery gateway connections
- **Security Protocols:** Cryptographic salted password hashing algorithms and token-based state checking


## 📁 Repository Structure

```text
├── ApexOMS.Web/            # ASP.NET Core MVC Controllers, Views, and Models
│   ├── Controllers/        # Business Logic Routing (Account, Order, Article, Dashboard)
│   ├── Models/             # Entity Definitions & Database Binding Models
│   └── Views/              # Frontend UI Components & Bootstrap Layout Grids
├── ApexOMS.Data/           # Entity Framework Core Data Context and Migrations
│   ├── tbl_Articles.cs     # Master Shoe Article Specifications Schema
│   ├── tbl_BOM.cs          # Material and Recipe Matrix Constraints
│   └── tbl_Orders.cs       # Production Ledger & Status Transaction Store
├── PowerBI_Templates/      # PBIX layouts, workspace mapping configuration scripts
└── README.md               # Repository Overview Documentation
