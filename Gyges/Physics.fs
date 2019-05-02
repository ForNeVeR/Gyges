module Gyges.Physics

open Gyges.Components

let motionStep (Position pos) (Velocity vel) (dt: float32): Position =
    pos + vel * dt |> Position