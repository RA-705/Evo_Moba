using System;

namespace Evo.AI.SimpleBehaviorTree
{
    public class BTConditionNode : BTNode
    {
        private Func<bool> _condition;

        public BTConditionNode(Func<bool> condition)
        {
            _condition = condition;
        }

        public override BTResult Execute()
        {
            return _condition != null && _condition() ? BTResult.Success : BTResult.Failure;
        }
    }
}
