namespace Gyges

open Microsoft.Xna.Framework

type Bullet =
    { Pos: Vector2
      Velocity: float32
      Box: Rectangle }
    
module Bullet =
    let init(pos: Vector2): Bullet =
        { Pos = pos
          Velocity = 200.0f
          Box = Rectangle(0, 0, 15, 11) }
        
    let move (time: Time) (bullet: Bullet): Bullet =
        let dt = time.Delta
        { bullet with
            Pos = bullet.Pos - Vector2.UnitY * bullet.Velocity * dt }

