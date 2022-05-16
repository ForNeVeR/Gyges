// video.c
namespace Gyges

#nowarn "9"

open System
open System.Runtime.CompilerServices
open FSharp.NativeInterop
open Silk.NET.SDL
open Silk.NET.Maths

type ScalingMode =
| Center
| Integer
| Aspect85
| Aspect43

module Video =
    let clr256 (api: Sdl) (screen: nativeptr<Surface>) =
        let nullRef = &Unsafe.NullRef<Rectangle<int>>()
        api.FillRect(screen, &nullRef, 0u) |> ignore
        ()

type Video(api: Sdl, scaler: Scaler) as this =
    let mutable fullscreenDisplay: int = -1
    let mutable scaler = scaler
    let mutable scalerFunction = Unchecked.defaultof<ScalerFunction>
    let mutable scalingMode: ScalingMode = Center
    let mutable vgaScreen: nativeptr<Surface> = NativePtr.nullPtr
    let mutable vgaScreenSeg: nativeptr<Surface> = NativePtr.nullPtr
    let mutable gameScreen: nativeptr<Surface> = NativePtr.nullPtr
    let mutable vgaScreen2: nativeptr<Surface> = NativePtr.nullPtr
    let mutable mainWindow: nativeptr<Window> = NativePtr.nullPtr
    let mutable mainWindowRenderer: nativeptr<Renderer> = NativePtr.nullPtr
    let mutable mainWindowTextureFormat: nativeptr<PixelFormat> = NativePtr.nullPtr
    let mutable mainWindowTexture: nativeptr<Texture> = NativePtr.nullPtr

    let windowGetDisplayIndex () =
        api.GetWindowDisplayIndex(mainWindow)

    let windowCenterInDisplay (displayIndex: int) =
        let mutable windowHeight = 0
        let mutable windowWidth = 0
        api.GetWindowSize(mainWindow, &windowWidth, &windowHeight)

        let mutable bounds = Rectangle()
        api.GetDisplayBounds(displayIndex, &bounds) |> ignore

        api.SetWindowPosition(mainWindow, bounds.Origin.X + (bounds.Size.X - windowWidth) / 2, bounds.Origin.Y + (bounds.Size.Y - windowHeight) / 2);

        ()

    let initRenderer () =
        mainWindowRenderer <- api.CreateRenderer(mainWindow, -1, uint32 RendererFlags.RendererAccelerated)
        if NativePtr.isNullPtr mainWindowRenderer then
            failwithf $"error: failed to create renderer: {api.GetErrorS()}"

    let deinitRenderer () =
        if NativePtr.isNotNullPtr mainWindowRenderer then
            api.DestroyRenderer(mainWindowRenderer)
            mainWindowRenderer <- NativePtr.nullPtr

    let initTexture () =
        assert (NativePtr.isNotNullPtr mainWindowRenderer)
        let format = uint32 PixelFormatEnum.PixelformatRgb888
        mainWindowTextureFormat <- api.AllocFormat(format)
        mainWindowTexture <-
            api.CreateTexture(
                mainWindowRenderer,
                format,
                int TextureAccess.TextureaccessStreaming,
                scaler.Width,
                scaler.Height)

        if (NativePtr.isNullPtr mainWindowTexture) then
            failwithf $"error: failed to create scaler texture {scaler.Width}x{scaler.Height}x{api.GetPixelFormatNameS(format)}: {api.GetErrorS()}"

    let deinitTexture () =
        if NativePtr.isNotNullPtr mainWindowTexture then
            api.DestroyTexture(mainWindowTexture)
            mainWindowTexture <- NativePtr.nullPtr

        if NativePtr.isNotNullPtr mainWindowTextureFormat then
            api.FreeFormat(mainWindowTextureFormat)
            mainWindowTextureFormat <- NativePtr.nullPtr

    let initScaler (scaler: Scaler) =
        assert (NativePtr.isNotNullPtr mainWindow)
        assert (NativePtr.isNotNullPtr mainWindowTextureFormat)

        deinitTexture()
        initTexture()

        if fullscreenDisplay = -1 then
            // Changing scalers, when not in fullscreen mode, forces the window
            // to resize to exactly match the scaler's output dimensions.
            api.SetWindowSize(mainWindow, scaler.Width, scaler.Height)
            windowCenterInDisplay(windowGetDisplayIndex())

        let textureFormat = mainWindowTextureFormat |> NativePtr.read
        let result =
            match textureFormat.BitsPerPixel with
            | 32uy ->
                scalerFunction <- scaler.Scaler
                true
            | _ ->
                false

        result

    do
        if api.WasInit(Sdl.InitVideo) = 0u then
            if api.InitSubSystem(Sdl.InitVideo) = -1 then
                failwithf $"error: failed to initialize SDL video: {api.GetErrorS()}"

        // Create the software surfaces that the game renders to. These are all 320x200x8 regardless
        // of the window size or monitor resolution.
        vgaScreen <- api.CreateRGBSurface(0u, Vga.Width, Vga.Height, 8, 0u, 0u, 0u, 0u)
        vgaScreenSeg <- vgaScreen
        vgaScreen2 <- api.CreateRGBSurface(0u, Vga.Width, Vga.Height, 8, 0u, 0u, 0u, 0u)
        gameScreen <- api.CreateRGBSurface(0u, Vga.Width, Vga.Height, 8, 0u, 0u, 0u, 0u)

        // The game code writes to surface->pixels directly without locking, so make sure that we
        // indeed created software surfaces that support this.
        // TODO SDL_MUSTLOCK
        //assert not (SDL_MUSTLOCK(vgaScreen |> NativePtr.toNativeInt))
        //assert not (SDL_MUSTLOCK(vgaScreen2 |> NativePtr.toNativeInt))
        //assert not (SDL_MUSTLOCK(gameScreen |> NativePtr.toNativeInt))

        Video.clr256 api vgaScreen

        // Create the window with a temporary initial size, hidden until we set up the
        // scaler and find the true window size
        mainWindow <-
            api.CreateWindow(
                "Gyges",
                Sdl.WindowposCentered,
                Sdl.WindowposCentered,
                Vga.Width, Vga.Height,
                (uint32 WindowFlags.WindowResizable) ||| (uint32 WindowFlags.WindowHidden))

        if NativePtr.isNullPtr mainWindow then
            failwithf $"error: failed to create window: {api.GetErrorS()}"

        this.ReinitFullscreen(fullscreenDisplay)
        initRenderer()
        initTexture()
        initScaler(scaler) |> ignore

        api.ShowWindow(mainWindow)

    interface IDisposable with
        member _.Dispose() =
            deinitRenderer()
            deinitTexture()
            api.DestroyWindow(mainWindow)

            api.FreeSurface(vgaScreenSeg)
            api.FreeSurface(vgaScreen2)
            api.FreeSurface(gameScreen)

            api.QuitSubSystem(Sdl.InitVideo)

    member _.ReinitFullscreen(newDisplay: int) =
        fullscreenDisplay <- newDisplay

        if fullscreenDisplay >= api.GetNumVideoDisplays() then
            fullscreenDisplay <- 0

        api.SetWindowFullscreen(mainWindow, uint32 SdlBool.False) |> ignore
        api.SetWindowSize(mainWindow, scaler.Width, scaler.Height);

        if fullscreenDisplay = -1 then
            windowCenterInDisplay(windowGetDisplayIndex())
            ()
        else
            windowCenterInDisplay(windowGetDisplayIndex())
            if api.SetWindowFullscreen(mainWindow, uint32 WindowFlags.WindowFullscreenDesktop) <> 0 then
                this.ReinitFullscreen(-1)
