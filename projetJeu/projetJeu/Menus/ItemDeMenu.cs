using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projetJeu.Menus
{
    // Classe implantant un item de menu. Cette classe dispose seulement d'Attributs
    // membres car elle sert uniquement à stocker de l'information relative à un item de menu.
    public class ItemDeMenu
    {
        // Attribut indiquant si l'item est la sélection courante d'un menu.
        private bool selection = false;
        // Attribut indiquant le nom de l'item (pour fins d'identification).
        private string nom = string.Empty;
        // Attribut indiquant le titre de l'item (pour fins d'affichage).
        private string titre = string.Empty;

        // Attribut indiquant l'indentation horizontale de l'item (en pixels) en rapport
        // à la position du menu dans lequel l'item est affiché). Noter que c'est au menu
        // de gérer l'indentation lors de l'affichage du menu.
        private int indentation = 0;
        // Propriété (accesseur de titre) retournant et modifiant le titre de l'item
        // (pour fins d'affichage).
        public string Titre
        {
            get { return this.titre; }
            set { this.titre = value; }
        }
        // Propriété (accesseur de nom) retournant et modifiant le nom de l'item
        // (pour fins d'identification).
        public string Nom
        {
            get { return this.nom; }
            set { this.nom = value; }
        }

        // Propriété (accesseur de selection) retournant et modifiant l'indication si
        // l'item est la sélection courante d'un menu.
        public bool Selection
        {
            get { return this.selection; }
            set { this.selection = value; }
        }
        // Propriété (accesseur de indentation) retournant et modifiant l'indentation
        // horizontale de l'item (voir description de indentation).
        public int Indentation
        {
            get { return this.indentation; }
            set { this.indentation = value; }
        }
    }
}
