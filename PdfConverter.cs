using System;
using System.IO;
using System.Collections.Generic;
using PDFiumSharp;

namespace PDFImageConverter
{
    /// <summary>
    /// PDF转图片转换器
    /// </summary>
    public class PdfConverter
    {
        /// <summary>
        /// 图片格式
        /// </summary>
        public enum ImageFormat
        {
            PNG,
            JPEG,
            BMP
        }

        /// <summary>
        /// 转换配置
        /// </summary>
        public class ConvertOptions
        {
            /// <summary>
            /// 输出图片格式，默认PNG
            /// </summary>
            public ImageFormat Format { get; set; } = ImageFormat.PNG;

            /// <summary>
            /// DPI（分辨率），默认300
            /// </summary>
            public int Dpi { get; set; } = 300;

            /// <summary>
            /// JPEG质量（1-100），仅对JPEG格式有效
            /// </summary>
            public int JpegQuality { get; set; } = 90;

            /// <summary>
            /// 是否转换所有页面，默认true
            /// </summary>
            public bool ConvertAllPages { get; set; } = true;

            /// <summary>
            /// 指定要转换的页面（从1开始），仅当ConvertAllPages为false时有效
            /// </summary>
            public List<int> SpecificPages { get; set; } = new List<int>();

            /// <summary>
            /// 是否合并所有页面为一个长图，默认false
            /// </summary>
            public bool MergeToLongImage { get; set; } = false;

            /// <summary>
            /// 长图页面间距（像素），默认0
            /// </summary>
            public int PageSpacing { get; set; } = 0;
        }

        /// <summary>
        /// 将PDF转换为图片
        /// </summary>
        /// <param name="pdfPath">PDF文件路径</param>
        /// <param name="outputDirectory">输出目录</param>
        /// <param name="options">转换选项</param>
        /// <returns>生成的图片文件路径列表</returns>
        public List<string> Convert(string pdfPath, string outputDirectory, ConvertOptions? options = null)
        {
            options ??= new ConvertOptions();

            if (!File.Exists(pdfPath))
            {
                throw new FileNotFoundException($"PDF文件不存在: {pdfPath}");
            }

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var outputFiles = new List<string>();
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(pdfPath);
            var extension = GetExtension(options.Format);

            Console.WriteLine($"开始转换PDF: {pdfPath}");
            Console.WriteLine($"输出目录: {outputDirectory}");
            Console.WriteLine($"格式: {options.Format}, DPI: {options.Dpi}");
            if (options.MergeToLongImage)
            {
                Console.WriteLine("模式: 合并为长图");
            }

            try
            {
                using (var document = new PdfDocument(pdfPath))
                {
                    int pageCount = document.Pages.Count;
                    Console.WriteLine($"总页数: {pageCount}");

                    var pagesToConvert = GetPagesToConvert(pageCount, options);

                    if (options.MergeToLongImage)
                    {
                        // 生成长图模式
                        var longImagePath = Path.Combine(outputDirectory, $"{fileNameWithoutExt}_long{extension}");
                        ConvertToLongImage(document, pagesToConvert, longImagePath, options);
                        outputFiles.Add(longImagePath);
                    }
                    else
                    {
                        // 单页模式
                        foreach (var pageIndex in pagesToConvert)
                        {
                            var page = document.Pages[pageIndex];
                            var outputPath = Path.Combine(outputDirectory, $"{fileNameWithoutExt}_page{pageIndex + 1}{extension}");

                            ConvertPage(page, outputPath, options);
                            outputFiles.Add(outputPath);

                            Console.WriteLine($"✓ 已转换第 {pageIndex + 1}/{pageCount} 页");
                        }
                    }
                }

                Console.WriteLine($"\n转换完成！共生成 {outputFiles.Count} 个图片文件。");
            }
            catch (Exception ex)
            {
                throw new Exception($"转换PDF时发生错误: {ex.Message}", ex);
            }

            return outputFiles;
        }

        /// <summary>
        /// 将多个页面合并为一个长图
        /// </summary>
        private void ConvertToLongImage(PdfDocument document, List<int> pageIndices, string outputPath, ConvertOptions options)
        {
            double scale = options.Dpi / 72.0;
            var pageBitmaps = new List<System.Drawing.Bitmap>();
            int maxWidth = 0;
            int totalHeight = 0;
            int spacing = (int)(options.PageSpacing * scale);

            try
            {
                // 第一步：渲染所有页面到内存
                Console.WriteLine("正在渲染各页面...");
                foreach (var pageIndex in pageIndices)
                {
                    var page = document.Pages[pageIndex];
                    int width = (int)(page.Width * scale);
                    int height = (int)(page.Height * scale);

                    using (var pdfBitmap = new PDFiumBitmap(width, height, true))
                    {
                        // 填充白色背景
                        FillBitmapWhite(pdfBitmap);

                        // 渲染页面（使用默认标志以确保兼容性）
                        page.Render(pdfBitmap);

                        // 转换为GDI+ Bitmap
                        var gdiBitmap = new System.Drawing.Bitmap(width, height, pdfBitmap.Stride,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb, pdfBitmap.Scan0);
                        
                        // 创建副本以避免内存问题
                        var bitmapCopy = new System.Drawing.Bitmap(gdiBitmap);
                        pageBitmaps.Add(bitmapCopy);
                        gdiBitmap.Dispose();
                    }

                    maxWidth = Math.Max(maxWidth, width);
                    totalHeight += height;
                    
                    Console.WriteLine($"✓ 已渲染第 {pageIndex + 1}/{document.Pages.Count} 页");
                }

                // 添加页面间距
                if (pageIndices.Count > 1)
                {
                    totalHeight += spacing * (pageIndices.Count - 1);
                }

                // 第二步：创建长图并合并所有页面
                Console.WriteLine($"\n正在合并为长图 ({maxWidth}x{totalHeight})...");
                using (var longBitmap = new System.Drawing.Bitmap(maxWidth, totalHeight))
                {
                    using (var graphics = System.Drawing.Graphics.FromImage(longBitmap))
                    {
                        // 设置高质量渲染
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                        // 填充白色背景
                        graphics.Clear(System.Drawing.Color.White);

                        // 绘制每一页
                        int currentY = 0;
                        for (int i = 0; i < pageBitmaps.Count; i++)
                        {
                            var bitmap = pageBitmaps[i];
                            // 居中对齐
                            int x = (maxWidth - bitmap.Width) / 2;
                            graphics.DrawImage(bitmap, x, currentY);
                            currentY += bitmap.Height + spacing;
                        }
                    }

                    // 保存长图
                    SaveGdiBitmap(longBitmap, outputPath, options);
                }

                Console.WriteLine($"✓ 长图已保存");
            }
            finally
            {
                // 清理内存
                foreach (var bitmap in pageBitmaps)
                {
                    bitmap.Dispose();
                }
            }
        }

