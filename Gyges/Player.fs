namespace Gyges

open Microsoft.Xna.Framework

type Player =
    { Pos: Vector2
      FireRate: float32
      LastFireTime: float32
      Velocity: float32 }
    
module Player = 
    let init(): Player =
        { Pos = Vector2(50.0f, 50.0f)
          FireRate = 0.1f
          LastFireTime = 0.0f
          Velocity = 100.0f }
    
    let move (dir: Vector2) (time: Time) (player: Player): Player =
        let dt = time.Delta
        { player with Pos = player.Pos + player.Velocity*dir*dt }

    let fire (time: Time) (player: Player): Player =
        { player with LastFireTime = time.Total }