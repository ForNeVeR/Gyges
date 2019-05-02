module Gyges.Components

open Microsoft.Xna.Framework
open Gyges.Math

type Position = Position of Vector2

type Velocity = Velocity of Vector2

type Collider = Collider of Rectangle

module Collider =
    let offset (Position position) (Collider rectangle): Collider =
        offset position rectangle |> Collider
        
    let checkCollision (Collider lhsRectangle) (Collider rhsRectangle): bool =
        lhsRectangle.Intersects(rhsRectangle)

type Engine = { Speed: float32 }

type Movement = Time -> Engine -> Velocity

module Movement =
    
    let verticalUp (time: Time) (engine: Engine): Velocity =
        -Vector2.UnitY * engine.Speed |> Velocity

    let verticalDown (time: Time) (engine: Engine): Velocity =
        Vector2.UnitY * engine.Speed |> Velocity

type WeaponRecharger =
    { FireRate: float32
      LastFireTime: float32  
    }

type WeaponType =
    | Vertical
    
type Weapon =
    { Type: WeaponType
      Recharger: WeaponRecharger
    }