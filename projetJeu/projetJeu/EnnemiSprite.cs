//-----------------------------------------------------------------------
// <copyright file="EnnemiSprite.cs" company="Marco Lavoie">
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
// <summary>Implantation de la classe EnnemiSprite.</summary>
//-----------------------------------------------------------------------

namespace projetJeu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using IFM20884;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Classe implantant le sprite représentant un astéroïde en rotation.
    /// </summary>
    public class EnnemiSprite : Sprite
    {
        /// <summary>
        /// Attribut statique contenant la texture de l'astéroïde.
        /// </summary>
        private static Texture2D ennemi;

        /// <summary>
        /// Vitesse de déplacement verticale du sprite.
        /// </summary>
        private float vitesseDeplacement;

        /// <summary>
        /// Angle de rotation courante du sprite.
        /// </summary>
        private float angleRotation;

        /// <summary>
        /// Contrôle de vitesse de rotation du sprite.
        /// </summary>
        private float vitesseRotation;

        /// <summary>
        /// Contrôle d'échelle de dimensionnement du sprite.
        /// </summary>
        private float echelle;

        /// <summary>
        /// Initialise une nouvelle instance de la classe EnnemiSprite.
        /// </summary>
        /// <param name="x">Position en x du sprite.</param>
        /// <param name="y">Position en y du sprite.</param>
        public EnnemiSprite(float x, float y)
            : base(x, y)
        {
            this.Initialize();
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe EnnemiSprite.
        /// </summary>
        /// <param name="position">Position du sprite.</param>
        public EnnemiSprite(Vector2 position)
            : this(position.X, position.Y)
        {
        }

        /// <summary>
        /// On doit surcharger l'accesseur texture en conséquence (toute classe à instancier dérivée 
        /// de Sprite doit surcharger cet accesseur).
        /// </summary>
        public override Texture2D Texture
        {
            get { return ennemi; }
        }

        /// <summary>
        /// Propriété (accesseur pour vitesseDeplacement) retournant ou changeant la vitesse de déplacement 
        /// verticale du sprite.
        /// </summary>
        /// <value>Position du sprite.</value>
        public float VitesseDeplacement
        {
            get { return this.vitesseDeplacement; }
            set { this.vitesseDeplacement = value; }
        }

        /// <summary>
        /// Fonction membre chargeant les ressources associées au sprite. Nous chargeons la texture de l'astéroïde.
        /// </summary>
        /// <param name="content">Gestionnaire de contenu.</param>
        /// <param name="graphics">Gestionnaire de périphérique d'affichage.</param>
        public static void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
        {
            // Charger la texture de l'astéroïde.
            ennemi = content.Load<Texture2D>("Ennemi/WB_baseu3_d0.png");
        }

        /// <summary>
        /// Fonction d'initialisation du sprite. On y assigne des valeurs par défaut aux attributs membres.
        /// </summary>
        public void Initialize()
        {
            // Vitesse de déplacement vertical par défaut.
            this.vitesseDeplacement = 0.2f;

            // Angle initial de rotation (i.e. aucun)
            this.angleRotation = 0.0f;

            // Attribuer une vitesse de rotation aléatoire entre -0.002 et +0.002.
            Random random = new Random();
            this.vitesseRotation = -0.002f + (float)(random.NextDouble() * 0.004f);

            // Attribuer un facteur d'échelle aléatoire entre 0.5 et 2.0.
            //this.echelle = 0.5f + (float)(random.NextDouble() * 1.5f);
            this.echelle = 1f;
        }

        /// <summary>
        /// Fonction membre abstraite mettant à jour le sprite selon les touches de clavier pressées.
        /// La rotation est effectuée modulo 360 degrés.
        /// </summary>
        /// <param name="gameTime">Indique le temps écoulé depuis la dernière invocation.</param>
        /// <param name="graphics">Gestionnaire de périphérique d'affichage.</param>
        public override void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            // Décaler l'ennemi vers la gauche.
            this.Position = new Vector2(Position.X - (gameTime.ElapsedGameTime.Milliseconds * this.vitesseDeplacement), Position.Y);

            this.angleRotation += gameTime.ElapsedGameTime.Milliseconds * this.vitesseRotation;
            this.angleRotation %= MathHelper.Pi * 2;
        }

        /// <summary>
        /// Fonction membre à surcharger pour dessiner le sprite. Par défaut la texture est
        /// affichée, centrée à Position.
        /// </summary>
        /// <param name="camera">Caméra indiquant la partie du monde présentement visible à l'écran.</param>
        /// <param name="spriteBatch">Tampon d'affichage de sprites.</param>
        public override void Draw(float angle, Camera camera, SpriteBatch spriteBatch)
        {
            // Le point autour duquel apliquer la rotation est le centre de la texture.
            Vector2 origine = new Vector2(this.Width / 2, this.Height / 2);

            spriteBatch.Draw(this.Texture, this.Position, null, Color.White, this.angleRotation, origine, this.echelle, SpriteEffects.None, 0f);
            //spriteBatch.Draw(this.Texture, destinationRectangle: base.PositionRect, rotation: this.angleRotation);
        }
    }
}
