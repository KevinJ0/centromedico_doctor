# Centro Médico — Panel de administración (`centromedico_doctor`)

Backend y panel de administración del ecosistema **Centro Médico**: una aplicación integral para la gestión de un centro médico dominicano.

| Repositorio | Rol |
| --- | --- |
| [`centromedico_database`](https://github.com/KevinJ0/centromedico_database) | Capa de datos central (EF Core, 30 entidades) |
| [`centromedico_doctor`](https://github.com/KevinJ0/centromedico_doctor) | **Este repositorio**: API de administración + panel web |
| [`centromedico_cliente`](https://github.com/KevinJ0/centromedico_cliente) | Portal público para pacientes + API |

## Características

- **Autenticación y roles**: ASP.NET Identity + JWT Bearer. Roles Doctor / Secretary / Patient, con autorización por recurso (cada médico solo accede a su consultorio).
- **Tiempo real sobre la base de datos**: `SqlTableDependency` detecta cambios en `Citas`, `Turnos` y `Pacientes` y los propaga por **SignalR** a grupos por médico/secretario.
- **Agenda inteligente**: horarios por slot, duración de consulta configurable, feriados, bloque de almuerzo y generación automática de turnos con código de verificación único.
- **Gestión clínica y operativa**: pacientes, citas, laboratorio, cobros con cobertura de seguros, reportes y estado de cuenta.
- **Notificaciones**: emails transaccionales con plantilla HTML vía **MailKit** (con reintentos resilientes con **Polly**); notificaciones por **WhatsApp (Twilio)**.
- **Archivos**: fotos de perfil de médicos y pacientes en **AWS S3**.
- **API documentada**: Swagger/OpenAPI con comentarios XML.

## Stack

- **Backend**: ASP.NET Core 5 (Web API) + Entity Framework Core 5 + AutoMapper
- **Frontend**: Angular 14 + Angular Material + Bootstrap
- **Datos**: SQL Server (modelo central en `centromedico_database`)

## Estructura

```
CentromedicoDoctor/    # API + ClientApp (Angular)
Doctor.Repository/     # Repositorios (data access)
Doctor.DTO/            # Objetos de transferencia de datos
```

## Demostración

[Ver demo en YouTube](https://youtu.be/lxWGj7Vem54)

## Configuración

> ⚠️ **Seguridad**: este repositorio es **público**. No uses credenciales reales en `appsettings.json` — muévelas a variables de entorno, *User Secrets* o un gestor de secretos, y rota cualquier llave que haya sido expuesta.