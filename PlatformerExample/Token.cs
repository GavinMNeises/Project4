using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerExample
{
    public enum TokenState
    {
        Active,
        Inactive
    }

    public class Token : IBoundable
    {
        /// <summary>
        /// The token's bounds
        /// </summary>
        BoundingRectangle bounds;

        /// <summary>
        /// The token's sprite
        /// </summary>
        Sprite sprite;

        /// <summary>
        /// The bounding rectangle of the token
        /// </summary>
        public BoundingRectangle Bounds => bounds;

        TokenState state;

        public TokenState State => state;

        /// <summary>
        /// Constructs a new token
        /// </summary>
        /// <param name="bounds">The token's bounds</param>
        /// <param name="sprite">The token's sprite</param>
        public Token(BoundingRectangle bounds, Sprite sprite)
        {
            this.bounds = bounds;
            this.sprite = sprite;

            state = TokenState.Active;
        }

        public void Collect()
        {
            state = TokenState.Inactive;
        }

        public void Reset()
        {
            state = TokenState.Active;
        }

        /// <summary>
        /// Draws the platform
        /// </summary>
        /// <param name="spriteBatch">The spriteBatch to render to</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            #if VISUAL_DEBUG
                VisualDebugging.DrawRectangle(spriteBatch, bounds, Color.Green);
            #endif

            if(state == TokenState.Active) sprite.Draw(spriteBatch, bounds, Color.White);
        }
    }
}
