# Uso del Gemelo Digital como Simulador de Diseño para Optimizar la Navegación y la Comprensión en la Exposición "Explorando la Cuántica"

![Gemelo Digital](https://github.com/user-attachments/assets/2d707096-ba58-4c13-8582-5886a6696063)


## Tecnologías usadas
- Unity 2022.3.62f2
- Ubiq Framework
- XR Interaction Toolkit

## Pre-requisitos
- **Unity 2022.3.62f2** instalado
- XR Plug-in Management configurado
- Auriculares VR y controladores

## Descripción
Este proyecto utiliza un gemelo digital de la exposición "Explorando la Cuántica" para simular la disposición del espacio expositivo, optimizar la navegación de los visitantes y facilitar la comprensión de los contenidos mediante interacciones en 3D. Además, permite experiencias multiusuario al conectarse a un servidor Ubiq, lo que hace posible que varios usuarios interactúen en el mismo entorno virtual de manera simultánea.
### Características
- Interacción con objetos y paneles explicativos en VR.
- Conexión a servicios de Ubiq para colaboración en tiempo real.
- Compatibilidad con entre diferentes modelos Quest.

## Instrucciones para desarrollo
1. Abrir la carpeta del proyecto con **Unity Hub** (Unity 2022.3.62f2).  
2. Instalar los paquetes requeridos desde **Package Manager**:
   - `com.ucl.ubiq`
   - `XR Interaction Toolkit`  
3. Ejecutar el proyecto desde Unity para pruebas locales.

---

## Instrucciones para usar el ejecutable
Descargar apk y usar **SideQuest** o `adb install archivo.apk`  

### Conectarse al servidor Ubiq (opcional, para multiusuario)
Para la conexión se puede hacer unicamente con las quest conectadas por cable al computador y ejecutando el demo en unity. Se debe realizar la union a un room ya creado o crear uno nuevo. 
