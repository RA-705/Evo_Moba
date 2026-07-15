namespace Evo.AI.SimpleBehaviorTree
{
    public class BTNode
    {
        public virtual BTResult Execute() { return BTResult.Success; }
    }
}
