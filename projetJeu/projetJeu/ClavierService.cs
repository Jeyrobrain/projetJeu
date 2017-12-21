//-----------------------------------------------------------------------
// <copyright file="ClavierService.cs" company="Marco Lavoie">
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
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Classe implantant un service de gestion de clavier.
    /// </summary>
    public class ClavierService : GameComponent, IInputService
    {
        /// <summary>
        /// Instance permettant d'extraire l'état des touches du clavier.
        /// </summary>
        private KeyboardState keyboardState;      // pour récupérer l'état du clavier

        /// <summary>
        /// Constructeur paramétré.
        /// </summary>
        /// <param name="game">Instance de la classe de jeu gérant la partie.</param>
        public ClavierService(Game game)
            : base(game)
        {
            ServiceHelper.Add<IInputService>(this);
        }

        /// <summary>
        /// Destructeur désinstallant le service via ServiceHelper.
        /// </summary>
        ~ClavierService()
        {
            ServiceHelper.Remove<IInputService>(this);
        }

        /// <summary>
        /// Retourne 1.0f si la flèche gauche du clavier est pressée; 0.0 sinon.
        /// Le paramètre device est ignoré (un seul clavier).
        /// </summary>
        /// <param name="device">Le périphérique à lire.</param>
        /// <returns>Valeur entre 0.0 (aucun mouvement) et 1.0 (vitesse maximale).</returns>
        public float DeplacementGauche(int device)
        {
            if (this.keyboardState.IsKeyDown(Keys.Left))
            {
                return 1.0f;
            }
            else
            {
                return 0.0f;
            }
        }

        /// <summary>
        /// Retourne 1.0f si la flèche droite du clavier est pressée; 0.0 sinon.
        /// Le paramètre device est ignoré (un seul clavier).
        /// </summary>
        /// <param name="device">Le périphérique à lire.</param>
        /// <returns>Valeur entre 0.0 (aucun mouvement) et 1.0 (vitesse maximale).</returns>
        public float DeplacementDroite(int device)
        {
            if (this.keyboardState.IsKeyDown(Keys.Right))
            {
                return 1.0f;
            }
            else
            {
                return 0.0f;
            }
        }

        /// <summary>
        /// Retourne 1.0f si la flèche en haut du clavier est pressée; 0.0 sinon.
        /// Le paramètre device est ignoré (un seul clavier).
        /// </summary>
        /// <param name="device">Le périphérique à lire.</param>
        /// <returns>Valeur entre 0.0 (aucun mouvement) et 1.0 (vitesse maximale).</returns>
        public float DeplacementAvant(int device)
        {
            if (this.keyboardState.IsKeyDown(Keys.Up))
            {
                return 1.0f;
            }
            else
            {
                return 0.0f;
            }
        }

        /// <summary>
        /// Retourne 1.0f si la flèche en bas du clavier est pressée; 0.0 sinon.
        /// Le paramètre device est ignoré (un seul clavier).
        /// </summary>
        /// <param name="device">Le périphérique à lire.</param>
        /// <returns>Valeur entre 0.0 (aucun mouvement) et 1.0 (vitesse maximale).</returns>
        public float DeplacementArriere(int device)
        {
            if (this.keyboardState.IsKeyDown(Keys.Down))
            {
                return 1.0f;
            }
            else
            {
                return 0.0f;
            }
        }

        /// <summary>
        /// Retourne 1.0f si la touche Esc du clavier est pressée; 0.0 sinon.
        /// Le paramètre device est ignoré (un seul clavier).
        /// </summary>
        /// <param name="device">Le périphérique à lire.</param>
        /// <returns>Booléen indiquant si on doit terminer la partie en cours.</returns>
        public bool Quitter(int device)
        {
            return this.keyboardState.IsKeyDown(Keys.Escape);
        }

        /// <summary>
        /// Récupère l'état du clavier.
        /// </summary>
        /// <param name="gameTime">Gestionnaire de temps.</param>
        public override void Update(GameTime gameTime)
        {
            this.keyboardState = Keyboard.GetState();
            base.Update(gameTime);
        }
    }
}
