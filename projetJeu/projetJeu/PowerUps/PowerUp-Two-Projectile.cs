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
    class PowerUp_Two_Projectile : PowerUp
    {
        public PowerUp_Two_Projectile(float x, float y) : base(x, y)
        {

        }

        public PowerUp_Two_Projectile(Vector2 pos)
            : this(pos.X, pos.Y)
        {

        }

        public void kappa(){}

        private static Texture2D texture;

        public override Texture2D Texture
        {
            get { return texture; }
        }

        public static void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
        {
            texture = content.Load<Texture2D>(@"Powerups\two-projectile-powerup");
        }
    }
}
