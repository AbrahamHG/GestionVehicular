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
- Ejecutar de Visual Studio (arriba a la izquierda), cambia el perfil a https de tu API no usar Container (Dockerfile) ni docker-compose o en dado caso usat http.
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

PRUEBAS HECHAS:
<img width="589" height="400" alt="Image" src="https://github.com/user-attachments/assets/2bb0a2a6-9e94-4af0-b0bf-1ebe7963cef7" />
<img width="589" height="211" alt="Image" src="https://github.com/user-attachments/assets/4ef05a73-cf98-407d-a794-1be33b30f6c0" />
<img width="589" height="352" alt="Image" src="https://github.com/user-attachments/assets/2d964553-3ad4-4964-9b98-ceb80d418fd2" />
<img width="589" height="325" alt="Image" src="https://github.com/user-attachments/assets/03087b40-24a8-4bad-ac40-29499e41292b" />
<img width="589" height="355" alt="Image" src="https://github.com/user-attachments/assets/4061028d-3d90-4954-93e2-a952266a09df" />
<img width="589" height="331" alt="Image" src="https://github.com/user-attachments/assets/c4737a21-4884-4dea-9038-1f962723de01" />
<img width="589" height="351" alt="Image" src="https://github.com/user-attachments/assets/b3ecfeef-a04c-41ef-b274-c40bfc29cdd5" />
<img width="589" height="320" alt="Image" src="https://github.com/user-attachments/assets/faa882ed-dea9-4ae9-a06c-8e8287751470" />
