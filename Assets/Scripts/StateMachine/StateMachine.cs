using Assets.Scripts.ObjectPool;

namespace Assets.Scripts.StateMachine
{
    /// <summary>
    /// Chargé d'instancier et enregistrer les états de notre machine à états
    /// </summary>
    /// <typeparam name="TContext">Contient les valeurs à lire et éditer.</typeparam>
    /// <typeparam name="TInput">Le type d'InputSystem à utilsier (Clavier, Manette, etc.)</typeparam>
    public class StateMachine<TContext, TInput, TBaseState>
        where TContext : IStateContext<TContext, TInput, TBaseState>
        where TInput : IStateInput
        where TBaseState : BaseState<TContext, TInput, TBaseState>
    {
        #region Instance

        /// <summary>
        /// La réserve contenant les états
        /// </summary>
        private ClassPooler<State> _statesPooler;

        #endregion

        #region Constructeurs

        /// <summary>
        /// A utiliser si plusieurs machines utilisent le même ClassPooler
        /// </summary>
        /// <param name="statesPooler">L'ObjectPooler</param>
        public StateMachine(ClassPooler<State> statesPooler)
        {
            _statesPooler = statesPooler;
        }

        /// <summary>
        /// A utiliser si la machine a besoin de son propre ClassPooler
        /// </summary>
        /// <param name="pools">Les réserves à créer</param>
        public StateMachine(params IPool<State>[] pools)
        {
            _statesPooler = new ClassPooler<State>(pools);
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Nettoyage
        /// </summary>
        public void Dispose(TContext ctx)
        {
            ctx.RootState.ExitStates();
        }

        /// <summary>
        /// Crée une nouvelle hiérarchie d'états
        /// </summary>
        /// <typeparam name="TState">Le type de l'état racine</typeparam>
        /// <param name="ctx">La classe utilisant la machine</param>
        /// <param name="input">Les commandes du client</param>
        public void SetRootState<TState>(TContext ctx, TInput input) where TState : BaseState<TContext, TInput, TBaseState>, new()
        {
            ctx.RootState?.ExitStates();
            ctx.RootState = (TBaseState)GetState<TState>(ctx, input);
            ctx.RootState.EnterStates();
        }

        /// <summary>
        /// Récupère un état de la réserve pour l'ajouter à la hiérarchie
        /// </summary>
        /// <typeparam name="TState">Le type de l'état à récupérer</typeparam>
        /// <param name="ctx">La classe utilisant la machine</param>
        /// <param name="input">Les commandes du client</param>
        public BaseState<TContext, TInput, TBaseState> GetState<TState>(TContext ctx, TInput input) where TState : BaseState<TContext, TInput, TBaseState>, new()
        {
            TState state = _statesPooler.GetFromPool<TState>();
            state.Init(ctx, input, this);
            return state;
        }

        /// <summary>
        /// Renvoie l'état dans la réserve lorsqu'on ne l'utilise plus
        /// </summary>
        /// <param name="pooledState"></param>
        public void ReturnState(State pooledState)
        {
            _statesPooler.ReturnToPool(pooledState);
        }

        /// <summary>
        /// Affiche le nom de tous les états de la hiérarchie
        /// </summary>
        /// <param name="curState">L'état évalué</param>
        /// <returns>Une string au format "Etat1/Etat2/etc."</returns>
        public string GetCurStateHierarchy(BaseState<TContext, TInput, TBaseState> curState)
        {
            return curState.ToString();
        }

        #endregion
    }
}