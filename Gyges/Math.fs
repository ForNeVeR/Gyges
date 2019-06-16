module Gyges.Math

open Microsoft.Xna.Framework

module Vector2 =
    let toPoint (vec: Vector2) =
        Point(vec.X |> int, vec.Y |> int)

let norm (vec: Vector2) =
    let vec = Vector2(vec.X, vec.Y)
    if vec <> Vector2.Zero then vec.Normalize()
    vec
    
let offset (amount: Point) (rect: Rectangle) =
    let mutable newRect = Rectangle.Empty
    newRect.Location <- rect.Location
    newRect.Width <- rect.Width
    newRect.Height <- rect.Height

    let shift =
        Point(amount.X - (rect.Width/2),
                amount.Y - (rect.Height/2))
    rect.Offset(shift); rect

