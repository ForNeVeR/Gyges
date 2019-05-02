open Gyges
open Gyges.Utils
open Gyges.Math
open Gyges.Components

open System
open Microsoft.Xna.Framework

type Model =
    { Player: Player
      Bullets: Map<Guid, Bullet>
      Enemies: Map<Guid, Enemy>
      Score: int }

let init(): Model =     
    { Player = Player.create()
      Bullets = Map.empty
      Enemies = Map.empty
      Score = 0 }

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
        
    { model with
        Player = model.Player
                 |> Player.updateVelocity dir
                 |> Player.updatePosition time }

let handleFire (input: Input) (time: Time) (model: Model): Model =
    let player = model.Player
    let isFireAllowed = 
        input.Pressed |> List.contains Fire &&
        time.Total - player.Weapon.Recharger.LastFireTime > (1.0f/player.Weapon.Recharger.FireRate)
    
    let (Position(pos)) = player.Position
    
    if isFireAllowed then
        let bullets =
            model.Bullets
            |> Map.addWithGuid (Bullet.create (pos + Vector2(0.0f, -10.0f)))
            
        { model with
            Player = player |> Player.fire time
            Bullets = bullets }
        
    else
        model

let spawnEnemy (model: Model): Model =
    let rnd = System.Random()
    let x = rnd.Next(19, 237) |> float32
    let enemy = { Enemy.create() with Position = Vector2(x, -29.0f) |> Position }
    
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
        Enemies = model.Enemies |> Map.mapValues (Enemy.updatePosition time) }

let clearEmenies (model: Model): Model =    
    let filter: Enemy -> bool =
        fun { Position = Position value; Health = health } ->
            value.Y < 192.0f + 19.0f && health > 0
    
    let filtered =
        model.Enemies
        |> Map.filterValues filter
        
    { model with
        Score = model.Score + model.Enemies.Count - filtered.Count
        Enemies = filtered }

let updateBullets (time: Time) (model: Model): Model =
    { model with
        Bullets = model.Bullets |> Map.mapValues (Bullet.updatePosition time) }

let clearBullets (model: Model): Model =
    let filter: Bullet -> bool =
        fun { Position = Position value; Health = health } ->
            value.Y > 0.0f && health > 0
    
    let filtered =
        model.Bullets
        |> Map.filterValues filter
        
    { model with Bullets = filtered }
         
let checkCollisions (model: Model): Model =
    let bullets = model.Bullets
    let enemies = model.Enemies
    let collided = seq {
        for KeyValue(bulletId, bullet) in bullets do
            for KeyValue(enemyId, enemy) in enemies do
                let bulletCollider = bullet.Collider |> Collider.offset bullet.Position
                let enemyCollider = enemy.Collider |> Collider.offset enemy.Position
                if Collider.checkCollision bulletCollider enemyCollider then
                    yield (bulletId, enemyId)
        }
    
    let folder (model: Model) (bulletId, enemyId) =
        let bullet = model.Bullets.[bulletId]
        let enemy = model.Enemies.[enemyId]
        
        { model with
            Bullets = model.Bullets |> Map.add bulletId (Bullet.updateHealth 1 bullet)
            Enemies = model.Enemies |> Map.add enemyId (Enemy.updateHealth 1 enemy) }

    collided |> Seq.fold folder model
       
let update (input: Input) (time: Time) (model: Model): Model = 
    model
    |> handleMove input time
    |> handleFire input time
    |> (every 1.0f time spawnEnemy)
    |> updateEmenies time
    |> updateBullets time
    |> checkCollisions
    |> clearEmenies
    |> clearBullets

let draw (canvas: Canvas) (content: Content) (model: Model) =
    canvas.Clear(Color.DarkBlue)
    
    for KeyValue(_, { Position = Position value }) in model.Bullets do
        canvas.DrawTexture content.Bullet value
        
    for KeyValue(_, { Position = Position value }) in model.Enemies do
        canvas.DrawTexture content.Enemy value
    
    let (Position(pos)) = model.Player.Position    
    canvas.DrawTexture content.Ship pos
    
    canvas.DrawText content.ScoreFont (sprintf "%i" model.Score) (Vector2(230.0f, 10.0f))

[<EntryPoint>]
let main argv =
    let config =
        { GameWidth = 320
          GameHeight = 240
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
    
    use loop =
        game
        |> GameLoop.makeWithConfig config
        |> GameLoop.run

    0
