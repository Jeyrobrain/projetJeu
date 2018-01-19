//-----------------------------------------------------------------------
// <copyright file="EnnemiSprite.cs" company="Marco Lavoie">
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
// <summary>Implantation de la classe EnnemiSprite.</summary>
//-----------------------------------------------------------------------

namespace projetJeu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using IFM20884;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using projetJeu.Managers;


    /// <summary>
    /// Classe implantant le sprite représentant un astéroïde en rotation.
    /// </summary>
    public abstract class EnnemiSprite : Sprite
    {
        public delegate void Shoot(Obus obus, bool isPlayer = false);
        public delegate float AngleToPlayer(Sprite source);

        private Shoot shootObus;
        public Shoot ShootObus
        {
            get { return shootObus; }
            set { this.shootObus = value; }
        }

        private AngleToPlayer angleToPlayer;
        public AngleToPlayer GetAngleToPlayer
        {
            get { return this.angleToPlayer; }
            set { this.angleToPlayer = value; }
        }

        private int health;
        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public EnnemiSprite(Vector2 pos)
            : base(pos)
        {

        }

        public EnnemiSprite(float x, float y)
            : this(new Vector2(x, y))
        {

        }

        /// <summary>
        /// Vitesse de déplacement verticale du sprite.
        /// </summary>
        private float vitesseDeplacement;

        /// <summary>
        /// Propriété (accesseur pour vitesseDeplacement) retournant ou changeant la vitesse de déplacement 
        /// verticale du sprite.
        /// </summary>
        /// <value>Position du sprite.</value>
        public float VitesseDeplacement
        {
            get { return this.vitesseDeplacement; }
            set { this.vitesseDeplacement = value; }
        }
    }
}
