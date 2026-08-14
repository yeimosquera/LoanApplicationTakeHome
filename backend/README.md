# LoanApplication.Api — Prueba Técnica (Senior .NET Backend)

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-9.0-blueviolet)](https://docs.microsoft.com/dotnet/csharp/)
[![EF Core](https://img.shields.io/badge/EF%20Core-8.0-brightgreen)](https://docs.microsoft.com/ef/core/)
[![Docker](https://img.shields.io/badge/Docker-ready-blue)](https://www.docker.com/)
[![GitHub Actions](https://img.shields.io/badge/GitHub%20Actions-CI-green)](https://github.com/features/actions)
[![xUnit](https://img.shields.io/badge/xUnit-tests-orange)](https://xunit.net/)

---

## 1. Resumen ejecutivo

Este repositorio contiene una implementación de backend para el procesamiento de solicitudes de crédito (Loan Application). Está diseñado como una prueba técnica de nivel senior y demuestra buenas prácticas de arquitectura y calidad de software: separación por capas (Domain / Application / Infrastructure), uso de MediatR (patrón CQRS para comandos/handlers), motor de reglas extensible, persistencia con EF Core, validación con FluentValidation, manejo de errores con middleware RFC 7807 (ProblemDetails), pruebas unitarias con xUnit y un pipeline mínimo para procesamiento asíncrono de eventos con System.Threading.Channels y BackgroundService.

Objetivo funcional: exponer una API para recepción de solicitudes de préstamo, validar reglas de negocio (rechazar/aceptar), persistir cliente y aplicación y enviar notificaciones asíncronas a un servicio externo simulado.

---

## 2. Descripción de la solución

- Endpoint principal: `POST /api/applications`. Recibe datos de la solicitud, valida, aplica reglas, persiste Customer y LoanApplication y publica un evento `ApplicationSavedEvent` a un canal interno.
- Background worker: `EventProcessingBackgroundService` lee eventos del canal y los envía al servicio externo simulador (named HttpClient `ExternalMock`).
- Reglas de negocio: implementadas mediante la interfaz `ILoanRule` y registradas en DI como `IEnumerable<ILoanRule>`. Reglas incluidas (ejemplos): `NyStateRule`, `BlacklistedSsnRule`, `MinRequestedAmountRule`, `MaxRequestedAmountRule`, `SsnFormatRule`.
- Validaciones: FluentValidation valida el comando antes de ejecutar el handler (pipeline de MediatR `ValidationBehavior`).

---

## 3. Arquitectura y Patrones de Diseño

- Clean Architecture (capas):
  - Domain: entidades y modelos (Customer, LoanApplication).
  - Application: casos de uso (SubmitApplicationCommand + Handler), reglas (ILoanRule), validadores y tests.
  - Infrastructure: persistencia (LoanDbContext), mensajería (Channel), background workers y middleware.

- CQRS / MediatR:
  - Comando: `SubmitApplicationCommand`.
  - Handler: `SubmitApplicationCommandHandler` implementa la lógica de negocio y persistencia.

- Motor de reglas (Open-Closed Principle):
  - `ILoanRule` define `bool Evaluate(SubmitApplicationCommand, out string? denialReason)`.
  - Nuevas reglas se agregan sin modificar el handler, simplemente registrándolas en DI.

- Procesamiento asíncrono:
  - `Channel<ApplicationSavedEvent>` como bus local in-memory.
  - `ChannelEventPublisher` escribe en el canal.
  - `EventProcessingBackgroundService` consume y entrega eventos a través de `HttpClientFactory`.

---

## 4. Stack Tecnológico

- Lenguaje: C# (.NET 8)
- Framework: ASP.NET Core (Minimal API)
- Persistencia: Entity Framework Core 8 (SQL Server opcional; InMemory para tests)
- Messaging/Background: System.Threading.Channels + BackgroundService
- Validación: FluentValidation
- Resiliencia: IHttpClientFactory + (recomendado) Polly
- Testing: xUnit
- Contenedores: Docker / docker-compose
- CI: GitHub Actions (sugerido)

---

## 5. Ejecutar localmente

Requisitos:
- .NET 8 SDK instalado
- (Opcional) Docker y docker-compose

Ejecutar con `dotnet run` (modo rápido con InMemory DB si no hay connection string configurada):

1. Abrir terminal en la carpeta `backend`.
2. Ejecutar:

```bash
dotnet run --project "C:\path\to\backend\LoanApplication.Api.csproj" --urls "http://localhost:5000"
```

El API quedará disponible en `http://localhost:5000`.

Ejecutar con SQL Server en contenedores (docker-compose):

1. Crear `docker-compose.yml` (ejemplo abajo) o usar el suministrado.
2. Levantar contenedores:

```bash
docker-compose up -d
```
3. Aplicar migraciones (desde la carpeta backend):

```bash
dotnet ef database update --project ./LoanApplication.Api.csproj
```

Ejemplo mínimo de `docker-compose.yml` (skeleton):

```yaml
version: '3.8'
services:
  mssql:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=Your_password123
    ports:
      - 1433:1433
    healthcheck:
      test: ["CMD", "/opt/mssql-tools/bin/sqlcmd", "-S", "localhost", "-U", "sa", "-P", "Your_password123", "-Q", "SELECT 1"]
      interval: 10s
      timeout: 5s
      retries: 10
```

---

## 6. Pruebas unitarias

- Framework: xUnit
- Tests de reglas y handler usan EF Core InMemory para aislamiento.
- Número de tests: 12+ (reglas individuales + handler scenarios). Ejecución:

```bash
dotnet test "./LoanApplication.Api.Tests/LoanApplication.Api.Tests.csproj"
```

Los tests incluyen:
- Validación de reglas (Min/Max amount, SSN format, blacklisted SSN, NY state)
- Handler tests:
  - Creación de nuevo cliente y aplicación (happy path)
  - Actualización de cliente recurrente (mismo SSN)
  - Rechazo por regla (e.g. NY state)

---

## 7. API — Referencia

### POST /api/applications
Crea o actualiza una aplicación de préstamo.

Request body (ejemplo exitoso):

```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "address": "123 Main St",
  "state": "CA",
  "companyName": "Acme Inc",
  "requestedAmount": 1500.00,
  "ssn": "123-45-6789"
}
```

Responses:
- 200 OK — body:

```json
{ "isApproved": true, "denialReason": null }
```

- 400 Bad Request — validación fallida (ejemplo con ProblemDetails):

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation errors occurred",
  "status": 400,
  "errors": {
    "Ssn": ["Invalid SSN format. Required format: XXX-XX-XXXX"]
  }
}
```

### GET /health
Retorna 200 si la aplicación está saludable. Mapearse con Health Checks y verificar DB.

---

## 8. Manejo de Errores, Validaciones y Resiliencia

- Middleware de excepciones: `ExceptionHandlingMiddleware` captura `FluentValidation.ValidationException` y devuelve `ValidationProblemDetails` con status 400; captura excepciones no esperadas y devuelve `ProblemDetails` con status 500.
- Validación: FluentValidation valida el comando de entrada mediante `SubmitApplicationCommandValidator`. Esta validación se integra con MediatR a través de `ValidationBehavior<TRequest,TResponse>` (pipeline behavior) que lanza `ValidationException` si hay errores.
- Resiliencia: El servicio de background usa `IHttpClientFactory` y un cliente nombrado `ExternalMock`. Recomendado configurar Polly para retries, circuit-breaker y timeouts sobre ese client (se sugiere añadir: `WaitAndRetryAsync` + `TimeoutAsync`).

---

## 9. Decisions & ADR (Registro de decisiones arquitectónicas)

A continuación se resumen las decisiones técnicas más relevantes y su justificación:

- ADR 001 — Clean Architecture
  - Decisión: separar dominio, aplicación e infraestructura. Justificación: Permite pruebas aisladas, facilita mantenimiento y extensibilidad.

- ADR 002 — Uso de MediatR (CQRS-light)
  - Decisión: usar MediatR para comandos (SubmitApplicationCommand). Justificación: Handler desacoplado, pipeline para cross-cutting concerns (validation, logging, retries si aplica).

- ADR 003 — Motor de reglas (ILoanRule)
  - Decisión: reglas implementan `ILoanRule` y se registran en DI como `IEnumerable<ILoanRule>`. Justificación: Satisface OCP (Open-Closed Principle), facilita agregar reglas sin tocar handler.

- ADR 004 — Procesamiento asíncrono (Channel + BackgroundService)
  - Decisión: usar Channel como bus local y BackgroundService para envío a servicio externo. Justificación: simplicidad y decoupling; para producción se puede sustituir por una cola duradera (RabbitMQ, Azure Service Bus) sin cambiar el handler.

- ADR 005 — Validación y manejo centralizado de errores
  - Decisión: FluentValidation + pipeline behavior y middleware ProblemDetails. Justificación: respuestas estandarizadas (RFC 7807) y validación declarativa.

---
