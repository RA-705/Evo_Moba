namespace Evo.AI.SimpleBehaviorTree
{
    public class BTLoopNode : BTNode
    {
        private BTNode _child;
        private int _maxLoops;
        private int _currentLoop;

        public BTLoopNode(BTNode child, int maxLoops = -1)
        {
            _child = child;
            _maxLoops = maxLoops;
            _currentLoop = 0;
        }

        public override BTResult Execute()
        {
            if (_child == null) return BTResult.Failure;
            if (_maxLoops > 0 && _currentLoop >= _maxLoops)
            {
                _currentLoop = 0;
                return BTResult.Success;
            }
            var result = _child.Execute();
            if (result == BTResult.Failure)
            {
                _currentLoop = 0;
                return BTResult.Failure;
            }
            _currentLoop++;
            return BTResult.Running;
        }
    }
}
