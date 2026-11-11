# AD01: Diseño de Arquitecturas Distribuidas — Comercio Multibodega

**Carrera:** Desarrollo de Software  
**Materia:** Aplicaciones Distribuidas – Unidad 1 – Práctica 1  
**Estudiante:** Luis Tuquinga  
**Código:** AD01  

---

## 📌 Descripción del Proyecto
Diseño e implementación de 3 microservicios REST independientes desarrollados con **ASP.NET Core Web API** y **SQL Server**, cada uno con su propia base de datos desacoplada (patrón *Database-per-service*), aplicando principios de arquitecturas distribuidas, operaciones CRUD, validaciones de entrada con códigos de estado HTTP apropiados (200, 201, 400, 404, 502, 503) y comunicación inter-servicio mediante `IHttpClientFactory`.

---

## 🏗️ Arquitectura General

```
                        Cliente / Swagger / Postman
                                    │
                                    ▼
                              Pedidos.Api
                     (Cliente · Pedido · DetallePedido)
                       Puerto: 5003 | BD: PedidosDB
                                    │
                    ┌───────────────┴───────────────┐
                    ▼                               ▼
               Catalogo.Api                   Inventario.Api
      (Producto · Categoria · Marca)  (Bodega · Existencia · Movimiento · Reserva)
       Puerto: 5001 | BD: CatalogoDB        Puerto: 5002 | BD: InventarioDB
```

---

## 🗄️ Modelo de Datos y Bases de Datos (SQL Server)

El script SQL completo para la creación de las 3 bases de datos y sus datos iniciales de prueba se encuentra en:
📁 [`BaseDatos_AD01.sql`](./BaseDatos_AD01.sql)

### 1. CatalogoDB (Microservicio 1: Catalogo.Api)
- **Categoria:** `IdCategoria`, `Nombre`, `Descripcion`
- **Marca:** `IdMarca`, `Nombre`, `PaisOrigen`
- **Producto:** `IdProducto`, `IdCategoria` (FK física), `IdMarca` (FK física), `Nombre`, `SKU`, `Precio`, `Estado`

### 2. InventarioDB (Microservicio 2: Inventario.Api)
- **Bodega:** `IdBodega`, `Nombre`, `Ciudad`, `Direccion`
- **Existencia:** `IdExistencia`, `IdProducto` (FK lógica a Catalogo), `IdBodega` (FK física), `Stock`
- **MovimientoInventario:** `IdMovimiento`, `IdProducto` (FK lógica), `IdBodega` (FK física), `Tipo`, `Cantidad`, `Fecha`
- **ReservaStock:** `IdReserva`, `IdProducto` (FK lógica), `IdBodega` (FK física), `Cantidad`, `Estado`

### 3. PedidosDB (Microservicio 3: Pedidos.Api)
- **Cliente:** `IdCliente`, `Nombres`, `Cedula`, `Correo`
- **Pedido:** `IdPedido`, `IdCliente` (FK física), `Fecha`, `Total`, `Estado`
- **DetallePedido:** `IdDetalle`, `IdPedido` (FK física), `IdProducto` (FK lógica), `Cantidad`, `PrecioUnitario`

---

## 🚀 Puertos de Ejecución y Swagger

Cada microservicio cuenta con su perfil configurado en `Properties/launchSettings.json` con apertura automática de Swagger UI:

| Microservicio | URL Base | Documentación Interactiva |
| :--- | :--- | :--- |
| **Catalogo.Api** | `http://localhost:5001` | `http://localhost:5001/swagger` |
| **Inventario.Api** | `http://localhost:5002` | `http://localhost:5002/swagger` |
| **Pedidos.Api** | `http://localhost:5003` | `http://localhost:5003/swagger` |

---

## 🔄 Endpoints Distribuidos (Comunicación entre Microservicios)

- **`GET http://localhost:5001/api/productos/{id}/con-inventario`**  
  `Catalogo.Api` consulta su base de datos local y realiza una petición HTTP asíncrona a `Inventario.Api` (`/api/existencias/por-producto/{id}`) utilizando `IHttpClientFactory`, combinando los datos del producto con su stock físico.
- **`GET http://localhost:5003/api/pedidos/{id}/con-catalogo`**  
  `Pedidos.Api` consulta el pedido con sus detalles y resuelve la información de nombres y precios de los productos consumiendo `Catalogo.Api`.

---

## 📋 Documentación de Política de IA

- Documento Word: [`Declaracion_Uso_IA_TuquingaLuis_AD01.docx`](./Declaracion_Uso_IA_TuquingaLuis_AD01.docx)
- Documento PDF: [`Declaracion_Uso_IA_TuquingaLuis_AD01.pdf`](./Declaracion_Uso_IA_TuquingaLuis_AD01.pdf)
