namespace Assets.Scripts.StateMachine
{
    /// <summary>
    /// Représente un contexte passé aux états d'une machine à états.
    /// Puisque la machine elle-même ne peut pas contenir l'état racine,
    /// chaque contexte est responsable de son propre état racine.
    /// Ca nous permet de partager une machine entre plusieurs contextes.
    /// </summary>
    /// <typeparam name="TContext">Le type héritant de cette interface</typeparam>
    /// <typeparam name="TInput">Le type des commandes utilisées</typeparam>
    /// <typeparam name="TBaseState">Le type de la classe de base dont hériteront les état utilisés</typeparam>
    public interface IStateContext<TContext, TInput, TBaseState>
        where TContext : IStateContext<TContext, TInput, TBaseState>
        where TInput : IStateInput
        where TBaseState : BaseState<TContext, TInput, TBaseState>
    {
        /// <summary>
        /// Etat racine du contexte
        /// </summary>
        public TBaseState RootState { get; set; }
    }
}