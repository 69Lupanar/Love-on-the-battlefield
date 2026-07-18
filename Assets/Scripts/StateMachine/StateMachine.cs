using Assets.Scripts.ObjectPool;

namespace Assets.Scripts.StateMachine
{
    /// <summary>
    /// Chargé d'instancier et enregistrer les états de notre machine à états
    /// </summary>
    /// <typeparam name="TContext">Contient les valeurs à lire et éditer.</typeparam>
    /// <typeparam name="TInput">Le type d'InputSystem à utilsier (Clavier, Manette, etc.)</typeparam>
    public class StateMachine<TContext, TInput>
    {
        #region Instance

        /// <summary>
        /// L'état racine actuel
        /// </summary>
        private BaseState<TContext, TInput> CurRootState { get; set; }

        /// <summary>
        /// Contient les valeurs à lire et éditer
        /// </summary>
        private TContext Ctx { get; }

        /// <summary>
        /// Lit les actions du joueur
        /// </summary>
        private TInput Input { get; }

        /// <summary>
        /// La réserve contenant les états
        /// </summary>
        private ClassPooler<State> StatesPooler { get; }

        #endregion

        #region Constructeurs

        /// <summary>
        /// A utiliser si plusieurs machines utilisent le même ClassPooler
        /// </summary>
        /// <param name="context">La classe utilisant la machine</param>
        /// <param name="input">Les commandes du client</param>
        /// <param name="statesPooler">L'ObjectPooler</param>
        public StateMachine(TContext context, TInput input, ClassPooler<State> statesPooler)
        {
            Ctx = context;
            Input = input;
            StatesPooler = statesPooler;
        }

        /// <summary>
        /// A utiliser si la machine a besoin de son propre ClassPooler
        /// </summary>
        /// <param name="context">La classe utilisant la machine</param>
        /// <param name="input">Les commandes du client</param>
        /// <param name="pools">Les réserves à créer</param>
        public StateMachine(TContext context, TInput input, params IPool<State>[] pools)
        {
            Ctx = context;
            Input = input;
            StatesPooler = new ClassPooler<State>(pools);
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Crée une nouvelle hiérarchie d'états
        /// </summary>
        /// <typeparam name="TState">Le type de l'état racine</typeparam>
        public void SetRootState<TState>() where TState : BaseState<TContext, TInput>, new()
        {
            CurRootState?.ExitStates();
            CurRootState = GetState<TState>();
            CurRootState.EnterStates();
        }

        /// <summary>
        /// Callback appelé quand on màj la machine dans une méthode Update()
        /// </summary>
        public void Update()
        {
            CurRootState.UpdateStates();
        }

        /// <summary>
        /// Callback appelé quand on màj la machine dans une méthode FixedUpdate()
        /// </summary>
        public void FixedUpdate()
        {
            CurRootState.FixedUpdateStates();
        }

        /// <summary>
        /// Récupère un état de la réserve pour l'ajouter à la hiérarchie
        /// </summary>
        /// <typeparam name="TState">Le type de l'état à récupérer</typeparam>
        /// <param name="key">Le nom de l'état pour le retrouver dans la réserve, si besoin</param>
        public BaseState<TContext, TInput> GetState<TState>(string key = null) where TState : BaseState<TContext, TInput>, new()
        {
            TState state = StatesPooler.GetFromPool<TState>(key);
            state.SetContextAndInput(Ctx, Input, this);
            return state;
        }

        /// <summary>
        /// Renvoie l'état dans la réserve lorsqu'on ne l'utilise plus
        /// </summary>
        /// <param name="pooledState"></param>
        /// <param name="key"></param>
        public void ReturnState(State pooledState, string key = null)
        {
            StatesPooler.ReturnToPool(pooledState, key);
        }

        /// <summary>
        /// Affiche le nom de tous les états de la hiérarchie
        /// </summary>
        /// <returns>Une string au format "Etat1/Etat2/etc."</returns>
        public string GetCurStateHierarchy()
        {
            return CurRootState.ToString();
        }

        /// <summary>
        /// Récupère l'état tout en bas de la hiérarchie et indique s'il est du type renseigné
        /// </summary>
        /// <returns>true s'il est du type renseigné</returns>
        public bool Is<TState>() where TState : BaseState<TContext, TInput>
        {
            return CurRootState.Is<TState>();
        }

        /// <summary>
        /// Indique si l'état est racine
        /// </summary>
        /// <returns>true si l'état est racine</returns>
        public bool IsRootState<TState>(TState state) where TState : BaseState<TContext, TInput>
        {
            return ReferenceEquals(CurRootState, state);
        }

        #endregion

    }
}