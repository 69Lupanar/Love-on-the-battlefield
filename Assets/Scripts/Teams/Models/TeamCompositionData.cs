using System;
using System.Collections.Generic;

namespace Assets.Scripts.Teams
{
    /// <summary>
    /// Réprésente une combinaison de joueurs spécifiques (ppaux et remplaçants).
    /// Cela nous permet d'enregistrer une composition pour une équipe
    /// afin d'éviter de la renseigner à chaque fois
    /// </summary>
    [Serializable]
    public struct TeamCompositionData
    {
        #region Instance

        /// <summary>
        /// Joueurs ppaux de l'équipe
        /// </summary>
        public List<CharacterData> MainCharacters;

        /// <summary>
        /// Joueurs remplaçants de l'équipe
        /// </summary>
        public List<CharacterData> Substitutes;

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="mainCharacters">Joueurs ppaux de l'équipe</param>
        /// <param name="substitutes">Joueurs remplaçants de l'équipe</param>
        public TeamCompositionData(IEnumerable<CharacterData> mainCharacters, IEnumerable<CharacterData> substitutes)
        {
            MainCharacters = new List<CharacterData>(mainCharacters);
            Substitutes = new List<CharacterData>(substitutes);
        }

        #endregion
    }
}