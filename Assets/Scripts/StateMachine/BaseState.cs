using System.Text;

namespace Assets.Scripts.StateMachine
{
    /// <summary>
    /// L'état de base dont tous les autres états devront dériver
    /// </summary>
    /// <typeparam name="TContext">Contient les valeurs à lire et éditer.</typeparam>
    /// <typeparam name="TInput">Le type d'InputSystem à utilsier (Clavier, Manette, etc.)</typeparam>
    public abstract class BaseState<TContext, TInput, TBaseState> : State
        where TContext : IStateContext<TContext, TInput, TBaseState>
        where TInput : IStateInput
        where TBaseState : BaseState<TContext, TInput, TBaseState>
    {
        #region Instance

        /// <summary>
        /// Contient les valeurs à lire et éditer
        /// </summary>
        protected TContext Ctx { get; private set; }

        /// <summary>
        /// Lit les actions du joueur
        /// </summary>
        protected TInput Input { get; private set; }

        /// <summary>
        /// Instancie et active les états
        /// </summary>
        protected StateMachine<TContext, TInput, TBaseState> Factory { get; private set; }

        /// <summary>
        /// L'état suivant dans la hiérarchie
        /// </summary>
        private BaseState<TContext, TInput, TBaseState> SubState { get; set; }

        /// <summary>
        /// L'état précédent dans la hiérarchie
        /// </summary>
        private BaseState<TContext, TInput, TBaseState> SuperState { get; set; }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Sets TContext, TInput and the factory manually when we retrieve a state from the factory
        /// </summary>
        /// <param name="context">Contient les valeurs à lire et éditer</param>
        /// <param name="input">Lit les actions du joueur</param>
        /// <param name="factory"> Instancie et active les états</param>
        public void Init(TContext context, TInput input, StateMachine<TContext, TInput, TBaseState> factory)
        {
            Ctx = context;
            Input = input;
            Factory = factory;
        }

        /// <summary>
        /// Sets the context and input
        /// </summary>
        /// <param name="context">Contient les valeurs à lire et éditer</param>
        /// <param name="input">Lit les actions du joueur</param>
        public void SetContextAndInput(TContext context, TInput input)
        {
            Ctx = context;
            Input = input;
            SubState?.SetContextAndInput(context, input);
        }

        /// <summary>
        /// Entre dans l'état pour l'initialiser
        /// </summary>
        public void EnterStates()
        {
            InitSubState();
            Enter();
        }

        /// <summary>
        /// Quitte l'état et le renvoie dans la machine
        /// </summary>
        public void ExitStates()
        {
            SubState?.ExitStates();
            Exit();

            Factory.ReturnState(this);
        }

        /// <summary>
        /// Màj l'état depuis l'Update() de la classe cliente
        /// </summary>
        public void UpdateStates()
        {
            Update();
            SubState?.UpdateStates();
            CheckSwitchStates();
        }

        /// <summary>
        /// Màj l'état depuis la FixedUpdate() de la classe cliente
        /// </summary>
        public void FixedUpdateStates()
        {
            FixedUpdate();
            SubState?.FixedUpdateStates();
        }

        /// <summary>
        /// Affiche le nom de tous les états de la hiérarchie
        /// </summary>
        /// <param name="sb">Le StringBuilder, à laisser null</param>
        /// <returns>Une string au format "Etat1/Etat2/etc."</returns>
        public string ToString(StringBuilder sb = null)
        {
            sb ??= new StringBuilder(100);
            sb.Append(GetType().Name);

            if (SubState != null)
            {
                sb.Append("/");
                SubState.ToString(sb);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Récupère l'état tout en bas de la hiérarchie et renvoie TRUE s'il est du type renseigné
        /// </summary>
        public bool Is<TState>() where TState : BaseState<TContext, TInput, TBaseState>
        {
            if (SubState != null)
            {
                return SubState.Is<TState>();
            }

            return this is TState;
        }

        #endregion

        #region Fonctions protégées

        /// <summary>
        /// Change l'état actuel (racine ou non) 
        /// lorsque les conditions implémentées dans la classe fille sont remplies
        /// </summary>
        protected virtual void CheckSwitchStates() { }

        /// <summary>
        /// Assigne un sous-état si besoin
        /// </summary>
        protected virtual void InitSubState() { }

        /// <summary>
        /// Echange l'état actuel par un autre du type renseigné
        /// </summary>
        /// <typeparam name="TState">Le type du nouvel état</typeparam>
        protected void SwitchState<TState>() where TState : BaseState<TContext, TInput, TBaseState>, new()
        {
            // Quitte l'état actuel ainsi que tous ses sous-états
            ExitStates();

            if (SuperState != null)
            {
                // Si c'est un état racine, on démarre une nouvelle hiérarchie depuis la machine
                Factory.SetRootState<TState>(Ctx, Input);
            }
            else
            {
                //Sinon, on transfère notre super-état à ce nouvel état
                SuperState.SetSubState<TState>();
            }
        }

        /// <summary>
        /// Assigne un sous-état à cet état
        /// </summary>
        /// <typeparam name="TState">L'état du sous-état</typeparam>
        protected void SetSubState<TState>() where TState : BaseState<TContext, TInput, TBaseState>, new()
        {
            TState newSubState = (TState)Factory.GetState<TState>(Ctx, Input);
            SubState = newSubState;
            newSubState.SetSuperState(this);
            newSubState.EnterStates();
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Assigne un super-état à celui-ci
        /// </summary>
        /// <param name="newSuperState">Le super-état qui dirige celui-ci</param>
        private void SetSuperState(BaseState<TContext, TInput, TBaseState> newSuperState)
        {
            SuperState = newSuperState;
        }


        #endregion
    }
}