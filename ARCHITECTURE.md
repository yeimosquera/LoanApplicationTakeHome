# Arquitectura (resumen)

Este proyecto sigue una aproximación simple y práctica inspirada en Clean Architecture y "vertical slicing" para características (features) individuales.

## Estructura general

- `Application/` — Contiene la lógica de aplicación por características (por ejemplo `Features/Loans`). Aquí están los comandos de MediatR, handlers y reglas de negocio que se aplican a las solicitudes.
- `Domain/` — Modelos y objetos de dominio (si aplica).
- `Infrastructure/` — Implementaciones de persistencia (EF Core), mensajería local y otros detalles técnicos.
- `Api` (LoanApplication.Api) — Minimal API que expone los endpoints y compone las dependencias.

Esta organización favorece cortes verticales por caso de uso: cada feature (por ejemplo "submit loan application") agrupa su comando/handler/validaciones y dependencias específicas, facilitando la evolución por características.

## MediatR y Vertical Slicing

- Las solicitudes entrantes se mapearán a `IRequest<T>` (por ejemplo `SubmitApplicationCommand`) y son manejadas por handlers registrados por MediatR.
- Esto permite separación clara entre el contract (command/query) y su implementación, mejora testabilidad y mantiene los endpoints del API delgados.

## Motor de Reglas (Patrón Strategy)

- Se define una interfaz `ILoanRule` que expone un método `IsSatisfiedAsync(SubmitApplicationCommand, CancellationToken)`.
- Cada regla concreta (por ejemplo `NyStateRule`, `BlacklistedSsnRule`) implementa `ILoanRule`.
- Las reglas se registran en DI (con `AddScoped<ILoanRule, XRule>()`) y el handler o servicio que las consume las itera para evaluar si la solicitud cumple todas las reglas.

Cómo añadir una nueva regla:
1. Crear una clase que implemente `ILoanRule` en `Application/Rules`:

```csharp
public sealed class MyNewRule : ILoanRule
{
    public Task<bool> IsSatisfiedAsync(SubmitApplicationCommand command, CancellationToken cancellationToken = default)
    {
        // Lógica: devolver true si la regla está satisfecha
    }
}
```

2. Registrarla en `Program.cs`:

```csharp
builder.Services.AddScoped<ILoanRule, MyNewRule>();
```

3. Si la regla necesita dependencias (repositorios, servicios externos), inyectarlas vía constructor y registrar esas dependencias en DI.

## Transacciones y publicación de eventos (Unit of Work)

- El handler que persiste la solicitud (EF Core) debe realizar todas las operaciones relacionadas (crear cliente, crear aplicación, crear evento de dominio) dentro de una transacción/Unit of Work.
- Si la persistencia es exitosa, el evento se coloca en un `Channel<T>` en memoria para su procesamiento asíncrono por un `BackgroundService`.
- El uso de `Channel<T>` permite desacoplar la persistencia de la publicación externa y mantener la simplicidad: la transacción garantiza durabilidad en la base de datos, y el canal garantiza entrega eventual al procesador en segundo plano.

## Background service y `Channel<T>`

- El `EventProcessingBackgroundService` consume eventos desde el `Channel<ApplicationSavedEvent>` y realiza envíos HTTP externos (o a una cola externa si se requiere en producción).
- El uso de `Channel<T>` mantiene la solución ligera y sin infraestructura adicional (sin RabbitMQ ni Kafka) lo que reduce complejidad para esta prueba técnica.

## Trade-offs

- Simplicidad sobre escalabilidad: se eligió `Channel<T>` y un `BackgroundService` en proceso en lugar de integrar una cola de mensajes externa. Esto es más simple y adecuado para pruebas locales y cargas bajas, pero no es tan resiliente ni escalable como una cola dedicada en entornos distribuidos.
- No se añadieron pruebas unitarias para el frontend por decisión de alcance y simplicidad (la evaluación penaliza la sobreingeniería). Para un producto en producción se recomienda añadir tests de reglas y handlers.
- El mock external service es intencionadamente minimalista para facilitar pruebas locales.

---

Si quieres, se puede extender esta documentación con diagramas, secuencias y ejemplos de payloads (JSON) usados por el `EventProcessingBackgroundService`.