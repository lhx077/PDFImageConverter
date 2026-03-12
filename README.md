# PDF转图片工具 (PDFImageConverter)

[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/lhx077/PDFImageConverter)

一个简单易用的PDF转图片工具，使用C#编写，支持将PDF文档转换为PNG、JPEG或BMP格式的图片。

## ✨ 特性

- **多种格式支持**：支持转换为PNG、JPEG、BMP格式
- **自定义DPI**：可设置输出图片的分辨率
- **灵活的页面选择**：可选择转换全部页面或指定页面
- **长图模式**：支持将PDF所有页面合并为一张长图
- **双模式操作**：支持交互式界面和命令行参数
- **高质量输出**：使用PDFium引擎，确保转换质量
- **简单易用**：无需复杂配置，开箱即用

## 🚀 快速开始

### 前置要求

- .NET 8.0 SDK 或更高版本

### 编译项目

```bash
# 克隆或下载项目后，在项目目录下运行：
dotnet restore
dotnet build

# 发布为独立可执行文件（可选）
dotnet publish -c Release -r win-x64 --self-contained
```

### 运行程序

#### 方式一：交互模式

直接运行程序，按提示输入参数：

```bash
dotnet run
```

或运行编译后的可执行文件：

```bash
.\PDFImageConverter.exe
```

#### 方式二：命令行模式

使用命令行参数直接转换：

```bash
# 基本使用
dotnet run -- -i document.pdf

# 指定输出目录和格式
dotnet run -- -i document.pdf -o images -f JPEG

# 指定DPI和质量
dotnet run -- -i document.pdf -d 150 -q 85

# 仅转换指定页面
dotnet run -- -i document.pdf -p 1,3,5
```

## 命令行参数

| 参数 | 简写 | 说明 | 默认值 |
|------|------|------|--------|
| `--input` | `-i` | PDF文件路径（必需） | - |
| `--output` | `-o` | 输出目录 | `output` |
| `--format` | `-f` | 输出格式：PNG/JPEG/BMP | `PNG` |
| `--dpi` | `-d` | 图片分辨率（DPI） | `300` |
| `--quality` | `-q` | JPEG质量（1-100） | `90` |
| `--pages` | `-p` | 指定页面（如：1,3,5） | 全部页面 |
| `--long` | `-l` | 合并为一个长图 | `false` |
| `--spacing` | `-s` | 长图页面间距（像素） | `0` |
| `--help` | `-h` | 显示帮助信息 | - |

## 💡 使用示例

### 示例 1：转换整个PDF为PNG（默认设置）

```bash
dotnet run -- -i document.pdf
```

输出：
- `output/document_page1.png`
- `output/document_page2.png`
- ...

### 示例 2：转换为高质量JPEG

```bash
dotnet run -- -i document.pdf -f JPEG -d 300 -q 95 -o my_images
```

### 示例 3：只转换第1、3、5页

```bash
dotnet run -- -i document.pdf -p 1,3,5
```

### 示例 4：低DPI快速预览

```bash
dotnet run -- -i document.pdf -d 72 -o preview
```

### 示例 5：合并为长图（适合连续阅读）

```bash
dotnet run -- -i document.pdf -l
```

### 示例 6：长图模式with页面间距

```bash
dotnet run -- -i document.pdf -l -s 20 -f JPEG
```

## 🏗️ 项目结构

```
PDFImageConverter/
├── PDFImageConverter.csproj  # 项目文件
├── Program.cs                # 主程序入口
├── PdfConverter.cs           # PDF转换核心类
├── README.md                 # 说明文档
└── .gitignore                # Git忽略文件
```

## 🔧 核心依赖

- **PDFiumSharp**: PDF渲染引擎（基于Google的PDFium）
- **System.Drawing.Common**: 图片处理库

## 📝 API使用

如果你想在自己的C#项目中使用转换功能：

```csharp
using PDFImageConverter;

// 创建转换器
var converter = new PdfConverter();

// 配置选项
var options = new PdfConverter.ConvertOptions
{
    Format = PdfConverter.ImageFormat.PNG,
    Dpi = 300,
    JpegQuality = 90,
    ConvertAllPages = true
};

// 执行转换
var outputFiles = converter.Convert("document.pdf", "output", options);

Console.WriteLine($"生成了 {outputFiles.Count} 个图片");
```

### 仅转换特定页面

```csharp
var options = new PdfConverter.ConvertOptions
{
    ConvertAllPages = false,
    SpecificPages = new List<int> { 1, 3, 5 } // 页码从1开始
};

var outputFiles = converter.Convert("document.pdf", "output", options);
```

### 合并为长图

```csharp
var options = new PdfConverter.ConvertOptions
{
    Format = PdfConverter.ImageFormat.PNG,
    Dpi = 200,
    MergeToLongImage = true,  // 启用长图模式
    PageSpacing = 20          // 页面间距20像素
};

var outputFiles = converter.Convert("document.pdf", "output", options);
// 将生成一个 document_long.png 文件
```

## ⚙️ 性能建议

1. **DPI设置**：
   - 屏幕预览：72-96 DPI
   - 普通打印：150-200 DPI
   - 高质量打印：300 DPI
   - 专业印刷：600 DPI

2. **格式选择**：
   - **PNG**：无损压缩，适合文字和图表
   - **JPEG**：有损压缩，适合照片和复杂图像
   - **BMP**：无压缩，文件最大但速度快

3. **内存占用**：
   - 单页模式：逐页处理，内存占用较小
   - 长图模式：需要将所有页面载入内存，大文件建议使用较低DPI

## 🐛 故障排除

### 问题：找不到PDF文件

确保文件路径正确，使用引号包含包含空格的路径：

```bash
dotnet run -- -i "My Documents/file.pdf"
```

### 问题：运行时错误

确保已安装.NET 8.0 SDK：

```bash
dotnet --version
```

### 问题：转换失败

检查PDF文件是否损坏或加密保护。

### 问题：部分文字显示为空白

这通常是PDF字体嵌入问题，不是转换工具的问题。解决方法：

1. **提高DPI**：`dotnet run -- -i doc.pdf -d 600`
2. **重新生成PDF**：使用Adobe Acrobat重新保存，选择"嵌入所有字体"
3. **查看详细说明**：阅读 [FONT_ISSUES.md](FONT_ISSUES.md) 获取完整解决方案

## 📄 许可证

MIT License

## 🤝 贡献

欢迎提交Issue和Pull Request！

## 📧 联系

如有问题或建议，请提交Issue。

---

**开发环境**: .NET 8.0  
**最后更新**: 2025年10月

