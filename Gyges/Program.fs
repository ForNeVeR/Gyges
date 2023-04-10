open Gyges
open Gyges.Utils
open Gyges.Components

open System
open System.Numerics
open Raylib_CsLo

type World =
    { Player: Player
      Bullets: Map<Guid, Bullet>
      Enemies: Map<Guid, Enemy>
      EnemiesBullets: Map<Guid, Bullet>
      Score: int }

let init(): World =     
    { Player = Player.create()
      Bullets = Map.empty
      Enemies = Map.empty
      EnemiesBullets = Map.empty
      Score = 0 }

let handleMove (input: Input) (time: Time) (model: World): World =
    let keyToDir key =
        match key with
        | Up    -> -Vector2.UnitY
        | Left  -> -Vector2.UnitX
        | Down  ->  Vector2.UnitY
        | Right ->  Vector2.UnitX
        | _ -> Vector2.Zero
        
    let dir =
        let vec = input.Down |> List.sumBy keyToDir
        if RayMath.Vector2Length(vec) > 0.0f
        then RayMath.Vector2Normalize(vec)
        else vec
        
    { model with
        Player = model.Player
                 |> Player.updateVelocity dir
                 |> Player.updatePosition time }

let handleFire (input: Input) (time: Time) (model: World): World =
    let player = model.Player  
    if input.Down |> List.contains Fire then
        let bullet, weapon = player.Weapon |> Weapon.fire time
        let bullets =
            match bullet with
            | Some bullet -> model.Bullets |> Map.addNew bullet
            | None -> model.Bullets        
        let player = { player with Weapon = weapon }  
            
        { model with
            Player = player
            Bullets = bullets }
    else
        model

let spawnEnemy (model: World): World =
    let rnd = Random()
    let x = rnd.Next(19, 237) |> float32
    let enemy = { Enemy.create() with Position = Vector2(x, -29.0f) |> Position }
    
    { model with Enemies = model.Enemies |> Map.addNew enemy }

let every (period: float) (time: Time) (action: World -> World) (model: World): World =
    let t = time.Total
    let dt = time.Delta
    if floor (t/ float period) <> floor ((t + float dt)/period) then
        action model
    else
        model

let updateEnemies (time: Time) (world: World): World =
    let enemies =
        world.Enemies
        |> Map.mapValues (Enemy.updatePosition time)
        |> Map.mapValues (Enemy.aim world.Player.Position)
    
    let folder (world: World) (enemyId: Guid) (enemy: Enemy): World =
        let bullet, weapon = enemy.Weapon |> Weapon.fire time
        let enemy = { enemy with Weapon = weapon }
        let enemiesBullets =
            match bullet with
            | Some bullet -> world.EnemiesBullets |> Map.addNew bullet
            | None -> world.EnemiesBullets
        
        { world with
            Enemies = world.Enemies |> Map.add enemyId enemy
            EnemiesBullets = enemiesBullets }
    
    enemies |> Map.fold folder world

let clearEnemies (model: World): World =    
    let filterByPosition: Enemy -> bool =
        fun { Position = Position value } -> value.Y < 240.0f + 19.0f
    
    let onScreen = model.Enemies |> Map.filterValues filterByPosition
            
    let filterByHealth: Enemy -> bool =
        fun { Health = health } -> health > 0
    
    let alive = onScreen |> Map.filterValues filterByHealth
        
    { model with
        Score = model.Score + onScreen.Count - alive.Count
        Enemies = alive }

let updateBullets (time: Time) (model: World): World =
    { model with
        Bullets = model.Bullets |> Map.mapValues (Bullet.updatePosition time)
        EnemiesBullets = model.EnemiesBullets |> Map.mapValues (Bullet.updatePosition time) }

let clearBullets (model: World): World =
    let filter: Bullet -> bool =
        fun { Position = Position value; Health = health } ->
            value.Y > 0.0f && health > 0
        
    { model with
        Bullets = model.Bullets |> Map.filterValues filter
        EnemiesBullets = model.EnemiesBullets |> Map.filterValues filter }
         
