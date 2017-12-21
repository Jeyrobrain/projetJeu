using IFM20884;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace projetJeu.Menus
{
    // Définition de fonction déléguée exécutée lorsqu'un item de menu est sélectionné.
    public delegate void TypeSelectionItemMenu(string nomMenu, ItemDeMenu nomItem);

    public class Menu
    {
        private static SoundEffect menuitem_change;

        // Attribut conservant la fonction déléguée invoquée par l'instance de Menu lorsqu'un
        // item de menu est sélectionné.
        private TypeSelectionItemMenu selectionItemMenu = null;
        // Attribut gérant la liste des items associés au menu.
        private List<ItemDeMenu> items = new List<ItemDeMenu>();
        // Attribut indiquant le nom du menu (pour fins d'identification).
        private string nom = string.Empty;
        // Attribut indiquant le titre du menu (pour fins d'affichage).
        private string titre = string.Empty;
        // Attribut contenant l'index de l'item du menu présentement actif pour sélection.
        private int indexSelection = 0;
        // Attribut gérant la position du menu à l'écran. Le coin supérieur
        // gauche du menu sera positionné à ces coordonnées.
        private Vector2 origine = new Vector2();
        // Attribut indiquant le temps écoulé (en millisecondes) depuis que la sélection
        // active fut sélectionnée. Cet attribut sert à insérer un délai entre les
        // changements de sélection lorsque ceux-ci sont trop rapides (e.g. la touche du
        // clavier est pressée continuellement).
        private double heureDernierChangementItem = 0.0;
        // Propriété (accesseur de selectionItemMenu) retournant et modifiant la fonction
        // déléguée invoquée par l'instance de Menu lorsqu'un item de menu est sélectionné.
        public TypeSelectionItemMenu SelectionItemMenu
        {
            get { return this.selectionItemMenu; }
            set { this.selectionItemMenu = value; }
        }
        // Propriété (accesseur de nom) retournant et modifiant le nom du menu (pour
        // fins d'identification).
        public string Nom
        {
            get { return this.nom; }
            set { this.nom = value; }
        }
        // Propriété retournant l'item de menu présentement actif pour sélection.
        private ItemDeMenu Selection
        {
            get
            {
                // L'instance de ItemDeMenu correspondant à la sélection active est
                // extraite (mais non retirée) de items.
                if (this.indexSelection >= 0 && this.indexSelection < this.items.Count)
                    return this.items[this.indexSelection];
                else
                    return null;
            }
        }

        public static void LoadContent(ContentManager content) 
        {
            menuitem_change = content.Load<SoundEffect>(@"SoundFX\menuitem_change");
        }

        // Charge le menu d'un fichier XML dont le nom est fourni en paramètre.
        public void Load(string nomMenu)
        {
            // Initialiser un lexeur de fichier XML.
            XmlTextReader lecteurXML = new XmlTextReader(nomMenu);
            // Item de menu en construction lors de la lecture.
            ItemDeMenu itemCourant = null;
            // Lire les champs XML un à la fois, séquentiellement.
            while (lecteurXML.Read())
            {
                // Traiter le champ XML en fonction de son nom.
                switch (lecteurXML.Name)
                {
                    // Le nom du menu.
                    case "MenuNom":
                        this.nom = lecteurXML.ReadElementContentAsString();
                        break;
                    // Le titre du menu.
                    case "MenuTitre":
                        this.titre = lecteurXML.ReadElementContentAsString();
                        break;
                    // Un nouvel item de menu.
                    case "MenuItem":
                        // Vérifier premièrement si on était en processus de lecture
                        // d'un item de menu précédent. Si c'est le cas, ajouté cet
                        // item à la liste avant de commencer à lire le nouvel item.
                        if (itemCourant != null)
                            // Puisqu'un item de menu DOIT avoir un nom pour pouvoir le
                            // gérer, on conserve celui lu seulement s'il a un nom.
                            if (itemCourant.Nom != string.Empty)
                                this.items.Add(itemCourant);
                        // Créer un nouvel item de menu à construire.
                        itemCourant = new ItemDeMenu();
                        break;
                    // Le nom de l'item de menu en cours de lecture.
                    case "MenuItemNom":
                        itemCourant.Nom = lecteurXML.ReadElementContentAsString();
                        break;
                    // Le titre de l'item de menu en cours de lecture.
                    case "MenuItemTitre":
                        itemCourant.Titre = lecteurXML.ReadElementContentAsString();
                        break;
                    // L'indentation horizontale (en pixels) de l'item de menu en cours de lecture.
                    case "MenuItemIndent":
                        itemCourant.Indentation = lecteurXML.ReadElementContentAsInt();
                        break;
                    // L'item de menu actif par défaut (lorsque le menu est affiché, ce sera l’item actif).
                    case "IndexSelectionItem":
                        this.indexSelection = lecteurXML.ReadElementContentAsInt();
                        break;
                    // Position horizontale de l'origine du menu (coin supérieur gauche).
                    case "PositionX":
                        this.origine.X = lecteurXML.ReadElementContentAsInt();
                        break;
                    // Position verticale de l'origine du menu (coin supérieur gauche).
                    case "PositionY":
                        this.origine.Y = lecteurXML.ReadElementContentAsInt();
                        break;
                    default:
                        break;
                }
            }

            // Si un item de menu est actif pour sélection, l'en informer.
            if (this.Selection != null)
                this.Selection.Selection = true;
        }

        // Fonction faisant passer la sélection active courante au prochain item de menu (i.e.
        // celui en dessous de la sélection courante.
        public void SelectionItemSuivant()
        {
            menuitem_change.Play();
            // Désactiver l'item de menu actif précédent.
            this.items[this.indexSelection].Selection = false;
            // S'il y a un autre item en dessous.
            if (this.indexSelection < this.items.Count - 1)
                // Mettre à jour l'index d'item actif de sorte qu'il indique
                // le prochain item de menu.
                this.indexSelection++;
            else
                // L'item courant est le dernier du menu, donc mettre à jour l'index d'item
                // actif de sorte qu'il indique le premier item du menu.
                this.indexSelection = 0;
            // Activer le nouvel item de menu.
            this.items[this.indexSelection].Selection = true;
        }

        // Fonction faisant passer la sélection active courante à l'item de menu précédent
        // (i.e. celui au dessus de la sélection courante.
        public void SelectionItemPrecedent()
        {
            menuitem_change.Play();
            // Désactiver l'item de menu actif précédent.
            this.items[this.indexSelection].Selection = false;
            // S'il y a un autre item au dessus.
            if (this.indexSelection > 0)
                // Mettre à jour l'index d'item actif de sorte qu'il indique
                // l'item de menu précédent.
                this.indexSelection--;
            else
                // L'item courant est le premier du menu, donc mettre à jour l'index
                // d'item actif de sorte qu'il indique le dernier item du menu.
                this.indexSelection = this.items.Count - 1;
            // Activer le nouvel item de menu.
            this.items[this.indexSelection].Selection = true;
        }

        // Met à jour l'item de menu actif (pour sélection) en fonction des inputs. La fonction
        // exploite le service de lecture d'inputs pour gérer la sélection des items de menu. De
        // plus, elle impose un délai minimum entre deux changements de sélection.
        public void GetInput(GameTime gameTime)
        {
            // Délai minimum imposé (en millisecondes) avant que la sélection active puisse
            // être changée à nouveau.
            const double DelaiChangementItem = 150.0;
            // Vérifier si assez de temps s'est écoulé depuis le dernier changement de
            // sélection. On s'assure ainsi à l'utilisateur un meilleur contrôle du
            // changement de sélection (sinon les changements seraient beaucoup trop
            // rapides pour qu'il puisse choisir un item spécifique).
            if (this.heureDernierChangementItem > DelaiChangementItem)
            {
                // Doit-on changer la sélection active à l'item précédent du menu?
                if (ServiceHelper.Get<IInputService>().MenuItemPrecedent(1))
                {
                    this.SelectionItemPrecedent();
                    // Changement de sélection, remettre le compteur de temps écoulé à 0.
                    this.heureDernierChangementItem = 0.0;
                }
                else if (ServiceHelper.Get<IInputService>().MenuItemSuivant(1))
                {
                    // On doit changer la sélection active à l'item suivant du menu.
                    this.SelectionItemSuivant();
                    // Changement de sélection, remettre le compteur de temps écoulé à 0.
                    this.heureDernierChangementItem = 0.0;
                }
                else if (ServiceHelper.Get<IInputService>().MenuItemSelection(1))
                {
                    // Une sélection a été demandée alors on doit invoquer la fonction déléguée fournie.
                    if (this.selectionItemMenu != null)
                        this.selectionItemMenu(this.nom, this.Selection);
                    // Changement de sélection, remettre le compteur de temps écoulé à 0.
                    this.heureDernierChangementItem = 0.0;
                }
            }
            else
                // Mettre à jour le compteur de temps écoulé à 0.
                this.heureDernierChangementItem += gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        // Affiche le menu à l'écran.
        public void Draw(
         SpriteBatch spriteBatch, // tampon d'affichage
         SpriteFont fontTitre, // police pour afficher le titre du menu
         SpriteFont fontItem, // police pour afficher les items du menu
         Color couleurTitre, // couleur du texte du titre du menu
         Color couleurItem, // couleur du texte des items du menu
         Color couleurItemSelectionne)
        { // couleur pour afficher l'item présentement actif
            // Afficher le titre du menu.
            spriteBatch.DrawString(fontTitre, this.titre, this.origine, couleurTitre);
            // Calculer la hauteur du texte pour un titre d'item de menu.
            float hauteurTexte = fontItem.MeasureString(this.items[0].Nom).Y;
            // Calculer la hauteur du texte pour le titre de menu. On ajoute un espace
            // vertical supplémentaire pour distinguer le titre du menu de ses items.
            float titreHauteurTexte = fontTitre.MeasureString(this.titre).Y + 10.0f;
            // Où on en est rendu verticalement avec l'affichage des items.
            int noLigne = 0;
            // Afficher chaque item de menu. Chaque item est positionné en fonction de la hauteur
            // (en pixels) d'un item de menu et du nombre d'items affichés lors des itérations
            // précédentes de la boucle.
            foreach (ItemDeMenu item in this.items)
            {
                // Est-ce que l'item est présentement actif pour sélection?
                if (item.Selection)
                    // Afficher l'item avec la couleur de sélection active.
                    spriteBatch.DrawString(fontItem, item.Titre, new Vector2(this.origine.X + item.Indentation,
                    this.origine.Y + titreHauteurTexte + (hauteurTexte * noLigne)),
                    couleurItemSelectionne);
                else
                    // Pas la sélection active, donc afficher l'item avec la couleur d'items non sélectionnés.
                    spriteBatch.DrawString(fontItem, item.Titre, new Vector2(this.origine.X + item.Indentation,
                    this.origine.Y + titreHauteurTexte + (hauteurTexte * noLigne)),
                    couleurItem);
                noLigne++; // ne pas oublier de compter les lignes
            }
        }
    }
}