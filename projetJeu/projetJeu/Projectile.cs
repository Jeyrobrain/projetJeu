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
        public Vector2 velocity;

        public Vector2 speed = new Vector2(5f, 5f);

        public bool visible;

        public Projectile(float x, float y) : base(x, y)
        {

        }

        public Projectile(Vector2 position) : base(position)
        {

        }

        /// <summary>
        /// Attribut statique (i.e. partagé par toutes les instances) constituant une 
        /// liste de palettes à exploiter selon la direction et l'état du projectile.
        /// </summary>
        private static List<Palette> palettes = new List<Palette>();

        /// <summary>
        /// Surchargé afin de retourner la palette correspondante au projectile
        /// </summary>
        protected override Palette PaletteAnimation {
            get { return palettes[0]; }
        }

        /// <summary>
        /// Charge les images associées au sprite du joueur.
        /// </summary>
        /// <param name="content">Gestionnaire de contenu permettant de charger les images du vaisseau.</param>
        /// <param name="graphics">Gestionanire de périphérique d'affichage permettant d'extraire
        /// les caractéristiques de celui-ci (p.ex. l'écran).</param>
        public static void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
        {
            palettes.Add(new Palette(content.Load<Texture2D>(@"Projectiles\blue-orb"), 100, 100));
        }

        /// <summary>
        /// Ajuste la position du sprite en fonction de l'input.
        /// </summary>
        /// <param name="gameTime">Gestionnaire de temps de jeu.</param>
        /// <param name="graphics">Gestionnaire de périphérique d'affichage.</param>
        public override void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            this.Position += this.velocity;
            base.Update(gameTime, graphics);
        }
    }
}
