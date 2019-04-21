namespace Gyges

open Microsoft.Xna.Framework.Input

type Key = Left | Right | Up | Down | Fire

type Input = { Pressed: Key list }

module Input =
    let handle(): Input =
        let keyboard = Keyboard.GetState()
        let keys =
            [
                if keyboard.IsKeyDown(Keys.W) then yield Up
                if keyboard.IsKeyDown(Keys.A) then yield Left
                if keyboard.IsKeyDown(Keys.S) then yield Down
                if keyboard.IsKeyDown(Keys.D) then yield Right
                if keyboard.IsKeyDown(Keys.Space) then yield Fire
            ]
            
        { Pressed = keys }

