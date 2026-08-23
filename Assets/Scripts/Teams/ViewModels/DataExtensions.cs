using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Teams
{
    /// <summary>
    /// Extensions permettant de manipuler les classes Data des SOs
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// Clone les données
        /// </summary>
        /// <param name="source">Données d'origine</param>
        /// <returns>Une copie des données</returns>
        public static TeamRosterSO Clone(this TeamRosterSO source)
        {
            TeamRosterSO clone = ScriptableObject.CreateInstance<TeamRosterSO>();
            clone.TeamData = source.TeamData;
            clone.CompositionData = new TeamCompositionData(source.CompositionData.MainCharacters, source.CompositionData.Substitutes);
            return clone;
        }

        /// <summary>
        /// Obtient le montant d'expérience requis pour atteindre le niveau suivant
        /// </summary>
        /// <param name="level">Le niveau actuel du personnage</param>
        /// <param name="expProgressCurve">Courbe de progression de gain d'expérience du personnage</param>
        /// <returns>Le montant d'expérience requis pour atteindre le niveau suivant</returns>
        public static uint GetExpUntilNextLevel(uint level, AnimationCurve expProgressCurve)
        {
            return (uint)math.round(expProgressCurve.Evaluate((float)(level + 1) / (float)Constants.MAX_LEVEL) * Constants.MAX_EXP);
        }
    }
}