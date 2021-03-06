﻿// <copyright file="JoueurSprite.cs" company="Marco Lavoie">
//-----------------------------------------------------------------------
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

namespace projetJeu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using IFM20884;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using projetJeu.Managers;

    /// <summary>
    /// Classe implantant le sprite représentant le vaisseau contrôlé par le joueur. Cette
    /// classe gère trois textures statiques (i.e. partagées par toutes les instances de
    /// la classe) permettant d'afficher le vaisseau dans ses trois différentes positions
    /// (déplacement vers l'avant, vers la gauche et vers la droite).
    /// </summary>
    public class JoueurSprite : Sprite
    {
        public delegate void Shoot(Obus obus, bool isPlayer = true);

        private ProjectileType projectileType = ProjectileType.smallFireShot;
        public ProjectileType ProjectileType
        {
            get { return projectileType; }
            set { projectileType = value; }
        }

        private int projectileCount = 1;
        public int ProjectileCount
        {
            get { return this.projectileCount; }
            set { this.projectileCount = value; }
        }

        private bool isRespawned = false;

        public bool IsRespawned
        {
            get { return isRespawned; }
            set { isRespawned = value; }
        }

        private float projectileAngle = 0.0f;

        private Texture2D previousTexture;

        private Vector2 positionInitiale;
        public Vector2 PositionInitiale {
            get { return this.positionInitiale; }
            set { this.positionInitiale = value; }
        }

        private float health;

        private int nbVies = 1;

        public int NbVies
        {
            get { return nbVies; }
            set { nbVies = value; }
        }

        public float Health
        {
            get { return health; }
            set { health = value; }
        }

        private float maxHealth;

        /// <summary>
        /// Effet sonore contenant le bruitage des moteurs.
        /// </summary>
        private static SoundEffect moteurs;

        /// <summary>
        /// Attribut statique contenant la texture du vaisseau lorsqu'il se déplace vers l'avant.
        /// </summary>
        private static Texture2D vaisseauAvant;

        /// <summary>
        /// Attribut statique contenant la texture du vaisseau lorsqu'il se déplace vers la gauche.
        /// </summary>
        private static Texture2D vaisseauGauche;

        /// <summary>
        /// Attribut statique contenant la texture du vaisseau lorsqu'il se déplace vers la droite.
        /// </summary>
        private static Texture2D vaisseauDroite;

        /// <summary>
        /// L'attribut vaisseau va pointer à l'une des trois textures statiques selon la direction latérale
        /// du vaisseau (voir Update). On doit surcharger l'accesseur Texture en conséquence (toute classe
        /// à instancier dérivée de Sprite doit surcharger cet accesseur).
        /// </summary>
        private Texture2D vaisseau;

        /// <summary>
        /// Attribut contrôlant la vitesse de déplacement latérale du sprite du joueur.
        /// Cette vitesse varie selon le temps afin de simuler l'accélération et la
        /// décélération (voir fonction Update).
        /// </summary>
        private float vitesseLaterale = 0.0f;

        /// <summary>
        /// Attribut contrôlant la vitesse de déplacement vertical du sprite du joueur.
        /// Cette vitesse varie selon le temps afin de simuler l'accélération et la
        /// décélération (voir fonction Update).
        /// </summary>
        private float vitesseFrontale = 0.0f;

        /// <summary>
        /// Attribut indiquant l'index du périphérique contrôlant le sprite (voir
        /// dans Update (1 par défaut).
        /// </summary>
        private int indexPeripherique = 1;

        /// <summary>
        /// Instance de bruitage des moteurs en cours de sonorisation durant le jeu.
        /// </summary>
        private SoundEffectInstance moteursActif;

        private static SoundEffect bulletShoot;

        /// <summary>
        /// Constructeur paramétré recevant la position du sprite.
        /// </summary>
        /// <param name="x">Coordonnée initiale x (horizontale) du sprite.</param>
        /// <param name="y">Coordonnée initiale y (verticale) du sprite.</param>
        public JoueurSprite(float x, float y)
            : base(x, y)
        {
            this.vaisseau = vaisseauAvant;  // par défaut, c'est le vaisseau avant qui est affiché

            // Sélectionner et paramétrer le bruitage de fond.
            this.moteursActif = moteurs.CreateInstance();
            this.moteursActif.Volume = 0.0f;
            this.moteursActif.IsLooped = true;

            this.Health = 20f;
            this.maxHealth = this.Health;

            this.PositionInitiale = new Vector2(x, y);
        }

        /// <summary>
        /// Constructeur paramétré recevant la position du sprite. On invoque l'autre constructeur.
        /// </summary>
        /// <param name="position">Coordonnées initiales horizontale et verticale du sprite.</param>
        public JoueurSprite(Vector2 position)
            : this(position.X, position.Y)
        {
        }

        /// <summary>
        /// On doit surcharger l'accesseur texture en conséquence (toute classe à instancier dérivée 
        /// de Sprite doit surcharger cet accesseur).
        /// </summary>
        public override Texture2D Texture
        {
            get { return this.vaisseau; }
        }

        private Shoot shootObus;

        public Shoot ShootObus
        {
            get { return shootObus; }
            set { this.shootObus = value; }
        }

        /// <summary>
        /// Propriété contrôlant la vitesse de déplacement latérale du sprite du joueur.
        /// Cette vitesse varie selon le temps afin de simuler l'accélération et la
        /// décélération (voir fonction Update).
        /// </summary>
        public float VitesseLaterale
        {
            get { return this.vitesseLaterale; }
            set { this.vitesseLaterale = value; }
        }

        /// <summary>
        /// Propriété contrôlant la vitesse de déplacement vertical du sprite du joueur.
        /// Cette vitesse varie selon le temps afin de simuler l'accélération et la
        /// décélération (voir fonction Update).
        /// </summary>
        public float VitesseFrontale
        {
            get { return this.vitesseFrontale; }
            set { this.vitesseFrontale = value; }
        }

        /// <summary>
        /// Propriété indiquant l'index du périphérique contrôlant le sprite (1 à 4).
        /// </summary>
        public int IndexPeripherique
        {
            get { return this.indexPeripherique; }
            set { this.indexPeripherique = value; }
        }

        /// <summary>
        /// Fonction membre chargeant les ressources associées au sprite. Nous chargeons les trois textures du vaisseeau.
        /// </summary>
        /// <param name="content">Gestionnaire de contenu permettant de charger les images du vaisseau.</param>
        /// <param name="graphics">Gestionanire de périphérique d'affichage permettant d'extraire
        /// les caractéristiques de celui-ci (p.ex. l'écran).</param>
        public static void LoadContent(ContentManager content, GraphicsDeviceManager graphics)
        {
            // Charger les trois textures du vaisseau.
            vaisseauAvant = content.Load<Texture2D>(@"Vaisseau\shipSprite");
            vaisseauGauche = content.Load<Texture2D>(@"Vaisseau\shipLeft");
            vaisseauDroite = content.Load<Texture2D>(@"Vaisseau\shipRight");

            // Charger le bruitage des moteurs.
            moteurs = content.Load<SoundEffect>(@"Pipeline\SoundFX\misc291");
            bulletShoot = content.Load<SoundEffect>(@"Pipeline\SoundFX\bulletshoot");
        }

        private const float _invulnDelay = 3;
        private float _remainingInvulnDelay = _invulnDelay;
        private const float _blinkDelay = 0.1f;
        private float _remainingBlinkDelay = _blinkDelay;

        /// <summary>
        /// Fonction membre abstraite mettant à jour le sprite selon les touches de clavier pressées.
        /// Les déplacement tiennent compte de l'accélération (au début du mouvement) et de l'inertie (à
        /// la fin du mouvement).
        /// </summary>
        /// <param name="gameTime">Gestionnaire de temps de jeu.</param>
        /// <param name="graphics">Gestionnaire de périphérique d'affichage.</param>
        public override void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            const float FacteurAcceleration = 0.02f;     // facteur d'accélération et de décélération
            const float VitesseMaximale = 0.4f;          // vitesse latérale et frontale maximale

            this.projectileAngle = 0f;

            float delayTimer = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsRespawned)
            {
                if (this.Health != this.maxHealth)
                {
                    this.Health = this.maxHealth;
                }

                _remainingBlinkDelay -= delayTimer;
                if (_remainingBlinkDelay <= 0)
                {
                    _remainingBlinkDelay = _blinkDelay;
                    if (this.vaisseau == GameManager.invisiblePixel)
                    {
                        this.vaisseau = previousTexture;
                    }
                    else
                    {
                        previousTexture = this.vaisseau;
                        this.vaisseau = GameManager.invisiblePixel;
                    }
                }

                _remainingInvulnDelay -= delayTimer;
                if (_remainingInvulnDelay <= 0)
                {
                    _remainingInvulnDelay = _invulnDelay;
                    IsRespawned = false;
                }
            }

            // Changer le vaisseau affiché et ajuster sa position horizontale selon la touche pressée
            // (en tenant compte de l'accélération / décélération)
            float facteurInputs = ServiceHelper.Get<IInputService>().DeplacementDroite(this.IndexPeripherique) -
                                  ServiceHelper.Get<IInputService>().DeplacementGauche(this.IndexPeripherique);
            if (facteurInputs < 0.0f)
            {
                this.vitesseLaterale = Math.Max(this.vitesseLaterale - FacteurAcceleration, -VitesseMaximale);
            }
            else if (facteurInputs > 0.0f)
            {
                this.vitesseLaterale = Math.Min(this.vitesseLaterale + FacteurAcceleration, VitesseMaximale);
            }
            else
            {
                if (!IsRespawned)
                {
                    this.vaisseau = vaisseauAvant;
                }
                else
                {
                    this.previousTexture = vaisseauAvant;
                }

                // Décélération latérale au besoin
                if (Math.Abs(this.vitesseLaterale) >= FacteurAcceleration)
                {
                    this.vitesseLaterale -= Math.Sign(this.vitesseLaterale) * FacteurAcceleration;
                }
                else
                {
                    this.vitesseLaterale = 0.0f;
                }
            }

            // Changer la position verticale selon la touche pressée (en tenant compte de l'accélération / décélération)
            facteurInputs = ServiceHelper.Get<IInputService>().DeplacementAvant(this.IndexPeripherique) -
                            ServiceHelper.Get<IInputService>().DeplacementArriere(this.IndexPeripherique);
            if (facteurInputs > 0.0f)
            {
                if (this.vitesseLaterale >= 0.0f)
                {
                    if (!IsRespawned)
                    {
                        this.vaisseau = vaisseauGauche;
                    }
                    else
                    {
                        this.previousTexture = vaisseauGauche;
                    }
                    this.projectileAngle = -MathHelper.ToRadians(35f);
                }
                else
                {
                    if (!IsRespawned)
                    {
                        this.vaisseau = vaisseauDroite;
                    }
                    else
                    {
                        this.previousTexture = vaisseauDroite;
                    }
                    this.projectileAngle = MathHelper.ToRadians(35f);
                }

                this.vitesseFrontale = Math.Max(this.vitesseFrontale - FacteurAcceleration, -VitesseMaximale);
            }
            else if (facteurInputs < 0.0f)
            {
                if (this.vitesseLaterale >= 0.0f)
                {
                    if (!IsRespawned)
                    {
                        this.vaisseau = vaisseauDroite;
                    }
                    else
                    {
                        this.previousTexture = vaisseauDroite;
                    }
                    this.projectileAngle = MathHelper.ToRadians(35f);
                }
                else
                {
                    if (!IsRespawned)
                    {
                        this.vaisseau = vaisseauGauche;
                    }
                    else
                    {
                        this.previousTexture = vaisseauGauche;
                    }
                    this.projectileAngle = -MathHelper.ToRadians(35f);
                }

                this.vitesseFrontale = Math.Min(this.vitesseFrontale + FacteurAcceleration, VitesseMaximale);
            }
            else
            {
                if (!IsRespawned)
                {
                    this.vaisseau = vaisseauAvant;
                }
                else
                {
                    this.previousTexture = vaisseauAvant;
                }
                // Décélération frontale au besoin
                if (Math.Abs(this.vitesseFrontale) >= FacteurAcceleration)
                {
                    this.vitesseFrontale -= Math.Sign(this.vitesseFrontale) * FacteurAcceleration;
                }
                else
                {
                    this.vitesseFrontale = 0.0f;
                }
            }

            // Activer les effets sonores associés aux moteurs lorsque ceux-ci sont actifs.
            if (this.vitesseFrontale != 0.0f || this.vitesseLaterale != 0.0f)
            {
                if (this.moteursActif.State != SoundState.Playing)
                {
                    this.moteursActif.Play();
                }
            }
            else
            {
                if (this.moteursActif.State != SoundState.Paused)
                {
                    this.moteursActif.Pause();
                }
            }

            // Si l'effet sonore associé aux moteurs est actif, ajuster le volume
            // en fonction de la vitesse de réplacement (le volume est ainsi
            // relatif aux accélérations et décélérations).
            if (this.moteursActif.State == SoundState.Playing)
            {
                this.moteursActif.Volume = Math.Max(Math.Abs(this.vitesseFrontale), Math.Abs(this.vitesseLaterale)) / VitesseMaximale;
            }

            int changeProjectileCount = ServiceHelper.Get<IInputService>().SetProjectileCount(this.IndexPeripherique);
            if (changeProjectileCount != 0)
            {
                this.projectileCount = changeProjectileCount;
            }

            bool changeProjectile = ServiceHelper.Get<IInputService>().SetProjectile(this.IndexPeripherique);
            if (changeProjectile)
            {
                this.projectileType = (ProjectileType)(((int)this.projectileType + 1) % ((int)ProjectileType.blueEnergyBall + 1));
            }

            if (!this.IsRespawned && ServiceHelper.Get<IInputService>().Shoot(this.IndexPeripherique, this.projectileType))
            {

                Vector2 pos;

                if (projectileCount == 1 || projectileCount == 3)
                {
                    pos = new Vector2(this.Position.X + (this.Width / 2), this.Position.Y);
                }
                else
                {
                    pos = new Vector2(this.Position.X, this.Position.Y + this.Height / 2);
                }

                bulletShoot.Play(0.025f, 0f, 0f);

                Obus obus = new Obus(pos, this.projectileType, this.projectileAngle);
                obus.Source = this;
                this.shootObus(obus);

                if (projectileCount == 2)
                {
                    pos = new Vector2(this.Position.X, this.Position.Y - this.Height / 2);
                    obus = new Obus(pos, this.projectileType, this.projectileAngle);
                    obus.Source = this;
                    this.shootObus(obus);
                }
                else if (projectileCount == 3)
                {
                    pos = new Vector2(this.Position.X, this.Position.Y + this.Height / 2);
                    obus = new Obus(pos, this.projectileType, MathHelper.ToRadians(35f) + projectileAngle);
                    obus.Source = this;
                    this.shootObus(obus);

                    pos = new Vector2(this.Position.X, this.Position.Y - this.Height / 2);
                    obus = new Obus(pos, this.projectileType, -MathHelper.ToRadians(35f) + projectileAngle);
                    obus.Source = this;
                    this.shootObus(obus);
                }
            }

            // Déplacer le vaisseau en fonction des vitesses latérales et frontales
            this.Position = new Vector2(
                this.Position.X + (gameTime.ElapsedGameTime.Milliseconds * this.vitesseLaterale),
                this.Position.Y + (gameTime.ElapsedGameTime.Milliseconds * this.vitesseFrontale));
        }

        /// <summary>
        /// Suspend temporairement (pause) ou réactive les effets sonores du vaisseau.
        /// </summary>
        /// <param name="suspendre">Indique si les effets sonores doivent être suspendus ou réactivés.</param>
        public void SuspendreEffetsSonores(bool suspendre)
        {
            if (suspendre)
            {
                // Suspendre au besoin les effets sonores associés aux moteurs
                if (this.moteursActif.State == SoundState.Playing)
                {
                    this.moteursActif.Pause();
                }
            }
            else
            {
                // Réactiver au besoin les effets sonores associés aux moteurs
                if (this.moteursActif.State == SoundState.Paused)
                {
                    this.moteursActif.Play();
                }
            }
        }

        public void DrawHealth(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                GameManager.redPixel,
                position: new Vector2(this.Position.X - vaisseauAvant.Width / 2f, this.Position.Y - vaisseauAvant.Height / 2f - vaisseauAvant.Height / 6f),
                scale: new Vector2(vaisseauAvant.Width, 5f),
                color: Color.White);

            spriteBatch.Draw(
                GameManager.greenPixel,
                position: new Vector2(this.Position.X - vaisseauAvant.Width / 2f, this.Position.Y - vaisseauAvant.Height / 2f - vaisseauAvant.Height / 6f),
                scale: new Vector2(vaisseauAvant.Width / (this.maxHealth / this.health), 5f),
                color: Color.White);
        }
    }
}
