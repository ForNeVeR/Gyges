namespace Gyges

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics;

type DeltaTime = DeltaTime of float32
    
type Config =
    {
        Width: int
        Height: int
    }
    
type Game<'Model, 'Content, 'Input> =
    {
        Config: Config
        LoadContent: ContentManager -> 'Content
        Init: unit -> 'Model
        HandleInput: unit -> 'Input
        Update: 'Input -> DeltaTime -> 'Model -> 'Model
        Draw: SpriteBatch -> 'Content -> 'Model -> unit
    }

type GameLoop<'Model, 'Content, 'Input>(game: Game<_, _, _>) =
    inherit Game()

    let mutable renderTarget: RenderTarget2D = null
    let mutable onScreenRect: Rectangle = Unchecked.defaultof<Rectangle>
    let mutable spriteBatch: SpriteBatch = null
    
    let mutable model = Unchecked.defaultof<'Model>
    let mutable content = Unchecked.defaultof<'Content>
    let mutable input = Unchecked.defaultof<'Input>
    
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
        let gd = this.GraphicsDevice
        let config = game.Config
        
        renderTarget <- new RenderTarget2D(gd, config.Width, config.Height)
        let screenWidth = gd.PresentationParameters.BackBufferWidth;
        let screenHeight = gd.PresentationParameters.BackBufferHeight;
        let scale = min (screenWidth/config.Width) (screenHeight/config.Height)
        let width = scale * config.Width
        let height = scale * config.Height
        onScreenRect <- Rectangle(screenWidth/2 - width/2, screenHeight/2 - height/2,
                                  width, height)
        
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
                
        model <- game.Init()
        base.Initialize()

    override this.LoadContent() =
        fpsFont <- this.Content.Load<SpriteFont> "./connection"
        content <- game.LoadContent (this.Content)
        base.LoadContent()

    override __.Update(gameTime) =
        let delta = DeltaTime (gameTime.ElapsedGameTime.TotalSeconds |> float32)
        
        input <- game.HandleInput()
        model <- game.Update input delta model
        base.Update(gameTime)

    override this.Draw(gameTime) =
        let gd = this.GraphicsDevice
        gd.Clear Color.Black
        
        gd.SetRenderTarget(renderTarget)
        game.Draw spriteBatch content model
        
        gd.SetRenderTarget(null)
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp)
        spriteBatch.Draw(renderTarget, onScreenRect, Color.White)
        updateAndPrintFPS gameTime spriteBatch
        spriteBatch.End()
        base.Draw(gameTime)
        