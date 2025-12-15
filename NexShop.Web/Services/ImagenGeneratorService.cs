using System.Drawing;
using System.Drawing.Imaging;

namespace NexShop.Web.Services
{
    /// <summary>
    /// Servicio para generar imágenes PNG localmente
    /// </summary>
    public interface IImagenGeneratorService
    {
        Task GenerarImagenesAsync();
    }

    public class ImagenGeneratorService : IImagenGeneratorService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ImagenGeneratorService> _logger;

        public ImagenGeneratorService(IWebHostEnvironment env, ILogger<ImagenGeneratorService> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Genera 100 imágenes PNG en wwwroot/imagenes/productos/
        /// </summary>
        public async Task GenerarImagenesAsync()
        {
            try
            {
                var imagePath = Path.Combine(_env.WebRootPath, "imagenes", "productos");

                // Crear directorio si no existe
                if (!Directory.Exists(imagePath))
                {
                    Directory.CreateDirectory(imagePath);
                    _logger.LogInformation("Carpeta creada: {ImagePath}", imagePath);
                }

                // Colores para gradientes
                var colors = new[]
                {
                    new[] { Color.FromArgb(52, 152, 219), Color.FromArgb(41, 128, 185) },       // Azul
                    new[] { Color.FromArgb(46, 204, 113), Color.FromArgb(39, 174, 96) },        // Verde
                    new[] { Color.FromArgb(231, 76, 60), Color.FromArgb(192, 57, 43) },         // Rojo
                    new[] { Color.FromArgb(241, 196, 15), Color.FromArgb(230, 126, 34) },       // Amarillo
                    new[] { Color.FromArgb(155, 89, 182), Color.FromArgb(142, 68, 173) },       // Púrpura
                    new[] { Color.FromArgb(26, 188, 156), Color.FromArgb(22, 160, 133) }        // Turquesa
                };

                int generadas = 0;
                int existentes = 0;

                // Generar 100 imágenes
                for (int i = 1; i <= 100; i++)
                {
                    string fileName = $"producto_{i}.png";
                    string filePath = Path.Combine(imagePath, fileName);

                    // Si ya existe, saltar
                    if (File.Exists(filePath))
                    {
                        existentes++;
                        continue;
                    }

                    try
                    {
                        // Crear bitmap
                        using (var bitmap = new Bitmap(400, 400))
                        using (var graphics = Graphics.FromImage(bitmap))
                        {
                            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                            // Seleccionar colores
                            var colorPair = colors[(i - 1) % colors.Length];
                            var color1 = colorPair[0];
                            var color2 = colorPair[1];

                            // Crear gradiente
                            var rect = new Rectangle(0, 0, 400, 400);
                            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, color1, color2, 45f))
                            {
                                graphics.FillRectangle(brush, rect);
                            }

                            // Dibujar texto principal
                            using (var font = new Font("Arial", 48, FontStyle.Bold))
                            using (var whiteBrush = new SolidBrush(Color.White))
                            {
                                var text1 = $"Producto {i}";
                                var textSize = graphics.MeasureString(text1, font);
                                var x1 = (400 - textSize.Width) / 2;
                                var y1 = (400 - textSize.Height) / 2 - 40;
                                graphics.DrawString(text1, font, whiteBrush, x1, y1);
                            }

                            // Dibujar dimensión
                            using (var font = new Font("Arial", 14))
                            using (var whiteBrush = new SolidBrush(Color.White))
                            {
                                var text2 = "400x400 px";
                                var textSize = graphics.MeasureString(text2, font);
                                var x2 = (400 - textSize.Width) / 2;
                                var y2 = (400 - textSize.Height) / 2 + 60;
                                graphics.DrawString(text2, font, whiteBrush, x2, y2);
                            }

                            // Guardar
                            bitmap.Save(filePath, ImageFormat.Png);
                        }

                        generadas++;

                        if (i % 20 == 0)
                        {
                            _logger.LogInformation("Generadas {Count} imágenes...", i);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error generando imagen {FileName}", fileName);
                    }
                }

                _logger.LogInformation("Generación de imágenes completada. Generadas: {Generadas}, Existentes: {Existentes}", generadas, existentes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar imágenes");
                throw;
            }
        }
    }
}
