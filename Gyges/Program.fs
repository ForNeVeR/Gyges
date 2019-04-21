open Gyges

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open FSharpx.Collections

type Player =
    {
        Pos: Vector2
        FireRate: int
        Speed: float32
    }
    
type Bullet =
    {
        Pos: Vector2
        Lifetime: float32
        Speed: float32
    }

type Model =
    {
        Player: Player
        Bullets: PersistentHashMap<int, Bullet>
    }

let init(): Model =
    let player: Player =
        { Pos = Vector2(50.0f, 50.0f)
          FireRate = 10
          Speed = 100.0f }
    
    { Player = player
      Bullets = PersistentHashMap.empty }
    
let update (input: Input) (DeltaTime dt) (model: Model): Model =
    let keyToDir = function
        | Up    -> -Vector2.UnitY
        | Left  -> -Vector2.UnitX
        | Down  ->  Vector2.UnitY
        | Right ->  Vector2.UnitX
        | _ -> Vector2.Zero

    let norm (vec: Vector2) = vec.Normalize(); vec
    
    let moveWith (speed: float32) (dir: Vector2) =
        (norm dir) * speed
            
    let shift =
        input.Pressed
        |> List.map (keyToDir >> moveWith model.Player.Speed)
        |> List.sum
    
    let player = { model.Player with Pos = model.Player.Pos + shift*dt }
    
    { model with Player = player }

let draw (spriteBatch: SpriteBatch) (content: Content) (model: Model) =
    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp)
    spriteBatch.GraphicsDevice.Clear(Color.Red)
    spriteBatch.Draw(content.Ship, model.Player.Pos, Color.White)
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
