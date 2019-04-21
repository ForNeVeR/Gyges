module Gyges.Math

open Microsoft.Xna.Framework

let norm (vec: Vector2) =
    if vec <> Vector2.Zero then vec.Normalize()
    vec

