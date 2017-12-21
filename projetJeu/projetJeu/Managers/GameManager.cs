using IFM20884;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projetJeu.Managers
{
    public class GameManager
    {
        private Game game;

        private MenuManager menuManager;

        private GraphicsDeviceManager graphics;

        Texture2D mainmenuImage;

        /// <summary>
        /// Attribut gérant l'affichage en batch à l'écran.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Attribut représentant la camera.
        /// </summary>
        private Camera camera;

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
            get { return etatJeu; }
            set { etatJeu = value; }
        }

        /// <summary>
        /// Attribut représentant le vaisseau contrôlé par le joueur.
        /// </summary>
        private JoueurSprite vaisseauJoueur;

        /// <summary>
        /// Effet sonore contenant le bruitage de fond du jeu.
        /// </summary>
        private Song bruitageFond;

        /// <summary>
        /// Attribut représentant l'arrière plan d'étoiles à défilement vertical du du jeu.
        /// </summary>
        private DefilementArrierePlan arrierePlanEspace;

        private Texture2D _faderTexture;
        private float _faderAlpha;
        private float _faderAlphaIncrement = 10;
        private bool fading;
        private bool switchScenes;





        public GameManager(Game _game)
        {
            this.game = _game;
            this.graphics = new GraphicsDeviceManager(game);
            this.game.Content.RootDirectory = "Content";
        }

        public ContentManager GetContent()
        {
            return game.Content;
        }

        public void Exit()
        {
            this.game.Exit();
        }

        public void Initialize()
        {
            _faderTexture = new Texture2D(graphics.GraphicsDevice, 1, 1);
            var colors = new Color[] { Color.White };
            _faderTexture.SetData<Color>(colors);
            this.menuManager = new MenuManager(this);

            // Ajust le nom du jeu
            this.game.Window.Title = "projetJeu";

            // Ajuste les dimensions du jeu
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 480;
            this.graphics.ApplyChanges();

            // Activer le service de gestion du clavier
            ServiceHelper.Game = this.game;
            this.game.Components.Add(new ClavierService(this.game));

            // Initialiser la vue de la caméra à la taille de l'écran.
            this.camera = new Camera(new Rectangle(0, 0, this.graphics.GraphicsDevice.Viewport.Width, this.graphics.GraphicsDevice.Viewport.Height));

            // Le jeu est en cours de démarrage. Notez qu'on évite d'exploiter la prorpiété EtatJeu
            // car le setter de cette dernière manipule des effets sonores qui ne sont pas encore
            // chargées par LoadContent()
            this.etatJeu = Etats.Demarrer;
        }

        public void LoadContent()
        {
            this.menuManager.LoadContent();
            this.menuManager.MenuCourant = this.menuManager.TrouverMenu("MainMenu");

            // Créer un nouveau SpriteBatch, utilisée pour dessiner les textures.
            this.spriteBatch = new SpriteBatch(this.graphics.GraphicsDevice);

            // Charger les sprites.
            JoueurSprite.LoadContent(this.game.Content, this.graphics);
            ArrierePlanEspace.LoadContent(this.game.Content, this.graphics);

            // Créer les sprites du jeu. Premièrement le sprite du joueur centrer au bas de l'écran. On limite ensuite
            // ses déplacements à l'écran.
            this.vaisseauJoueur = new JoueurSprite(this.graphics.GraphicsDevice.Viewport.Width / 8f, this.graphics.GraphicsDevice.Viewport.Height / 2f);
            this.vaisseauJoueur.BoundsRect = new Rectangle(0, 0, this.graphics.GraphicsDevice.Viewport.Width, this.graphics.GraphicsDevice.Viewport.Height);

            // Créer ensuite les sprites représentant les arrière-plans.
            this.arrierePlanEspace = new ArrierePlanEspace(this.graphics);

            // Charger le bruitage de fond du jeu.
            this.bruitageFond = this.game.Content.Load<Song>(@"Songs\scifi072");

            this.mainmenuImage = this.game.Content.Load<Texture2D>(@"ArrieresPlans\mainmenu.jpg");

            // Paramétrer la musique de fond et la démarrer.
            MediaPlayer.Volume = 0.5f;         // pour mieux entendre les autres effets sonores
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(bruitageFond);
            MediaPlayer.Pause();
        }

        public void Update(GameTime gameTime)
        {
            if (fading)
            {
                _faderAlpha += _faderAlphaIncrement;

                // Lighting things back!
                if (_faderAlpha >= 255)
                {
                    switchScenes = true;
                    _faderAlphaIncrement = -_faderAlphaIncrement;
                }
                else if (_faderAlpha <= 0)
                {
                    fading = false;
                }
            }

            if (this.EtatJeu == Etats.Demarrer)
            {
                if (!fading)
                {
                    this.menuManager.MenuCourant.GetInput(gameTime);
                }
                else
                {
                    if (switchScenes)
                    {
                        this.EtatJeu = Etats.Jouer;
                    }
                }
            }
            else if (this.EtatJeu == Etats.Jouer || this.EtatJeu == Etats.Pause)
            {
                if (!fading)
                {
                    if (switchScenes)
                    {
                        switchScenes = false;
                        SuspendreEffetsSonores(false);
                    }
                    // Permettre de quitter le jeu via le service d'input.
                    if (ServiceHelper.Get<IInputService>().Quitter(1))
                    {
                        this.game.Exit();
                        Environment.Exit(0);
                    }

                    // Est-ce que le bouton de pause a été pressé?
                    if (ServiceHelper.Get<IInputService>().Pause(1))
                    {
                        this.Pause = !Pause;
                    }

                    // Si le jeu est en pause, interrompre la mise à jour
                    if (Pause)
                    {
                        return;
                    }

                    // Mettre à joueur les sprites du jeu
                    this.vaisseauJoueur.Update(gameTime, this.graphics);
                    this.arrierePlanEspace.Update(gameTime, this.graphics);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            this.graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // À FAIRE: Ajoutez votre code d'affichage ici.
            this.spriteBatch.Begin();

            if (EtatJeu == Etats.Demarrer)
            {
                this.spriteBatch.Draw(mainmenuImage, graphics.GraphicsDevice.Viewport.Bounds, Color.White);
                this.menuManager.Draw(spriteBatch);
            }
            else if (EtatJeu == Etats.Jouer || this.EtatJeu == Etats.Pause)
            {
                // Afficher l'arrière-plan.
                this.arrierePlanEspace.Draw(this.camera, this.spriteBatch);

                // Afficher le sprite contrôlé par le joueur.
                this.vaisseauJoueur.Draw(this.camera, this.spriteBatch);
            }

            if (fading)
            {
                spriteBatch.Draw(_faderTexture, graphics.GraphicsDevice.Viewport.Bounds, new Color(Color.Black, (byte)MathHelper.Clamp(_faderAlpha, 0, 255)));
            }

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
                this.SuspendreEffetsSonores(Pause);
            }
        }

        /// <summary>
        /// Suspend temporairement (pause) ou réactive les effets sonores du jeu.
        /// </summary>
        /// <param name="suspendre">Indique si les effets sonores doivent être suspendus ou réactivés.</param>
        public void SuspendreEffetsSonores(bool suspendre)
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
                    MediaPlayer.Resume();
                }
            }
        }

        public void FadeOut()
        {
            fading = true;
        }
    }
}
