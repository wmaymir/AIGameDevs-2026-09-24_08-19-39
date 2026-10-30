using UnityEngine;

public class Leaf_Evade : TreeNode_Base
{
    Vector2Int randomEvadePosition = new Vector2Int();

    //------------------------------------------------------------------------------

    public Leaf_Evade()
    {
        //Leaf - No children.

        //Choose a random position off the map to evade to - The ghost will never reach it,
        //but will keep circling the map in search of it.
        System.Random rnd = new System.Random();
        randomEvadePosition.x = rnd.Next(0,1) * GameWorld.BoardRows;
        randomEvadePosition.x = rnd.Next(0, 1) * GameWorld.BoardColumns;
    }

    //------------------------------------------------------------------------------

    public override Status OnUpdate(Ghost ghost, Player player)
    {
        //This is for visual debug output.
        ghost.AddToCombinedAIString("Leaf_Evade");

        //Change visuals of ghost to show they are evading
        ghost.ghostVisuals.SetSpriteSet(SpriteSet.Evading);

        //Set desired position to randomly generated position that is outside of environment
        ghost.SetTargetBoardPosition(randomEvadePosition);

        //Move ghost
        ghost.Move();

        //Since the ghost will never reach this position, always return status of RUNNING
        return Status.RUNNING;
    }

    //------------------------------------------------------------------------------
}
