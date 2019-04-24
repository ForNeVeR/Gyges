namespace Gyges

open Microsoft.Xna.Framework

type Enemy =
    { Pos: Vector2
      Velocity: float32
      Box: Rectangle
      Hitpoints: int }
    
module Enemy =
    let init () =
        { Pos = Vector2(0.0f, 0.0f)
          Velocity = 50.0f
          Box = Rectangle(0, 0, 37, 29)
          Hitpoints = 2 }
        
    let move (time: Time) (enemy: Enemy): Enemy =
        let dt = time.Delta
        { enemy with Pos = enemy.Pos + enemy.Velocity*Vector2.UnitY*dt }