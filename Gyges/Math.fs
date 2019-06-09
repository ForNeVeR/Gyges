module Gyges.Math

open Microsoft.Xna.Framework

let norm (vec: Vector2) =
    let vec = Vector2(vec.X, vec.Y)
    if vec <> Vector2.Zero then vec.Normalize()
    vec
    
let offset (amount: Vector2) (rect: Rectangle) =
    let rect = Rectangle(rect.Location, rect.Size)
    let shift =
        Vector2(amount.X - (rect.Width/2 |> float32),
                amount.Y - (rect.Height/2 |> float32))
    rect.Offset(shift); rect

