namespace Gyges

open Components

open Raylib_CsLo

type Bullet =
    { Position: Position
      Engine: Engine
      Movement: Movement
      Collider: Collider
      Health: int }
    
module Bullet =
    let create (position: Position) (movement: Movement): Bullet =
        { Position = position
          Engine = { Speed = 200.0f }
          Movement = movement
          Collider = Rectangle(0.0f, 0.0f, 15.0f, 11.0f) |> Collider
          Health = 1 }
        
    let updatePosition (time: Time) (bullet: Bullet): Bullet =
        let dt = time.Delta
        let velocity = bullet.Movement time bullet.Engine        
        { bullet with
            Position = Physics.motionStep bullet.Position velocity dt }

    let updateHealth (damage: int) (bullet: Bullet): Bullet =
        { bullet with Health = bullet.Health - damage }