open Gyges

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics

type Content = { Ship: Texture2D }
    
let loadContent (contentManager: ContentManager): Content =
    { Ship = contentManager.Load("ship") }

type Model = { Pos: Vector2 }
    
let init(): Model = { Pos = Vector2(50.0f, 50.0f) }
    
let update (input: InputState) (model: Model): Model = model

let draw (spriteBatch: SpriteBatch) (content: Content) (model: Model) =
    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp)
    spriteBatch.Draw(content.Ship, model.Pos, Color.White)
    spriteBatch.End()

let config: Config = { Width = 128; Height = 128 }

[<EntryPoint>]
let main argv =
    use game = new GameLoop<Model, Content>(config, loadContent, init, update, draw)
    game.IsFixedTimeStep <- false

    use graphics = new GraphicsDeviceManager(game)
    graphics.IsFullScreen <- false
    graphics.SynchronizeWithVerticalRetrace <- false
    graphics.PreferredBackBufferWidth <- 1000      
    graphics.PreferredBackBufferHeight <- 640
    graphics.ApplyChanges()
    
    game.Run()
    0
