using UnityEngine;

public class Selector_Chase : TreeNode_Base
{
    private int previousRunningNode = 0;

    //------------------------------------------------------------------------------

    public Selector_Chase()
    {
        //the four ghosts' behaviors
        childrenNodes.Add(new Leaf_MoveToPlayer());

        childrenNodes.Add(new Leaf_MoveAheadOfPlayer());

        childrenNodes.Add(new Leaf_MoveBehindPlayer());

        childrenNodes.Add(new Leaf_MoveToRandomPosition());

        //generates a random index from the number of ghosts to use as the
        //child for the tree to flow through
        System.Random rnd = new System.Random();
        previousRunningNode = rnd.Next(0, childrenNodes.Count);
    }

    //------------------------------------------------------------------------------

    public override Status OnUpdate(Ghost ghost, Player player)
    {
        ghost.AddToCombinedAIString("Selector_Chase");

        //returns a status useful for if we need a different child behavior to run on the next frame
        Status ghostStatus = childrenNodes[previousRunningNode].OnUpdate(ghost, player);

        //if the status is sucess we can return a random index again like in the constructor method
        if (ghostStatus == Status.SUCCESS)
        {
            System.Random rnd = new System.Random();

            previousRunningNode = rnd.Next(0, childrenNodes.Count);
        }

        return ghostStatus;
    }

    //------------------------------------------------------------------------------
}