namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère les inputs du joueur et de l'IA
    /// </summary>
    public interface ICharacterInput
    {
        /// <summary>
        /// Active les commandes
        /// </summary>
        public void Enable();

        /// <summary>
        /// Désactive les commandes
        /// </summary>
        public void Disable();
    }
}