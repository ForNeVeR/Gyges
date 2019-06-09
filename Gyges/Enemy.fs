namespace Gyges

open Microsoft.Xna.Framework
open Components

type Enemy =
    { Position: Position
      Engine: Engine
      Movement: Movement
      Collider: Collider
      Weapon: Weapon
      Health: int }
    
module Enemy =
    let weaponOffset = Vector2(0.0f, 10.0f) |> Position
    
    let create(): Enemy =
        let position = Vector2(0.0f, 0.0f) |> Position
        let recharger: WeaponRecharger =
            { FireRate = 0.5f
              LastFireTime = 0.0f }
            
        let weapon: Weapon =
            { Position = position + weaponOffset
              Pattern = Weapon.Patterns.empty
              Recharger = recharger
            }        
        
        let engine: Engine = { Speed = 50.0f }
        
        { Position = position
          Engine = engine
          Movement = Movement.verticalDown
          Collider = Rectangle(0, 0, 37, 29) |> Collider
          Weapon = weapon
          Health = 5 }

    let updatePosition (time: Time) (enemy: Enemy): Enemy =
        let dt = time.Delta
        let velocity = enemy.Movement time enemy.Engine
        let position = Physics.motionStep enemy.Position velocity dt
        let weapon = { enemy.Weapon with Position = position + weaponOffset }
        { enemy with
            Position = position
            Weapon = weapon }
        
    let updateHealth (damage: int) (enemy: Enemy): Enemy =
        { enemy with Health = enemy.Health - damage }
        
    let aim (target: Position) (enemy: Enemy): Enemy =
        let weapon = { enemy.Weapon with Pattern = Weapon.Patterns.targeted target }
        { enemy with Weapon = weapon }