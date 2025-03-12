# prueba_practica_payphone
API REST para gestionar transferencias de saldo.

Esta es una API REST desarrollada en **.NET 8** para gestionar billeteras y transacciones. Los usuarios pueden crear billeteras, realizar transferencias entre billeteras, y consultar el historial de transacciones.

## Requisitos

- **.NET 8**: Asegúrate de tener instalada la versión más reciente de .NET 8.
- **SQL Server**: La base de datos de la API está configurada para usar SQL Server.

## Pasos para Levantar el Proyecto

### 1. Clonar el Repositorio

```bash
git clone https://github.com/Edu4rdoBG/prueba_practica_payphone.git
```

### 2. Configurar la Base de Datos

Asegúrate de tener una instancia de **SQL Server** en ejecución. Luego, configura la cadena de conexión en el archivo `appsettings.json`:

```bash
json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=WalletDb;User Id=admin;Password=tu-contraseña;"
}
```

### 3. Ejecutar las Migraciones

Para aplicar las migraciones y crear la base de datos, ejecuta el siguiente comando en la terminal:
dotnet ef database update

### 4. Ejecutar el Proyecto
Para levantar el servidor y que la API esté en funcionamiento, ejecuta el siguiente comando:
dotnet run
O bien iniciar el proyecto desde Visual Studio para visualizar el Swagger

La API estará disponible en http://localhost:{puerto}.

### 5. Probar la API
Usa una herramienta como Postman o Insomnia para probar los endpoints disponibles.

#### Crear Token
Ejemplo de solicitud de login para obtener el token:
Para obtener un token de autenticación, realiza una solicitud POST a la ruta `/api/auth/login`:

```bash
curl --location 'https://localhost:{puerto}/api/auth/login' \
--header 'Content-Type: application/json' \
--data '{
  "username": "admin",
  "password": "1234"
}'
```

Ejemplo de respuesta:
```bash
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoxMjM0NTY3ODkwLCJpYXQiOjE2Njg0MzQ5ODZ9.nosd8FKjdsfLKz3Xy"
}
```

#### Crear Wallet/Billetera

```bash
curl --location 'https://localhost:{puerto}/api/Wallet' \
--header 'Authorization: Bearer ...' \
--header 'Content-Type: application/json' \
--data '{
  "documentId": "123456789",
  "name": "Juan Pérez",
  "balance": 2000.50
}'
```

Ejemplo de respuesta:
```bash
{
  "id": 1,
  "documentId": "123456789",
  "name": "Juan Pérez",
  "balance": 2000.50,
  "createdAt": "2025-03-12T12:34:56",
  "updatedAt": "2025-03-12T12:34:56"
}
```

#### Crear Transacción

```bash
curl --location 'https://localhost:{puerto}/api/Transaction/wallet/{walletId}' \
--header 'Authorization: Bearer ...' \
--header 'Content-Type: application/json' \
--data '{
  "amount": 200,
  "type": "Debit"
}'
```

Ejemplo de respuesta:
```bash
{
  "id": 0,
  "walletId": 0,
  "amount": 200,
  "type": "Debit",
  "createdAt": "2025-03-12T19:10:26.737Z"
}
```

Para ver todos los metodos puede consultar la siguiente url una vez inicie el proyecto de forma local:
https://localhost:{puerto}/swagger/index.html

### Metodos disponibles:

![image](https://github.com/user-attachments/assets/c9f6a1b4-4508-4153-9196-78ae4175e196)

### 6. Pruebas
El proyecto contiene pruebas unitarias e integración que puedes ejecutar con el siguiente comando o utilizar las herramientas de prueba de Visual Studio:
dotnet test

