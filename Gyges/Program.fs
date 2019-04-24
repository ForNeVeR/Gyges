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

let handleMove (input: Input) (time: Time) (model: Model): Model =
    let keyToDir key =
        match key with
        | Up    -> -Vector2.UnitY
        | Left  -> -Vector2.UnitX
        | Down  ->  Vector2.UnitY
        | Right ->  Vector2.UnitX
        | _ -> Vector2.Zero
        
    let dir =
        input.Pressed |> List.sumBy keyToDir
        
    { model with Player = model.Player |> Player.move dir time }

let handleFire (input: Input) (time: Time) (model: Model): Model =
    let player = model.Player
    let isFireAllowed = 
        input.Pressed |> List.contains Fire &&
        time.Total - player.LastFireTime > player.FireRate
    
    if isFireAllowed then
        let bullets =
            model.Bullets
            |> GuidMap.add (Bullet.init (player.Pos + Vector2(0.0f, -10.0f)))
            
        { model with Player = player |> Player.fire time
                     Bullets = bullets }
        
    else
        model

let processBullets (time: Time) (model: Model): Model =
    { model with Bullets = model.Bullets |> Map.mapValues (Bullet.update time) }

let clearBullets (model: Model): Model =
    let filtered =
        model.Bullets
        |> Map.filterValues (fun bullet -> bullet.Pos.Y > 0.0f)
        
    { model with Bullets = filtered }
         
let update (input: Input) (time: Time) (model: Model): Model = 
    model
    |> handleMove input time
    |> handleFire input time
    |> processBullets time
    |> clearBullets

let draw (canvas: Canvas) (content: Content) (model: Model) =
    canvas.Clear(Color.DarkBlue)
    
    for KeyValue(_, bullet) in model.Bullets do
        canvas.DrawTexture content.Bullet bullet.Pos
        
    canvas.DrawTexture content.Ship model.Player.Pos

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
