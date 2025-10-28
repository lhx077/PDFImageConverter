# 快速开始指南

## 1️⃣ 安装依赖

确保你已安装 .NET 8.0 SDK：

```bash
dotnet --version
```

如果没有安装，请访问：https://dotnet.microsoft.com/download

## 2️⃣ 编译项目

在项目目录下运行：

```bash
# 恢复依赖包
dotnet restore

# 编译项目
dotnet build
```

## 3️⃣ 运行程序

### 方法一：交互式使用（推荐新手）

```bash
dotnet run
```

然后按照提示输入：
1. PDF文件路径
2. 输出目录（可留空使用默认）
3. 输出格式（1:PNG, 2:JPEG, 3:BMP）
4. DPI分辨率（可留空使用默认300）

### 方法二：命令行使用（推荐熟练用户）

```bash
# 最简单的用法
dotnet run -- -i your_document.pdf

# 完整参数示例
dotnet run -- -i your_document.pdf -o images -f JPEG -d 150
```

## 4️⃣ 查看结果

转换完成后，图片会保存在指定的输出目录（默认为 `output` 文件夹）。

## 🎯 常用场景

### 场景1：转换PDF为PNG（默认高质量）

```bash
dotnet run -- -i document.pdf
```

### 场景2：快速预览（低分辨率）

```bash
dotnet run -- -i document.pdf -d 72 -o preview
```

### 场景3：打印质量JPEG

```bash
dotnet run -- -i document.pdf -f JPEG -d 300 -q 95
```

### 场景4：只转换封面

```bash
dotnet run -- -i document.pdf -p 1
```

### 场景5：合并为长图（适合连续阅读）

```bash
dotnet run -- -i document.pdf -l
```

### 场景6：长图模式with页面间距

```bash
dotnet run -- -i document.pdf -l -s 20 -d 200
```

## 📦 发布独立程序

如果想创建一个不依赖.NET运行时的可执行文件：

```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# 生成的文件在：bin\Release\net8.0\win-x64\publish\
```

之后可以直接运行 `PDFImageConverter.exe`，无需安装.NET。

## ❓ 遇到问题？

查看完整文档：`README.md`

---

祝使用愉快！ 🎉

