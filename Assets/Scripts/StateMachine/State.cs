namespace Assets.Scripts.StateMachine
{
    /// <summary>
    /// Classe de convenance pour ajouter des états à la machine plus facilement
    /// </summary>
    public abstract class State
    {
        #region Méthodes protégées

        /// <summary>
        /// Callback appelé quand on entre dans l'état
        /// </summary>
        protected virtual void Enter() { }

        /// <summary>
        /// Callback appelé quand on quitte l'état
        /// </summary>
        protected virtual void Exit() { }

        /// <summary>
        /// Callback appelé quand on màj l'état dans une méthode Update()
        /// </summary>
        protected virtual void Update() { }

        /// <summary>
        /// Callback appelé quand on màj l'état dans une méthode FixedUpdate()
        /// </summary>
        protected virtual void FixedUpdate() { }

        #endregion
    }
}