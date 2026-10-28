using UnityEngine;

public class Leaf_ExitHome : TreeNode_Base
{
    private Vector2Int exitPos = new Vector2Int(14, 11);

    //------------------------------------------------------------------------------

    public Leaf_ExitHome()
    {
        //Leaf - No children.
    }

    //------------------------------------------------------------------------------

    public override Status OnUpdate(Ghost ghost, Player player)
    {
        //this is for visual debug output
        ghost.AddToCombinedAIString("Leaf_ExitHome");

        //changes the ghost's sprite to chasing
        ghost.ghostVisuals.SetSpriteSet(SpriteSet.Chasing);

        //sets the ghost's target position for the exit
        ghost.SetTargetBoardPosition(exitPos);

        //MoveHome allows for movement in AND out of the home
        ghost.MoveHome();

        //failure is not an option for this behavior. Ghost has either SUCCESS or RUNNING
        if (ghost.GetBoardPosition() == exitPos)
        {
            return Status.SUCCESS;
        }
        else
        {
            return Status.RUNNING;
        }
    }

    //------------------------------------------------------------------------------
}