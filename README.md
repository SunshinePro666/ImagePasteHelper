# ImagePasteHelper

ImagePasteHelper is a Windows WinForms utility that watches the clipboard and converts copied image **files** into clipboard **image data**.

This makes it possible to paste directly into apps like Microsoft Excel as a real image with **Ctrl + V**, instead of only pasting a file reference.

## Problem It Solves

When you copy an image file from Windows File Explorer, clipboard contents may be treated as a file-drop entry instead of image bitmap data. In those cases, pasting into Microsoft Excel does not insert the image directly as expected.

ImagePasteHelper bridges that gap by detecting a copied supported image file and rewriting clipboard contents to image data.

## Supported Formats

- `.jpg`
- `.jpeg`
- `.png`
- `.bmp`

## Usage

1. Run `ImagePasteHelper`.
2. Keep **automatic monitoring** enabled.
3. In Windows File Explorer, copy one supported image file.
4. In Microsoft Excel, paste with **Ctrl + V**.

## Tray Behavior

- Minimizing the app hides it to the system tray.
- Clicking the window close button hides it to the tray.
- Tray **Show** restores the main window.
- Tray **Exit** fully closes the app.

## Requirements

If you download the **framework-dependent** version, you need to install **.NET 8 Desktop Runtime** first:

[Download .NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

If you download the **self-contained** version, you do not need to install .NET separately.

## Download Notes

### Self-Contained Version

Download this version if you want to run the app directly without installing .NET separately.

Recommended for most users:

- `ImagePasteHelper-v0.3.0-win-x64-self-contained.zip`

### Framework-Dependent Version

Download this version if you want a smaller package and already have **.NET 8 Desktop Runtime** installed.

- `ImagePasteHelper-v0.3.0-win-x64-framework-dependent.zip`

If you do not have it installed, download it here:

[Download .NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

## Limitations

- Only one image file is supported at a time.
- While monitoring is enabled, the app intentionally converts copied image files into image clipboard data.
- If you need to copy image files as files, disable monitoring or exit the app.

## Publish

### Windows x64, Self-Contained Release

This version includes the required .NET runtime. Users do not need to install .NET separately.

Run this from the project directory:

```bash
dotnet publish .\ImagePasteHelper.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o .\publish\win-x64
```

### Windows x64, Framework-Dependent Release

This version is smaller, but users need to install **.NET 8 Desktop Runtime** first.

Download .NET 8 here:

[Download .NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

Run this from the project directory:

```bash
dotnet publish .\ImagePasteHelper.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o .\publish\win-x64-framework-dependent
```

> This project targets Windows WinForms, so publish output is intended to be run on Windows.
