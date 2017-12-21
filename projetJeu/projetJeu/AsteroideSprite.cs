//-----------------------------------------------------------------------
// <copyright file="AsteroideSprite.cs" company="Marco Lavoie">
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
// <summary>Implantation de la classe AsteroideSprite.</summary>
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
    public class AsteroideSprite : SpriteAnimation
    {
        /// <summary>
        /// Attribut statique contenant la palette d'animation de l'astéroïde composé d'argent.
        /// </summary>
        private static Palette paletteAg;

        /// <summary>
        /// Attribut statique contenant la palette d'animation de l'astéroïde composé de fer.
        /// </summary>
        private static Palette paletteFe;

        /// <summary>
        /// Attribut statique contenant la palette d'animation de l'astéroïde composé de magnésium.
        /// </summary>
        private static Palette paletteMg;

        /// <summary>
        /// Attribut statique fournissant un générateur de nombres aléatoires commun à toutes
        /// les instances.
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// Vitesse de déplacement verticale du sprite.
        /// </summary>
        private float vitesseDeplacement;

        /// <summary>
        /// Attribut indiquant le matériau de composition de l'astéroïde.
        /// </summary>
        private Composition materiau;

        /// <summary>
        /// Initialise une nouvelle instance de la classe AsteroideSprite.
        /// </summary>
        /// <param name="x">Position en x du sprite.</param>
        /// <param name="y">Position en y du sprite.</param>
        public AsteroideSprite(float x, float y)
            : base(x, y)
        {
            this.vitesseDeplacement = 0.2f;     // vitesse de déplacement vertical par défaut
            this.VitesseAnimation = 0.07f;      // vitesse d'animation pour fluidité

            // Choisir le matériau au hasard.
            switch (random.Next(3))
            {
                case 0:
                    this.materiau = Composition.Argent;
                    break;
                case 1:
                    this.materiau = Composition.Fer;
                    break;
                default:
                    this.materiau = Composition.Magnesium;
                    break;
            }
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe AsteroideSprite.
        /// </summary>
        /// <param name="position">Position du sprite.</param>
        public AsteroideSprite(Vector2 position)
            : this(position.X, position.Y)
        {
        }

        /// <summary>
        /// Enumération des composition d'astéroïdes disponibles.
        /// </summary>
        public enum Composition
        {
            /// <summary>
            /// Astéroïde composé principalement d'argent.
            /// </summary>
            Argent,

            /// <summary>
            /// Astéroïde composé principalement de fer.
            /// </summary>
            Fer,

            /// <summary>
            /// Astéroïde composé principalement de magnésium.
            /// </summary>
            Magnesium
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
        /// On doit surcharger l'accesseur PaletteAnimation en conséquence (toute classe à instancier dérivée 
        /// de Sprite doit surcharger cet accesseur).
        /// </summary>
        protected override Palette PaletteAnimation
        {
            get
            {
                switch (this.materiau)
                {
                    case Composition.Argent:
                        return paletteAg;

                    case Composition.Fer:
                        return paletteFe;

                    default:
                        return paletteMg;
                }
            }
        }

        /// <summary>
        /// Fonction membre chargeant les ressources associées au sprite. Nous chargeons la palette
        /// d'animation de l'astéroïde.
        /// </summary>
        /// <param name="content">Gestionnaire de contenu.</param>
        /// <param name="graphics">Gestionnaire de périphérique d'affichage.</param>
        public static void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
        {
            // Créer la palette d'animation des trois types d'astéroïde.
            paletteAg = new Palette(content.Load<Texture2D>("Asteroides\\RockAgSheet"), 64, 64);
            paletteFe = new Palette(content.Load<Texture2D>("Asteroides\\RockFeSheet"), 64, 64);
            paletteMg = new Palette(content.Load<Texture2D>("Asteroides\\RockMgSheet"), 64, 64);
        }

        /// <summary>
        /// Fonction membre mettant à jour la position du sprite.
        /// </summary>
        /// <param name="gameTime">Indique le temps écoulé depuis la dernière invocation.</param>
        /// <param name="graphics">Gestionnaire de périphérique d'affichage.</param>
        public override void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            // Déplacer l'astériode vers le bas en fonction de sa vitesse.
            this.Position = new Vector2(this.Position.X, this.Position.Y + (gameTime.ElapsedGameTime.Milliseconds * this.vitesseDeplacement));

            // La classe de base gère l'animation.
            base.Update(gameTime, graphics);
        }
    }
}
