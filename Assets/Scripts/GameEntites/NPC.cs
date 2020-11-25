using System;
using System.Collections.Generic;
using DUCK.FSM;
using UnityEngine;

namespace rqgames.GameEntities
{
    public class NPC : MonoBehaviour, IPooledGameEntities
    {
        protected FiniteStateMachine<States> _fsm;
        private const string IDLE_COMMAND = "idle";
        private const string ATTACK_COMMAND = "move";

        public Stack<GameObject> Container { get; set; }

        public enum States
        {
            Idle,
            Attack
        }

        private void Start()
        {
            AttackTransition _atk = new AttackTransition(States.Idle, States.Attack);

            _fsm = FiniteStateMachine<States>.FromEnum();
            _fsm.AddTransition(States.Idle, States.Attack, ATTACK_COMMAND, _atk);
            _fsm.AddTransition(States.Attack, States.Idle, IDLE_COMMAND);
            _fsm.Begin(States.Idle);
            InitNPC();
        }

        virtual protected void InitNPC()
        {

        }

        public void OnDie()
        {
            Container.Push(this.gameObject);
        }
    }

    public class AttackTransition : Transition<NPC.States>
    {
        public AttackTransition(NPC.States from, NPC.States to, Func<bool> testConditionFunction = null)
            : base (from, to, testConditionFunction) { }

        public override void Begin()
        {
            throw new System.NotImplementedException("i shoot");
        }
    }
}