namespace Gyges

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics;

type GameLoop() =
    inherit Game()

    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable fpsFont = Unchecked.defaultof<SpriteFont>

    let mutable fps = 0
    let mutable lastFpsUpdate = 0.
    let fpsUpdateInterval = 100.

    let updateAndPrintFPS (gameTime : GameTime) (spriteBatch: SpriteBatch) = 
        if gameTime.TotalGameTime.TotalMilliseconds - lastFpsUpdate > fpsUpdateInterval then
            fps <- int (1. / gameTime.ElapsedGameTime.TotalSeconds)
            lastFpsUpdate <- gameTime.TotalGameTime.TotalMilliseconds
    
        spriteBatch.DrawString
            ( spriteFont = fpsFont,
              text = sprintf "%i" fps,
              position = Vector2(20.0f, 20.0f),
              color = Color.White )


    override this.LoadContent() = 
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        fpsFont <- this.Content.Load<SpriteFont> "./connection"

    override __.Update(gameTime) = ()

    override this.Draw(gameTime) = 
        this.GraphicsDevice.Clear Color.Black
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp)
        updateAndPrintFPS gameTime spriteBatch
        spriteBatch.End()


