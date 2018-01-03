using IFM20884;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projetJeu.Managers
{
    public class MenuManager
    {
        private GameManager gameManager;

        // Liste de tous les menus du jeu (chargés dans LoadContent()).
        private List<Menu> listeMenus = new List<Menu>();

        // Menu présentement affiché.
        private Menu menuCourant = null;

        // Police exploitée pour afficher les menus.
        private SpriteFont policeMenuTitre; // titre du menu
        public SpriteFont policeMenuItem; // items de menu

        // Couleur de la police exploitée pour afficher les menus.
        private Color couleurMenuTitre = Color.White; // titre du menu
        private Color couleurMenuItem = Color.White; // items non sélectionnés
        private Color couleurMenuItemSelectionne = Color.Yellow; // item sélectionné

        // Propriété (accesseur pour menuCourant) retournant ou changeant le
        // menu affiché. Lorsque sa valeur est null, aucun menu n'est affiché.
        public Menu MenuCourant
        {
            get { return this.menuCourant; }
            set
            {
                this.menuCourant = value;
                // Mettre le jeu en pause si un menu est affiché.
                if (this.gameManager.EtatJeu == GameManager.Etats.Jouer)
                    this.gameManager.Pause = this.menuCourant != null;
            }
        }

        public MenuManager(GameManager _gameManager) 
        {
            this.gameManager = _gameManager;
        }

        public void LoadContent()
        {
            Menu.LoadContent(this.gameManager.GetContent());

            // Charger les polices.
            this.policeMenuTitre = this.gameManager.GetContent().Load<SpriteFont>(@"Pipeline\Polices\PoliceTitre");
            this.policeMenuItem = this.gameManager.GetContent().Load<SpriteFont>(@"Pipeline\Polices\PoliceItem");

            // Charger tous les menus disponibles et les stocker dans la liste des menus.
            // Obtenir d’abord une liste des fichiers XML de définition de menu.
            string[] fichiersDeMenu = Directory.GetFiles(this.gameManager.GetContent().RootDirectory + @"\Pipeline\Menus\");
            // Itérer pour chaque fichier XML trouvé.
            foreach (string nomFichier in fichiersDeMenu)
            {
                Menu menu = new Menu(); // créer un nouveau menu
                menu.Load(nomFichier); // configurer le nouveau menu à partir de son fichier XML
                // Assigner la fonction déléguée de Game au nouveau menu.
                menu.SelectionItemMenu = this.SelectionItemMenu;
                // Ajouter le nouveau menu à la liste des menus du jeu.
                this.listeMenus.Add(menu);
            }
        }

        // Fonction déléguée fournie à tous les menus du jeu pour traiter les sélections de l'usager.
        protected void SelectionItemMenu(string nomMenu, ItemDeMenu item)
        {
            if (nomMenu == "MainMenu")
                switch (item.Nom)
                {
                    case "Commencer": // l'usager veut commencer le jeu
                        gameManager.FadeOut(GameManager.Etats.Jouer);
                        break;
                    case "Information":
                        gameManager.FadeOut(GameManager.Etats.Info);
                        break;
                    case "Quitter": // l'usager veut quitter le jeu
                        gameManager.Exit();
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
            else if (nomMenu == "Information")
                switch (item.Nom)
                {
                    case "Retour": // l'usager veut commencer le jeu
                        gameManager.FadeOut(GameManager.Etats.Demarrer);
                        break;
                    default:
                        break;
                }
        }

        // Trouve le menu dont le nom est fourni dans la liste des menus gérée par le jeu.
        public Menu TrouverMenu(string nomMenu)
        {
            // Itérer parmi la liste des menus disponibles
            foreach (Menu menu in this.listeMenus)
                // Si le menu recherché est trouvé, le retourner
                if (menu.Nom == nomMenu)
                    return menu;
            return null; // aucun menu correspondant au fourni
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.MenuCourant != null)
                this.MenuCourant.Draw(spriteBatch, this.policeMenuTitre, this.policeMenuItem, this.couleurMenuTitre, this.couleurMenuItem, this.couleurMenuItemSelectionne);
        }
    }
}
