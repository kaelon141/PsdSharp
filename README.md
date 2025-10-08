# 🎨 PsdSharp
[![NuGet](https://img.shields.io/nuget/v/PsdSharp.svg?style=flat-square)](https://www.nuget.org/packages/PsdSharp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](LICENSE)

## 📖 About
**PsdSharp** is a fast, cross-platform .NET library for **reading and rendering Photoshop PSD and PSB files** in accordance with [Adobe's Photoshop File Formats Specification](https://www.adobe.com/devnet-apps/photoshop/fileformatashtml/), written entirely in managed C#.

PsdSharp is designed primarily for:
- Reading PSD/PSB image data
- Converting Photoshop channels into interleaved color buffers
- Rendering images via third-party graphics libraries (e.g. ImageSharp, SkiaSharp)
- Inspecting metadata, layers, and image resources
> ✏️ PsdSharp does not (yet) support writing or editing PSD files.

 **Duotone** and **multichannel** images are rendered as **grayscale** due to an intentional lack of documentation by Adobe.
### Supported color modes
✅ RGB\
✅ CMYK\
✅ Lab\
✅ Bitmap\
✅ Grayscale\
✅ Indexed\
⚠️ Duotone → Rendered as grayscale, due to lack of documentation by Adobe.\
⚠️ Multichannel → Rendered as grayscale, due to lack of documentation by Adobe.
---
## 🚀 Getting started

### Installation
Simply add the [PsdSharp NuGet package](https://www.nuget.org/packages/PsdSharp/) to your project:
```
dotnet add package PsdSharp
```
### Optional extension packages
| **Package**                                                                    | **Description**                                                                                                 |
|--------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------|
| **[PsdSharp.SkiaSharp](https://www.nuget.org/packages/PsdSharp.SkiaSharp/)**   | Convert `ImageData` to a [SkiaSharp](https://github.com/mono/SkiaSharp) `SKBitmap`                              |
| **[PsdSharp.ImageSharp](https://www.nuget.org/packages/PsdSharp.ImageSharp/)** | Convert `ImageData` to an [ImageSharp](https://github.com/SixLabors/ImageSharp) `Image`                         |    
| **[PsdSharp.Avalonia](https://www.nuget.org/packages/PsdSharp.Avalonia/)**     | Avalonia control for viewing PSD files                                                                          |
| **[PsdSharp.WPF](https://www.nuget.org/packages/PsdSharp.WPF/)**               | **🪟 Windows-only**. Convert `ImageData` to a `BitMapSource` for WPF.                                           |
| **[PsdSharp.Win32](https://www.nuget.org/packages/PsdSharp.Win32/)**           | **🪟 Windows-only**. Convert `ImageData` to a `System.Drawing.Bitmap`, i.e. for WinForms or the Win32 GDI+ API. |

---

### 💻 Usage
To open a PSD file, simply use the PsdFile.Open method. This method accepts a stream.
```csharp
using PsdSharp;

using var fileStream = File.Open("myFile.psd", FileMode.Open, FileAccess.Read, FileShare.Read);
var psdFile = PsdFile.Open(fileStream);

// Do something with the file...
var width = psdFile.Header.WidthInPixels;
var height = psdFile.Header.HeightInPixels;

//Get the raw composite image data as stored in the PSD file
var rawData = psdFile.ImageData.GetChannels().SelectMany(channel => channel.GetData()).ToArray();

//Or get a buffer with interleaved RGB data, which is more useful for rendering
var buffer = PixelDataConverter.GetInterleavedBuffer(psdFile.ImageData, ColorType.Rgba8888);

//You can also get the image data for a specific layer.
var layer = psdFile.Layers.First();
var buffer = PixelDataConverter.GetInterleavedBuffer(layer.ImageData, ColorType.Rgba8888);
```

If you installed an extension package, you will find extension methods for converting the image data to a format suitable for the corresponding graphics library on the **ImageData** class. You can access the composite image data trough **PsdFile.ImageData**, and layer image data through **Layer.ImageData**.

The PsdSharp.Avalonia package adds a PsdView control. Sample usage is as follows:
```xaml
//MainWindow.axaml
<avalonia:Window xmlns="https://github.com/avaloniaui"
                 xmlns:psdsharp="clr-namespace:PsdSharp.AvaloniaControls;assembly=PsdSharp.AvaloniaControls"
                 mc:Ignorable="d"
                 x:Class="PsdSharp.Avalonia.Sample.MainWindow"
                 Title="PsdSharp.Avalonia Sample"
                 Height="450"
                 Width="800">
    <psdsharp:PsdView
                x:Name="PsdView"
                HorizontalAlignment="Stretch"
                VerticalAlignment="Stretch"
                Height="1024"
                Width="1024"
            />
</avalonia:Window>
```
```csharp
//MainWindow.axaml.cs
var psdStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PsdSharp.Samples.Avalonia.Assets.test.psd");
PsdView.PsdFile = psdStream;
```

---

## ⚙️ Configuration
The PsdFile.Open method accepts an optional second **PsdLoadOptions** parameter. This object can be used to configure how the library loads PSD files and whether image data should immediately be buffered into memory, cached to disk, or skipped entirely based on your needs. 

The following parameters are available:
 - **LeaveInputOpen** (bool, default **true**): Whether PsdSharp should leave the underlying stream open. Set this to false if you want the stream closed after parsing.\
Note: Do not set this to **false** if you intend on using ImageDataLoading.OnDemand.
 - **ImageDataLoading** How the library should behave when encountering image data.
    - **Auto** (default): The library will automatically decide whether to cache the image data in memory or whether to write to disk. By default, images over 64MB will be held in memory, and larger images will be written to disk.
    - **LoadImmediately**: Always cache images in memory, no matter their size.
    - **CacheOnDisk**: Always write images to disk, no matter their size. The files will be written to the OS's temp directory.
    - **LoadOnDemand**: Do nothing with the image data. Load it from the original stream only when requested. This requires a seekable stream.
    - **Skip**: Skip loading the image data entirely, useful if you're only interested in metadata. The `PsdFile.ImageData` and `Layer.ImageData` properties will be `null`.
 - **AutoImageLoadingDiskCacheThresholdBytes**: The threshold at which ImageDataLoading.Auto decides that an image is too large to be held in memory and will cache the image to disk instead. 64MB by default. If ImageDataLoading is not set to Auto, this option is ignored.

---

## ❓ FAQ
###  **Can I programatically generate PSD files with PsdSharp?**
No, this library currently only supports **reading** and **rendering** PSD and PSB files. There's currently no capability for creating new files or for modifying existing ones.
### **Why is my image being rendered as a grayscale image?**
If your image is in color in Photoshop but grayscale when rendered through PsdSharp, you're most likely working with a **Duotone** or **Multichannel** image. These formats are intentionally undocumented by Adobe, and Adobe advises third-party vendors to render them as grayscale. To render them in color, convert the image to RGB, CMYK, Lab or Indexed color.
### How can I export my image to a different format?
The recommended way is to use ImageSharp in tandem with PsdSharp. If you have the [PsdSharp](https://www.nuget.org/packages/PsdSharp/), [PsdSharp.ImageSharp](https://www.nuget.org/packages/PsdSharp.ImageSharp/) and [ImageSharp](https://www.nuget.org/packages/SixLabors.ImageSharp/) NuGet packages installed, exporting becomes trivial:
```csharp
var fileStream = File.Open("myFile.psd", FileMode.Open, FileAccess.Read, FileShare.Read);

var psdFile = PsdFile.Open(fileStream);

var image = psdFile.ImageData.ToImageSharpImage();
image.Save("myFile.png");
```
### Will you add blending modes / layer effects?
No. This library reads PSD file metadata and converts Photoshop's image data into a more usable format. Compositing effects is out of scope.

---
## 🤝 Contributing 
Pull requests are welcome!
If unsure about something, or if you want to make a major change, please open an issue first so we can discuss.

---

## 📜 License
This project is licensed under the [MIT License](https://github.com/kaelon141/PsdSharp/blob/main/LICENSE).
