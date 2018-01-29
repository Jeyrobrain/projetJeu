//-----------------------------------------------------------------------
// <copyright file="Obus.cs" company="Marco Lavoie">
// Marco Lavoie, 2010-2016. Tous droits réservés
// 
// L'utilisation de ce matériel pédagogique (présentations, code source 
// et autres) avec ou sans modifications, est permise en autant que les 
// conditions suivantes soient respectées:
//
// 1. La diffusion du matériel doit se limiter à un intranet dont l'accès
//    est imité aux étudiants inscrits à un cours exploitant le dit 
//    matériel. IL EST STRICTEMENT INTERDIT DE DIFFUSER CE MATÉRIEL 
//    LIBREMENT SUR INTERNET.
// 2. La redistribution des présentations contenues dans le matériel 
//    pédagogique est autorisée uniquement en format Acrobat PDF et sous
//    restrictions stipulées à la condition #1. Le code source contenu 
//    dans le matériel pédagogique peut cependant être redistribué sous 
//    sa forme  originale, en autant que la condition #1 soit également 
//    respectée.
// 3. Le matériel diffusé doit contenir intégralement la mention de 
//    droits d'auteurs ci-dessus, la notice présente ainsi que la
//    décharge ci-dessous.
// 
// CE MATÉRIEL PÉDAGOGIQUE EST DISTRIBUÉ "TEL QUEL" PAR L'AUTEUR, SANS 
// AUCUNE GARANTIE EXPLICITE OU IMPLICITE. L'AUTEUR NE PEUT EN AUCUNE 
// CIRCONSTANCE ÊTRE TENU RESPONSABLE DE DOMMAGES DIRECTS, INDIRECTS, 
// CIRCONSTENTIELS OU EXEMPLAIRES. TOUTE VIOLATION DE DROITS D'AUTEUR 
// OCCASIONNÉ PAR L'UTILISATION DE CE MATÉRIEL PÉDAGOGIQUE EST PRIS EN 
// CHARGE PAR L'UTILISATEUR DU DIT MATÉRIEL.
// 
// En utilisant ce matériel pédagogique, vous acceptez implicitement les
// conditions et la décharge exprimés ci-dessus.
// </copyright>
//-----------------------------------------------------------------------

namespace IFM20884
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Classe représentant un obus.
    /// </summary>
    public class Obus : SpriteAnimation
    {
        /// <summary>
        /// Attribut statique (i.e. partagé par toutes les instances) constituant une 
        /// liste de palettes à exploiter selon la direction et l'état du projectile.
        /// </summary>
        private static List<Palette> palettes = new List<Palette>();

        private ProjectileType projectileType;

        private float angle;
        public float Angle
        {
            get { return this.angle; }
            set { this.angle = value; }
        }

        private float damage;
        public float Damage
        {
            get { return damage; } 
            set {damage = value; }
        }

        private bool isEnnemi;

        /// <summary>
        /// Vitesses de propulsion de l'obus lors du tir.
        /// </summary>
        private Vector2 vitessesPropulsion;
        private Vector2 vitesseBase;

        /// <summary>
        /// Sprite à l'origine du tir.
        /// </summary>
        private Sprite source = null;

        /// <summary>
        /// Initialise une nouvelle instance de la classe Obus.
        /// </summary>
        /// <param name="x">Position en x du sprite.</param>
        /// <param name="y">Position en y du sprite.</param>
        /// <param name="vitesses">Vitesses (horizontale et verticale) de propultion de l'obus.</param>
        public Obus(float x, float y, ProjectileType type, float angle = 0f, bool isEnnemi = false)
            : base(x, y)
        {
            this.projectileType = type;

            this.angle = angle;

            switch (projectileType)
            {
                case ProjectileType.smallFireShot:
                    this.vitesseBase = new Vector2(2.5f, 2.5f);
                    this.damage = 1f;
                    break;
                case ProjectileType.blueEnergyBall:
                    this.vitesseBase = new Vector2(3.5f, 3.5f);
                    this.damage = 5f;
                    break;
                //case ProjectileType.disque:
                //    this.vitesseBase = new Vector2(3.5f, 3.5f);
                //    this.damage = 10;
                //    break;
            }

            this.isEnnemi = isEnnemi;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe Obus.
        /// </summary>
        /// <param name="position">Position du sprite.</param>
        /// <param name="vitesses">Vitesses (horizontale et verticale) de propultion de l'obus.</param>
        public Obus(Vector2 position, ProjectileType type, float angle = 0f, bool isEnnemi = false)
            : this(position.X, position.Y, type, angle, isEnnemi)
        {
        }

        /// <summary>
        /// Accesseur et mutateur (attribut vitessesPropulsion) retournant ou changeant les vitesses donnés à l'obus lors du tir.
        /// </summary>
        /// <value>Vitesses de la source de l'obus lors du tir.</value>
        public Vector2 VitessesPropulsion
        {
            get { return this.vitessesPropulsion; }
            set { this.vitessesPropulsion = value; }
        }

        /// <summary>
        /// Accesseur et mutateur (attribut source) retournant ou changeant la source du tir de l'obus.
        /// </summary>
        /// <value>Vitesses de la source de l'obus lors du tir.</value>
        public Sprite Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        /// <summary>
        /// On doit surcharger l'accesseur texture en conséquence (toute classe à instancier dérivée 
        /// de Sprite doit surcharger cet accesseur).
        /// </summary>
        protected override Palette PaletteAnimation
        {
            get { return palettes[(int)projectileType]; }
        }

        /// <summary>
        /// Fonction membre chargeant les ressources associées au sprite.
        /// </summary>
        /// <param name="content">Gestionnaire de contenu permettant de charger les images du vaisseau.</param>
        /// <param name="graphics">Gestionanire de périphérique d'affichage permettant d'extraire
        /// les caractéristiques de celui-ci (p.ex. l'écran).</param>
        public static void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
        {
            //palettes.Add(new Palette(content.Load<Texture2D>(@"Projectiles\disque"), 25, 25));
            palettes.Add(new Palette(content.Load<Texture2D>(@"Projectiles\small-fire-shot"), 17, 17));
            palettes.Add(new Palette(content.Load<Texture2D>(@"Projectiles\blue-energy-ball"), 54, 53));
        }

        /// <summary>
        /// Fonction membre mettant à jour la position de l'obus en fonction de sa vitesse.
        /// </summary>
        /// <param name="gameTime">Indique le temps écoulé depuis la dernière invocation.</param>
        /// <param name="graphics">Gestionnaire de périphérique d'affichage.</param>
        public override void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            Vector2 newNormAngl = Vector2FromAngle(this.angle);

            this.VitessesPropulsion = newNormAngl;

            if (isEnnemi)
            {
                this.VitessesPropulsion *= -1f;
            }

            VitessesPropulsion /= vitesseBase;

            // Déplacer le vaisseau en fonction des vitesses latérales et frontales
            this.Position = new Vector2(
                this.Position.X + (gameTime.ElapsedGameTime.Milliseconds * (this.vitessesPropulsion.X)),
                this.Position.Y + (gameTime.ElapsedGameTime.Milliseconds * (this.vitessesPropulsion.Y)));

            base.Update(gameTime, graphics);
        }

        public static Vector2 Vector2FromAngle(double angle, bool normalize = true)
        {
            Vector2 vector = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            if (vector != Vector2.Zero && normalize)
                vector.Normalize();
            return vector;
        }
    }
}
