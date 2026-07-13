using UnityEngine;

namespace Assets.Scripts.Utilities.Views
{
    /// <summary>
    /// Echange l'état dactif de l'objet
    /// </summary>
    public class ToggleActiveState : MonoBehaviour
    {
        /// <summary>
        /// Echange l'état dactif de l'objet
        /// </summary>
        public void Toggle(GameObject go)
        {
            go.SetActive(!go.activeSelf);
        }
    }
}