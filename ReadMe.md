# ImageSharpProcessorLib feat. SixLabors.ImageSharp

使用SixLabors.ImageSharp处理图像的.NET类库。专门用于修改ppi和转换格式。
A .NET library for image processing using SixLabors.ImageSharp, specifically designed for PPI modification and format conversion.

这个类库是为了替代在我的PowerShell脚本中使用的IamegMagick而编写的。该脚本专用于大量图片的批处理，由于图片大小经过手动确定调整，故该脚本主要关注于分辨率与格式。
This library was created to replace ImageMagick in my PowerShell scripts. These scripts are used for batch processing large quantities of images, focusing primarily on resolution and format conversion since image sizes are manually verified and adjusted.

同时，由于脚本经过长期使用的积累，考虑到了部分特殊情况，其设置更贴合作者实际使用中的需求。
Based on long-term usage experience, this library addresses various edge cases and is tailored to meet practical requirements.

## 主要功能 | Key Features

- 修改ppi：默认值为144。虽然屏幕常用ppi为72和96，但这两个数值在部分程序修改图片后，元数据记录的大小与实际值不同，故统一更改为144，也可手动更改，或保持原样。
- PPI Modification: Default value is 144. While screens commonly use 72 or 96 PPI, these values sometimes cause metadata inconsistencies in certain programs after image modification. Therefore, we standardize to 144 PPI, with options for custom values or preserving original PPI.

- “线性”ppi模式：默认为true（此时会忽略其他ppi设置）。为保证在pdf文件中所有图片都能拥有相同的显示大小，此模式下所有图片ppi都被设置为横向像素值的1/10（有取整），这可保证生成pdf文档的页面横向大小均为10英尺。
- "Linear" PPI Mode: Enabled by default (overrides other PPI settings). Sets image PPI to 1/10 of its horizontal pixel count (rounded), ensuring consistent display sizes in PDF documents where physical dimensions matter. This guarantees all pages maintain a 10-inch width.

- 转换为png格式：默认输出为原始格式，但是可选择全部转换为png格式。这是由于使用Acrobat生成pdf时，jpg直接生成有概率出错，因此先转换为png，由Acrobat再转换为jpg。
- PNG Conversion: While default output maintains original format, optional PNG conversion is available. This addresses potential JPEG processing errors when generating PDFs with Acrobat.


## 其他说明 | Additional Notes

- 该类库由Gemini-2.5-Pro-Experimental-0325（temperature=0.3）编写初版，GitHub Copilot feat. Claude 3.5 Sonnet检查与修正。感谢LLM🙏。Gemini 2.5可在[Google AI Studio](http://aistudio.google.com/app/prompts/new_chat?model=gemini-2.5-pro-exp-03-25)体验
- This library was initially drafted by Gemini-2.5-Pro-Experimental-0325 (temperature=0.3) and refined by GitHub Copilot feat. Claude 3.5 Sonnet. Thanks LLM🙏. Try Gemini 2.5 in [Google AI Studio](http://aistudio.google.com/app/prompts/new_chat?model=gemini-2.5-pro-exp-03-25)

- 本来计划使用Windows本机功能完全替代Imagemagick，然而System.Drawing.Common不稳定，改用SixLabors.ImageSharp后提高了处理速度，并降低了磁盘占用（Imagemagick：50M，ImageSharp dll：2M）。
- Originally intended to use Windows native functionality, System.Drawing.Common reliability issues led to adopting SixLabors.ImageSharp instead, improving processing speed and reducing disk footprint (ImageMagick: 50MB vs ImageSharp dll: 2MB).

- 为与新版本PowerShell保持一致，本库基于.NET 9编写，版本与PowerShell 7.5相同。ImageSharp适用于.NET 6+。
- Built on .NET 9 for compatibility with PowerShell 7.5. ImageSharp supports .NET 6+.


## PowerShell上使用方法 | PowerShell Usage

- 此类库使用`ImageSharpProcessorLib`命名空间，提供`ImageProcessor`类，公开静态方法: | The Library provides the method:

```csharp
public static void ProcessImage(
    string imagePath,
    bool convertToPng = false,
    bool linear = true,         // Override other PPI settings
    bool no_ppi = false,        // Preserve original PPI, if lnear = false
    int ppi = 144               // Specified PPI value, if linear = false && no_ppi = false
)
```

- 在PowerShell中的使用方式 | How to use in PowerShell

```powershell
# Import the library
Add-Type -Path "C:\Path\To\ImageSharpProcessorLib.dll"

# Call the method with ::
[ImageSharpProcessorLib.ImageProcessor]::ProcessImage(
    imagePath = "C:\path\to\image.jpg"
    convertToPng = $true
    linear = $false
    no_ppi = $false
    ppi = 256
)
```

> ⚠️ **警告 | Warning**
> 当图片成功处理时，本库将会覆写或删除原文件。
> Original file will be overwritten or deleted when format conversion succeeds.

> 💡 **提示 | Tips**
> 本库提供的方法只处理单个文件。批处理需结合循环语句或另写逻辑。
> This library only processes single files. For batch processing, use PowerShell loop statements

## 更新计划 | Roadmap

- 未来计划：计划将常用功能汇总为脚本模块。原本计划制作完整的二进制模块，但当前版本已满足核心需求。
- Future Plans: Consolidating commonly used functions into a script module. Initial binary module plan scaled back as current version meets core requirements.

### 更新历史 | Update History:
- feature update #1 - 新增linear和no_ppi模式。与原PowerShell脚本的功能对齐。(2025/3/30) | Added linear and no_ppi modes, completing feature parity with original PowerShell script. (30/3/2025)


## 许可证
考虑到ImageSharp许可证的要求，此类库使用MIT许可证。见[LICENSE](./LICENSE)。
This library uses the MIT License in compliance with ImageSharp's licensing requirements. See [LICENSE](./LICENSE).