# AppComposer

AppComposer is a Windows WPF application for creating monochrome image compositions from multiple layers.

## Features

* Image layers
* Text layers with configurable font and size
* Composition layers for importing other saved compositions
* Layer selection and ordering
* Move, scale and rotate layers
* Optional white-pixel transparency
* 1 bpp internal image representation
* Anti-aliased text rendering using SkiaSharp
* Image processing and rendering using OpenCV
* Project save/load using `.comp` archives

## Project Format

A `.comp` file is a compressed project archive containing:

```text
project.json
rendered.bmp
preview.bmp
layers/
    <layer>.bmp
```

The project JSON contains the composition structure, layer properties and transformations parameters. Image data is stored separately.

## Usage

1. Create a new composition and configure the canvas.
2. Add image or text layers.
3. Select layers to move, scale or rotate them.
4. Enable `TransparentWhite` on image/composition layers in config.ini when white pixels should reveal underlying layers.
5. Import another composition as a `CompositionLayer`.
6. Save the project using **Save As** to create a `.comp` archive.
7. Use **Save** to update the current project.

When a composition is loaded, its layers and associated image data are reconstructed from the project archive.

## Architecture

The application follows an MVVM architecture:

* **Models** represent project and layer data.
* **ViewModels** handle UI interaction.
* **ImageService** handles image loading, conversion, previews and rendering.
* **CompositionService** handles project-level operations.

Layer transformations are represented conceptually by a single affine transformation built from scale, rotation and translation, and are applied during rendering with OpenCV.
