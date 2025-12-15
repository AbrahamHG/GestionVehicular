# API Gestion Vehicular (Docker)

Este proyecto implementa una API para la gestión de vehículos, conductores y asignaciones, utilizando **.NET 8**, **SQL Server** y **Docker Compose**.

## Configuración del entorno

### Requisitos previos
- Docker Desktop instalado.
- Visual Studio 2022 o VS Code.
- SQL Server Management Studio (SSMS) o Azure Data Studio para pruebas.

### Estructura de servicios
- **gestionvehicular.api** → API en .NET 8.
- **sqlserver_gestion** → SQL Server con base de datos `GestionVehicular`.
- Credenciales: `Id=sa;Password=TuPassword123!;`

### Puertos 
-salvo que cambien 
- API: `http://localhost:5000`
- SQL Server: `localhost,1433`

## SQL Server Management Studio (SSMS)

1. Conéctate desde SSMS a:
   - Servidor: `localhost,1433`
   - Usuario: `sa`
   - Password: `TuPassword123!`

2. Ejecuta el script `init.sql` para crear en la carpeta Scripts:
   - Base de datos `GestionVehicular`.
   - Tablas: `Vehiculo`, `Conductor`, `Asignacion`.
   - Stored Procedures (SPs) para CRUD.

## Pruebas con Swagger

1. Accede a `http://localhost:5000/swagger`.
2. Endpoints disponibles:
   - `POST /Vehiculo` → Crear vehículo.
   - `GET /Vehiculo` → Listar vehículos.
   - `POST /Conductor` → Crear conductor.
   - `GET /Conductor` → Listar conductores.
   - `POST /Asignacion` → Crear asignación.
   - `GET /Asignacion/{id}` → Consultar asignación.
  
    ## Cambiar Direccion del Fronend para redigir la api a esa direcion
   Probar la logica de Front end.
   cambiar en Program.cs cambiar la direccion de la api en dado caso
   
4. Los cambios se reflejan directamente en la base de datos.
ejecutar 

# API Gestion Vehicular (Ejecución Local)

Este proyecto implementa una API con frontend Razor y base de datos SQL Server.

## Requisitos previos
- Visual Studio 2022 con .NET 8 SDK.
- SQL Server instalado localmente.
- SQL Server Management Studio (SSMS) o Azure Data Studio.
- Base de datos `GestionVehicular` creada con el script `init.sql`

## Pasos de ejecución

1. **Clonar el repositorio**
   Clonar o descargar el repositorio limpiar y recompilar el proyecto
   2. **Configurar la base de datos**
- Abrir init.sql en SSMS.
- Ejecutar el script para crear tablas y Stored Procedures.
3- **Configurar la cadena de conexión**
  En appsettings.json de GestionVehicular.Api

"ConnectionStrings": {
  "DefaultConnection": "CAMBIAR POR LA BASE DE DATOS A UTILIZAR;"
}

4- **Ejecutar la API**
- Ejecutar de Visual Studio (arriba a la izquierda), cambia el perfil a http o https de tu API no usar Container (Dockerfile) ni docker-compose.
- Seleccionar proyecto GestionVehicular.Api. o selecionar multiple de API y FRONTEND para probar las 2

## PROBAR
1. Accede a swagger`.
2. Endpoints disponibles:
   - `POST /Vehiculo` → Crear vehículo.
   - `GET /Vehiculo` → Listar vehículos.
   - `POST /Conductor` → Crear conductor.
   - `GET /Conductor` → Listar conductores.
   - `POST /Asignacion` → Crear asignación.
   - `GET /Asignacion/{id}` → Consultar asignación.

   3-Probar desde en frond end
 ## Cambiar Direccion del Fronend para redigir la api a esa direcion
   Probar la logica de Front end.
   cambiar en Program.cs cambiar la direccion de la api en dado caso

## Pruebas (xUnit)

Este proyecto incluye un conjunto de pruebas unitarias e integración usando **xUnit**:

- Proyecto de pruebas: `GestionVehicular.Tests`
- Integración con .NET 8 y EF Core
- Validación de:
  - Endpoints de la API
  - Stored Procedures de SQL Server
  - Lógica de negocio en controladores y servicios

### Ejecutar pruebas

Este proyecto está configurado para ejecutarse **solo con HTTP** en `http://localhost:5000`.  
Si aparece un error relacionado con certificados HTTPS, asegúrate de:

- Usar el perfil `GestionVehicular.Api` en Visual Studio (no IIS Express).
- Abrir `http://localhost:5000` en el navegador.
- Verificar que `launchSettings.json` no incluya `https://localhost:5001`.
