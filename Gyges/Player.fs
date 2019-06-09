namespace Gyges

open Microsoft.Xna.Framework
open Components

type Player =
    { Position: Position
      Velocity: Velocity
      Engine: Engine
      Weapon: Weapon
      Collider: Collider
      Health: int }
    
module Player =
    let weaponOffset = Vector2(0.0f, -10.0f) |> Position
    
    let create(): Player =
        let position = Vector2(50.0f, 50.0f) |> Position
        
        let recharger: WeaponRecharger =
            { FireRate = 7.0f
              LastFireTime = 0.0f }
            
        let weapon: Weapon =
            { Position = position + weaponOffset
              Pattern = Weapon.Patterns.verticalUp
              Recharger = recharger
            }
        
        let engine: Engine = { Speed = 100.0f }
        
        { Position = position
          Velocity = Vector2.Zero |> Velocity
          Engine = engine
          Weapon = weapon
          Collider = Rectangle(0, 0, 21, 27) |> Collider
          Health = 5 }
    
    let updatePosition (time: Time) (player: Player): Player =
        let dt = time.Delta
        let position = Physics.motionStep player.Position player.Velocity dt
        let weapon = { player.Weapon with Position = position + weaponOffset }        
        { player with
            Position = position
            Weapon = weapon }
    
    let updateVelocity (dir: Vector2) (player: Player): Player =
        let speed = player.Engine.Speed
        { player with Velocity = Velocity (dir * speed) }
        
    let updateHealth (damage: int) (enemy: Player): Player =
        { enemy with Health = enemy.Health - damage }