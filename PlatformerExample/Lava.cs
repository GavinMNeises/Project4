using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PlatformerExample
{
    public class Lava
    {
        Sprite topSprite;

        Sprite bottomSprite;

        public Lava(Sprite top, Sprite bottom)
        {
            topSprite = top;
            bottomSprite = bottom;
        }

        public void Draw(SpriteBatch spriteBatch, float left, float right, float bottom)
        {
            for(float i = left; i < right; i += 21)
            {
                topSprite.Draw(spriteBatch, new Vector2(i, 500-21), Color.White, 0, new Vector2(), 1, SpriteEffects.None, 1);
                for(float j = 500; j < bottom; j += 21)
                {
                    bottomSprite.Draw(spriteBatch, new Vector2(i, j), Color.White, 0, new Vector2(), 1, SpriteEffects.None, 1);
                }
            }
        }
    }
}
