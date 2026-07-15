namespace Evo.AI.SimpleBehaviorTree
{
    public class BTSelectNode : BTNode
    {
        private BTNode[] _children;
        private int _index;

        public BTSelectNode(params BTNode[] children)
        {
            _children = children;
            _index = 0;
        }

        public override BTResult Execute()
        {
            if (_children == null || _children.Length == 0)
                return BTResult.Failure;

            while (_index < _children.Length)
            {
                var result = _children[_index].Execute();
                if (result == BTResult.Running)
                    return BTResult.Running;
                if (result == BTResult.Success)
                {
                    _index = 0;
                    return BTResult.Success;
                }
                _index++;
            }
            _index = 0;
            return BTResult.Failure;
        }

        public void Reset() { _index = 0; }
    }
}
