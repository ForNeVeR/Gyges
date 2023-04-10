module Gyges.Components

open System.Numerics
open Raylib_CsLo

type Position = Position of Vector2 with
    static member (+) (Position lhs, Position rhs): Position =
        lhs + rhs |> Position
    
    static member (-) (Position lhs, Position rhs): Position =
        lhs - rhs |> Position

type Velocity = Velocity of Vector2

type Collider = Collider of Rectangle

module Collider =
    let offset (Position position) (Collider rectangle): Collider =
        Rectangle(rectangle.x + position.X,
                  rectangle.y + position.Y,
                  rectangle.width,
                  rectangle.height) 
        |> Collider
        
    let checkCollision (Collider lhsRectangle) (Collider rhsRectangle): bool =
        Raylib.CheckCollisionRecs(lhsRectangle, rhsRectangle)

type Engine = { Speed: float32 }

type Movement = Time -> Engine -> Velocity

module Movement =
    
    let verticalUp (_time: Time) (engine: Engine): Velocity =
        -Vector2.UnitY * engine.Speed |> Velocity

    let verticalDown (_time: Time) (engine: Engine): Velocity =
        Vector2.UnitY * engine.Speed |> Velocity

    let directed (Position direction) (_time: Time) (engine: Engine): Velocity =
        let dir =
            if RayMath.Vector2Length(direction) > 0.0f 
            then RayMath.Vector2Normalize(direction)
            else direction
        dir * engine.Speed |> Velocity