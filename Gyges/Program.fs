open Gyges

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

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
          LoadContent = Content.load
          Init = init
          HandleInput = Input.handle
          Update = update
          Draw = draw
        }
    
    use loop = new GameLoop<_, _, _>(game)
    loop.IsFixedTimeStep <- false

    use graphics = new GraphicsDeviceManager(loop)
    graphics.IsFullScreen <- false
    graphics.SynchronizeWithVerticalRetrace <- false
    graphics.PreferredBackBufferWidth <- 640      
    graphics.PreferredBackBufferHeight <- 480
    graphics.ApplyChanges()
    
    loop.Run()
    0
