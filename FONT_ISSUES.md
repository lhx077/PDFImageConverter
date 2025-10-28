# PDF 字体显示问题说明

## 问题现象

转换某些PDF文件时，部分文字（特别是中文字符）可能显示为空白或方框。

## 问题原因

### 1. PDF字体嵌入问题
PDF文件可能使用以下方式处理字体：

- **完整嵌入字体**：字体文件完全包含在PDF中 ✅ 通常正常显示
- **字体子集嵌入**：只嵌入使用到的字符 ⚠️ 可能出现问题
- **引用系统字体**：依赖本地字体库 ⚠️ 可能出现问题
- **替换字体**：使用PDF内部字体映射 ⚠️ 可能出现问题

### 2. 字体编码问题
- PDF使用特殊字符编码（如CID字体）
- 字体映射表不完整
- Unicode映射错误

### 3. PDF创建工具问题
- 某些工具创建的PDF可能有字体嵌入缺陷
- 扫描版PDF（图片型）不存在此问题

## 当前解决方案

### v1.1.1 改进（已实现）

1. **启用Alpha通道**
   ```csharp
   new PDFiumBitmap(width, height, true) // true启用alpha
   ```

2. **白色背景填充**
   - 使用unsafe代码快速填充白色背景
   - 确保透明区域正确显示

3. **提高DPI**
   - 建议使用300 DPI或更高
   - 可以改善字体渲染效果

## 用户可以尝试的方法

### 方法1：提高DPI分辨率

```bash
# 使用更高的DPI
dotnet run -- -i document.pdf -d 600
```

**原理**：更高分辨率可以更清晰地渲染字体细节。

### 方法2：使用不同的输出格式

```bash
# 尝试PNG格式（无损）
dotnet run -- -i document.pdf -f PNG

# 或JPEG格式
dotnet run -- -i document.pdf -f JPEG -q 100
```

### 方法3：重新生成PDF

如果你有原始文件，尝试：

1. 使用Adobe Acrobat重新保存PDF
2. 选择"字体：嵌入所有字体"
3. 转换为PDF/A格式（强制嵌入所有字体）

### 方法4：PDF预处理

使用以下工具预处理PDF：

**Adobe Acrobat：**
1. 打开PDF
2. 文件 → 另存为其他 → 优化的PDF
3. 勾选"嵌入字体"选项

**Ghostscript（免费）：**
```bash
gswin64c -dNOPAUSE -dBATCH -sDEVICE=pdfwrite -dEmbedAllFonts=true -sOutputFile=output.pdf -f input.pdf
```

## 测试示例

### 良好显示的PDF特征
- ✅ 所有字体完整嵌入
- ✅ 使用标准字体（宋体、黑体等）
- ✅ 由专业工具生成（Adobe、LaTeX等）

### 可能有问题的PDF特征
- ⚠️ 网页导出的PDF
- ⚠️ 某些在线转换工具生成的PDF
- ⚠️ 使用罕见字体的PDF
- ⚠️ 经过多次转换的PDF

## 检查PDF字体信息

### 使用Adobe Reader
1. 打开PDF
2. 文件 → 属性 → 字体选项卡
3. 查看字体是否显示"嵌入"或"嵌入子集"

### 使用pdffonts命令（Linux/Mac）
```bash
pdffonts document.pdf
```

## 技术限制

### PDFium渲染引擎
- PDFium（Google的PDF引擎）对大多数标准PDF支持良好
- 某些特殊情况可能无法完美处理：
  - 非标准字体编码
  - 损坏的字体表
  - 加密保护的字体

### 当前实现
```csharp
// 已经使用最佳实践
using (var bitmap = new PDFiumBitmap(width, height, true))
{
    FillBitmapWhite(bitmap);  // 白色背景
    page.Render(bitmap);       // 默认渲染标志
}
```

## 其他解决方案（高级）

### 方案A：使用不同的PDF库

如果字体问题严重，可以考虑尝试其他库：

**1. PdfiumViewer（基于PDFium）**
```bash
dotnet add package PdfiumViewer
```

**2. Docnet.Core（基于Docnet）**
```bash
dotnet add package Docnet.Core
```

**3. IronPDF（商业）**
```bash
dotnet add package IronPdf
```

### 方案B：使用系统API

Windows可以使用：
- Windows.Data.Pdf（UWP API）
- 需要Windows 10+

### 方案C：先转文本再重新排版

对于纯文本PDF：
1. 提取文本内容
2. 使用系统字体重新渲染
3. 生成新图片

## 报告问题

如果遇到字体显示问题，请提供以下信息：

1. **PDF信息**
   - 文件大小
   - 创建工具（在属性中查看）
   - 字体嵌入情况

2. **问题截图**
   - 原始PDF截图
   - 转换后图片截图

3. **测试命令**
   ```bash
   dotnet run -- -i problem.pdf -d 300 -f PNG
   ```

4. **PDF样本**
   - 如果可能，提供问题PDF样本

## 常见问题解答

**Q: 为什么Adobe能正常显示，但转换后有问题？**

A: Adobe Reader有强大的字体替换系统，可以：
- 自动使用系统字体
- 智能字体匹配
- 字形替换

而PDFium严格按照PDF规范渲染，不会自动替换字体。

**Q: 所有PDF都会有这个问题吗？**

A: 不会。大多数标准PDF都能正常转换。通常问题出现在：
- 网页生成的PDF
- 使用特殊字体的文档
- 字体嵌入不完整的PDF

**Q: 有办法100%解决吗？**

A: 最可靠的方法是：
1. 重新生成PDF并完整嵌入字体
2. 或使用原始文件重新创建

**Q: 扫描版PDF会有字体问题吗？**

A: 不会！扫描版PDF是图片，没有字体概念，转换效果通常很好。

## 建议

### 最佳实践

1. **PDF创建时**：
   - 选择"嵌入所有字体"
   - 使用标准字体
   - 保存为PDF/A格式

2. **转换时**：
   - 使用高DPI（300-600）
   - 选择无损PNG格式
   - 保持原始PDF质量

3. **批量处理**：
   - 先测试单个文件
   - 确认效果后再批量处理

### 快速测试

```bash
# 测试命令 - 高质量输出
dotnet run -- -i test.pdf -d 600 -f PNG -o test_output

# 检查输出图片
# 如果仍有问题，考虑预处理PDF
```

---

## 总结

字体显示问题主要源于PDF本身的字体嵌入方式，不是转换工具的问题。最好的解决方案是从源头确保PDF字体完整嵌入。

如果无法修改PDF，可以尝试提高DPI或使用其他PDF处理工具预处理。

**大多数标准PDF都能完美转换！** 🎉

---

更新时间：2025-10-27  
版本：v1.1.1

