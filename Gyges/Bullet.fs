namespace Gyges

open Microsoft.Xna.Framework

type Bullet =
    { Pos: Vector2
      Speed: float32 }
    
module Bullet =
    let init(pos: Vector2): Bullet =
        { Pos = pos
          Speed = 200.0f }
        
    let update (time: Time) (bullet: Bullet): Bullet =
        let dt = time.Delta
        { bullet with
            Pos = bullet.Pos - Vector2.UnitY * bullet.Speed * dt }

