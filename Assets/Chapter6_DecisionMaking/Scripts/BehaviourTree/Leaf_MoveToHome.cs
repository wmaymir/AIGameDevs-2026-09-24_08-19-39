using UnityEngine;

public class Leaf_MoveToHome : TreeNode_Base
{
    private Vector2Int homePos = new Vector2Int(14, 13);

    //------------------------------------------------------------------------------

    public Leaf_MoveToHome()
    {
        //Leaf - No children.
    }

    //------------------------------------------------------------------------------

    public override Status OnUpdate(Ghost ghost, Player player)
    {
        //This is for visual debug output.
        ghost.AddToCombinedAIString("Leaf_MoveToHome");

        //Change visuals of ghost to show only eyes
        ghost.ghostVisuals.SetSpriteSet(SpriteSet.Eyes);

        //Set desired position to home position
        ghost.SetTargetBoardPosition(homePos);

        //Move ghost
        ghost.MoveHome();

        //Failure is not an option, so check if ghost is at home position, and if so, return SUCCESS
        if (ghost.GetBoardPosition() == homePos || !ghost.HasBeenEaten())
        {
            ghost.SetEaten(false);
            return Status.SUCCESS;
        }

        //Otherwise, we must still be trying to go home, so return RUNNING
        else
        {
            return Status.RUNNING;
        }
    }

    //------------------------------------------------------------------------------
}