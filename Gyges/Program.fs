open Gyges
open Gyges.Utils

open System
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type Model =
    { Player: Player
      Bullets: Map<Guid, Bullet> }

let init(): Model =     
    { Player = Player.init()
      Bullets = Map.empty }
    
let update (input: Input) (time: Time) (model: Model): Model = 
    let keyToDir = function
        | Up    -> -Vector2.UnitY
        | Left  -> -Vector2.UnitX
        | Down  ->  Vector2.UnitY
        | Right ->  Vector2.UnitX
        | _ -> Vector2.Zero
    
    let dir =
        input.Pressed |> List.sumBy keyToDir
    
    let isFireAllowed = 
        input.Pressed |> List.contains Fire &&
        time.Total - model.Player.LastFireTime > model.Player.FireRate
    
    let player =
        model.Player
        |> Player.update
               [ yield Player.Move dir
                 if isFireAllowed then
                     yield Player.Fire ]
               time
    
    let bullets =
        model.Bullets
        |> Map.mapValues (Bullet.update time)
        |> Map.filterValues (fun bullet -> bullet.Pos.Y > 0.0f)
    
    let bullets =
        if isFireAllowed then
            bullets |> Map.add (Guid.NewGuid()) (Bullet.init (model.Player.Pos + Vector2(0.0f, -10.0f)))
        else
            bullets

      
    { model with Player = player
                 Bullets = bullets }

let draw (spriteBatch: SpriteBatch) (content: Content) (model: Model) =
    
    let drawTexture (texture: Texture2D) (pos: Vector2) =
        let width, height =
            texture.Width |> float32, texture.Height |> float32
            
        spriteBatch.Draw(texture, pos - Vector2(width, height)/2.0f, Color.White)
    
    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp)
    spriteBatch.GraphicsDevice.Clear(Color.DarkBlue)
   
    for KeyValue(_, bullet) in model.Bullets do
        drawTexture content.Bullet bullet.Pos
        
    drawTexture content.Ship model.Player.Pos
     
    spriteBatch.End()

[<EntryPoint>]
let main argv =
    let config =
        { GameWidth = 256
          GameHeight = 192
          ScreenWidth = 640
          ScreenHeight = 480
          IsFullscreen = false
          IsFixedTimeStep = false
        }
    
    let game =
        { Config = config
          LoadContent = Content.load
          Init = init
          HandleInput = Input.handle
          Update = update
          Draw = draw
        }
    
    game
    |> GameLoop.make
    |> GameLoop.run

    0
