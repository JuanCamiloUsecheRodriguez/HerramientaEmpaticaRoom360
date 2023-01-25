# Herramienta Empatia ROOM 360

### Descripción: 
El proyecto hace parte de una investigación que busca apoyar la generación de empatía que se puede crear mediante videos de realidad virtual 360. Para tal fin, se desarrolló una aplicación de realidad virtual en la cual se pueden reproducir e intervenir videos 360. Los usuarios de la aplicación pueden comentar y leer bullet comments dejados sobre el video 360.


> 📺 Ver Trailer: https://www.youtube.com/watch?v=NoRnkz74sc8 

---

## Prerrequisitos, descarga y ejecución:

A continuación, se describen los prerequisitos, las instrucciones y consideraciones adicionales para poder correr ROOM360 en tu propia maquina.

### Prerrequisitos
Hardware:
- [x] Meta Quest 2
- [x] Un computador desde el cual se instalará el juego. Este debe tener minimo un procesador de +2.0GHz y 2GB de RAM.
- [x] Cable USB C compatible con el Meta Quest 2

Software:
- [x] One Drive
- [x] Unity Hub (De preferencia 3.x.x)
- [x] Unity 2020.3.1f1 
- [x] Android SDK
- [x] Aplicación Oculus Escritorio
- [x] Aplicación Oculus Dispositivo Móvil 


### Instrucciones para configurar ambiente de trabajo: 

1. **Unity Hub**: Para instalar Unity, se debe dirigir a la página de Unity e instalar [Unity Hub](https://unity.com/download). Asegurese de elegir el archivo correspondiente a su sistema operativo.

2. **Unity 2020.3.1f1**: Una vez tenga instalado Unity Hub en su computador, dirijase a [esta pagina](https://unity.com/releases/editor/archive) y seleccione la version solicitada de unity, en este caso la 2020.3.1f1. Haga click en la opción de Unity Hub para instalar.

3. **Android SDK**: Al seleccionar la opción de Unity Hub en el último paso, se abrirá su aplicación de Unity Hub con más detalles y opciones para instalar la versión solicitada de Unity. En esta pestaña, seleccione las opciones de *Android Build Support* como se muestra en la imagen de abajo y de click en continuar.

4. **Oculus Escritorio**: Descargue e instale la aplicación de [Oculus para escritorio](https://www.meta.com/quest/setup/). Una vez iniciada la aplicación, verifique que la conexión entre su dispositivo Quest 2 y escritorio funcionan al conectar estos y verificar la sección de dispositivos. En esta se debería mostrar el dispositivo y el estado "conectado".

5. En caso de que no tenga una cuenta de desarrollador y no pertenezca a ninguna organización, dirijase a https://dashboard.oculus.com/organizations/create/ y diligencie la información necesaria para crear una organización. 

6. **Oculus Móvil**: Descargue e instale la aplicación de Oculus en su dispositivo móvil. Siga las instrucciones en el dispositivo para iniciar sesión con su cuenta de desarrollador de Oculus. En el apartado de configuraciones, active el modo de desarrollador.

**Una vez se sigan estos pasos de configuración, se puede seguir a descargar e importar el proyecto en Unity**

### Instrucciones para descargar e importar el proyecto de Wizard Chess en Unity:

1. Descargue el proyecto del link de github
2. Descromprima el proyecto en la carpeta que desea utilizar
3. Desde Unity Hub, seleccione la opción de abrir un nuevo proyecto y seleccione el directorio donde descargó el proyecto.

### Instrucciones para ejecutar el proyecto desde Unity:

1. Conecte el dispositivo Quest 2 a su computador y verifique la conexión de este mediante la aplicación de Oculus de escritorio. 

2. Abra el proyecto en Unity

3. Abra *File* -> *Build and Run* 

4. Finalmente, de click en el botón Build and Run para construir el APK y ejecutarlo desde su dispositivo. 

### Instalar con APK
Para instalar el APK, se debe tener instalado [SIDEQUEST](https://sidequestvr.com/) en el computador donde se encuentra el APK de la aplicación. Una vez se tenga configurado SIDEQUEST, se debe conectar el casco de realidad virtual Quest 2 al computador y verificar su conexión en la aplicación SIDEQUEST. Luego, se debe seleccionar la opción de “Instalar archivo APK desde computador” y seleccionar el APK de Wizard Chess. Al finalizar, la aplicación se puede encontrar en la sección de “Fuentes desconocidas” del Quest 2. 
