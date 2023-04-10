namespace Gyges

open Components

type WeaponRecharger =
    { FireRate: float
      LastFireTime: float  
    }

type WeaponPattern = Time -> Position -> Bullet option
    
type Weapon =
    { Position: Position
      Pattern: WeaponPattern
      Recharger: WeaponRecharger
    }
    
module Weapon =
    
    module Patterns =
        let empty (_time: Time) (_position: Position): Bullet option = None
        
        let verticalUp (_time: Time) (position: Position): Bullet option =
            Bullet.create position Movement.verticalUp |> Some
            
        let targeted (target: Position) (_time: Time) (position: Position): Bullet option =
            Bullet.create position (Movement.directed (target - position)) |> Some
        
    let fire (time: Time) (weapon: Weapon): Bullet option * Weapon =
        let isFireAllowed = 
            time.Total - weapon.Recharger.LastFireTime > (1.0/weapon.Recharger.FireRate)
            
        if isFireAllowed then
            let bullet = weapon.Pattern time weapon.Position
            let recharger = { weapon.Recharger with LastFireTime = time.Total }
            let weapon = { weapon with Recharger = recharger }
            
            bullet, weapon
        else
            None, weapon