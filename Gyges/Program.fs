open Gyges

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type Player =
    {
        Pos: Vector2
        FireRate: float32
        LastFire: float32
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
        Bullets: Bullet list
    }

let init(): Model =
    let player: Player =
        { Pos = Vector2(50.0f, 50.0f)
          FireRate = 0.1f
          LastFire = 0.0f
          Speed = 100.0f }
    
    { Player = player
      Bullets = List.empty }
    
let update (input: Input) (time: Time) (model: Model): Model =
    let dt = time.Delta
    
    let keyToDir = function
        | Up    -> -Vector2.UnitY
        | Left  -> -Vector2.UnitX
        | Down  ->  Vector2.UnitY
        | Right ->  Vector2.UnitX
        | _ -> Vector2.Zero

    let norm (vec: Vector2) =
        if vec <> Vector2.Zero then vec.Normalize()
        vec
    
    let moveWith (speed: float32) (dir: Vector2) =
        (norm dir) * speed
            
    let shift =
        input.Pressed
        |> List.map keyToDir
        |> List.sum
        |> moveWith model.Player.Speed
    
    let player = { model.Player
                   with Pos = model.Player.Pos + shift*dt
                        LastFire =
                            if input.Pressed |> List.contains Fire &&
                               time.Total - model.Player.LastFire > model.Player.FireRate
                            then time.Total
                            else model.Player.LastFire}
    
    let bullets =
        model.Bullets
        |> List.map (fun x ->
            { x with Pos = x.Pos - Vector2.UnitY * x.Speed * dt
                     Lifetime = x.Lifetime + dt })
        |> List.filter (fun x -> x.Lifetime < 10.0f)
        
    let bullets =
        [
            yield! bullets
            if input.Pressed |> List.contains Fire &&
               time.Total - model.Player.LastFire > model.Player.FireRate
            then yield { Pos = model.Player.Pos + Vector2(0.0f, -10.0f)
                         Lifetime = 0.0f
                         Speed = 200.0f }
        ]
       
    { model with Player = player
                 Bullets = bullets }

let draw (spriteBatch: SpriteBatch) (content: Content) (model: Model) =
    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp)
    spriteBatch.GraphicsDevice.Clear(Color.DarkBlue)
   
    for bullet in model.Bullets do
        spriteBatch.Draw(content.Bullet, bullet.Pos - Vector2(15.0f, 11.0f)/2.0f, Color.White)
    
    spriteBatch.Draw(content.Ship, model.Player.Pos - Vector2(25.0f, 29.0f)/2.0f, Color.White)
     
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
