# ?? GUÍA PARA SUBIR A GITHUB

## ?? Pasos Previos

### 1. Verificar que tienes Git instalado
```powershell
git --version
```

### 2. Configurar Git (primera vez)
```powershell
git config --global user.name "Tu Nombre"
git config --global user.email "tu.email@example.com"
```

---

## ?? OPCIÓN 1: Crear un Repositorio NUEVO en GitHub

### Paso 1: Crear repositorio en GitHub.com
1. Abre https://github.com/new
2. Nombre del repositorio: `NexShop`
3. Descripción: `Plataforma de E-Commerce con ASP.NET Core 8`
4. Selecciona "Public" o "Private"
5. **NO** inicializes con README, .gitignore o LICENSE (ya los tenemos)
6. Haz clic en "Create repository"

### Paso 2: Ejecutar estos comandos en PowerShell
```powershell
cd "E:\Proyectos Visual\NexShop"

# Inicializar git
git init

# Agregar todos los archivos
git add .

# Commit inicial
git commit -m "Initial commit: NexShop e-commerce platform"

# Agregar el repositorio remoto (reemplaza TU_USUARIO con tu usuario de GitHub)
git remote add origin https://github.com/TU_USUARIO/NexShop.git

# Cambiar la rama principal a main (si es necesario)
git branch -M main

# Subir los cambios
git push -u origin main
```

---

## ?? OPCIÓN 2: Si ya existe el repositorio

```powershell
cd "E:\Proyectos Visual\NexShop"

# Agregar los cambios
git add .

# Verificar estado
git status

# Hacer commit
git commit -m "Correcciones finales: ErrorController y scripts SQL"

# Subir cambios
git push origin main
```

---

## ?? CONFIGURACIÓN CON SSH (Recomendado para futuro)

### Generar clave SSH
```powershell
ssh-keygen -t rsa -b 4096 -C "tu.email@example.com"
```

### Agregar clave a GitHub
1. Copia el contenido de `~/.ssh/id_rsa.pub`
2. Abre https://github.com/settings/keys
3. Click en "New SSH key"
4. Pega tu clave pública

### Usar SSH en lugar de HTTPS
```powershell
git remote set-url origin git@github.com:TU_USUARIO/NexShop.git
```

---

## ?? Verificar que todo está bien

```powershell
# Ver los repositorios remotos configurados
git remote -v

# Ver el estado actual
git status

# Ver el historial de commits
git log --oneline -5
```

---

## ? Resultado esperado

Si ves esto en `git remote -v`:
```
origin  https://github.com/TU_USUARIO/NexShop.git (fetch)
origin  https://github.com/TU_USUARIO/NexShop.git (push)
```

¡Todo está configurado correctamente! ?

---

## ?? Solución de Problemas

### Error: "fatal: remote origin already exists"
```powershell
git remote remove origin
git remote add origin https://github.com/TU_USUARIO/NexShop.git
```

### Error: "fatal: 'origin' does not appear to be a 'git' repository"
```powershell
git remote add origin https://github.com/TU_USUARIO/NexShop.git
```

### Error de autenticación con HTTPS
Usa un Personal Access Token en lugar de contraseña:
1. Abre https://github.com/settings/tokens
2. Crea un nuevo token con permisos `repo`
3. Usa el token como contraseña

---

## ?? Estructura que se subirá

```
NexShop/
??? .gitignore                    ? Archivos a ignorar
??? README.md                     ? Documentación principal
??? LICENSE                       ? MIT License
??? NexShop.Web/
?   ??? .gitignore
?   ??? Controllers/              ? Todos los controladores
?   ??? Views/                    ? Todas las vistas Razor
?   ??? Areas/Identity/           ? Autenticación
?   ??? Models/                   ? Modelos de datos
?   ??? Services/                 ? Servicios de negocio
?   ??? Migrations/               ? Migraciones EF Core
?   ??? Scripts/                  ? Scripts SQL
?   ??? Program.cs                ? Configuración
?   ??? appsettings.json          ? Configuración (sin secrets)
?   ??? wwwroot/                  ? Archivos estáticos
?   ??? NexShop.Web.csproj        ? Archivo de proyecto
??? .git/                         (creado automáticamente)
```

### ?? Lo que NO se sube
```
NexShop.Web/bin/              (binarios compilados)
NexShop.Web/obj/              (archivos compilación)
.vs/                          (Visual Studio)
.vscode/                      (Visual Studio Code)
wwwroot/uploads/              (multimedia local)
*.db                          (base de datos local)
.env                          (secretos)
```

---

## ?? Comandos Rápidos

```powershell
# Después de hacer cambios en el código:
git add .
git commit -m "Descripción del cambio"
git push origin main

# Ver cambios pendientes
git status

# Ver historial
git log --oneline

# Descargar cambios del remoto
git pull origin main
```

---

## ?? Próximos Pasos

1. ? Ejecuta los comandos de la OPCIÓN 1
2. ? Verifica en https://github.com/TU_USUARIO/NexShop que todo está
3. ? Añade descripción y temas al repositorio
4. ? Activa Pages si quieres documentación web
5. ? Configura Actions para CI/CD (opcional)

---

**¡Tu proyecto estará en GitHub en menos de 5 minutos!** ??

