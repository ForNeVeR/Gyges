namespace Gyges

open Microsoft.Xna.Framework

type Player =
    { Pos: Vector2
      FireRate: float32
      LastFireTime: float32
      Speed: float32 }
    
module Player =
    type Msg =
        | Move of dir: Vector2
        | Fire
    
    let init(): Player =
        { Pos = Vector2(50.0f, 50.0f)
          FireRate = 0.1f
          LastFireTime = 0.0f
          Speed = 100.0f }
        
    let update (messages: Msg list) (time: Time) (player: Player): Player =
        let dt = time.Delta        
        let folder player = function
            | Move dir ->
                let shift = (Math.norm dir) * player.Speed
                { player with Pos = player.Pos + shift*dt }
                
            | Fire ->
                { player with LastFireTime = time.Total }
        
        messages |> List.fold folder player

