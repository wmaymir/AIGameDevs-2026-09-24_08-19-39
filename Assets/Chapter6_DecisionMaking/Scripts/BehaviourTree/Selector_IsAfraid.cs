using UnityEngine;

public class Selector_IsAfraid : TreeNode_Base
{
    //------------------------------------------------------------------------------

    public Selector_IsAfraid()
    {
        //becomes node 0
        childrenNodes.Add(new Leaf_ExitHome());
        //becomes node 1
        childrenNodes.Add(new Leaf_Evade());
        //becomes node 2
        childrenNodes.Add(new Selector_Chase());
    }

    //------------------------------------------------------------------------------

    public override Status OnUpdate(Ghost ghost, Player player)
    {
        ghost.AddToCombinedAIString("Selector_IsAfraid");

        if (ghost.IsInHome())
        {
            return childrenNodes[0].OnUpdate(ghost, player);
        }

        if (ghost.IsPowerPillActive())
        {
            return childrenNodes[1].OnUpdate(ghost, player);
        }
        else
        {
            return childrenNodes[2].OnUpdate(ghost, player);
        }
    }

    //------------------------------------------------------------------------------
}