namespace Gyges

open Components
open Microsoft.Xna.Framework

type Bullet =
    { Position: Position
      Engine: Engine
      Movement: Movement
      Collider: Collider
      Health: int }
    
module Bullet =
    let create(position: Vector2): Bullet =
        { Position = position |> Position
          Engine = { Speed = 200.0f }
          Movement = Movement.verticalUp
          Collider = Rectangle(0, 0, 15, 11) |> Collider
          Health = 1 }
        
    let updatePosition (time: Time) (bullet: Bullet): Bullet =
        let dt = time.Delta
        let velocity = bullet.Movement time bullet.Engine        
        { bullet with
            Position = Physics.motionStep bullet.Position velocity dt }

    let updateHealth (damage: int) (bullet: Bullet): Bullet =
        { bullet with Health = bullet.Health - damage }