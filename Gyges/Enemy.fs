namespace Gyges

open Microsoft.Xna.Framework
open Components

type Enemy =
    { Position: Position
      Engine: Engine
      Movement: Movement
      Collider: Collider
      Health: int }
    
module Enemy =
    let create(): Enemy =        
        { Position = Position (Vector2(0.0f, 0.0f))
          Engine = { Speed = 50.0f }
          Movement = Movement.verticalDown
          Collider = Rectangle(0, 0, 37, 29) |> Collider
          Health = 2 }

    let updatePosition (time: Time) (enemy: Enemy): Enemy =
        let dt = time.Delta
        let velocity = enemy.Movement time enemy.Engine        
        { enemy with
            Position = Physics.motionStep enemy.Position velocity dt }
        
    let updateHealth (damage: int) (enemy: Enemy): Enemy =
        { enemy with Health = enemy.Health - damage }