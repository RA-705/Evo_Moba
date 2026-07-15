using System;

namespace Evo.AI.SimpleBehaviorTree
{
    public class BTActionNode : BTNode
    {
        private Func<BTResult> _action;

        public BTActionNode(Func<BTResult> action)
        {
            _action = action;
        }

        public override BTResult Execute()
        {
            return _action != null ? _action() : BTResult.Failure;
        }
    }
}
