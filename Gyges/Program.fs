open Gyges

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type Content = { Ship: Texture2D }
    
let loadContent (contentManager: ContentManager): Content =
    { Ship = contentManager.Load("ship") }
    
type Key = Left | Right | Up | Down | NoKey

type Input = { KeyPressed: Key }

let (|KeyPressed|_|) (key: Keys) (keyboard: KeyboardState) =
    if keyboard.IsKeyDown(key) then Some()
    else None

let handleInput(): Input =
    let keyboard = Keyboard.GetState()
    let key: Key =
        match keyboard with
        | KeyPressed Keys.W -> Up
        | KeyPressed Keys.A -> Left
        | KeyPressed Keys.S -> Down
        | KeyPressed Keys.D -> Right
        | _ -> NoKey
        
    { KeyPressed = key }

type Model = { Pos: Vector2 }
    
let init(): Model = { Pos = Vector2(50.0f, 50.0f) }
    
let update (input: Input) (DeltaTime dt) (model: Model): Model =
    let shift =
        match input.KeyPressed with
        | Up -> Vector2(0.0f, -100.0f)
        | Left -> Vector2(-100.0f, 0.0f)
        | Down -> Vector2(0.0f, 100.0f)
        | Right -> Vector2(100.0f, 0.0f)
        | _ -> Vector2.Zero
        
    { model with Pos = model.Pos + shift*dt }

let draw (spriteBatch: SpriteBatch) (content: Content) (model: Model) =
    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp)
    spriteBatch.GraphicsDevice.Clear(Color.Red)
    spriteBatch.Draw(content.Ship, model.Pos, Color.White)
    spriteBatch.End()

[<EntryPoint>]
let main argv =
    
    let game =
        { Config = { Width = 256; Height = 192 }
          LoadContent = loadContent
          Init = init
          HandleInput = handleInput
          Update = update
          Draw = draw
        }
    
    use loop = new GameLoop<_, _, _>(game)
    loop.IsFixedTimeStep <- false

    use graphics = new GraphicsDeviceManager(loop)
    graphics.IsFullScreen <- false
    graphics.SynchronizeWithVerticalRetrace <- false
    graphics.PreferredBackBufferWidth <- 800      
    graphics.PreferredBackBufferHeight <- 600
    graphics.ApplyChanges()
    
    loop.Run()
    0
