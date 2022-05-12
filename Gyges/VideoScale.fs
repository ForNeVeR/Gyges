// video_scale.c
namespace Gyges

open type SDL2.SDL

type ScalerFunction = (*source*) SDL_Surface -> (*destination*) SDL_Surface -> unit

[<NoComparison; NoEquality>]
type Scaler = {
    Width: int
    Height: int
    Scaler16: ScalerFunction
    Scaler32: ScalerFunction
    Name: string
}

module VideoScale =
    let none16 (source: SDL_Surface) (destination: SDL_Surface) =
        failwithf "Not implemented"

    let none32 (source: SDL_Surface) (destination: SDL_Surface) =
        failwithf "Not implemented"

    let scalers: Scaler array = [|
        { Width = Vga.Width; Height = Vga.Height; Scaler16 = none16; Scaler32 = none32; Name = "None" }
        { Width = 2 * Vga.Width; Height = 2 * Vga.Height; Scaler16 = none16; Scaler32 = none32; Name = "2x" }
        { Width = 3 * Vga.Width; Height = 3 * Vga.Height; Scaler16 = none16; Scaler32 = none32; Name = "3x" }
        { Width = 4 * Vga.Width; Height = 4 * Vga.Height; Scaler16 = none16; Scaler32 = none32; Name = "4x" }
    |]
