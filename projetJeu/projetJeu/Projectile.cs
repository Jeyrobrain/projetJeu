using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFM20884;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace projetJeu
{
    class Projectile : SpriteAnimation
    {
        private Vector2 velocity;
        private Vector2 speed;
        private ProjectileType projectileType;
        private Vector2 startPos;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Vector2 Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public Vector2 StartPos
        {
            get { return startPos; }
            set { startPos = value; }
        }

        public float angle = 0f;

        public Projectile(float x, float y, ProjectileType type)
            : base(x, y)
        {
            switch (type)
            {
                case ProjectileType.blueEnergyBall:
                    Speed = new Vector2(8f, 8);
                    break;
                case ProjectileType.smallFireShot:
                    Speed = new Vector2(10f, 10f);
                    break;
                case ProjectileType.disque:
                    Speed = new Vector2(4f, 4f);
                    break;
                default:
                    break;
            }
            this.StartPos = base.Position;
            this.projectileType = type;
        }

        public Projectile(Vector2 position, ProjectileType type)
            : base(position)
        {
            switch (type)
            {
                case ProjectileType.blueEnergyBall:
                    Speed = new Vector2(8f, 8f);
                    break;
                case ProjectileType.smallFireShot:
                    Speed = new Vector2(10f, 10f);
                    break;
                case ProjectileType.disque:
                    Speed = new Vector2(4f, 4f);
                    break;
                default:
                    break;
            }
            this.StartPos = base.Position;
            this.projectileType = type;
        }

        /// <summary>
        /// Attribut statique (i.e. partagé par toutes les instances) constituant une 
        /// liste de palettes à exploiter selon la direction et l'état du projectile.
        /// </summary>
        private static List<Palette> palettes = new List<Palette>();

        /// <summary>
        /// Surchargé afin de retourner la palette correspondante au projectile
        /// </summary>
        protected override Palette PaletteAnimation
        {
            get { return palettes[(int)projectileType]; }
        }

        /// <summary>
        /// Charge les images associées au sprite du joueur.
        /// </summary>
        /// <param name="content">Gestionnaire de contenu permettant de charger les images du vaisseau.</param>
        /// <param name="graphics">Gestionanire de périphérique d'affichage permettant d'extraire
        /// les caractéristiques de celui-ci (p.ex. l'écran).</param>
        public static void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
        {
            palettes.Add(new Palette(content.Load<Texture2D>(@"Projectiles\disque"), 25, 25));
            palettes.Add(new Palette(content.Load<Texture2D>(@"Projectiles\small-fire-shot"), 17, 17));
            palettes.Add(new Palette(content.Load<Texture2D>(@"Projectiles\blue-energy-ball"), 100, 100));
        }

        /// <summary>
        /// Ajuste la position du sprite en fonction de l'input.
        /// </summary>
        /// <param name="gameTime">Gestionnaire de temps de jeu.</param>
        /// <param name="graphics">Gestionnaire de périphérique d'affichage.</param>
        public override void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            switch (projectileType)
            {
                case ProjectileType.disque:
                    angle += 0.1f;
                    break;
                case ProjectileType.smallFireShot:
                case ProjectileType.blueEnergyBall:
                    angle = Velocity.X != Speed.X ? angle = Velocity.Y < 0f ? -(MathHelper.Pi / 5.142859f) : (MathHelper.Pi / 5.142859f) : 0f; // 35 rad
                    break;
                default:
                    break;
            }
            this.Position += gameTime.ElapsedGameTime.Milliseconds * (this.Velocity * 0.06f);
            base.Update(gameTime, graphics);
        }
    }
}
