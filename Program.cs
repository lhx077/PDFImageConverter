using System;
using System.IO;
using PDFImageConverter;

namespace PDFImageConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("===========================================");
            Console.WriteLine("     PDF转图片工具 v1.0");
            Console.WriteLine("===========================================\n");

            try
            {
                if (args.Length == 0)
                {
                    ShowUsage();
                    InteractiveMode();
                }
                else
                {
                    CommandLineMode(args);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n错误: {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// 交互式模式
        /// </summary>
        static void InteractiveMode()
        {
            Console.WriteLine("请输入PDF文件路径:");
            var pdfPath = Console.ReadLine()?.Trim().Trim('"');

            if (string.IsNullOrEmpty(pdfPath) || !File.Exists(pdfPath))
            {
                Console.WriteLine("文件不存在，程序退出。");
                return;
            }

            Console.WriteLine("\n请输入输出目录（回车使用默认'output'目录）:");
            var outputDir = Console.ReadLine()?.Trim().Trim('"');
            if (string.IsNullOrEmpty(outputDir))
            {
                outputDir = "output";
            }

            Console.WriteLine("\n选择输出格式:");
            Console.WriteLine("1. PNG（默认）");
            Console.WriteLine("2. JPEG");
            Console.WriteLine("3. BMP");
            Console.Write("请选择（1-3）: ");

            var formatChoice = Console.ReadLine()?.Trim();
            var format = PdfConverter.ImageFormat.PNG;

            switch (formatChoice)
            {
                case "2":
                    format = PdfConverter.ImageFormat.JPEG;
                    break;
                case "3":
                    format = PdfConverter.ImageFormat.BMP;
                    break;
                default:
                    format = PdfConverter.ImageFormat.PNG;
                    break;
            }

            Console.Write("\n请输入DPI（回车使用默认300）: ");
            var dpiInput = Console.ReadLine()?.Trim();
            int dpi = 300;
            if (!string.IsNullOrEmpty(dpiInput) && int.TryParse(dpiInput, out int parsedDpi))
            {
                dpi = parsedDpi;
            }

            Console.WriteLine("\n是否合并为一个长图？");
            Console.WriteLine("1. 否（默认，每页单独保存）");
            Console.WriteLine("2. 是（合并所有页面为一张长图）");
            Console.Write("请选择（1-2）: ");
            var mergeChoice = Console.ReadLine()?.Trim();
            bool mergeToLong = mergeChoice == "2";

            int pageSpacing = 0;
            if (mergeToLong)
            {
                Console.Write("页面间距（像素，回车使用默认0）: ");
                var spacingInput = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(spacingInput) && int.TryParse(spacingInput, out int spacing))
                {
                    pageSpacing = spacing;
                }
            }

            var options = new PdfConverter.ConvertOptions
            {
                Format = format,
                Dpi = dpi,
                MergeToLongImage = mergeToLong,
                PageSpacing = pageSpacing
            };

            Console.WriteLine("\n开始转换...\n");

            var converter = new PdfConverter();
            var outputFiles = converter.Convert(pdfPath, outputDir, options);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n成功！生成了 {outputFiles.Count} 个图片文件。");
            Console.ResetColor();
            Console.WriteLine($"保存位置: {Path.GetFullPath(outputDir)}");

            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }

        /// <summary>
        /// 命令行模式
        /// </summary>
        static void CommandLineMode(string[] args)
        {
            string? pdfPath = null;
            string outputDir = "output";
            var format = PdfConverter.ImageFormat.PNG;
            int dpi = 300;
            int jpegQuality = 90;
            bool convertAll = true;
            var specificPages = new List<int>();
            bool mergeToLong = false;
            int pageSpacing = 0;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-i":
                    case "--input":
                        if (i + 1 < args.Length)
                            pdfPath = args[++i];
                        break;
                    case "-o":
                    case "--output":
                        if (i + 1 < args.Length)
                            outputDir = args[++i];
                        break;
                    case "-f":
                    case "--format":
                        if (i + 1 < args.Length)
                        {
                            var formatStr = args[++i].ToUpper();
                            format = formatStr switch
                            {
                                "PNG" => PdfConverter.ImageFormat.PNG,
                                "JPEG" or "JPG" => PdfConverter.ImageFormat.JPEG,
                                "BMP" => PdfConverter.ImageFormat.BMP,
                                _ => PdfConverter.ImageFormat.PNG
                            };
                        }
                        break;
                    case "-d":
                    case "--dpi":
                        if (i + 1 < args.Length && int.TryParse(args[++i], out int parsedDpi))
                            dpi = parsedDpi;
                        break;
                    case "-q":
                    case "--quality":
                        if (i + 1 < args.Length && int.TryParse(args[++i], out int quality))
                            jpegQuality = quality;
                        break;
                    case "-p":
                    case "--pages":
                        if (i + 1 < args.Length)
                        {
                            convertAll = false;
                            var pageStr = args[++i];
                            var pageParts = pageStr.Split(',');
                            foreach (var part in pageParts)
                            {
                                if (int.TryParse(part.Trim(), out int page))
                                {
                                    specificPages.Add(page);
                                }
                            }
                        }
                        break;
                    case "-l":
                    case "--long":
                        mergeToLong = true;
                        break;
                    case "-s":
                    case "--spacing":
                        if (i + 1 < args.Length && int.TryParse(args[++i], out int spacing))
                            pageSpacing = spacing;
                        break;
                    case "-h":
                    case "--help":
                        ShowUsage();
                        return;
                    default:
                        if (pdfPath == null && !args[i].StartsWith("-"))
                        {
                            pdfPath = args[i];
                        }
                        break;
                }
            }

            if (string.IsNullOrEmpty(pdfPath))
            {
                Console.WriteLine("错误: 未指定PDF文件路径");
                ShowUsage();
                return;
            }

            var options = new PdfConverter.ConvertOptions
            {
                Format = format,
                Dpi = dpi,
                JpegQuality = jpegQuality,
                ConvertAllPages = convertAll,
                SpecificPages = specificPages,
                MergeToLongImage = mergeToLong,
                PageSpacing = pageSpacing
            };

            var converter = new PdfConverter();
            var outputFiles = converter.Convert(pdfPath, outputDir, options);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n成功！生成了 {outputFiles.Count} 个图片文件。");
            Console.ResetColor();
        }

        /// <summary>
        /// 显示使用说明
        /// </summary>
        static void ShowUsage()
        {
            Console.WriteLine("使用方法:");
            Console.WriteLine("  1. 直接运行程序进入交互模式");
            Console.WriteLine("  2. 使用命令行参数:\n");
            Console.WriteLine("命令行参数:");
            Console.WriteLine("  -i, --input <path>      PDF文件路径（必需）");
            Console.WriteLine("  -o, --output <path>     输出目录（默认: output）");
            Console.WriteLine("  -f, --format <format>   输出格式: PNG, JPEG, BMP（默认: PNG）");
            Console.WriteLine("  -d, --dpi <number>      分辨率DPI（默认: 300）");
            Console.WriteLine("  -q, --quality <number>  JPEG质量 1-100（默认: 90）");
            Console.WriteLine("  -p, --pages <pages>     指定页面，如: 1,3,5（默认: 全部）");
            Console.WriteLine("  -l, --long              合并为一个长图");
            Console.WriteLine("  -s, --spacing <number>  长图页面间距（像素，默认: 0）");
            Console.WriteLine("  -h, --help              显示此帮助信息\n");
            Console.WriteLine("示例:");
            Console.WriteLine("  PDFImageConverter -i document.pdf");
            Console.WriteLine("  PDFImageConverter -i document.pdf -o images -f JPEG -d 150");
            Console.WriteLine("  PDFImageConverter -i document.pdf -p 1,3,5");
            Console.WriteLine("  PDFImageConverter -i document.pdf -l -s 10");
            Console.WriteLine("  PDFImageConverter -i document.pdf -l -f JPEG -d 200\n");
        }
    }
}