let collideEnemiesAndBullets (model: World): World =
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
    
    let folder (model: World) (bulletId, enemyId) =
        let bullet = model.Bullets.[bulletId]
        let enemy = model.Enemies.[enemyId]
        
        { model with
            Bullets = model.Bullets |> Map.add bulletId (Bullet.updateHealth 1 bullet)
            Enemies = model.Enemies |> Map.add enemyId (Enemy.updateHealth 1 enemy) }

    collided |> Seq.fold folder model

let collidePlayerAndBullets (model: World): World =
    let player = model.Player
    let bullets = model.EnemiesBullets
    let collided = seq {
       for KeyValue(bulletId, bullet) in bullets do
          let bulletCollider = bullet.Collider |> Collider.offset bullet.Position
          let playerCollider = player.Collider |> Collider.offset player.Position
          if Collider.checkCollision bulletCollider playerCollider then
                yield bulletId
    }
    
    let folder (model: World) bulletId =
        let bullet = model.EnemiesBullets.[bulletId]
        { model with
            EnemiesBullets = model.EnemiesBullets |> Map.add bulletId (Bullet.updateHealth 1 bullet)
            Player = model.Player |> Player.updateHealth 1 }

    collided |> Seq.fold folder model    
       
let update (input: Input) (time: Time) (model: World): World = 
    model
    |> handleMove input time
    |> handleFire input time
    |> (every 1.0 time spawnEnemy)
    |> updateEnemies time
    |> updateBullets time
    |> collideEnemiesAndBullets
    |> collidePlayerAndBullets
    |> clearEnemies
    |> clearBullets

let draw (canvas: Canvas) (content: Content) (model: World) =
    canvas.Clear(Raylib.DARKBLUE)
    
    for KeyValue(_, { Position = Position pos; Collider = Collider rect }) in model.Bullets do
        canvas.DrawTexture content.Bullet pos
        canvas.DrawRectangle (pos.X - rect.width/2.0f) (pos.Y - rect.height/2.0f) rect.width rect.height Raylib.RED
    
    for KeyValue(_, { Position = Position pos; Collider = Collider rect }) in model.EnemiesBullets do
        canvas.DrawTexture content.Bullet pos
        canvas.DrawRectangle (pos.X - rect.width/2.0f) (pos.Y - rect.height/2.0f) rect.width rect.height Raylib.RED
        
    for KeyValue(_, { Position = Position pos; Collider = Collider rect }) in model.Enemies do
        canvas.DrawTexture content.Enemy pos
        canvas.DrawRectangle (pos.X - rect.width/2.0f) (pos.Y - rect.height/2.0f) rect.width rect.height Raylib.RED
    
    let (Position pos) = model.Player.Position
    let (Collider rect) = model.Player.Collider   
    canvas.DrawTexture content.Ship pos
    canvas.DrawRectangle (pos.X - rect.width/2.0f) (pos.Y - rect.height/2.0f) rect.width rect.height Raylib.RED
    
    canvas.DrawText content.ScoreFont $"{model.Score}" (Vector2(230.0f, 10.0f)) (float32 content.ScoreFont.baseSize) 2.0f
    canvas.DrawText content.ScoreFont $"{model.Player.Health}" (Vector2(230.0f, 50.0f)) (float32 content.ScoreFont.baseSize) 2.0f

[<EntryPoint>]
let main argv =
    let config = { 
        ScreenTitle = "Gyges"
        GameWidth = 320
        GameHeight = 240
        ScreenWidth = 640
        ScreenHeight = 480
        TargetFps = 60
    }
    
    let game = { 
        LoadContent = Content.load
        Init = init
        HandleInput = Input.handle
        Update = update
        Draw = draw
    }
    
    GameState.run config game

    0
