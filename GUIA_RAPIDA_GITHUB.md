# ?? CÓMO SUBIR NEXSHOP A GITHUB EN 5 PASOS

## OPCIÓN A: Usar el Script Automático (RECOMENDADO)

### En Windows (PowerShell):
```powershell
# 1. Abre PowerShell en la carpeta raíz del proyecto
cd "E:\Proyectos Visual\NexShop"

# 2. Ejecuta el script
.\subir-a-github.ps1

# 3. Ingresa tu usuario de GitHub y token
# 4. El script hará todo automáticamente
```

### En macOS/Linux (Terminal):
```bash
# 1. Abre Terminal en la carpeta raíz del proyecto
cd ~/Proyectos/NexShop

# 2. Ejecuta el script
bash subir-a-github.sh

# 3. Ingresa tu usuario de GitHub y token
# 4. El script hará todo automáticamente
```

---

## OPCIÓN B: Pasos Manuales (Si prefiero hacerlo a mano)

### Paso 1: Crear Repositorio en GitHub
1. Ve a https://github.com/new
2. Nombre: `NexShop`
3. Descripción: `Plataforma de E-Commerce con ASP.NET Core 8`
4. Selecciona "Public" (si quieres que sea público)
5. **NO** inicialices con archivos
6. Click en "Create repository"

### Paso 2: Obtener tu Token de Acceso
1. Ve a https://github.com/settings/tokens
2. Click en "Generate new token" ? "Generate new token (classic)"
3. Dale un nombre: `NexShop-Upload`
4. Selecciona estos permisos:
   - ? `repo` (acceso completo a repositorios)
5. Click en "Generate token"
6. **COPIA el token** (lo necesitarás en el siguiente paso)

### Paso 3: Inicializar Git en tu Proyecto
Abre PowerShell en la carpeta raíz:
```powershell
cd "E:\Proyectos Visual\NexShop"

# Inicializar repositorio
git init

# Configurar Git (primera vez)
git config --global user.name "Tu Nombre Completo"
git config --global user.email "tu.email@gmail.com"
```

### Paso 4: Agregar Archivos y Hacer Commit
```powershell
# Ver estado
git status

# Agregar todos los archivos
git add .

# Hacer commit
git commit -m "Initial commit: NexShop e-commerce platform"
```

### Paso 5: Subir a GitHub
```powershell
# Reemplaza TU_USUARIO con tu usuario de GitHub
git remote add origin https://github.com/TU_USUARIO/NexShop.git

# Cambiar a rama main
git branch -M main

# Subir los cambios
# Cuando pida contraseña, pega tu token
git push -u origin main
```

---

## ?? IMPORTANTE: Generar Personal Access Token

### ¿Por qué necesito un token?
GitHub requiere autenticación de dos factores. No puedes usar tu contraseña normal en la terminal.

### Cómo crear un token (paso a paso):
1. **Ve a:** https://github.com/settings/tokens
2. **Click en:** "Generate new token" (parte superior derecha)
3. **Selecciona:** "Generate new token (classic)"
4. **Configura:**
   - **Note:** `NexShop-Upload`
   - **Expiration:** 90 days (o lo que prefieras)
   - **Scopes:** Marca solo `repo` ?
5. **Click en:** "Generate token" (al final de la página)
6. **COPIA el token** (aparecerá en verde)
   - ?? Solo aparecerá una vez, cópialo ahora
   - Si lo pierdes, crea uno nuevo

### Cómo usar el token:
En el comando `git push`:
```powershell
# Cuando pida contraseña, escribe el token
git push -u origin main
# Usuario: tu_usuario_github
# Contraseña: tu_token_aqui
```

---

## ?? Verificar que Todo Funcionó

Después de subir, ve a GitHub:
```
https://github.com/TU_USUARIO/NexShop
```

Deberías ver:
- ? Tu código en la rama `main`
- ? Carpetas: Controllers, Views, Models, Services, etc.
- ? Archivos: Program.cs, appsettings.json, README.md, etc.
- ? Historial de commits

---

## ?? Recursos Útiles

### Comandos Git Útiles:
```powershell
# Ver estado
git status

# Ver historial
git log --oneline

# Ver ramas
git branch -a

# Crear rama nueva
git checkout -b nombre-rama

# Ver cambios sin subir
git diff

# Deshacer cambios locales
git restore archivo.cs

# Cambiar último commit
git commit --amend
```

### Problemas Comunes:

**Problema:** "fatal: remote origin already exists"
```powershell
git remote remove origin
git remote add origin https://github.com/TU_USUARIO/NexShop.git
```

**Problema:** "Authentication failed"
```powershell
# Usa HTTPS con token
git remote set-url origin https://TU_USUARIO:TOKEN@github.com/TU_USUARIO/NexShop.git
```

**Problema:** "Permission denied (publickey)"
```powershell
# Verifica SSH
ssh -T git@github.com
# O usa HTTPS en lugar de SSH
```

---

## ?? Siguientes Pasos (Opcional)

Una vez subido a GitHub:

### 1. Añade Descripción al Repositorio
- Ve a Settings
- En "About" escribe:
  ```
  ??? Plataforma de E-Commerce con ASP.NET Core 8 y SQL Server
  
  - ?? Autenticación con Identity
  - ?? CRUD de Productos
  - ?? Carrito de Compras
  - ?? Estadísticas de Vendedor
  - ?? Sistema de Preguntas y Respuestas
  ```

### 2. Añade Temas (Topics)
- Click en "About" ? "Topics"
- Añade: `aspnetcore`, `ecommerce`, `csharp`, `entity-framework`, `sql-server`

### 3. Habilita GitHub Pages (opcional)
- Settings ? Pages
- Source: main
- Folder: docs
- Podrás tener documentación en: `https://tu_usuario.github.io/NexShop`

### 4. Crea Issues para Tareas
- Abre issues para features que falta implementar
- Usa labels: enhancement, bug, documentation

### 5. Habilita Discussions (opcional)
- Settings ? Features
- Marca "Discussions"
- Permite que otros hagan preguntas

---

## ?? ¿Necesitas Ayuda?

- **Docs de Git:** https://git-scm.com/doc
- **Docs de GitHub:** https://docs.github.com
- **Tutorial Git:** https://github.com/git-tips/tips

---

**¡Tu proyecto estará en GitHub en menos de 10 minutos!** ??

Fecha: 2025-11-28  
Status: ? LISTO PARA GITHUB
