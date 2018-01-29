using IFM20884;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projetJeu
{
    public abstract class PowerUp : Sprite
    {
        public PowerUp(Vector2 pos)
            : base(pos)
        {

        }

        public PowerUp(float x, float y)
            : this(new Vector2(x, y))
        {

        }

        private float vitesseDeplacement;
        public float VitesseDeplacement
        {
            get { return this.vitesseDeplacement; }
            set { this.vitesseDeplacement = value; }
        }

        public override void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            this.Position = new Vector2(Position.X - (gameTime.ElapsedGameTime.Milliseconds * 0.2f), Position.Y);
        }
    }
}
