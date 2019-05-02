namespace Gyges

open Microsoft.Xna.Framework
open Components

type Player =
    { Position: Position
      Velocity: Velocity
      Engine: Engine
      Weapon: Weapon
      Health: int }
    
module Player = 
    let create(): Player =
        let recharger: WeaponRecharger =
            { FireRate = 10.0f
              LastFireTime = 0.0f }
            
        let weapon: Weapon =
            { Type = WeaponType.Vertical
              Recharger = recharger
            }
        
        let engine: Engine = { Speed = 100.0f }
        
        { Position = Vector2(50.0f, 50.0f) |> Position
          Velocity = Vector2.Zero |> Velocity
          Engine = engine
          Weapon = weapon
          Health = 5 }
    
    let updatePosition (time: Time) (player: Player): Player =
        let dt = time.Delta
        { player with
            Position = Physics.motionStep player.Position player.Velocity dt }
    
    let updateVelocity (dir: Vector2) (player: Player): Player =
        let speed = player.Engine.Speed
        { player with Velocity = Velocity (dir * speed) }
    
    let fire (time: Time) (player: Player): Player =
        let recharger = { player.Weapon.Recharger with LastFireTime = time.Total }
        let weapon = { player.Weapon with Recharger = recharger }
        { player with Weapon = weapon }
        
    let updateHealth (damage: int) (enemy: Player): Player =
        { enemy with Health = enemy.Health - damage }