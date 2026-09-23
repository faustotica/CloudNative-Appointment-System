# 🏥 AppointmentManager (Turnero) - Cloud-Native API

![.NET 10](https://img.shields.io/badge/.NET-10.0-purple.svg)
![Docker](https://img.shields.io/badge/Docker-Enabled-blue.svg)
![Architecture](https://img.shields.io/badge/Architecture-Clean_Architecture-green.svg)
![CI/CD](https://img.shields.io/badge/CI%2FCD-GitHub_Actions-black.svg)

**AppointmentManager** es un sistema de gestión de turnos médicos de nivel de producción (*Enterprise-grade*), construido para demostrar la implementación de patrones de arquitectura modernos en el ecosistema .NET.

Este proyecto forma parte de mi portfolio personal y fue evolucionado desde un CRUD monolítico básico hacia un sistema distribuido y escalable.

## 🚀 Arquitectura y Patrones Implementados

- **Clean Architecture & Domain-Driven Design (DDD):** El código está estructurado en capas (`API`, `Application`, `Core`, `Infrastructure`) asegurando bajo acoplamiento y alta cohesión.
- **CQRS (Command Query Responsibility Segregation):** Uso de **MediatR** para separar las operaciones de lectura (Queries) y escritura (Commands), eliminando los *Fat Controllers*.
- **Mensajería Asíncrona (Event-Driven):** Integración de **MassTransit** y **RabbitMQ** para desacoplar procesos. Cuando se crea un turno, la API publica un evento que es consumido en *background* por un **Worker Service** dedicado (simulando envío de emails).
- **Seguridad & Identidad:** Sistema de usuarios con Roles implementado sobre **ASP.NET Core Identity**, protegido mediante autenticación con tokens **JWT**.
- **Resiliencia (Polly):** Manejo inteligente de fallos y middleware global para estandarizar respuestas de error.
- **Testing Automatizado:** Proyecto separado con pruebas unitarias usando **xUnit** y **Moq**.

## 🛠️ Stack Tecnológico
- **Framework:** .NET 10 (C# 14)
- **Base de Datos:** SQL Server (Vía Entity Framework Core)
- **Message Broker:** RabbitMQ
- **Librerías Clave:** MediatR, MassTransit, xUnit, Moq, EF Core.
- **DevOps:** Docker (Multi-stage builds), Docker Compose, GitHub Actions, Azure Bicep (IaC).

## 🐳 Cómo correr el proyecto localmente

Gracias a **Docker Compose**, levantar toda la infraestructura (Base de datos, RabbitMQ, API y Worker) requiere de un solo comando. No necesitas instalar SQL Server ni RabbitMQ en tu máquina local.

### Prerrequisitos
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) iniciado.
- .NET 10 SDK (opcional, solo si deseas correr los tests localmente).

### Pasos
1. Clona el repositorio:
   ```bash
   git clone https://github.com/tu-usuario/AppointmentManager.git
   cd AppointmentManager
   ```

2. Levanta los contenedores (esto descargará las imágenes de SQL y RabbitMQ, y compilará la API y el Worker):
   ```bash
   docker-compose up --build
   ```

3. **¡Listo!** La API aplicará las migraciones de base de datos automáticamente al iniciar.
   - Accede a **Swagger** para probar la API: [http://localhost:8080/swagger](http://localhost:8080/swagger)
   - Accede al panel de **RabbitMQ**: [http://localhost:15672](http://localhost:15672) (Usuario: `guest`, Clave: `guest`)

## 🏗️ Flujo de CI/CD (GitHub Actions)
El repositorio cuenta con un pipeline configurado (`.github/workflows/main.yml`) que:
1. Compila la solución en cada *push*.
2. Ejecuta automáticamente los tests unitarios (`dotnet test`).
3. Construye las imágenes Docker para asegurar que todo es desplegable.

Además, cuenta con un script de **Azure Bicep** (`InfrastructureAsCode/main.bicep`) listo para aprovisionar los recursos en la nube en segundos.

## 🧪 Ejecutar los Tests
Si deseas correr las pruebas unitarias localmente, abre una terminal en la raíz y ejecuta:
```bash
dotnet test
```

---
*Desarrollado como muestra de arquitectura de software.*
