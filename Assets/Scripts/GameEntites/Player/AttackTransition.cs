using System;
using DUCK.FSM;
using UnityEngine;
using System.Threading.Tasks;

namespace rqgames.GameEntities.Playable
{
    public class AttackTransition : Transition<Player.PlayerStates>
    {
        public AttackTransition(Player.PlayerStates from, Player.PlayerStates to, Func<bool> testConditionFunction = null)
            : base(from, to, testConditionFunction)

        {
        }

        public override void Begin()
        {
            Debug.Log("start fire");
            Task.Run(Finish);
        }

        private void Finish()
        {
            Task.Delay(1000).Wait();
            Debug.Log("end fire");
            Complete();
        }
    }
}