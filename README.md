# Products CRUD (MAUI + SQLite)

## Requisitos
- Visual Studio 2022 **.NET MAUI**
- .NET SDK 8

- ## Setup
No requiere configuración manual.
- La app crea la base de datos automáticamente usando SQLite.

## Ejecución
1. Abre la solución en Visual Studio 2022.
2. Selecciona el target **Windows Machine** y presiona **Run (F5)**.
3. Listo: al iniciar, la app crea la base de datos y carga un **seed** con productos demo (si es la primera vez).

## Capturas (flujo principal)

### Búsqueda por Nombre
![Búsqueda por Nombre](Docs/screenshots/1-Busqueda-nombre.png)

### Búsqueda por SKU
![Búsqueda por SKU](Docs/screenshots/2-Busqueda-SKU.png)

### Listado (solo activos)
![Listado solo activos](Docs/screenshots/3-Listado-productos(solo-activos).png)

### Listado (activos e inactivos)
![Listado activos e inactivos](Docs/screenshots/4-Listado-productos(activos-inactivos).png)

### Agregar producto
![Agregar producto](Docs/screenshots/5-Agregar-producto.png)

### Validación al agregar
![Validación](Docs/screenshots/6-Validacion-campos-agregar-producto.png)

### Editar producto
![Editar producto](Docs/screenshots/7-Editar-producto.png)
