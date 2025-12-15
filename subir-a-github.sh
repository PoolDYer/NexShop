#!/bin/bash
# ===================================================
# SCRIPT PARA SUBIR NEXSHOP A GITHUB AUTOMÁTICAMENTE
# ===================================================
# Uso: bash subir-a-github.sh (en macOS/Linux)

echo "??????????????????????????????????????????????"
echo "?  ?? SUBIR NEXSHOP A GITHUB - AUTOMÁTICO   ?"
echo "??????????????????????????????????????????????"
echo ""

# Verificar que estamos en la carpeta correcta
if [ ! -f "NexShop.Web/NexShop.Web.csproj" ]; then
    echo "? Error: No estás en la carpeta raíz de NexShop"
    echo "   Ejecuta este script desde la carpeta raíz del proyecto"
    exit 1
fi

# Verificar Git
if ! command -v git &> /dev/null; then
    echo "? Error: Git no está instalado"
    exit 1
fi

echo "? Verificaciones previas completadas"
echo ""

# Solicitar datos
echo "?? CONFIGURACIÓN REQUERIDA:"
read -p "   Tu usuario de GitHub: " usuario
read -s -p "   Tu Personal Access Token: " token
echo ""
echo ""

# Verificar si ya existe repositorio git
if [ -d ".git" ]; then
    echo "??  Ya existe un repositorio .git"
    read -p "   ¿Deseas continuar? (s/n): " respuesta
    if [ "$respuesta" != "s" ]; then
        exit 0
    fi
else
    echo "?? Inicializando repositorio Git..."
    git init
fi

echo ""
echo "?? Agregando archivos al repositorio..."
git add .

echo ""
echo "?? Estado actual:"
git status

echo ""
read -p "   ¿Deseas continuar con el commit? (s/n): " respuesta
if [ "$respuesta" != "s" ]; then
    echo "??  Operación cancelada"
    exit 0
fi

echo ""
echo "?? Escribiendo mensaje de commit..."
read -p "   Mensaje de commit: " mensaje

git commit -m "$mensaje"

echo ""
echo "?? Configurando repositorio remoto..."

remoteUrl=$(git remote get-url origin 2>/dev/null)
if [ ! -z "$remoteUrl" ]; then
    echo "   Repositorio remoto encontrado: $remoteUrl"
    read -p "   ¿Cambiar repositorio remoto? (s/n): " respuesta
    if [ "$respuesta" = "s" ]; then
        git remote remove origin
    fi
fi

if [ -z "$(git remote get-url origin 2>/dev/null)" ]; then
    repoUrl="https://${usuario}:${token}@github.com/${usuario}/NexShop.git"
    echo "   Agregando: https://github.com/${usuario}/NexShop.git"
    git remote add origin "$repoUrl"
fi

echo ""
echo "Cambiando rama principal a 'main'..."
git branch -M main

echo ""
echo "?? Subiendo cambios a GitHub..."
git push -u origin main

if [ $? -ne 0 ]; then
    echo "? Error al subir cambios"
    echo "   Intenta nuevamente o verifica tu token de acceso"
    exit 1
fi

echo ""
echo "??????????????????????????????????????????????"
echo "?  ? ¡ÉXITO! NEXSHOP ESTÁ EN GITHUB        ?"
echo "??????????????????????????????????????????????"
echo ""
echo "?? Tu repositorio:"
echo "   https://github.com/$usuario/NexShop"
echo ""
echo "?? Próximos pasos:"
echo "   1. Visita tu repositorio en GitHub"
echo "   2. Añade una descripción al repositorio"
echo "   3. Añade temas: aspnetcore, ecommerce, c-sharp"
echo "   4. Activa GitHub Pages para documentación (opcional)"
echo ""
echo "? ¡Listo para colaborar!"
