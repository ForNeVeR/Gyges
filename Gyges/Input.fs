namespace Gyges

open Raylib_CsLo

type Key = Left | Right | Up | Down | Fire

type Input = { Down: Key list }

module Input =
    let keyBindings =
        Map.ofList
            [ 
                KeyboardKey.KEY_UP, Up
                KeyboardKey.KEY_DOWN, Down
                KeyboardKey.KEY_LEFT, Left
                KeyboardKey.KEY_RIGHT, Right
                KeyboardKey.KEY_SPACE, Fire 
            ]
    
    let handle(): Input =
        let keys = 
            [ for KeyValue(k, v) in keyBindings do
                if Raylib.IsKeyDown(k) then yield v ]
            
        { Down = keys }

