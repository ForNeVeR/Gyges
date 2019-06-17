namespace Gyges

open Microsoft.Xna.Framework.Input

type Key = Left | Right | Up | Down | Fire

type Input = { Pressed: Key ResizeArray }

module Input =
    let keyBindings =
        Map.ofList
            [ Keys.Up, Up
              Keys.Down, Down
              Keys.Left, Left
              Keys.Right, Right
              Keys.Space, Fire ]
    
    let handle(): Input =
        let keyboard = Keyboard.GetState()
        let keys = ResizeArray()
        for KeyValue(k, v) in keyBindings do
            if keyboard.IsKeyDown(k) then keys.Add v

        { Pressed = keys }

