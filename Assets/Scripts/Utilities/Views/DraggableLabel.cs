using TMPro;

namespace Assets.Scripts.Utilities.Views
{
    /// <summary>
    /// Label glissable et déposable sur l'interface
    /// </summary>
    public sealed class DraggableLabel : DraggableUIElement
    {
        #region Propriétés

        /// <summary>
        /// Le contenu du label
        /// </summary>
        public string Text => _label.text;

        #endregion

        #region Instance

        /// <summary>
        /// Le texte
        /// </summary>
        private TextMeshProUGUI _label;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _label = GetComponent<TextMeshProUGUI>();
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Assigne le texte du label
        /// </summary>
        public void SetText(string text)
        {
            _label.SetText(text);
        }

        #endregion
    }
}