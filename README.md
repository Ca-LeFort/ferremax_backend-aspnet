# API Principal del Proyecto Ferremas 🛍️🔩

¡Bienvenido a la **API Principal de Ferremas**! Esta es una solución backend robusta y eficiente, desarrollada con **ASP.NET**, diseñada para la gestión integral de productos en la plataforma de e-commerce Ferremas. Este proyecto forma parte del ramo Integración de Plataformas de DUOC UC.

[![C#](https://img.shields.io/badge/Python-3.8+-blue.svg)](https://www.python.org/downloads/)
[![FastAPI](https://img.shields.io/badge/FastAPI-0.100+-05998b.svg)](https://fastapi.tiangolo.com/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0+-4479a1.svg)](https://www.mysql.com/)
[![Uvicorn](https://img.shields.io/badge/Uvicorn-ASGI-green.svg)](https://www.uvicorn.org/)
---

## 📖 Índice

- [🌟 Características Destacadas](#características-destacadas)
- [📦 Requisitos Previos](#requisitos-previos)
- [🔧 Instalación y Configuración](#instalación-y-configuración)
  - [1. Clonar el Repositorio](#1-clonar-el-repositorio)
  - [2. Instalar Dependencias](#2-instalar-dependencias)
  - [3. Configurar appsettings.json](#3-configurar-appsettings-json)
  - [4. Configuración de la Base de Datos](#4-configuración-de-la-base-de-datos)
  - [5. Ejecutar el Servidor de Desarrollo](#5-ejecutar-el-servidor-de-desarrollo)
- [ Documentación interactiva](#documentación-interactiva)

---

## Características Destacadas

- **Framework ASP.NET**: Aprovecha el rendimiento y la simplicidad de FastAPI.
- **Integración con MySQL**: Diseñada para funcionar sin problemas con una base de datos MySQL.
- **Controladores completos**: Servicio Backend con los controladores completos con la base de datos.
- **Integración con Mercado Pago**: Controlador de Pago con Mercado Pago incorporado.

---

## Requisitos Previos

Asegúrate de tener instalados los siguientes componentes antes de comenzar:

- Framework .NET 8.0 o superior
- Servidor de base de datos MySQL
- Git

---

## Instalación y Configuración

Sigue estos pasos para configurar la API:

### 1. Clonar el Repositorio

```bash
git clone https://github.com/Ca-LeFort/ferremax_backend-aspnet.git
cd ferremax_backend-aspnet
```

### 2. Instalar dependencias

```bash
dotnet build
```

### 3. Configurar appsettings.json
Edita el archivo de JSON para aplicar las configuraciones:

```appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SERVIDOR;Database=BASE_DE_DATOS;User=USUARIO;Password=CONTRASEÑA"
  },
  "Jwt": {
    "Key": "KEY_JWT",
    "Issuer": "ISSUER",
    "Audience": "AUDIENCIA"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Resend": {
    "ApiKey": "KEY_RESEND"
  },
  "MercadoPago": {
    "AccessToken": "TOKEN_MERCADO_PAGO",
    "PublicKey": "PUBLIC_KEY_MERCADO_PAGO"
  },
  "AllowedHosts": "*"
}

```
Estas variables son esenciales para que la API se conecte a la base de datos MySQL, integración con Mercado Pago y autenticación con JWT.

### 5. Configuración de la base de datos
Asegúrate de que tu servidor MySQL esté en funcionamiento y de que la base de datos especificada en el archivo appsettings.json esté creada. Puedes usar el siguiente comando para crear la base de datos:

```sql
CREATE DATABASE base_de_datos;
```

### 6. Ejecutar el servidor de desarrollo
Con todas las configuraciones en su lugar, inicia el servidor de la API utilizando el comando:
```bash
dotnet run
```
El en caso de usar el Visual Studio ejecutar con el botón que dice https o http

---

## Documentación interactiva

Una vez que el servidor esté corriendo se accede a la documentación interactiva de la API (Swagger UI)

Aquí podrás explorar todos los endpoints disponibles, ver sus parámetros esperados y probar las solicitudes directamente desde el navegador.

---
