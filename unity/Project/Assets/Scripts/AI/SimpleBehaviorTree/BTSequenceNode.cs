namespace Evo.AI.SimpleBehaviorTree
{
    public class BTSequenceNode : BTNode
    {
        private BTNode[] _children;
        private int _index;

        public BTSequenceNode(params BTNode[] children)
        {
            _children = children;
            _index = 0;
        }

        public override BTResult Execute()
        {
            if (_children == null || _children.Length == 0)
                return BTResult.Success;

            while (_index < _children.Length)
            {
                var result = _children[_index].Execute();
                if (result == BTResult.Running)
                    return BTResult.Running;
                if (result == BTResult.Failure)
                {
                    _index = 0;
                    return BTResult.Failure;
                }
                _index++;
            }
            _index = 0;
            return BTResult.Success;
        }

        public void Reset() { _index = 0; }
    }
}
