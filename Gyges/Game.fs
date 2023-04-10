namespace Gyges

open System.Numerics
open Raylib_CsLo

type Time = { 
    Total: float
    Delta: float32 
}
    
type Config = { 
    GameWidth: int
    GameHeight: int
    ScreenWidth: int
    ScreenHeight: int
    ScreenTitle: string
    TargetFps: int
}

type Canvas = { 
    DrawTexture: Texture -> Vector2 -> unit
    DrawRectangle: float32 -> float32 -> float32 -> float32 -> Color -> unit
    DrawText: Font -> string -> Vector2 -> float32 -> float32 -> unit
    Clear: Color -> unit 
}
    
type Game<'World, 'Content, 'Input> = { 
    LoadContent: unit -> 'Content
    Init: unit -> 'World
    HandleInput: unit -> 'Input
    Update: 'Input -> Time -> 'World -> 'World
    Draw: Canvas -> 'Content -> 'World -> unit
}
    
type GameState<'World, 'Content, 'Input>(config: Config, game: Game<_, _, _>) =
    let mutable renderTarget: RenderTexture = Unchecked.defaultof<RenderTexture>
    let mutable gameRect: Rectangle = Unchecked.defaultof<Rectangle>
    let mutable onScreenRect: Rectangle = Unchecked.defaultof<Rectangle>
    let mutable canvas: Canvas = Unchecked.defaultof<Canvas>
    
    let mutable model = Unchecked.defaultof<'World>
    let mutable content = Unchecked.defaultof<'Content>
    let mutable input = Unchecked.defaultof<'Input>

    let drawTexture (texture: Texture) (pos: Vector2) =
        Raylib.DrawTexture(texture, int (pos.X - float32 (texture.width/2)), int (pos.Y - float32 (texture.height/2)), Raylib.WHITE)

    let drawRectangle (x: float32) (y: float32) (width: float32) (height: float32) (color: Color) =
        Raylib.DrawRectangleLines(int x, int y, int width, int height, color)    

    let drawText (font: Font) (text: string) (pos: Vector2) (fontSize: float32) (spacing: float32) =
        Raylib.SetTextureFilter(font.texture, TextureFilter.TEXTURE_FILTER_BILINEAR)
        Raylib.DrawTextEx(font, text, pos, fontSize, spacing, Raylib.WHITE)
    
    let clearScreen (color: Color) =
        Raylib.ClearBackground(color)

    member _.Initialize() =
        renderTarget <- Raylib.LoadRenderTexture(config.GameWidth, config.GameHeight)

        let screenWidth = config.ScreenWidth;
        let screenHeight = config.ScreenHeight;
        let scale = min (screenWidth/config.GameWidth) (screenHeight/config.GameHeight)
        let width = scale * config.GameWidth
        let height = scale * config.GameHeight
        onScreenRect <- Rectangle(float32 (screenWidth/2 - width/2), 
                                  float32 (screenHeight/2 - height/2),
                                  float32 width, float32 height)
        gameRect <- Rectangle(0.0f, 0.0f, float32 config.GameWidth, -float32 config.GameHeight)
        canvas <- { 
            DrawTexture = drawTexture
            DrawRectangle = drawRectangle
            DrawText = drawText
            Clear = clearScreen 
        }
        model <- game.Init()

    member _.LoadContent() =
        content <- game.LoadContent()

    member __.Update(time: Time) =
        input <- game.HandleInput()
        model <- game.Update input time model

    member _.Draw() =
        Raylib.BeginTextureMode(renderTarget)
        game.Draw canvas content model
        Raylib.EndTextureMode()
        
        Raylib.BeginDrawing()
        Raylib.DrawTexturePro(renderTarget.texture, gameRect, onScreenRect, Vector2.Zero, 0.0f, Raylib.WHITE)
        Raylib.DrawFPS(10, 10)
        Raylib.EndDrawing()

module GameState =
    let run (config: Config) (game: Game<'World, 'Content, 'Input>) =
        Raylib.InitWindow(config.ScreenWidth, config.ScreenHeight, config.ScreenTitle);
        Raylib.SetTargetFPS(config.TargetFps)

        let loop = new GameState<'World, 'Content, 'Input>(config, game)
        loop.Initialize()
        loop.LoadContent()

        while not (Raylib.WindowShouldClose()) do
            loop.Draw()

            let time = { 
                Total = Raylib.GetTime()
                Delta = Raylib.GetFrameTime() 
            }
            loop.Update(time)

        Raylib.CloseWindow();
