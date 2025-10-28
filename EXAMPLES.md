# 使用示例

本文档提供了PDF转图片工具的详细使用示例。

## 📖 目录

1. [基础转换](#基础转换)
2. [格式和质量控制](#格式和质量控制)
3. [页面选择](#页面选择)
4. [长图模式](#长图模式)
5. [实际应用场景](#实际应用场景)

---

## 基础转换

### 1. 最简单的用法

转换整个PDF为PNG格式（默认300 DPI）：

```bash
dotnet run -- -i document.pdf
```

输出：
- `output/document_page1.png`
- `output/document_page2.png`
- `output/document_page3.png`
- ...

### 2. 指定输出目录

```bash
dotnet run -- -i document.pdf -o my_images
```

输出到 `my_images/` 文件夹

---

## 格式和质量控制

### 3. 转换为JPEG格式

```bash
dotnet run -- -i document.pdf -f JPEG
```

### 4. 设置JPEG质量

```bash
dotnet run -- -i document.pdf -f JPEG -q 95
```

质量范围：1-100，数字越大质量越好，文件越大

### 5. 自定义DPI

**低分辨率（快速预览）：**
```bash
dotnet run -- -i document.pdf -d 72
```

**标准打印质量：**
```bash
dotnet run -- -i document.pdf -d 150
```

**高质量打印：**
```bash
dotnet run -- -i document.pdf -d 300
```

**超高质量（专业印刷）：**
```bash
dotnet run -- -i document.pdf -d 600
```

---

## 页面选择

### 6. 只转换第一页（封面）

```bash
dotnet run -- -i document.pdf -p 1
```

### 7. 转换多个特定页面

```bash
dotnet run -- -i document.pdf -p 1,3,5,7
```

### 8. 转换连续页面范围

虽然不支持 `1-5` 这样的范围语法，但可以列出：

```bash
dotnet run -- -i document.pdf -p 1,2,3,4,5
```

---

## 长图模式

### 9. 基础长图转换

将所有页面合并为一张长图：

```bash
dotnet run -- -i document.pdf -l
```

输出：
- `output/document_long.png`

### 10. 长图 + 自定义间距

页面之间添加20像素间距：

```bash
dotnet run -- -i document.pdf -l -s 20
```

### 11. 长图 + JPEG格式

```bash
dotnet run -- -i document.pdf -l -f JPEG -q 90
```

### 12. 长图 + 降低DPI（节省内存）

对于页数很多的PDF，建议降低DPI：

```bash
dotnet run -- -i document.pdf -l -d 150 -s 10
```

### 13. 仅将特定页面合并为长图

```bash
dotnet run -- -i document.pdf -l -p 1,2,3
```

---

## 实际应用场景

### 场景1：电子书转换

将电子书PDF转换为适合手机阅读的长图：

```bash
dotnet run -- -i ebook.pdf -l -d 150 -s 10 -f JPEG -q 85 -o ebook_images
```

**说明：**
- `-l`: 长图模式，便于连续滚动阅读
- `-d 150`: 较低DPI，减小文件大小
- `-s 10`: 10像素间距，页面间有分隔
- `-f JPEG -q 85`: JPEG格式，适中质量
- `-o ebook_images`: 输出到专门文件夹

### 场景2：漫画PDF转图片

```bash
dotnet run -- -i manga.pdf -l -d 200 -s 0 -f PNG -o manga_chapters
```

**说明：**
- `-s 0`: 无间距，页面紧密连接
- `-f PNG`: 无损格式，保持清晰度
- `-d 200`: 适中分辨率

### 场景3：PPT转图片（用于分享）

```bash
dotnet run -- -i presentation.pdf -d 150 -f JPEG -q 90 -o slides
```

**说明：**
- 不使用长图模式，每页单独保存
- JPEG格式，适合网络分享

### 场景4：文档截图（高质量）

只转换重要页面：

```bash
dotnet run -- -i report.pdf -p 1,5,10,15 -d 300 -f PNG -o screenshots
```

### 场景5：证书扫描件处理

```bash
dotnet run -- -i certificate.pdf -p 1 -d 600 -f PNG -o certificates
```

**说明：**
- 仅第一页
- 超高DPI确保细节清晰
- PNG无损格式

### 场景6：批量转换预览图

```bash
dotnet run -- -i catalog.pdf -d 96 -f JPEG -q 75 -o previews
```

**说明：**
- 低DPI快速生成
- JPEG低质量，文件小
- 适合生成缩略图

### 场景7：打印准备

```bash
dotnet run -- -i document.pdf -d 300 -f PNG -o print_ready
```

**说明：**
- 标准打印DPI
- PNG确保质量

### 场景8：长文章/论文阅读

```bash
dotnet run -- -i paper.pdf -l -s 30 -d 200 -f PNG -o reading
```

**说明：**
- 长图模式便于阅读
- 30像素间距区分页面
- 适中分辨率平衡质量和文件大小

---

## 交互式模式示例

直接运行程序：

```bash
dotnet run
```

然后按提示输入：

```
请输入PDF文件路径:
document.pdf

请输入输出目录（回车使用默认'output'目录）:
[直接回车]

选择输出格式:
1. PNG（默认）
2. JPEG
3. BMP
请选择（1-3）: 2

请输入DPI（回车使用默认300）: 150

是否合并为一个长图？
1. 否（默认，每页单独保存）
2. 是（合并所有页面为一张长图）
请选择（1-2）: 2

页面间距（像素，回车使用默认0）: 20
```

---

## 高级技巧

### 组合参数示例

**最小文件大小：**
```bash
dotnet run -- -i doc.pdf -d 72 -f JPEG -q 60
```

**最高质量：**
```bash
dotnet run -- -i doc.pdf -d 600 -f PNG
```

**平衡模式（推荐）：**
```bash
dotnet run -- -i doc.pdf -d 200 -f JPEG -q 90
```

---

## 性能对比

| 模式 | DPI | 格式 | 10页PDF估计大小 | 处理时间 |
|------|-----|------|----------------|---------|
| 快速预览 | 72 | JPEG(60) | ~500KB | 快 |
| 标准 | 150 | JPEG(85) | ~2MB | 中等 |
| 高质量 | 300 | PNG | ~15MB | 慢 |
| 超高质量 | 600 | PNG | ~60MB | 很慢 |
| 长图模式 | 150 | JPEG(85) | ~1.5MB | 中等 |

*实际大小和时间取决于PDF内容复杂度

---

## 常见问题

**Q: 如何选择DPI？**
- 屏幕查看：72-150 DPI
- 普通打印：150-200 DPI
- 高质量打印：300 DPI
- 专业需求：600+ DPI

**Q: PNG和JPEG如何选择？**
- PNG：文字、图表、需要透明度
- JPEG：照片、复杂图像、需要小文件

**Q: 长图模式会占用很多内存吗？**
- 是的，建议：
  - 页数少于50页使用
  - 或降低DPI到150-200
  - 使用JPEG格式

**Q: 可以一次处理多个PDF吗？**
- 目前不支持，需要分别运行命令
- 可以使用脚本批量处理

---

## 批处理示例（PowerShell）

处理文件夹中所有PDF：

```powershell
Get-ChildItem *.pdf | ForEach-Object {
    dotnet run -- -i $_.Name -o "output\$($_.BaseName)" -d 150 -f JPEG
}
```

处理文件夹中所有PDF为长图：

```powershell
Get-ChildItem *.pdf | ForEach-Object {
    dotnet run -- -i $_.Name -o "longimages" -l -d 150 -s 10
}
```

---

更多问题请查看 [README.md](README.md) 或提交 Issue。

