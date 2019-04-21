namespace Gyges

open Microsoft.Xna.Framework.Input

type Key = Left | Right | Up | Down | NoKey

type Input = { KeyPressed: Key }

module Input =
    let (|KeyPressed|_|) (key: Keys) (keyboard: KeyboardState) =
        if keyboard.IsKeyDown(key) then Some()
        else None

    let handle(): Input =
        let keyboard = Keyboard.GetState()
        let key: Key =
            match keyboard with
            | KeyPressed Keys.W -> Up
            | KeyPressed Keys.A -> Left
            | KeyPressed Keys.S -> Down
            | KeyPressed Keys.D -> Right
            | _ -> NoKey
            
        { KeyPressed = key }

