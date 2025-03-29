# ImageSharpProcessorLib feat. SixLabos.ImageSharp

使用SixLabors.ImageSharp处理图像的.NET类库。专门用于修改ppi和转换格式。

这个类库是为了替代在我的PowerShell脚本中使用的IamegMagick而编写的。该脚本专用于大量图片的批处理，由于图片大小经过手动确定调整，故该脚本主要关注于分辨率与格式。

同时，由于脚本经过长期使用的积累，考虑到了部分特殊情况，其设置更贴合作者实际使用中的需求。

## 主要功能

- 修改ppi：默认值为144。虽然屏幕常用ppi为72和96，但这两个数值在部分程序修改图片后，元数据记录的大小与实际值不同，故统一更改为144，也可手动更改）
- 转换为png格式：默认输出为原始格式，但是可选择全部转换为png格式。这是由于使用Acrobat生成pdf时，jpg直接生成有概率出错，因此先转换为png，由Acrobat再转换为jpg。

## 其他说明

- 该类库由Gemini-2.5-Pro-Experimental-0325（temperature=0.3）编写初版，GitHub Copilot feat. Claude 3.5 Sonnet检查与修正，感谢LLM。（ps：Gemini 2.5可在Google的AI Studio上体验）
- 本来编写类库的初衷是使用Windows本机功能完全替代Imagemagick，然而System.Drawing.Common在我的电脑上老是无法正常工作，于是只好替换成SixLabors.ImageSharp。好在这至少相对Imagemagick提高了处理速度，并降低了磁盘上的总体积。（Imagemagick大小50M，SixLabors.ImageSharp大小2M）
- 为与新版本PowerShell保持一致，该库基于.NET 9编写，版本与PowerShell 7.5相同。ImageSharp适用于.NET 6+。

## 在PowerShell上使用方法

- 由于我主要以PowerShell批处理使用该库，因此仅提供PowerShell上的使用方法。反正和其他.NET平台语言上的使用方法没有大区别。
- 此类库使用`ImageSharpProcessorLib`命名空间，提供`ImageProcessor`类，公开静态方法:

```csharp
ProcessImage(string imagePath, int ppi = 144, bool convertToPng = false)
```

- 在PowerShell中的使用方式：

```powershell
# 导入dll文件中的类。
Add-Type -Path "C:\Path\To\ImageSharpProcessorLib.dll"

# 使用双冒号调用类的静态方法。
[ImageSharpProcessorLib.ImageProcessor]::ProcessImage(
    imagePath = "C:\path\to\image.jpg"
    ppi = 144
    convertToPng = $true
)
```

- 此类库只针对单张图片的操作，批处理请结合PowerShell的内置循环功能。

## 更新计划

- 我将常用的函数/cmdlet汇总为一个脚本模块。目前有计划将其全部迁移到C#，变成二进制模块，实际上最初正是打算制作完整模块的。但是GitHub Copilot不给力，又是降低需求，又是求助Gemini老师才得到当前的版本，好在使用上没有问题。


## 许可证
- 考虑到ImageSharp许可证的要求，此类库使用MIT许可证。见[LICENSE](./LICENSE)。