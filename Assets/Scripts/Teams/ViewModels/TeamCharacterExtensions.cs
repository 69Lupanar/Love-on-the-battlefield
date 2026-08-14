using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Teams
{
    /// <summary>
    /// Extensions permettant de manipuler les TeamCharacterSOs
    /// </summary>
    internal static class TeamCharacterExtensions
    {
        /// <summary>
        /// Obtient le montant d'expérience requis pour atteindre le niveau suivant
        /// </summary>
        /// <param name="level">Le niveau actuel du personnage</param>
        /// <param name="expProgressCurve">Courbe de progression de gain d'expérience du personnage</param>
        /// <returns>Le montant d'expérience requis pour atteindre le niveau suivant</returns>
        internal static int GetExpUntilNextLevel(int level, AnimationCurve expProgressCurve)
        {
            return (int)math.round(expProgressCurve.Evaluate((float)(level + 1) / (float)Constants.MAX_LEVEL) * Constants.MAX_EXP);
        }
    }
}