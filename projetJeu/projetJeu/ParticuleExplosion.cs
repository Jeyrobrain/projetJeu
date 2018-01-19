//-----------------------------------------------------------------------
// <copyright file="ParticuleExplosion.cs" company="Marco Lavoie">
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
    /// Classe représentant une particule d'explosion. Les particularités suivantes de la
    /// particule sont contrôlées de façon aléatoire
    ///   - Échelle d'affichage (appliquée dans Draw())
    ///   - Angle d'affichage (appliquée dans Draw())
    ///   - Facteur d'expansion
    ///   - Angle de dispersion
    ///   - Distance de dispersion
    /// </summary>
    public class ParticuleExplosion : Sprite
    {
        /// <summary>
        /// Délai d'attente avant d'altérer la transparence de l'image.
        /// </summary>
        private const double FadeDelai = 0.035;

        /// <summary>
        /// Générateur de nombres aléatoires exploité par les membres de la classe pour
        /// paramétrer aléatoirement l'instance.
        /// </summary>
        private static Random randomiseur = new Random();

        /// <summary>
        /// Attribut statique contenant la texture de la particule d'explosion. À noter que
        /// la classe PatriculeExplosion ne charge pas elle-même sa texture, mais reçoit 
        /// plutôt celle-ci via ses constructeurs. Ainsi, la classe ParticuleExplosion peut
        /// être utilisée avec différentes textures afin de représenter dans un jeu 
        /// différents types d'explosions (c.à.d. qu'on a pas à définir une classe dérivée
        /// de ParticuleExplosion pour chaque type d'explosion).
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// Échelle maximale d'affichage de l'image. La fonction Draw() applique une échelle 
        /// d'affichage sur la texture afin de contrôler sa taille d'affichage, et la fonction
        /// Update() change cette échelle d'affichage afin de simuler une croissantce de la
        /// taille de la particule durant son existance.
        /// </summary>
        private float maxEchelle;

        /// <summary>
        /// Échelle courante d'affichage de l'image. La fonction Draw() applique une échelle 
        /// d'affichage sur la texture afin de contrôler sa taille d'affichage, et la fonction
        /// Update() change cette échelle d'affichage afin de simuler une croissantce de la
        /// taille de la particule durant son existance. La valeur maximale de cet attribut
        /// est donnée par maxEchelle.
        /// </summary>
        private float echelle;

        /// <summary>
        /// Angle de rotation d'affichage de l'image. La fonction Draw() applique cette rotation 
        /// d'affichage sur la texture.
        /// </summary>
        private float rotation;

        /// <summary>
        /// Vitesse de rotation d'affichage de l'image. La fonction Update() exploite cet attribut 
        /// pour modifier l'attribut rotation.
        /// </summary>
        private float rotationVitesse;

        /// <summary>
        /// Facteur d'expansion courant de la particule. Sa valeur est modifiée dans Update(), qui
        /// l'exploite pour déplacer et redimensionner la particule en fonction du temps écoulé.
        /// </summary> 
        private float expansion;

        /// <summary>
        /// Angle de déplacement de la particule en rapport au centre de l'explosion.
        /// </summary>
        private float dispersionAngle;

        /// <summary>
        /// Distance maximale de déplacement de la particule (à cause de la dispersion) en 
        /// rapport au centre de l'explosion.
        /// </summary>
        private float dispersionDistanceMax;

        /// <summary>
        /// Déplacement en X et Y de la particule (à cause de la dispersion) en rapport au 
        /// centre de l'explosion.
        /// </summary>
        private Vector2 dispersionDelta;

        /// <summary>
        /// Vitesse de déplacement verticale du sprite
        /// </summary>
        private float vitesseDeplacement;

        /// <summary>
        /// Valeur Alpha (variant de 255 à 0) permettant de faire disparaître graduellement
        /// la particule à l'écran.
        /// </summary>
        private int fadeAlpha = 255;

        /// <summary>
        /// Facteur de décrémentation de fadeAlpha, appliqué à cette dernière à toutes
        /// les fadeDelai secondes.
        /// </summary>
        private int fadeDelta = 25;

        /// <summary>
        /// Contrôle de la vitesse d'effacement de l'image à l'écran. La valeur de fadeAlpha
        /// est décrémentée de fadeDelta à toutes les fadeDelai secondes.
        /// </summary>
        private double fadeDelai = FadeDelai;

        /// <summary>
        /// Initialise une nouvelle instance de la classe.
        /// </summary>
        /// <param name="x">Position en x du sprite.</param>
        /// <param name="y">Position en y du sprite.</param>
        /// <param name="texture">Image pour le sprite.</param>
        /// <param name="vitesse">Vitesse de déplacement vertical du sprite.</param>
        public ParticuleExplosion(float x, float y, Texture2D texture, float vitesse)
            : base(x, y)
        {
            // Initialiser la texture représentant la particule.
            this.texture = texture;

            // Initialiser la vitesse de déplacement vertical. Cette vitesse devrait correspondre
            // à l'objet à la source de l'explosion (e.g. un astéroïde se déplaçant).
            this.vitesseDeplacement = vitesse;

            // Paramétriser aléatoirement les attributs de l'instance.
            this.maxEchelle = 0.10f + ((float)randomiseur.NextDouble() * 0.20f);          // échelle maximale entre 0.1 et 0.3
            this.echelle = 0.005f + ((float)randomiseur.NextDouble() * 0.01f);            // échelle de départ entre 0.005 et 0.015

            // Angle de départ choisie aléatoirement entre 0 et 360 degrés.
            this.rotation = (float)randomiseur.NextDouble() * (2f * (float)Math.PI);

            // Déterminer le sens de la rotation au hasard, -1 ou +1. La probabilité d'avoir 0 est faible.
            int sensRotation = Math.Sign(randomiseur.NextDouble() - 0.5f);

            // La vitesse de rotation maximale est 1/(3 * 60 updates) x un tour complet.
            float deltaRotation = (float)(randomiseur.NextDouble() / 180f);

            // On ajuste la vitesse de rotation de sorte qu'elle applique une rotation maximale au 3 secondes.
            this.rotationVitesse = sensRotation * deltaRotation * (2f * (float)Math.PI);

            this.expansion = 0.0025f + ((float)randomiseur.NextDouble() * 0.005f);        // vitesse d'expansion entre 0.0025 et 0.0075
            this.dispersionAngle = (float)(randomiseur.NextDouble() * 2f * Math.PI);      // angle de déplacement de la particule (entre 0 et 360 degrés, en radians)
            this.dispersionDistanceMax = 30f;                                             // expansion maximale de la particule
            this.dispersionDelta = Vector2.Zero;                                          // expansion courante de la particule
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe.
        /// On invoque l'autre constructeur.
        /// </summary>
        /// <param name="position">Position du sprite.</param>
        /// <param name="texture">Image pour le sprite.</param>
        /// <param name="vitesse">Vitesse de déplacement vertical du sprite.</param>
        public ParticuleExplosion(Vector2 position, Texture2D texture, float vitesse)
            : this(position.X, position.Y, texture, vitesse)
        {
        }

        /// <summary>
        /// On doit surcharger l'accesseur texture en conséquence (toute classe à instancier dérivée 
        /// de Sprite doit surcharger cet accesseur).
        /// </summary>
        public override Texture2D Texture
        {
            get { return this.texture; }
        }

        /// <summary>
        /// Propriété (accesseur pour maxEchelle) retournant ou changeant l`échelle maximale 
        /// d'expansion de la particule.
        /// </summary>
        /// <value>État courant du jeu.</value>
        public float MaxEchelle
        {
            get { return this.maxEchelle; }
            set { this.maxEchelle = value; }
        }

        /// <summary>
        /// Indique si la particule est visible à l'écran (en fonction de son canal alpha). Lorsque
        /// la particule est complètement transparente, elle est considérée invisible.
        /// </summary>
        /// <param name="spriteBatch">Tampon d'affichage de sprites.</param>
        public bool Visible
        {
            get { return this.fadeAlpha > 0; }
        }

        /// <summary>
        /// Propriété (accesseur pour vitesseDeplacement) retournant ou changeant la vitesse 
        /// de déplacement verticale de la particule.
        /// </summary>
        /// <value>Vitesse de déplacement vertical de la particule.</value>
        public float VitesseDeplacement
        {
            get { return this.vitesseDeplacement; }
            set { this.vitesseDeplacement = value; }
        }

        /// <summary>
        /// Fonction membre mettant à jour la position, la taille et la transparence de l'image
        /// en fonction du temps écoulé et des attributs de l'instance. La particule se déplace
        /// graduellement jusqu'à son expansion maximale, après quoi un facteur de transparence
        /// est graduellement augmenté afin de faire disparaître l'image de la particule à l'écran.
        /// </summary>
        /// <param name="gameTime">Indique le temps écoulé depuis la dernière invocation.</param>
        /// <param name="graphics">Gestionnaire de périphérique d'affichage.</param>
        public override void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            // Premièrement décaler la particule vers le bas en fonction de sa vitesse.
            this.Position = new Vector2(Position.X - (gameTime.ElapsedGameTime.Milliseconds * this.vitesseDeplacement), Position.Y);

            // Mettre à jour la rotation de la texture.
            this.rotation += this.rotationVitesse;

            // Recalculer l'échelle d'affichage de l'image en fonction du facteur d'expansion courant, puis
            // réduire ce facteur de 25%.
            this.echelle = (float)Math.Min(this.echelle + (gameTime.ElapsedGameTime.Milliseconds * this.expansion), this.maxEchelle);
            this.expansion *= 0.75f;

            // Calculer le déplacement en X et en Y de la particule en fonction de son angle d'expansion et
            // de sa vitesse d'expansion.
            float ratio = this.echelle / this.maxEchelle;
            this.dispersionDelta.X = ratio * this.dispersionDistanceMax * (float)Math.Cos(this.dispersionAngle);
            this.dispersionDelta.Y = ratio * this.dispersionDistanceMax * (float)Math.Sin(this.dispersionAngle);

            // Si l'expansion est presque terminée, alors commencer à augmenter la transparence de l'image
            // afin que celle-ci disparaisse graduellement de l'écran.
            if (this.expansion < 0.001f)
            {
                // Décrémenter le délai selon le temps écoulé depuis l'invocation précédente de Update().
                this.fadeDelai -= gameTime.ElapsedGameTime.TotalSeconds;
                 
                // Si le délai est expiré, augmenter la transparence et réinitialiser le délai.
                if (this.fadeDelai <= 0)
                {
                    this.fadeDelai = FadeDelai;       // réinitialiser le délai

                    // Augmenter la transparence en diminuant le canal alpha appliqué à l'image dans Draw().
                    this.fadeAlpha = Math.Max(this.fadeAlpha - this.fadeDelta, 0);
                }
            }
        }

        /// <summary>
        /// Affiche à l'écran la particule (si elle est visible).
        /// </summary>
        /// <param name="camera">Caméra indiquant la partie du monde présentement visible à l'écran (peut être nulle).</param>
        /// <param name="spriteBatch">Tampon d'affichage de sprites.</param>
        public override void Draw(float angle, Camera camera, SpriteBatch spriteBatch, SpriteEffects effects = SpriteEffects.None)
        {
            if (this.Visible)
            {
                // Calculer le canal alpha à appliquer à la texture (permet à Update() de faire
                // graduellement diaparaître l'image à l'écran (fade out).
                Color fadeColor = new Color(255, 255, 255, (byte)MathHelper.Clamp(this.fadeAlpha, 0, 255));

                // Corriger la position d'affichage de la texture en fonction de l'expansion occasionnée
                // par l'explosion.
                Vector2 pos = new Vector2(this.Position.X + this.dispersionDelta.X, this.Position.Y + this.dispersionDelta.Y);

                // On doit travailler avec une copie de PositionRect car on va mapper ses coordonnées
                // en celles du monde si on nous a fourni une caméra.
                Rectangle destRect = this.PositionRect;

                // Si une caméra est fournie, on s'assure que le sprite y est visible.
                if (camera == null || camera.EstVisible(destRect))
                {
                    // Décaler la destination en fonction de la caméra avant de dessiner.
                    if (camera != null)
                    {
                        camera.Monde2Camera(ref destRect);
                    }

                    spriteBatch.Draw(this.Texture, pos, null, fadeColor, this.rotation, new Vector2(this.Texture.Width / 2, this.Texture.Height / 2), this.echelle, effects, 1);
                }
            }
        }
    }
}
