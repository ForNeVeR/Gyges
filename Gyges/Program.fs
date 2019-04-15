open Gyges

open Microsoft.Xna.Framework

[<EntryPoint>]
let main argv =
    use game = new GameLoop()
    game.IsFixedTimeStep <- false

    use graphics = new GraphicsDeviceManager(game)
    graphics.IsFullScreen <- true
    graphics.SynchronizeWithVerticalRetrace <- false
    graphics.PreferredBackBufferWidth <- 800
    graphics.PreferredBackBufferHeight <- 600
    graphics.ApplyChanges()
    
    game.Run()
    0
