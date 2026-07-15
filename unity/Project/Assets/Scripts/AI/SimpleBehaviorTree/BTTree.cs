namespace Evo.AI.SimpleBehaviorTree
{
    public class BTTree
    {
        public BTNode Root { get; protected set; }

        public virtual void BuildTree() { }

        public BTResult Execute()
        {
            if (Root == null)
            {
                BuildTree();
                if (Root == null) return BTResult.Failure;
            }
            return Root.Execute();
        }

        public void Reset()
        {
            if (Root is BTSequenceNode seq) seq.Reset();
            else if (Root is BTSelectNode sel) sel.Reset();
        }
    }
}
