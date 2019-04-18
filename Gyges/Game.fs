namespace Gyges

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics;
open Microsoft.Xna.Framework.Input

type InputState =
    {
        Keyboard: KeyboardState
    }
    
type Config =
    {
        Width: int
        Height: int
    }

type GameLoop<'Model, 'Content>( config: Config,
                                 loadContent: ContentManager -> 'Content,
                                 init: unit -> 'Model,
                                 update: InputState -> 'Model -> 'Model,
                                 draw: SpriteBatch -> 'Content -> 'Model -> unit ) =
    inherit Game()

    let mutable renderTarget: RenderTarget2D = null
    let mutable spriteBatch: SpriteBatch = null
    
    let mutable model = Unchecked.defaultof<'Model>
    let mutable content = Unchecked.defaultof<'Content>
    
    let mutable fpsFont: SpriteFont = null
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

    override this.Initialize() =
        renderTarget <- new RenderTarget2D(this.GraphicsDevice, config.Width, config.Height)
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        model <- init()
        base.Initialize()

    override this.LoadContent() =
        fpsFont <- this.Content.Load<SpriteFont> "./connection"
        content <- loadContent (this.Content)
        base.LoadContent()

    override __.Update(gameTime) =
        let input: InputState = { Keyboard = Keyboard.GetState() }
        model <- update input model
        base.Update(gameTime)

    override this.Draw(gameTime) =
        let gd = this.GraphicsDevice
        gd.Clear Color.Black
        
        gd.SetRenderTarget(renderTarget)
        draw spriteBatch content model
        
        let screenWidth = gd.PresentationParameters.BackBufferWidth;
        let screenHeight = gd.PresentationParameters.BackBufferHeight;
        
        let aspect = config.Width/config.Height
        
        let rect =
            if screenWidth > screenHeight
            then
                let actualWidth = aspect*screenHeight
                let topLeft = (screenWidth - actualWidth) / 2
                new Rectangle(topLeft, 0, actualWidth, screenHeight)
            else
                let actualHeight = screenWidth/aspect
                let topLeft = (screenHeight - actualHeight) / 2
                new Rectangle(0, topLeft, screenWidth, actualHeight)
        
        gd.SetRenderTarget(null)
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp)
        spriteBatch.Draw(renderTarget, rect, Color.White)
        updateAndPrintFPS gameTime spriteBatch
        spriteBatch.End()
        base.Draw(gameTime)
