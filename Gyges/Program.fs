open Gyges
open Gyges.Utils
open Gyges.Math

open Gyges
open System
open Microsoft.Xna.Framework

type Model =
    { Player: Player
      Bullets: Map<Guid, Bullet>
      Enemies: Map<Guid, Enemy> }

let init(): Model =     
    { Player = Player.init()
      Bullets = Map.empty
      Enemies = Map.empty }

let handleMove (input: Input) (time: Time) (model: Model): Model =
    let keyToDir key =
        match key with
        | Up    -> -Vector2.UnitY
        | Left  -> -Vector2.UnitX
        | Down  ->  Vector2.UnitY
        | Right ->  Vector2.UnitX
        | _ -> Vector2.Zero
        
    let dir =
        input.Pressed
        |> List.sumBy keyToDir
        |> norm
        
    { model with Player = model.Player |> Player.move dir time }

let handleFire (input: Input) (time: Time) (model: Model): Model =
    let player = model.Player
    let isFireAllowed = 
        input.Pressed |> List.contains Fire &&
        time.Total - player.LastFireTime > player.FireRate
    
    if isFireAllowed then
        let bullets =
            model.Bullets
            |> Map.addWithGuid (Bullet.init (player.Pos + Vector2(0.0f, -10.0f)))
            
        { model with
            Player = player |> Player.fire time
            Bullets = bullets }
        
    else
        model

let spawnEnemy (model: Model): Model =
    let rnd = System.Random()
    let x = rnd.Next(19, 237) |> float32
    let enemy = { Enemy.init() with Pos = Vector2(x, -29.0f) }
    
    { model with Enemies = model.Enemies |> Map.addWithGuid enemy }

let every (period: float32) (time: Time) (action: Model -> Model) (model: Model): Model =
    let t = time.Total
    let dt = time.Delta
    if floor (t/period) <> floor ((t + dt)/period) then
        action model
    else
        model

let updateEmenies (time: Time) (model: Model): Model =
    { model with
        Enemies = model.Enemies |> Map.mapValues (Enemy.move time) }

let updateBullets (time: Time) (model: Model): Model =
    { model with
        Bullets = model.Bullets |> Map.mapValues (Bullet.move time) }

let clearBullets (model: Model): Model =
    let filtered =
        model.Bullets
        |> Map.filterValues (fun bullet -> bullet.Pos.Y > 0.0f)
        
    { model with Bullets = filtered }
         
let update (input: Input) (time: Time) (model: Model): Model = 
    model
    |> handleMove input time
    |> handleFire input time
    |> (every 1.0f time spawnEnemy)
    |> updateEmenies time
    |> updateBullets time
    |> clearBullets

let draw (canvas: Canvas) (content: Content) (model: Model) =
    canvas.Clear(Color.DarkBlue)
    
    for KeyValue(_, bullet) in model.Bullets do
        canvas.DrawTexture content.Bullet bullet.Pos
        
    for KeyValue(_, enemy) in model.Enemies do
        canvas.DrawTexture content.Enemy enemy.Pos
        
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
        { LoadContent = Content.load
          Init = init
          HandleInput = Input.handle
          Update = update
          Draw = draw
        }
    
    game
    |> GameLoop.makeWithConfig config
    |> GameLoop.run

    0
