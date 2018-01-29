using IFM20884;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projetJeu
{
    class PowerUp_Fire_Shot : PowerUp
    {
        public PowerUp_Fire_Shot(float x, float y) : base(x, y)
        {

        }

        public PowerUp_Fire_Shot(Vector2 pos)
            : this(pos.X, pos.Y)
        {

        }

        private static Texture2D texture;

        public override Texture2D Texture
        {
            get { return texture; }
        }

        public static void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
        {
            texture = content.Load<Texture2D>(@"Powerups\small-fire-shot-powerup");
        }
    }
}
