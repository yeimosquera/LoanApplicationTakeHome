# 💻 Loan Application Platform — Cliente Frontend (React + TypeScript)

![React](https://img.shields.io/badge/React-18.0-61DAFB?style=for-the-badge&logo=react&logoColor=black)
![TypeScript](https://img.shields.io/badge/TypeScript-5.0-3178C6?style=for-the-badge&logo=typescript&logoColor=white)
![Vite](https://img.shields.io/badge/Vite-5.0-646CFF?style=for-the-badge&logo=vite&logoColor=white)
![Tailwind CSS](https://img.shields.io/badge/Tailwind-3.0-06B6D4?style=for-the-badge&logo=tailwindcss&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![Nginx](https://img.shields.io/badge/Nginx-Production-009639?style=for-the-badge&logo=nginx&logoColor=white)

---

## 📋 Resumen del Cliente Web

Cliente **Single Page Application (SPA)** moderno, desacoplado y optimizado para entornos de producción. La interfaz está centrada en un formulario dinámico de solicitud de crédito con **validación en tiempo real**, estados de carga reactivos (spinners, indicadores de estado y retroalimentación visual) y presentación clara de resultados (Aprobado, Rechazado o Lista de Errores). 

Su integración con la API Backend (.NET 8) se realiza mediante un cliente HTTP centralizado con interceptores que capturan y transforman el formato de error estándar **RFC 7807** (`ProblemDetails` / `ValidationProblemDetails`) en mensajes de interfaz amigables por campo.

---

## 📁 Estructura del Proyecto (`src/`)

```text
frontend/
├── package.json
├── vite.config.ts
├── .env.example
├── Dockerfile
├── nginx/
│   └── default.conf
├── public/
│   └── index.html
└── src/
    ├── main.tsx
    ├── App.tsx
    ├── index.css
    ├── assets/
    │   ├── logo.svg
    │   └── images/
    ├── components/
    │   ├── ApplicationForm/
    │   │   ├── ApplicationForm.tsx
    │   │   ├── ApplicationForm.styles.tsx
    │   │   └── ApplicationSummary.tsx
    │   └── ui/
    │       ├── Button.tsx
    │       ├── Input.tsx
    │       ├── Spinner.tsx
    │       └── Alert.tsx
    ├── pages/
    │   ├── Home.tsx
    │   └── Results.tsx
    ├── services/
    │   ├── apiClient.ts          # Cliente HTTP centralizado (Axios)
    │   └── applications.ts       # Consumo de API (POST /api/applications)
    ├── hooks/
    │   ├── useApplication.ts     # Custom hook para estado y side-effects
    │   └── useToast.ts
    ├── types/
    │   └── index.ts              # DTOs, interfaces y tipos RFC 7807
    ├── validators/
    │   └── applicationSchema.ts # Esquema de validación Zod
    ├── styles/
    │   ├── tailwind.css
    │   └── globals.css
    └── utils/
        └── problemDetailsMapper.ts # Transformador de errores API a Formulario
```
 Arquitectura Frontend y Patrones de Diseño
Separación UI / Lógica de Negocio: Encapsulamiento total mediante Custom Hooks (src/hooks). Los componentes de UI son mayoritariamente declarativos y sin estado pesado.

Cliente HTTP Centralizado (src/services/apiClient.ts):

Basado en una instancia única de Axios.

Interceptores de entrada para inyectar headers globales (Authorization, Correlation-Id).

Interceptores de salida que capturan códigos HTTP de error (400, 500) y parsean payloads RFC 7807 automáticamente.

Manejo Optimizado de Formularios:

React Hook Form para un renderizado mínimo e intencional del árbol DOM.

Zod para la definición y cumplimiento de esquemas estrictos de validación en el cliente.

Validación dinámica (onChange / onBlur) con feedback inline.

UX, Accesibilidad y Responsividad:

Enfoque Mobile-First utilizando utilidades semánticas de Tailwind CSS.

Cumplimiento de estándares de accesibilidad (WAI-ARIA, atributos aria-* y soporte de navegación por teclado).

Bloqueo de doble envío (double-submit prevention) mediante deshabilitación de inputs y spinners.

Despliegue Contenedorizado:

Compilación multi-stage con Vite servida por un servidor Nginx optimizado para cacheo y manejo de rutas en SPA.

🛠️ Stack Tecnológico
Core: React 18+, TypeScript 5+

Herramienta de Construcción: Vite

Formularios & Validación: React Hook Form, Zod

Cliente HTTP: Axios (con interceptores personalizados)

Estilos & UI: Tailwind CSS, CSS Modules

Pruebas Automatizadas: Jest, React Testing Library

Calidad de Código: ESLint, Prettier

Infraestructura: Docker (Multi-stage build), Nginx

🚀 Guía de Instalación y Ejecución Paso a Paso
Requisitos Previos
Node.js: v18.0.0 o superior

npm: v9.0.0 o superior (o yarn / pnpm)

API Backend: En ejecución en http://localhost:5000 (o ajustar URL en .env)

1. Preparación del Entorno Local
NAVEGAR a la carpeta del proyecto:

Bash
cd frontend
CONFIGURAR las variables de entorno. Copia el archivo .env.example o crea un archivo .env en la raíz de frontend/:

Bash
cp .env.example .env
DEFINIR la URL base de la API backend dentro del archivo .env:

Fragmento de código
VITE_API_BASE_URL=http://localhost:5000
2. Instalación de Dependencias
Ejecuta el gestor de paquetes para instalar todas las dependencias declaradas en package.json:

Bash
npm install
3. Ejecución en Modo Desarrollo
Inicia el servidor de desarrollo local de Vite con Hot Module Replacement (HMR):

Bash
npm run dev
🌐 URL Local: http://localhost:3000 (o http://localhost:5173 según configuración de Vite).

El servidor re-compilará automáticamente los cambios realizados en el código fuente.

4. Compilación y Previsualización de Producción
Para generar el bundle estático optimizado y minificado:

Bash
# Generar archivos de distribución en /dist
npm run build

# Probar el build de producción localmente
npm run preview
5. Despliegue en Contenedor Docker (Opción Alternativa)
Si prefieres ejecutar el cliente frontend dentro de un contenedor Nginx aislado:

CONSTRUIR la imagen Docker:

Bash
docker build -t loanapp-frontend:latest .
EJECUTAR el contenedor mapeando el puerto 3000:

Bash
docker run -d -p 3000:80 --name loanapp-frontend loanapp-frontend:latest
ACCEDER desde el navegador en http://localhost:3000.