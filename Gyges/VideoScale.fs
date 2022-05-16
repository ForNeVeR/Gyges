// video_scale.c
namespace Gyges

#nowarn "9"

open System
open System.Runtime.CompilerServices
open FSharp.NativeInterop
open Silk.NET.SDL
open Silk.NET.Maths

type ScalerFunction = (*api*) Sdl -> (*rbgPalette*) uint32 array -> (*source*) Surface -> (*destination*) nativeptr<Texture> -> unit

[<NoComparison; NoEquality>]
type Scaler = {
    Width: int
    Height: int
    Scaler: ScalerFunction
    Name: string
}

module VideoScale =
    let none (api: Sdl) (rbgPalette: uint32 array) (sourceSurface: Surface) (destinationTexture: nativeptr<Texture>) =
        let mutable dstWidth = 0
        let mutable dstHeight = 0
        api.QueryTexture(
            destinationTexture,
            NativePtr.nullPtr<uint32>,
            NativePtr.nullPtr<int>,
            &dstWidth, &dstHeight) |> ignore

        let width = sourceSurface.W
        let height = sourceSurface.H
        // let scale = dstHeight / height

        let mutable dstPitch = 0
        let mutable dstPixels = Unchecked.defaultof<voidptr>
        let nullRef = &Unsafe.NullRef<Rectangle<int>>()
        api.LockTexture(destinationTexture, &nullRef, &dstPixels, &dstPitch) |> ignore

        let src = Span<uint8>(sourceSurface.Pixels, sourceSurface.W * sourceSurface.H)
        let dst = Span<uint32>(dstPixels, dstWidth * dstHeight)

        // TODO Scale surface to texture
        for y = 0 to height - 1 do
            for x = 0 to width - 1 do
                dst[y*height + x] <- rbgPalette[int src[y*height + x]]

        api.UnlockTexture(destinationTexture)
    let scalers: Scaler array = [|
        { Width = Vga.Width; Height = Vga.Height; Scaler = none; Name = "None" }
    |]
