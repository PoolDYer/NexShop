# ?? NEXSHOP - LISTO PARA SUBIR A GITHUB

## ? Archivos Preparados para GitHub

Se han creado los siguientes archivos de configuración:

### ?? En la raíz del proyecto:
- ? `.gitignore` - Archivos a ignorar
- ? `README.md` - Documentación completa
- ? `LICENSE` - Licencia MIT
- ? `GUIA_GITHUB.md` - Guía técnica completa
- ? `GUIA_RAPIDA_GITHUB.md` - Guía rápida paso a paso
- ? `subir-a-github.ps1` - Script automático (Windows)
- ? `subir-a-github.sh` - Script automático (macOS/Linux)

### ?? En NexShop.Web/:
- ? `.gitignore` - Archivos específicos a ignorar

---

## ?? OPCIÓN MÁS RÁPIDA (Recomendado)

### En Windows (PowerShell):
```powershell
# 1. Abre PowerShell
# 2. Navega a la carpeta raíz
cd "E:\Proyectos Visual\NexShop"

# 3. Ejecuta el script
.\subir-a-github.ps1

# 4. Sigue las instrucciones (solo necesitas tu usuario y token de GitHub)
```

### En macOS/Linux (Terminal):
```bash
cd ~/Ruta/A/NexShop
bash subir-a-github.sh
```

---

## ?? PASOS MANUALES (Si prefieres control total)

### 1. Crear repositorio en GitHub
- Ve a https://github.com/new
- Nombre: `NexShop`
- Descripción: `Plataforma de E-Commerce con ASP.NET Core 8`
- Selecciona Public (recomendado)
- Click en "Create repository"

### 2. Generar Personal Access Token
- Ve a https://github.com/settings/tokens
- Click en "Generate new token (classic)"
- Dale permisos: `repo`
- Copia el token generado

### 3. Ejecutar comandos Git
```powershell
cd "E:\Proyectos Visual\NexShop"

# Inicializar git
git init

# Configurar usuario (primera vez)
git config --global user.name "Tu Nombre"
git config --global user.email "tu.email@gmail.com"

# Agregar archivos
git add .

# Hacer commit
git commit -m "Initial commit: NexShop e-commerce platform"

# Agregar repositorio remoto (reemplaza TU_USUARIO)
git remote add origin https://github.com/TU_USUARIO/NexShop.git

# Cambiar a rama main
git branch -M main

# Subir cambios (cuando pida contraseña, usa tu token)
git push -u origin main
```

---

## ?? Qué Se Subirá

### ? Se sube:
```
NexShop.Web/
??? Controllers/           (todos los controladores)
??? Views/                (todas las vistas)
??? Areas/Identity/       (autenticación)
??? Models/               (modelos de datos)
??? Services/             (servicios)
??? Migrations/           (migraciones EF Core)
??? Scripts/              (scripts SQL)
??? wwwroot/              (CSS, JS, imágenes)
??? Program.cs
??? appsettings.json
??? NexShop.Web.csproj
??? .gitignore

README.md
LICENSE
GUIA_*.md
subir-a-github.*
```

### ? NO se sube:
```
bin/                 (binarios compilados)
obj/                 (archivos temporales)
.vs/                 (Visual Studio)
wwwroot/uploads/     (multimedia local)
*.db                 (base de datos local)
.env                 (variables de entorno)
```

---

## ?? Obtener Personal Access Token

### Paso a paso:
1. **Abre:** https://github.com/settings/tokens
2. **Click en:** "Generate new token" (esquina superior derecha)
3. **Selecciona:** "Generate new token (classic)"
4. **Configura:**
   - Name: `NexShop-Upload`
   - Expiration: 90 days
   - Scopes: Marca `repo` ?
5. **Click en:** "Generate token" (al final)
6. **COPIA el token** (solo aparece una vez)

### Usar el token:
```powershell
# Cuando git pida contraseña, pega el token
git push -u origin main
# Usuario: tu_usuario_github
# Contraseña: <pega_tu_token_aqui>
```

---

## ? Resultado Final

Después de subir, podrás ver tu proyecto en:
```
https://github.com/TU_USUARIO/NexShop
```

Con:
- ? Código fuente completo
- ? README con documentación
- ? Licencia MIT
- ? 16 tablas de base de datos
- ? Autenticación con Identity
- ? CRUD de productos
- ? Sistema de órdenes
- ? Carrito de compras

---

## ?? Después de Subir (Opcional)

### Mejora tu repositorio:
1. **Descripción:** Añade descripción en Settings > About
2. **Temas:** Añade `aspnetcore`, `ecommerce`, `csharp`
3. **Documentación:** Crea un Wiki
4. **Issues:** Abre issues para features
5. **Actions:** Configura CI/CD (opcional)

---

## ?? Si Algo Falla

### Error: "fatal: remote origin already exists"
```powershell
git remote remove origin
git remote add origin https://github.com/TU_USUARIO/NexShop.git
git push -u origin main
```

### Error: "Authentication failed"
- Verifica que usaste el token correcto
- El token no debe estar expirado
- Genera uno nuevo si es necesario

### Error: "Permission denied"
- Verifica que tu usuario de GitHub es correcto
- Verifica que tienes permiso para crear repositorios
- Comprueba tu conexión de Internet

---

## ?? Contacto y Soporte

- **Documentación Git:** https://git-scm.com/doc
- **Documentación GitHub:** https://docs.github.com
- **Tutorial:** https://guides.github.com

---

## ? Checklist Final

- [ ] Creé repositorio en GitHub.com
- [ ] Generé Personal Access Token
- [ ] Ejecuté `git init` en la carpeta del proyecto
- [ ] Ejecuté `git add .`
- [ ] Ejecuté `git commit -m "mensaje"`
- [ ] Ejecuté `git remote add origin ...`
- [ ] Ejecuté `git push -u origin main`
- [ ] Verifiqué que el código está en GitHub
- [ ] Agregué descripción al repositorio
- [ ] Agregué temas/topics

---

**¡Tu proyecto NexShop estará en GitHub en menos de 10 minutos!** ??

Fecha: 2025-11-28  
Status: ? PREPARADO PARA GITHUB  
Siguiente paso: Ejecutar `.\subir-a-github.ps1` o comandos manuales