        /// <summary>
        /// 转换单个页面
        /// </summary>
        private void ConvertPage(PdfPage page, string outputPath, ConvertOptions options)
        {
            // 计算缩放比例（DPI转换）
            double scale = options.Dpi / 72.0;

            // 获取页面尺寸
            int width = (int)(page.Width * scale);
            int height = (int)(page.Height * scale);

            // 渲染页面到位图
            using (var bitmap = new PDFiumBitmap(width, height, true))
            {
                // 填充白色背景
                FillBitmapWhite(bitmap);
                
                // 渲染页面（使用默认标志以确保兼容性）
                page.Render(bitmap);

                // 保存图片
                SaveBitmap(bitmap, outputPath, options);
            }
        }

        /// <summary>
        /// 填充位图为白色背景
        /// </summary>
        private void FillBitmapWhite(PDFiumBitmap bitmap)
        {
            // 使用不安全代码快速填充白色
            unsafe
            {
                byte* scan0 = (byte*)bitmap.Scan0.ToPointer();
                int stride = bitmap.Stride;
                int height = bitmap.Height;
                int width = bitmap.Width;

                for (int y = 0; y < height; y++)
                {
                    byte* row = scan0 + (y * stride);
                    for (int x = 0; x < width; x++)
                    {
                        // BGRA格式，设置为白色 (255, 255, 255, 255)
                        row[x * 4] = 255;     // B
                        row[x * 4 + 1] = 255; // G
                        row[x * 4 + 2] = 255; // R
                        row[x * 4 + 3] = 255; // A
                    }
                }
            }
        }

        /// <summary>
        /// 保存位图到文件
        /// </summary>
        private void SaveBitmap(PDFiumBitmap bitmap, string outputPath, ConvertOptions options)
        {
            // 获取位图数据
            IntPtr buffer = bitmap.Scan0;
            int width = bitmap.Width;
            int height = bitmap.Height;
            int stride = bitmap.Stride;

            // 创建System.Drawing.Bitmap
            using (var gdibitmap = new System.Drawing.Bitmap(width, height, stride,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb, buffer))
            {
                SaveGdiBitmap(gdibitmap, outputPath, options);
            }
        }

        /// <summary>
        /// 保存GDI+ Bitmap到文件
        /// </summary>
        private void SaveGdiBitmap(System.Drawing.Bitmap bitmap, string outputPath, ConvertOptions options)
        {
            // 保存为指定格式
            switch (options.Format)
            {
                case ImageFormat.PNG:
                    bitmap.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
                    break;
                case ImageFormat.JPEG:
                    SaveAsJpeg(bitmap, outputPath, options.JpegQuality);
                    break;
                case ImageFormat.BMP:
                    bitmap.Save(outputPath, System.Drawing.Imaging.ImageFormat.Bmp);
                    break;
            }
        }

        /// <summary>
        /// 以指定质量保存JPEG
        /// </summary>
        private void SaveAsJpeg(System.Drawing.Bitmap bitmap, string path, int quality)
        {
            var encoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg);
            var encoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
            encoderParameters.Param[0] = new System.Drawing.Imaging.EncoderParameter(
                System.Drawing.Imaging.Encoder.Quality, (long)quality);

            bitmap.Save(path, encoder, encoderParameters);
        }

        /// <summary>
        /// 获取图片编码器
        /// </summary>
        private System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
        {
            var codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            throw new Exception("找不到编码器");
        }

        /// <summary>
        /// 获取要转换的页面索引列表
        /// </summary>
        private List<int> GetPagesToConvert(int totalPages, ConvertOptions options)
        {
            var pages = new List<int>();

            if (options.ConvertAllPages)
            {
                for (int i = 0; i < totalPages; i++)
                {
                    pages.Add(i);
                }
            }
            else
            {
                foreach (var pageNum in options.SpecificPages)
                {
                    if (pageNum >= 1 && pageNum <= totalPages)
                    {
                        pages.Add(pageNum - 1); // 转换为0索引
                    }
                }
            }

            return pages;
        }

        /// <summary>
        /// 获取文件扩展名
        /// </summary>
        private string GetExtension(ImageFormat format)
        {
            return format switch
            {
                ImageFormat.PNG => ".png",
                ImageFormat.JPEG => ".jpg",
                ImageFormat.BMP => ".bmp",
                _ => ".png"
            };
        }
    }
}

