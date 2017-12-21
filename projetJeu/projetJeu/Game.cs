//-----------------------------------------------------------------------
// <copyright file="Game.cs" company="Marco Lavoie">
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

    using IFM20884;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Classe principale du jeu.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Attribut permettant d'obtenir des infos sur la carte graphique et l'écran.
        /// </summary>
        private GraphicsDeviceManager graphics;

        /// <summary>
        /// Attribut gérant l'affichage en batch à l'écran.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Attribut représentant le vaisseau contrôlé par le joueur.
        /// </summary>
        private JoueurSprite vaisseauJoueur;

        /// <summary>
        /// Attribut représentant l'arrière plan à défilement vertical du du jeu.
        /// </summary>
        private DefilementArrierePlan arrierePlan;

        /// <summary>
        /// Attribut représentant la camera.
        /// </summary>
        private Camera camera;

        /// <summary>
        /// Liste des sprites représentant des astéroïdes.
        /// </summary>
        private List<Sprite> listeAsteroides;

        /// <summary>
        /// Générateur de nombres aléatoires pour générer des astéroïdes.
        /// </summary>
        private Random randomAsteroides;

        /// <summary>
        /// Probabilité de générer un astéroïde par cycle de Update().
        /// </summary>
        private float probAsteroides;

        /// <summary>
        /// Constructeur par défaut de la classe. Cette classe est générée automatiquement
        /// par Visual Studio lors de la création du projet.
        /// </summary>
        public Game()
        {
            this.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Permet au jeu d'effectuer toute initialisation avant de commencer à jouer.
        /// Cette fonction membre peut demander les services requis et charger tout contenu
        /// non graphique pertinent. L'invocation de base.Initialize() itèrera parmi les
        /// composants afin de les initialiser individuellement.
        /// </summary>
        protected override void Initialize()
        {
            // Activer le service de gestion du clavier
            ServiceHelper.Game = this;
            this.Components.Add(new ClavierService(this));

            // Initialiser la vue de la caméra à la taille de l'écran.
            this.camera = new Camera(new Rectangle(0, 0, this.graphics.GraphicsDevice.Viewport.Width, this.graphics.GraphicsDevice.Viewport.Height));

            // Créer les attributs de gestion des astéroïdes.
            this.listeAsteroides = new List<Sprite>();
            this.randomAsteroides = new Random();
            this.probAsteroides = 0.01f;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent est invoquée une seule fois par partie et permet de
        /// charger tous vos composants.
        /// </summary>
        protected override void LoadContent()
        {
            // Créer un nouveau SpriteBatch, utilisée pour dessiner les textures.
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            // Charger les sprites.
            JoueurSprite.LoadContent(this.Content, this.graphics);
            ArrierePlanEspace.LoadContent(this.Content, this.graphics);
            AsteroideSprite.LoadContent(this.Content, this.graphics);

            // Créer les sprites du jeu. Premièrement le sprite du joueur centrer au bas de l'écran. On limite ensuite
            // ses déplacements à l'écran.
            this.vaisseauJoueur = new JoueurSprite(this.graphics.GraphicsDevice.Viewport.Width / 2f, this.graphics.GraphicsDevice.Viewport.Height * 0.85f);
            this.vaisseauJoueur.BoundsRect = new Rectangle(0, 0, this.graphics.GraphicsDevice.Viewport.Width, this.graphics.GraphicsDevice.Viewport.Height);

            // Créer ensuite le sprite représentant l'arrière-plan.
            this.arrierePlan = new ArrierePlanEspace(this.graphics);
        }

        /// <summary>
        /// UnloadContent est invoquée une seule fois par partie et permet de
        /// libérer vos composants.
        /// </summary>
        protected override void UnloadContent()
        {
            // À FAIRE: Libérez tout contenu de ContentManager ici
        }

        /// <summary>
        /// Permet d'implanter les comportements logiques du jeu tels que
        /// la mise à jour du monde, la détection de collisions, la lecture d'entrées
        /// et les effets audio.
        /// </summary>
        /// <param name="gameTime">Fournie un instantané du temps de jeu.</param>
        protected override void Update(GameTime gameTime)
        {
            // Permettre de quitter le jeu via le service d'input.
            if (ServiceHelper.Get<IInputService>().Quitter(1))
            {
                this.Exit();
            }

            // Mettre à joueur les sprites du jeu
            this.vaisseauJoueur.Update(gameTime, this.graphics);
            this.arrierePlan.Update(gameTime, this.graphics);

            // Identifier les astéroïdes ayant quitté l'écran.
            List<AsteroideSprite> asteroidesFini = new List<AsteroideSprite>();
            foreach (AsteroideSprite asteroide in this.listeAsteroides)
            {
                if (this.camera.EstAuDessus(asteroide.PositionRect))
                {
                    asteroidesFini.Add(asteroide);
                }
            }

            // Se débarasser des astéroïdes ayant quitté l'écran.
            foreach (AsteroideSprite asteroide in asteroidesFini)
            {
                this.listeAsteroides.Remove(asteroide);
            }

            // Mettre à jour les astéroïdes existants.
            foreach (AsteroideSprite asteroide in this.listeAsteroides)
            {
                asteroide.Update(gameTime, this.graphics);
            }

            // Déterminer si on doit créer un nouvel astéroide.
            if (this.randomAsteroides.NextDouble() < this.probAsteroides)
            {
                // Créer le sprite
                AsteroideSprite asteriode = new AsteroideSprite(0, 0);

                // Positionner aléatoirement le sprite au haut de l'écran.
                Random random = new Random();
                asteriode.Position = new Vector2(random.Next(this.graphics.GraphicsDevice.Viewport.Width), -asteriode.Height / 2);

                // Aligner la vitesse de déplacement de l'astéroïde avec celui de l'arrière-plan.
                asteriode.VitesseDeplacement = this.arrierePlan.VitesseArrierePlan;

                // Ajouter le sprite à la liste d'astéroïdes.
                this.listeAsteroides.Add(asteriode);
            }

            // Déterminer si le sprite du joueur est entré en collision avec un astéroide
            foreach (AsteroideSprite asteroide in this.listeAsteroides)
            {
                if (this.vaisseauJoueur.Collision(asteroide))
                {
                    this.Exit();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Cette fonction membre est invoquée lorsque le jeu doit mettre à jour son 
        /// affichage.
        /// </summary>
        /// <param name="gameTime">Fournie un instantané du temps de jeu.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // À FAIRE: Ajoutez votre code d'affichage ici.
            this.spriteBatch.Begin();

            // Afficher l'arrière-plan.
            this.arrierePlan.Draw(this.camera, this.spriteBatch);
            this.vaisseauJoueur.Draw(this.camera, this.spriteBatch);

            // Afficher les astéroïdes.
            foreach (AsteroideSprite asteroide in this.listeAsteroides)
            {
                asteroide.Draw(this.camera, this.spriteBatch);
            }

            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
