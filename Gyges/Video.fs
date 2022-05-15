// video.c
namespace Gyges

#nowarn "9"

open System
open FSharp.NativeInterop
open type SDL2.SDL

type ScalingMode =
| Center
| Integer
| Aspect85
| Aspect43

module Video =
    let clr256 (screen: nativeptr<SDL_Surface>) =
        SDL_FillRect(screen |> NativePtr.toNativeInt, IntPtr.Zero, 0u) |> ignore

type Video(scaler: Scaler) as this =
    let mutable fullscreenDisplay: int = -1
    let mutable scaler = scaler
    let mutable scalerFunction = Unchecked.defaultof<ScalerFunction>
    let mutable scalingMode: ScalingMode = Center
    let mutable vgaScreen: nativeptr<SDL_Surface> = NativePtr.nullPtr
    let mutable vgaScreenSeg: nativeptr<SDL_Surface> = NativePtr.nullPtr
    let mutable gameScreen: nativeptr<SDL_Surface> = NativePtr.nullPtr
    let mutable vgaScreen2: nativeptr<SDL_Surface> = NativePtr.nullPtr
    let mutable mainWindow: nativeint = IntPtr.Zero
    let mutable mainWindowRenderer: nativeint = IntPtr.Zero
    let mutable mainWindowTextureFormat: nativeptr<SDL_PixelFormat> = NativePtr.nullPtr
    let mutable mainWindowTexture: nativeint = IntPtr.Zero

    let windowGetDisplayIndex () =
        SDL_GetWindowDisplayIndex(mainWindow)

    let windowCenterInDisplay(displayIndex: int) =
        let mutable windowHeight = 0
        let mutable windowWidth = 0
        SDL_GetWindowSize(mainWindow, &windowWidth, &windowHeight)

        let mutable bounds = SDL_Rect()
        SDL_GetDisplayBounds(displayIndex, &bounds) |> ignore

        SDL_SetWindowPosition(mainWindow, bounds.x + (bounds.w - windowWidth) / 2, bounds.y + (bounds.h - windowHeight) / 2);

        ()

    let initRenderer () =
        mainWindowRenderer <- SDL_CreateRenderer(mainWindow, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED)
        if mainWindowRenderer = IntPtr.Zero then
            failwithf $"error: failed to create renderer: {SDL_GetError()}"

    let deinitRenderer () =
        if mainWindowRenderer <> IntPtr.Zero then
            SDL_DestroyRenderer(mainWindowRenderer)
            mainWindowRenderer <- IntPtr.Zero

    let initTexture () =
        assert (mainWindowRenderer <> IntPtr.Zero)

        let bitsPerPixel = 32 // TODO SDL2???
        let format =
            if bitsPerPixel = 32 then
                SDL_PIXELFORMAT_RGB888
            else
                SDL_PIXELFORMAT_RGB565

        mainWindowTextureFormat <- SDL_AllocFormat(format) |> NativePtr.ofNativeInt
        mainWindowTexture <-
            SDL_CreateTexture(
                mainWindowRenderer,
                format,
                int SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
                scaler.Width,
                scaler.Height)

        if mainWindowTexture = IntPtr.Zero then
            failwithf $"error: failed to create scaler texture {scaler.Width}x{scaler.Height}x{SDL_GetPixelFormatName(format)}: {SDL_GetError()}"

    let deinitTexture () =
        if mainWindowTexture <> IntPtr.Zero then
            SDL_DestroyTexture(mainWindowTexture)
            mainWindowTexture <- IntPtr.Zero

        if not (NativePtr.isNullPtr mainWindowTextureFormat) then
            SDL_FreeFormat(mainWindowTextureFormat |> NativePtr.toNativeInt)
            mainWindowTextureFormat <- NativePtr.nullPtr

    let initScaler (scaler: Scaler) =
        assert (mainWindow <> IntPtr.Zero)
        assert (not (NativePtr.isNullPtr mainWindowTextureFormat))

        deinitTexture()
        initTexture()

        if fullscreenDisplay = -1 then
            // Changing scalers, when not in fullscreen mode, forces the window
            // to resize to exactly match the scaler's output dimensions.
            SDL_SetWindowSize(mainWindow, scaler.Width, scaler.Height)
            windowCenterInDisplay(windowGetDisplayIndex())

        let textureFormat = mainWindowTextureFormat |> NativePtr.read
        let result =
            match textureFormat.BitsPerPixel with
            | 32uy ->
                scalerFunction <- scaler.Scaler32
                true
            | 16uy ->
                scalerFunction <- scaler.Scaler16
                true
            | _ ->
                false

        result

    do
        if SDL_WasInit(SDL_INIT_VIDEO) = 0u then
            if SDL_InitSubSystem(SDL_INIT_VIDEO) = -1 then
                failwithf $"error: failed to initialize SDL video: {SDL_GetError()}"

        // Create the software surfaces that the game renders to. These are all 320x200x8 regardless
        // of the window size or monitor resolution.
        vgaScreen <- SDL_CreateRGBSurface(0u, Vga.Width, Vga.Height, 8, 0u, 0u, 0u, 0u) |> NativePtr.ofNativeInt
        vgaScreenSeg <- vgaScreen
        vgaScreen2 <- SDL_CreateRGBSurface(0u, Vga.Width, Vga.Height, 8, 0u, 0u, 0u, 0u) |> NativePtr.ofNativeInt
        gameScreen <- SDL_CreateRGBSurface(0u, Vga.Width, Vga.Height, 8, 0u, 0u, 0u, 0u) |> NativePtr.ofNativeInt

        // The game code writes to surface->pixels directly without locking, so make sure that we
        // indeed created software surfaces that support this.
        assert not (SDL_MUSTLOCK(vgaScreen |> NativePtr.toNativeInt))
        assert not (SDL_MUSTLOCK(vgaScreen2 |> NativePtr.toNativeInt))
        assert not (SDL_MUSTLOCK(gameScreen |> NativePtr.toNativeInt))

        Video.clr256(vgaScreen)

        // Create the window with a temporary initial size, hidden until we set up the
        // scaler and find the true window size
        mainWindow <-
            SDL_CreateWindow(
                "Gyges",
                SDL_WINDOWPOS_CENTERED,
                SDL_WINDOWPOS_CENTERED,
                Vga.Width, Vga.Height,
                SDL_WindowFlags.SDL_WINDOW_RESIZABLE ||| SDL_WindowFlags.SDL_WINDOW_HIDDEN)

        if mainWindow = IntPtr.Zero then
            failwithf $"error: failed to create window: {SDL_GetError()}"

        this.ReinitFullscreen(fullscreenDisplay)
        initRenderer()
        initTexture()
        initScaler(scaler) |> ignore

        SDL_ShowWindow(mainWindow)

    interface IDisposable with
        member _.Dispose() =
            deinitRenderer()
            deinitTexture()
            SDL_DestroyWindow(mainWindow)

            SDL_FreeSurface(vgaScreenSeg |> NativePtr.toNativeInt)
            SDL_FreeSurface(vgaScreen2 |> NativePtr.toNativeInt)
            SDL_FreeSurface(gameScreen |> NativePtr.toNativeInt)

            SDL_QuitSubSystem(SDL_INIT_VIDEO)

    member _.ReinitFullscreen(newDisplay: int) =
        fullscreenDisplay <- newDisplay

        if fullscreenDisplay >= SDL_GetNumVideoDisplays() then
            fullscreenDisplay <- 0

        SDL_SetWindowFullscreen(mainWindow, uint32 SDL_bool.SDL_FALSE) |> ignore
        SDL_SetWindowSize(mainWindow, scaler.Width, scaler.Height);

        if fullscreenDisplay = -1 then
            windowCenterInDisplay(windowGetDisplayIndex())
            ()
        else
            windowCenterInDisplay(windowGetDisplayIndex())
            if SDL_SetWindowFullscreen(mainWindow, uint32 SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP) <> 0 then
                this.ReinitFullscreen(-1)
