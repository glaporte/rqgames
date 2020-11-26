using System;
using DUCK.FSM;
using System.Threading.Tasks;

namespace rqgames.GameEntities.Playable
{
    public class DelayTransition : Transition<FSMCommon.State>
    {
        private int _duration;
        public DelayTransition(FSMCommon.State from, FSMCommon.State to, int duration = 20, Func<bool> testConditionFunction = null)
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