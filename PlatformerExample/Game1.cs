using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerExample
{
    enum GameState
    {
        Start,
        Playing,
        End
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteSheet sheet;
        Player player;
        List<Platform> platforms;
        AxisList platformAxis;
        List<Token> tokens;
        AxisList tokenAxis;
        GameText gameText;
        GameState state;
        Lava lava;

        int score;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            platforms = new List<Platform>();
            tokens = new List<Token>();
            gameText = new GameText();
            state = GameState.Start;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            #if VISUAL_DEBUG
                VisualDebugging.LoadContent(Content);
            #endif
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            var t = Content.Load<Texture2D>("spritesheet");
            sheet = new SpriteSheet(t, 21, 21, 3, 2);

            // Create the player with the corresponding frames from the spritesheet
            var playerFrames = from index in Enumerable.Range(19, 30) select sheet[index];
            List<Sprite> playerFramesPlus = playerFrames.ToList();
            playerFramesPlus.Add(sheet[112]);
            player = new Player(playerFramesPlus);

            // Create the platforms
            platforms.Add(new Platform(new BoundingRectangle(160, 200, 63, 21), sheet[3]));
            platforms.Add(new Platform(new BoundingRectangle(349, 200, 63, 21), sheet[3]));
            platforms.Add(new Platform(new BoundingRectangle(233, 258, 105, 21), sheet[3]));
            platforms.Add(new Platform(new BoundingRectangle(538, 142, 84, 21), sheet[3]));
            platforms.Add(new Platform(new BoundingRectangle(811, 374, 84, 21), sheet[3]));

            tokens.Add(new Token(new BoundingRectangle(370, 179, 21, 21), sheet[115]));
            tokens.Add(new Token(new BoundingRectangle(570, 121, 21, 21), sheet[115]));
            tokens.Add(new Token(new BoundingRectangle(843, 353, 21, 21), sheet[115]));

            // Add the platforms to the axis list
            platformAxis = new AxisList();
            foreach(Platform platform in platforms)
            {
                platformAxis.AddGameObject(platform);
            }

            tokenAxis = new AxisList();
            foreach(Token token in tokens)
            {
                tokenAxis.AddGameObject(token);
            }

            lava = new Lava(sheet[13], sheet[42]);

            score = 0;

            gameText.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if(state == GameState.End)
                {
                    player.Reset();
                    tokens.ForEach(token =>
                    {
                        token.Reset();
                    });
                    score = 0;
                }
                state = GameState.Playing;
            }

            if (state == GameState.Playing)
            {
                // TODO: Add your update logic here
                player.Update(gameTime);

                if(player.Life <= 0)
                {
                    state = GameState.End;
                }

                // Check for platform collisions
                var platformQuery = platformAxis.QueryRange(player.Bounds.X, player.Bounds.X + player.Bounds.Width);
                player.CheckForPlatformCollision(platformQuery);

                var tokenQuery = tokenAxis.QueryRange(player.Bounds.X, player.Bounds.X + player.Bounds.Width);
                score += player.CheckForTokenCollision(tokenQuery);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            var offset = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2) - player.Position;
            var t = Matrix.CreateTranslation(offset.X, offset.Y, 0);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, t);

            // Draw the platforms 
            platforms.ForEach(platform =>
            {
                platform.Draw(spriteBatch);
            });

            // Draw the tokens 
            tokens.ForEach(token =>
            {
                token.Draw(spriteBatch);
            });

            // Draw the player
            player.Draw(spriteBatch);

            Vector2 location = new Vector2(GraphicsDevice.Viewport.Width/2 + player.Position.X, player.Position.Y - GraphicsDevice.Viewport.Height/2);
            gameText.DrawScore(spriteBatch, "Score: " + score.ToString(), location);

            if (state == GameState.Start)
            {
                gameText.Draw(spriteBatch, "Press Enter To Begin", player.Position);
            }

            //Draw lava at bottom
            lava.Draw(spriteBatch, player.Position.X - GraphicsDevice.Viewport.Width / 2, player.Position.X + GraphicsDevice.Viewport.Width / 2, player.Position.Y + GraphicsDevice.Viewport.Height/2);

            //If game is over draw the end game and begin game message
            if (state == GameState.End)
            {
                gameText.Draw(spriteBatch, "Press Enter To Begin", player.Position);

                gameText.Draw(spriteBatch, "Game Over!", new Vector2(player.Position.X, player.Position.Y + GraphicsDevice.Viewport.Height/4));
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
