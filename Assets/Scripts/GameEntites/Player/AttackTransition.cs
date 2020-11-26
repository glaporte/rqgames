using System;
using DUCK.FSM;
using UnityEngine;
using System.Threading.Tasks;

namespace rqgames.GameEntities.Playable
{
    public class TimedTransition : Transition<Player.PlayerStates>
    {
        private int _duration;
        public TimedTransition(Player.PlayerStates from, Player.PlayerStates to, int duration, Func<bool> testConditionFunction = null)
            : base(from, to, testConditionFunction)

        {
            _duration = duration;
        }

        public override void Begin()
        {
            Task.Run(Finish);
        }

        private void Finish()
        {
            Task.Delay(_duration).Wait();
            Complete();
        }
    }
}