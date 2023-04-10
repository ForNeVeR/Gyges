module Gyges.Math

open System.Numerics
open Raylib_CsLo
    
let offset (amount: Vector2) (rect: Rectangle) =
    Rectangle(rect.x + amount.X, rect.y + amount.Y, rect.width, rect.height)
