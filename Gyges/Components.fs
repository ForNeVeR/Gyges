module Gyges.Components

open Microsoft.Xna.Framework
open Gyges.Math

type Position = Position of Vector2 with
    static member (+) (Position lhs, Position rhs): Position =
        lhs + rhs |> Position
    
    static member (-) (Position lhs, Position rhs): Position =
        lhs - rhs |> Position

type Velocity = Velocity of Vector2

type Collider = Collider of Rectangle

module Collider =
    let offset (Position position) (Collider rectangle): Collider =
        offset (position |> Vector2.toPoint) rectangle |> Collider
        
    let checkCollision (Collider lhsRectangle) (Collider rhsRectangle): bool =
        lhsRectangle.Intersects(rhsRectangle)

type Engine = { Speed: float32 }

type Movement = Time -> Engine -> Velocity

module Movement =
    
    let verticalUp (_time: Time) (engine: Engine): Velocity =
        -Vector2.UnitY * engine.Speed |> Velocity

    let verticalDown (_time: Time) (engine: Engine): Velocity =
        Vector2.UnitY * engine.Speed |> Velocity

    let directed (Position direction) (_time: Time) (engine: Engine): Velocity =
        (direction |> norm) * engine.Speed |> Velocity