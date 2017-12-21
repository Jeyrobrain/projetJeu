using IFM20884;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projetJeu
{
    class GameManager
    {
        private Game game;

        private GraphicsDeviceManager graphics;

        /// <summary>
        /// Attribut gérant l'affichage en batch à l'écran.
        /// </summary>
        private SpriteBatch spriteBatch;

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
        /// États disponibles du jeu.
        /// </summary>
        public enum Etats
        {
            /// <summary>
            /// En cours de démarrage.
            /// </summary>
            Demarrer,

            /// <summary>
            /// En cours de jeu.
            /// </summary>
            Jouer,

            /// <summary>
            /// En cours de fin de jeu.
            /// </summary>
            Quitter,

            /// <summary>
            /// En suspension temporaire.
            /// </summary>
            Pause
        }

        /// <summary>
        /// Attribut indiquant l'état du jeu
        /// </summary>
        private Etats etatJeu;

        /// <summary>
        /// Etat dans lequel état le jeu avant que la dernière pause ne soit activée.
        /// </summary>
        private Etats prevEtatJeu;

        /// <summary>
        /// Propriété (accesseur pour etatJeu) retournant ou changeant l'état du jeu.
        /// </summary>
        /// <value>État courant du jeu.</value>
        public Etats EtatJeu
        {
            get { return this.etatJeu; }
            set { this.etatJeu = value; }
        }

        /// <summary>
        /// Attribut représentant le vaisseau contrôlé par le joueur.
        /// </summary>
        private JoueurSprite vaisseauJoueur;

        /// <summary>
        /// Effet sonore contenant le bruitage de fond du jeu.
        /// </summary>
        private static Song bruitageFond;

        /// <summary>
        /// Attribut représentant l'arrière plan d'étoiles à défilement vertical du du jeu.
        /// </summary>
        private DefilementArrierePlan arrierePlanEspace;

        public GameManager(Game game)
        {
            this.game = game;
            this.graphics = new GraphicsDeviceManager(game);
            game.Content.RootDirectory = "Content";
        }

        public void Initialize()
        {
            // Ajust le nom du jeu
            this.game.Window.Title = "projetJeu";

            // Ajuste les dimensions du jeu
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 480;
            this.graphics.ApplyChanges();

            // Activer le service de gestion du clavier
            ServiceHelper.Game = game;
            this.game.Components.Add(new ClavierService(game));

            // Initialiser la vue de la caméra à la taille de l'écran.
            this.camera = new Camera(new Rectangle(0, 0, this.graphics.GraphicsDevice.Viewport.Width, this.graphics.GraphicsDevice.Viewport.Height));

            // Créer les attributs de gestion des astéroïdes.
            this.listeAsteroides = new List<Sprite>();
            this.randomAsteroides = new Random();

            // Le jeu est en cours de démarrage. Notez qu'on évite d'exploiter la prorpiété EtatJeu
            // car le setter de cette dernière manipule des effets sonores qui ne sont pas encore
            // chargées par LoadContent()
            this.etatJeu = Etats.Demarrer;
        }

        public void LoadContent()
        {
            // Créer un nouveau SpriteBatch, utilisée pour dessiner les textures.
            this.spriteBatch = new SpriteBatch(this.graphics.GraphicsDevice);

            // Charger les sprites.
            JoueurSprite.LoadContent(this.game.Content, this.graphics);
            ArrierePlanEspace.LoadContent(this.game.Content, this.graphics);

            // Créer les sprites du jeu. Premièrement le sprite du joueur centrer au bas de l'écran. On limite ensuite
            // ses déplacements à l'écran.
            this.vaisseauJoueur = new JoueurSprite(this.graphics.GraphicsDevice.Viewport.Width / 2f, this.graphics.GraphicsDevice.Viewport.Height * 0.85f);
            this.vaisseauJoueur.BoundsRect = new Rectangle(0, 0, this.graphics.GraphicsDevice.Viewport.Width, this.graphics.GraphicsDevice.Viewport.Height);

            // Créer ensuite les sprites représentant les arrière-plans.
            this.arrierePlanEspace = new ArrierePlanEspace(this.graphics);

            // Charger le bruitage de fond du jeu.
            bruitageFond = game.Content.Load<Song>(@"Songs\scifi072");

            // Paramétrer la musique de fond et la démarrer.
            MediaPlayer.Volume = 0.5f;         // pour mieux entendre les autres effets sonores
            MediaPlayer.IsRepeating = true;

            MediaPlayer.Play(bruitageFond);
        }

        public void Update(GameTime gameTime)
        {
            // Si le jeu est en cours de démarrage, passer à l'état de jouer
            if (this.EtatJeu == Etats.Demarrer)
            {
                this.EtatJeu = Etats.Jouer;
            }

            // Permettre de quitter le jeu via le service d'input.
            if (ServiceHelper.Get<IInputService>().Quitter(1))
            {
                this.game.Exit();
            }

            // Est-ce que le bouton de pause a été pressé?
            if (ServiceHelper.Get<IInputService>().Pause(1))
            {
                this.Pause = !this.Pause;
            }

            // Si le jeu est en pause, interrompre la mise à jour
            if (this.Pause)
            {
                return;
            }

            // Mettre à joueur les sprites du jeu
            this.vaisseauJoueur.Update(gameTime, this.graphics);
            this.arrierePlanEspace.Update(gameTime, this.graphics);
        }

        public void Draw(GameTime gameTime)
        {
            this.graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // À FAIRE: Ajoutez votre code d'affichage ici.
            this.spriteBatch.Begin();

            // Afficher l'arrière-plan.
            this.arrierePlanEspace.Draw(this.camera, this.spriteBatch);

            // Afficher le sprite contrôlé par le joueur.
            this.vaisseauJoueur.Draw(this.camera, this.spriteBatch);

            this.spriteBatch.End();
        }

        /// <summary>
        /// Propriété activant et désactivant l'état de pause du jeu. Cette propriété doit être utilisée
        /// pour mettre le jeu en pause (plutôt que EtatJeu) car elle stocke l'état précédent (i.e. avant 
        /// la pause) du jeu afin de le restaurer lorsque la pause est terminée.
        /// </summary>
        /// <value>Le jeu est en pause ou pas.</value>
        public bool Pause
        {
            get
            {
                return this.etatJeu == Etats.Pause;
            }

            set
            {
                // S'assurer qu'il y a changement de statut de pause
                if (value && this.EtatJeu != Etats.Pause)
                {
                    // Stocker l'état courant du jeu avant d'activer la pause
                    this.prevEtatJeu = this.EtatJeu;
                    this.EtatJeu = Etats.Pause;
                }
                else if (!value && this.EtatJeu == Etats.Pause)
                {
                    // Restaurer l'état du jeu à ce qu'il était avant la pause
                    this.EtatJeu = this.prevEtatJeu;
                }

                // Suspendre les effets sonores au besoin
                this.SuspendreEffetsSonores(this.Pause);
            }
        }

        /// <summary>
        /// Suspend temporairement (pause) ou réactive les effets sonores du jeu.
        /// </summary>
        /// <param name="suspendre">Indique si les effets sonores doivent être suspendus ou réactivés.</param>
        protected void SuspendreEffetsSonores(bool suspendre)
        {
            // Suspendre au besoin les effets sonores du vaisseau
            this.vaisseauJoueur.SuspendreEffetsSonores(suspendre);

            // Suspendre ou réactiver le bruitage de fond
            if (suspendre)
            {
                if (MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Pause();
                }
            }
            else
            {
                if (MediaPlayer.State == MediaState.Paused)
                {
                    MediaPlayer.Play(bruitageFond);
                }
            }
        }
    }
}
