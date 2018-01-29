using IFM20884;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace projetJeu.Managers
{
    public class GameManager
    {
        /// <summary>
        /// Pixels de couleur pour utiliser come texture
        /// </summary>
        public static Texture2D redPixel, greenPixel, invisiblePixel;

        /// <summary>
        /// Instance du jeu
        /// </summary>
        private Game game;

        /// <summary>
        /// Nos menus
        /// </summary>
        private MenuManager menuManager;

        /// <summary>
        /// Écran graphique
        /// </summary>
        private GraphicsDeviceManager graphics;

        /// <summary>
        /// Image de l'écran pricipale
        /// </summary>
        private Texture2D mainmenuImage;

        /// <summary>
        /// Liste d'obus du joueur
        /// </summary>
        private List<Obus> listeObusJoueur;
        /// <summary>
        /// Liste d'obus des ennemis
        /// </summary>
        private List<Obus> listeObusEnnemis;
        /// <summary>
        /// Liste d'obus à éffacer
        /// </summary>
        private List<Obus> listeObusFini;

        /// <summary>
        /// Effet sonore d'explosion d'astéroïde et de vaisseau.
        /// </summary>
        private static SoundEffect bruitageExplosion;

        /// <summary>
        /// Générateur de nombres aléatoires pour générer des explosions.
        /// </summary>
        private Random randomExplosions;

        /// <summary>
        /// Liste de gestion des particules d'explosions.
        /// </summary>
        private List<ParticuleExplosion> listeParticulesExplosions;

        /// <summary>
        /// Texture représentant une particule d'explosion d'astéroïdes.
        /// </summary>
        private Texture2D particulesExplosions;

        /// <summary>
        /// Liste des ennemis
        /// </summary>
        private List<Sprite> listeEnnemis;

        /// <summary>
        /// Liste des ennemis
        /// </summary>
        private List<Sprite> listeEnnemisFini;

        /// <summary>
        /// Liste des power-ups
        /// </summary>
        private List<PowerUp> listePowerUp;

        /// <summary>
        /// Liste des powerups à éffacer
        /// </summary>
        private List<PowerUp> listePowerUpFini;

        /// <summary>
        /// Générateur aléatoire pour les ennemis
        /// </summary>
        private Random randomEnnemis;
        /// <summary>
        /// Générateur aléatoire pour les powerups
        /// </summary>
        private Random randomPowerup;

        /// <summary>
        /// Probabilité de généré un ennemi
        /// </summary>
        private float probEnnemis;

        /// <summary>
        /// Probabilité de généré un ennemi de type spinner
        /// </summary>
        private float probEnnemiType;

        /// <summary>
        /// Probabilité de généré un powerup
        /// </summary>
        private float probPowerUp;

        /// <summary>
        /// Attribut gérant l'affichage en batch à l'écran.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Attribut représentant la camera.
        /// </summary>
        private Camera camera;

        /// <summary>
        /// Les points qui affectent la difficulté du jeu
        /// </summary>
        private float points;

        /// <summary>
        /// Les points qui n'affectent pas la difficulté du jeu
        /// </summary>
        private float pointsAutre;

        /// <summary>
        /// Police de taille petit
        /// </summary>
        private SpriteFont policePetit;

        /// <summary>
        /// Police de taille grand
        /// </summary>
        private SpriteFont policeGrand;

        /// <summary>
        /// États disponibles du jeu.
        /// </summary>
        public enum Etats
        {
            /// <summary>
            /// En cours de démarrage.
            /// </summary>
            Demarrer,

            Info,

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
        /// Est ce que le joueur à eu un game-over?
        /// </summary>
        bool jeuFini;

        /// <summary>
        /// Attribut indiquant l'état du jeu
        /// </summary>
        private Etats etatJeu;

        /// <summary>
        /// État de jeu précédent
        /// </summary>
        private Etats etatJeuPrecedent;

        /// <summary>
        /// État de jeu prochain
        /// </summary>
        private Etats etatJeuProchain;

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
        /// Bruit quand on ramasse un power-up
        /// </summary>
        private SoundEffect bruitageRamasser;

        /// <summary>
        /// Attribut représentant l'arrière plan d'étoiles à défilement vertical du du jeu.
        /// </summary>
        private DefilementArrierePlan arrierePlanEspace;

        /// <summary>
        /// Texture qu'on veut utiliser come fader
        /// </summary>
        private Texture2D _faderTexture;
        /// <summary>
        /// Opacité du fader
        /// </summary>
        private float _faderAlpha;
        /// <summary>
        /// Incrémentation de l'opacité du fader
        /// </summary>
        private float _faderAlphaIncrement;
        /// <summary>
        /// Si ont est en train de fade à une autre scène
        /// </summary>
        private bool fading;
        /// <summary>
        /// Si ont a changer de scènes
        /// </summary>
        private bool switchScenes;

        /// <summary>
        /// Delay pour une seconde (1)
        /// </summary>
        float delaySecond;
        /// <summary>
        /// Delay restant pour qu'une seconde soit passer
        /// </summary>
        float delaySecondRestant;

        /// <summary>
        /// Constructeur de notre manager
        /// </summary>
        /// <param name="_game">Variable du jeu</param>
        public GameManager(Game _game)
        {
            this.game = _game;
            this.graphics = new GraphicsDeviceManager(game);
            this.GetContent().RootDirectory = "Content";
        }

        /// <summary>
        /// Retourne le contentmanager du jeu
        /// </summary>
        public ContentManager GetContent()
        {
            return game.Content;
        }

        /// <summary>
        /// On ferme le jeu
        /// </summary>
        public void Exit()
        {
            this.game.Exit();
        }

        /// <summary>
        /// On initialize les variables
        /// </summary>
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

            this._faderAlphaIncrement = 10f;
            this.delaySecond = 1f;
            this.delaySecondRestant = 1f;

            this.listeEnnemis = new List<Sprite>();
            this.listeEnnemisFini = new List<Sprite>();
            this.listeObusEnnemis = new List<Obus>();
            this.listeObusJoueur = new List<Obus>();
            this.listeParticulesExplosions = new List<ParticuleExplosion>();
            this.listeObusFini = new List<Obus>();
            this.listePowerUp = new List<PowerUp>();
            this.listePowerUpFini = new List<PowerUp>();

            ClearLists();

            this.points = 0f;
            this.pointsAutre = 0f;

            // Activer le service de gestion du clavier
            if (ServiceHelper.Game == null)
            {
                ServiceHelper.Game = this.game;
                this.game.Components.Add(new ClavierService(this.game));
            }

            // Initialiser la vue de la caméra à la taille de l'écran.
            this.camera = new Camera(new Rectangle(0, 0, this.graphics.GraphicsDevice.Viewport.Width, this.graphics.GraphicsDevice.Viewport.Height));

            // Le jeu est en cours de démarrage. Notez qu'on évite d'exploiter la prorpiété EtatJeu
            // car le setter de cette dernière manipule des effets sonores qui ne sont pas encore
            // chargées par LoadContent()
            this.etatJeu = Etats.Demarrer;

            // Créer les attributs de gestion des ennemis.
            this.randomEnnemis = new Random();
            this.randomPowerup = new Random();
            this.probEnnemis = 0.005f;
            this.probEnnemiType = 0.4f;
            this.probPowerUp = 0.35f;

            this.jeuFini = false;

            redPixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            redPixel.SetData<Color>(new[] { Color.Red });
            greenPixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            greenPixel.SetData<Color>(new[] { Color.Green });
            invisiblePixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            invisiblePixel.SetData<Color>(new[] { Color.Transparent });
                
            // Créer les attributs de gestion des explosions.
            this.randomExplosions = new Random();
        }

        /// <summary>
        /// On charge nos variables
        /// </summary>
        public void LoadContent()
        {
            this.menuManager.LoadContent();
            this.menuManager.MenuCourant = this.menuManager.TrouverMenu("MainMenu");

            // Créer un nouveau SpriteBatch, utilisée pour dessiner les textures.
            this.spriteBatch = new SpriteBatch(this.graphics.GraphicsDevice);

            // Charger les sprites.
            JoueurSprite.LoadContent(this.GetContent(), this.graphics);
            ArrierePlanEspace.LoadContent(this.GetContent(), this.graphics);
            Obus.LoadContent(this.GetContent(), this.graphics);
            EnnemiShip.LoadContent(this.GetContent(), this.graphics);
            EnnemiSpinner.LoadContent(this.GetContent(), this.graphics);
            PowerUp_Fire_Shot.LoadContent(this.GetContent(), this.graphics);
            PowerUp_Energy_Ball.LoadContent(this.GetContent(), this.graphics);
            PowerUp_One_Projectile.LoadContent(this.GetContent(), this.graphics);
            PowerUp_Three_Projectile.LoadContent(this.GetContent(), this.graphics);
            PowerUp_Two_Projectile.LoadContent(this.GetContent(), this.graphics);

            this.policePetit = this.GetContent().Load<SpriteFont>(@"Pipeline/Polices/PoliceItem");
            this.policeGrand = this.GetContent().Load<SpriteFont>(@"Pipeline/Polices/PoliceTitre");

            // Créer les sprites du jeu. Premièrement le sprite du joueur centrer au bas de l'écran. On limite ensuite
            // ses déplacements à l'écran.
            this.vaisseauJoueur = new JoueurSprite(this.graphics.GraphicsDevice.Viewport.Width / 8f, this.graphics.GraphicsDevice.Viewport.Height / 2f);
            this.vaisseauJoueur.BoundsRect = new Rectangle(0, 0, this.graphics.GraphicsDevice.Viewport.Width, this.graphics.GraphicsDevice.Viewport.Height);
            this.vaisseauJoueur.ShootObus += Shoot;
            this.vaisseauJoueur.NbVies = 3;

            this.bruitageRamasser = this.GetContent().Load<SoundEffect>(@"Pipeline\SoundFX\pickup");

            // Créer ensuite les sprites représentant les arrière-plans.
            this.arrierePlanEspace = new ArrierePlanEspace(this.graphics);

            // Charger le bruitage de fond du jeu.
            this.bruitageFond = this.GetContent().Load<Song>(@"Pipeline\Songs\scifi072");

            this.mainmenuImage = this.GetContent().Load<Texture2D>(@"ArrieresPlans\mainmenu.jpg");

            // Charger les textures associées aux effets visuels gérées par Game.
            this.particulesExplosions = this.GetContent().Load<Texture2D>(@"Explosion\explosionAsteroides");
            bruitageExplosion = this.GetContent().Load<SoundEffect>(@"Pipeline\SoundFX\explosion001");

            // Paramétrer la musique de fond et la démarrer.
            MediaPlayer.Volume = 0.05f;         // pour mieux entendre les autres effets sonores
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(bruitageFond);
            MediaPlayer.Pause();
        }

        /// <summary>
        /// Déléguée pour tirer (les vaisseaux)
        /// </summary>
        /// <param name="obus">Obus en question</param>
        /// <param name="isPlayer">Est-ce-que l'obus vient du joueur</param>
        private void Shoot(Obus obus, bool isPlayer)
        {
            if (isPlayer)
            {
                listeObusJoueur.Add(obus);
            }
            else
            {
                listeObusEnnemis.Add(obus);
            }
        }

        /// <summary>
        /// Nettoye les listes
        /// </summary>
        private void ClearLists()
        {
            this.listeEnnemis.Clear();
            this.listeEnnemisFini.Clear();
            this.listeObusEnnemis.Clear();
            this.listeObusJoueur.Clear();
            this.listeParticulesExplosions.Clear();
            this.listeObusFini.Clear();
            this.listePowerUp.Clear();
            this.listePowerUpFini.Clear();
        }

        /// <summary>
        /// Boucle de jeu
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            if (vaisseauJoueur != null && vaisseauJoueur.NbVies < 1)
            {
                etatJeu = Etats.Pause;
                this.SuspendreEffetsSonores(true);
                this.ClearLists();
                this.vaisseauJoueur = null;
                jeuFini = true;
            }

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
                        this.EtatJeu = etatJeuProchain;
                        if (EtatJeu == Etats.Info)
                        {
                            menuManager.MenuCourant = menuManager.TrouverMenu("Information");
                        }
                    }
                }
            }
            else if (this.EtatJeu == Etats.Info)
            {
                if (!fading)
                {
                    if (switchScenes)
                    {
                        switchScenes = false;
                    }
                    this.menuManager.MenuCourant.GetInput(gameTime);
                }
                else
                {
                    if (switchScenes && this.etatJeuProchain == Etats.Demarrer)
                    {
                        switchScenes = false;
                        this.EtatJeu = etatJeuProchain;
                        menuManager.MenuCourant = menuManager.TrouverMenu("MainMenu");
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
                        menuManager.MenuCourant = null;
                        SuspendreEffetsSonores(false);
                    }
                    // Permettre de quitter le jeu via le service d'input.
                    if (ServiceHelper.Get<IInputService>().Quitter(1))
                    {
                        this.Initialize();
                        this.LoadContent();
                        return;
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

                    float delayTimer = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    delaySecondRestant -= delayTimer;

                    if (delaySecondRestant <= 0)
                    {
                        delaySecondRestant = delaySecond;
                        pointsAutre += 1f;
                    }
                    // Mettre à joueur les sprites du jeu
                    this.vaisseauJoueur.Update(gameTime, this.graphics);
                    this.arrierePlanEspace.Update(gameTime, this.graphics);

                    this.updateEnnemi(gameTime);

                    // Mettre à jour les obus
                    this.UpdateObus(gameTime);

                    // Mettre à jour les particules d'explosion
                    this.UpdateParticulesExplosions(gameTime);

                    foreach (EnnemiSprite sprite in listeEnnemis)
                    {
                        bool collision = sprite.Collision(vaisseauJoueur);

                        if (!this.vaisseauJoueur.IsRespawned && collision)
                        {
                            this.vaisseauJoueur.Health -= 5f;
                            this.listeEnnemisFini.Add(sprite);
                            this.CreerExplosion(sprite, particulesExplosions, gameTime, 1f);
                            bruitageExplosion.Play(0.25f, 0f, 0f);
                            if (vaisseauJoueur.Health < 1)
                            {
                                vaisseauJoueur.IsRespawned = true;
                                vaisseauJoueur.NbVies -= 1;
                                this.vaisseauJoueur.Position = this.vaisseauJoueur.PositionInitiale;
                            }
                        }
                    }

                    foreach (EnnemiSprite s in listeEnnemisFini)
                    {
                        listeEnnemis.Remove(s);
                    }

                    UpdatePowerUps(gameTime);
                }
            }
        }

        /// <summary>
        /// Routine mettant à jour les obus. Elle s'occupe de:
        ///   1 - Détruire les obus ayant quitté l'écran sans collision
        ///   2 - Déterminer si un des obus a frappé un sprite, et si c'est le cas
        ///       détruire les deux sprites (probablement un astéroïde)
        ///   3 - Mettre à jour la position des obus existants.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected void UpdateObus(GameTime gameTime)
        {
            // Identifier les obus ayant quitté l'écran.
            foreach (Obus obus in this.listeObusJoueur)
            {
                if (obus.Position.X + obus.Width > this.graphics.GraphicsDevice.Viewport.Width + obus.Width ||
                    obus.Position.X < 0 ||
                    obus.Position.Y < 0 ||
                    obus.Position.Y - obus.Height > this.graphics.GraphicsDevice.Viewport.Height)
                {
                    listeObusFini.Add(obus);
                }
            }

            foreach (Obus obus in this.listeObusEnnemis)
            {
                if (obus.Position.X + obus.Width > this.graphics.GraphicsDevice.Viewport.Width - obus.Width ||
                    obus.Position.X < 0 ||
                    obus.Position.Y < 0 ||
                    obus.Position.Y - obus.Height > this.graphics.GraphicsDevice.Viewport.Height)
                {
                    listeObusFini.Add(obus);
                }
            }

            // Determiner si un obus a frappé un astéroïde, et si c'est le cas détruire les deux sprites.
            foreach (Obus obus in this.listeObusJoueur)
            {
                // Premièrement, est-ce que l'obus a touché un astéroïde?
                EnnemiSprite cible = (EnnemiSprite)obus.Collision(this.listeEnnemis);
                // Si oui, détruire les deux sprites impliqués et produire une explosion
                if (cible != null && obus.Source != cible)
                {
                    cible.Health -= obus.Damage;
                    this.CreerExplosion(cible, this.particulesExplosions, gameTime, 0.5f);
                    listeObusFini.Add(obus);
                    if (cible.Health < 1)
                    {
                        // Détruire la cible.
                        this.listeEnnemis.Remove(cible);

                        points += 10f;
                        if (points % 100 == 0)
                            probEnnemis += 0.005f;

                        // Créer un nouvel effet visuel pour l'explosion.
                        this.CreerExplosion(cible, this.particulesExplosions, gameTime, 1f);

                        // Activer l'effet sonore de l'explosion.
                        bruitageExplosion.Play(0.25f, 0f, 0f);

                        if (randomPowerup.NextDouble() <= probPowerUp)
                        {
                            PowerUp powerup;
                            if (randomPowerup.NextDouble() > 0.5f)
                            {
                                if (probEnnemis / 0.005f == 1)
                                {
                                    powerup = new PowerUp_One_Projectile(cible.Position);
                                }
                                else if (probEnnemis / 0.005f == 2)
                                {
                                    if (randomPowerup.NextDouble() > 0.75f) 
                                    {
                                        powerup = new PowerUp_One_Projectile(cible.Position);
                                    }
                                    else
                                    {
                                        powerup = new PowerUp_Two_Projectile(cible.Position);
                                    }
                                }
                                else
                                {
                                    if (randomPowerup.NextDouble() > 0.5f)
                                    {
                                        powerup = new PowerUp_Three_Projectile(cible.Position);
                                    }
                                    else
                                    {
                                        if (randomPowerup.NextDouble() > 0.5f)
                                        {
                                            powerup = new PowerUp_One_Projectile(cible.Position);
                                        }
                                        else
                                        {
                                            powerup = new PowerUp_Two_Projectile(cible.Position);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (randomPowerup.NextDouble() > 0.5f)
                                {
                                    powerup = new PowerUp_Fire_Shot(cible.Position);
                                }
                                else
                                {
                                    powerup = new PowerUp_Energy_Ball(cible.Position);
                                }
                            }
                            this.listePowerUp.Add(powerup);
                        }
                    }
                }
            }

            foreach (Obus obus in this.listeObusEnnemis)
            {
                if (!vaisseauJoueur.IsRespawned && obus.Collision(this.vaisseauJoueur) && obus.Source != vaisseauJoueur)
                {
                    vaisseauJoueur.Health -= obus.Damage;
                    this.CreerExplosion(vaisseauJoueur, this.particulesExplosions, gameTime, 0.5f);
                    // Détruire la cible et l'obus.
                    //this.listeEnnemis.Remove(cible);
                    listeObusFini.Add(obus);

                    if (vaisseauJoueur.Health < 1)
                    {
                        vaisseauJoueur.IsRespawned = true;
                        this.vaisseauJoueur.NbVies -= 1;
                        // Créer un nouvel effet visuel pour l'explosion.
                        this.CreerExplosion(vaisseauJoueur, this.particulesExplosions, gameTime, 1f);
                        this.vaisseauJoueur.Position = this.vaisseauJoueur.PositionInitiale;
                        // Activer l'effet sonore de l'explosion.
                        bruitageExplosion.Play(0.25f, 0f, 0f);
                    }
                }
            }

            // Se débarasser des obus n'étant plus d'aucune utilité.
            foreach (Obus obus in listeObusFini)
            {
                if (obus.Source == vaisseauJoueur)
                {
                    this.listeObusJoueur.Remove(obus);
                }
                else
                {
                    this.listeObusEnnemis.Remove(obus);
                }
            }

            // Mettre à jour les obus existants.
            foreach (Obus obus in this.listeObusJoueur)
            {
                obus.Update(gameTime, this.graphics);
            }

            foreach (Obus obus in this.listeObusEnnemis)
            {
                obus.Update(gameTime, this.graphics);
            }
        }

        /// <summary>
        /// Mise à jour des power-ups
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected void UpdatePowerUps(GameTime gameTime)
        {
            foreach (PowerUp p in listePowerUp)
            {
                if (p.Position.X + p.Width > this.graphics.GraphicsDevice.Viewport.Width ||
                    p.Position.X < 0 ||
                    p.Position.Y < 0 ||
                    p.Position.Y - p.Height > this.graphics.GraphicsDevice.Viewport.Height)
                {
                    listePowerUpFini.Add(p);
                }
            }

            foreach (PowerUp p in listePowerUp)
            {
                if (p.Collision(vaisseauJoueur))
                {
                    bruitageRamasser.Play(0.5f, 0f, 0f);
                    listePowerUpFini.Add(p);
                    ActivatePowerUp(p);
                }
            }

            foreach (PowerUp p in listePowerUpFini)
            {
                listePowerUp.Remove(p);
            }

            foreach (PowerUp p in listePowerUp)
            {
                p.Update(gameTime, graphics);
            }
        }

        /// <summary>
        /// On active l'effet du power-up
        /// </summary>
        /// <param name="p">Power-up en question</param>
        private void ActivatePowerUp(PowerUp p)
        {
            if (p is PowerUp_Energy_Ball)
            {
                vaisseauJoueur.ProjectileType = ProjectileType.blueEnergyBall;
            }
            else if (p is PowerUp_Fire_Shot)
            {
                vaisseauJoueur.ProjectileType = ProjectileType.smallFireShot;
            }
            else if (p is PowerUp_One_Projectile)
            {
                vaisseauJoueur.ProjectileCount = 1;
            }
            else if (p is PowerUp_Two_Projectile)
            {
                vaisseauJoueur.ProjectileCount = 2;
            }
            else if (p is PowerUp_Three_Projectile)
            {
                vaisseauJoueur.ProjectileCount = 3;
            }
            this.pointsAutre += 10;
        }

        /// <summary>
        /// Dessine note jeu
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime)
        {
            this.graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            this.spriteBatch.Begin();

            if (EtatJeu == Etats.Demarrer || EtatJeu == Etats.Info)
            {
                this.spriteBatch.Draw(mainmenuImage, graphics.GraphicsDevice.Viewport.Bounds, Color.White);
                if (EtatJeu == Etats.Info)
                {
                    spriteBatch.DrawString(menuManager.policeMenuItem, 
                                           "Numpad pour changer # de projectiles\n  + pour changer le type de projectile", 
                                           new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 173f, 
                                                       graphics.GraphicsDevice.Viewport.Height / 2 - 50f), 
                                           Color.White);
                }
                else
                {
                    string str = "Par Justyn, Marwan, et Jeremi.";
                    spriteBatch.DrawString(policePetit,
                                           str,
                                           new Vector2(graphics.GraphicsDevice.Viewport.Width - policePetit.MeasureString(str).X, 0),
                                           Color.White);
                }
                this.menuManager.Draw(spriteBatch);
            }

            else if (EtatJeu == Etats.Jouer || this.EtatJeu == Etats.Pause)
            {

                // Afficher l'arrière-plan.
                this.arrierePlanEspace.Draw(0f, this.camera, this.spriteBatch);

                foreach (PowerUp p in listePowerUp)
                {
                    p.Draw(0f, this.camera, this.spriteBatch);
                }

                // Afficher le sprite contrôlé par le joueur.
                if (this.vaisseauJoueur != null)
                {
                    this.vaisseauJoueur.Draw(0f, this.camera, this.spriteBatch);
                    this.vaisseauJoueur.DrawHealth(this.spriteBatch);
                }

                foreach (Obus obus in listeObusJoueur)
                {
                    obus.Draw(obus.Angle, this.camera, this.spriteBatch);
                }

                foreach (Obus obus in listeObusEnnemis)
                {
                    obus.Draw(obus.Angle, this.camera, this.spriteBatch, SpriteEffects.FlipHorizontally);
                }

                // Afficher les ennemis.
                foreach (EnnemiSprite ennemi in this.listeEnnemis)
                {
                    ennemi.Draw(0f, this.camera, this.spriteBatch);
                    ennemi.DrawHealth(this.spriteBatch);
                }

                // Afficher les explosions
                foreach (ParticuleExplosion particule in this.listeParticulesExplosions)
                {
                    particule.Draw(0f, this.camera, this.spriteBatch);
                }

                string points = (!this.jeuFini ? "Points       : " : "Points: ") + (this.points + pointsAutre);
                string vies = "";
                if (this.vaisseauJoueur != null)
                {
                    vies = "Vies          : " + vaisseauJoueur.NbVies;
                }
                string difficulté = "Difficultée : " + ((int)(this.probEnnemis / 0.005f)) + "x";

                if (!jeuFini)
                {
                    spriteBatch.DrawString(policePetit, points, new Vector2(5, 5), Color.DarkCyan);
                    spriteBatch.DrawString(policePetit, vies, new Vector2(5, 25), Color.DarkCyan);
                    spriteBatch.DrawString(policePetit, difficulté, new Vector2(5, 45), Color.DarkCyan);
                }
                else
                {

                    spriteBatch.DrawString(policeGrand, "Game over",
                        new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - policeGrand.MeasureString("Game over").X / 2,
                                    graphics.GraphicsDevice.Viewport.Height / 2 - policeGrand.MeasureString("Game over").Y / 2),
                    Color.DarkCyan);

                    spriteBatch.DrawString(policeGrand, points,
                                         new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - policeGrand.MeasureString(points).X / 2,
                                          graphics.GraphicsDevice.Viewport.Height / 2 + policeGrand.MeasureString(points).Y / 2),
                                            Color.DarkCyan);

                    spriteBatch.DrawString(policeGrand, "Appuyez sur ESC",
                                         new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - policeGrand.MeasureString("Appuyez sur ESC").X / 2,
                                          graphics.GraphicsDevice.Viewport.Height / 2 + policeGrand.MeasureString("Appuyez sur ESC").Y * 2),
                                            Color.DarkCyan);
                }
            }

            if (fading)
            {
                spriteBatch.Draw(_faderTexture, graphics.GraphicsDevice.Viewport.Bounds, new Color(Color.Black, (byte)MathHelper.Clamp(_faderAlpha, 0, 255)));
            }
            

            this.spriteBatch.End();
        }

        /// <summary>
        /// Mise à jour des particules d'explosions
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected void UpdateParticulesExplosions(GameTime gameTime)
        {
            // Liste de particules à détruire
            List<ParticuleExplosion> particulesFinies = new List<ParticuleExplosion>();

            // Mettre à jour les particules d'explosion
            foreach (ParticuleExplosion particule in this.listeParticulesExplosions)
            {
                particule.Update(gameTime, this.graphics);

                // Si la particule est devenue invisible, alors on peut l'ignorer à partir
                // de maintenant
                if (!particule.Visible)
                {
                    particulesFinies.Add(particule);
                }
            }

            // Éliminer les particules ayant disparu de l'écran
            foreach (ParticuleExplosion particule in particulesFinies)
            {
                this.listeParticulesExplosions.Remove(particule);
            }
        }

        /// <summary>
        /// Mise à jour des ennemis
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected void updateEnnemi(GameTime gameTime)
        {
            // Identifier les ennemis ayant quitté l'écran.
            List<EnnemiSprite> ennemisFini = new List<EnnemiSprite>();
            foreach (EnnemiSprite ennemi in this.listeEnnemis)
                if (this.camera.EstADroite(ennemi.PositionRect))
                    ennemisFini.Add(ennemi);

            // Se débarrasser des ennemis ayant quitté l'écran.
            foreach (EnnemiSprite ennemi in ennemisFini)
                this.listeEnnemis.Remove(ennemi);

            // Mettre à jour les ennemis existants.
            foreach (EnnemiSprite ennemi in this.listeEnnemis)
                ennemi.Update(gameTime, this.graphics);

            // Déterminer si on doit créer un nouvel ennemi.
            if (this.randomEnnemis.NextDouble() < this.probEnnemis)
            {
                Random random = new Random();

                EnnemiSprite ennemi = null;

                if (random.NextDouble() < probEnnemiType)
                {
                    ennemi = new EnnemiSpinner(0, 0);
                }
                else
                {
                    ennemi = new EnnemiShip(0, 0);
                }

                if (ennemi == null) return;
                
                ennemi.ShootObus += Shoot;
                ennemi.GetAngleToPlayer += AngleToPlayer;

                // Positionner aléatoirement le sprite au haut de l'écran.
                do {
                    ennemi.Position = new Vector2(this.graphics.GraphicsDevice.Viewport.Width + ennemi.Width / 2,
                    random.Next(ennemi.Height, this.graphics.GraphicsDevice.Viewport.Height - ennemi.Height));
                } while (EnnemiSpawnError(ennemi));

                // Aligner la vitesse de déplacement de l'ennemi avec celui de l'arrière-plan.
                ennemi.VitesseDeplacement = this.arrierePlanEspace.VitesseArrierePlan;

                // Ajouter le sprite à la liste d'ennemis.
                this.listeEnnemis.Add(ennemi);
            }
        }

        /// <summary>
        /// On vérifie si on peut pondre l'ennemi
        /// </summary>
        /// <param name="ennemi">Ennemi en question</param>
        private bool EnnemiSpawnError(EnnemiSprite ennemi) {
            bool error = false;
            foreach(EnnemiSprite e in listeEnnemis) {
                if (e.Collision(ennemi)) {
                    error = true;
                    break;
                }
            }
            return error;
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
                    this.etatJeuPrecedent = this.EtatJeu;
                    this.EtatJeu = Etats.Pause;
                }
                else if (!value && this.EtatJeu == Etats.Pause)
                {
                    // Restaurer l'état du jeu à ce qu'il était avant la pause
                    this.EtatJeu = this.etatJeuPrecedent;
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

        /// <summary>
        /// On change d'état en faisant un fadeout
        /// <param name="etat">Prochain etat</param>
        public void FadeOut(Etats etat)
        {
            etatJeuProchain = etat;
            _faderAlpha = 0f;
            _faderAlphaIncrement = 10;
            fading = true;
        }

        /// <summary>
        /// Créer une explosion aux sprite choisi
        /// <param name="sprite">Sprite cible</param>
        /// <param name="particleTexture">Texture de la particule.</param>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="grosseur">Grosseur de l'explosion</param>
        private List<ParticuleExplosion> CreerExplosion(Sprite sprite, Texture2D particuleTexture, GameTime gameTime, float grosseur)
        {
            // Liste des nouvelles particules
            List<ParticuleExplosion> nouvellesParticules = new List<ParticuleExplosion>();

            // Déterminer au hasard le nombre de particules pour représenter l'explosion
            int nombreDeParticules = 10 + this.randomExplosions.Next(11);   // entre 10 et 20 particules

            // Créer les particules et les ajouter à la liste de particules d'explosions à gérer
            for (int i = 0; i < nombreDeParticules; i++)
            {
                ParticuleExplosion particule = new ParticuleExplosion(
                    sprite.Position,                                 // positionné au départ sur le sprite
                    particuleTexture,                                // texture à utiliser
                    this.arrierePlanEspace.VitesseArrierePlan,       // vitesse de déplacement vertical
                    grosseur);      

                nouvellesParticules.Add(particule);
                this.listeParticulesExplosions.Add(particule);
            }

            return nouvellesParticules;
        }

        /// <summary>
        /// Get angle to the player
        /// <param name="source">Sprite cible</param>
        public float AngleToPlayer(Sprite source)
        {
            float angle = (float)Math.Atan2(source.Position.Y - this.vaisseauJoueur.Position.Y, source.Position.X - this.vaisseauJoueur.Position.X);
            if (this.vaisseauJoueur.Position.X > source.Position.X - source.Width / 2)
            {
                return 0f;
            }
            else
            {
                return angle;
            }
        }
    }
}
