using UnityEngine;

public class Leaf_MoveToRandomPosition : TreeNode_Base
{
    private const float kMaxDuration = 10.0f;
    private float duration = kMaxDuration;

    Vector2Int randomPosition = new Vector2Int();

    //------------------------------------------------------------------------------

    public Leaf_MoveToRandomPosition()
    {
        //Leaf - No children.

        //This is for visual debug output.
        ResetRandomPosition();
    }

    //------------------------------------------------------------------------------

    private void ResetRandomPosition()
    {
        System.Random rnd = new System.Random();
        randomPosition.x = rnd.Next(1, GameWorld.BoardRows - 2);
        randomPosition.x = rnd.Next(1, GameWorld.BoardColumns - 2);
    }

    //------------------------------------------------------------------------------

    public override Status OnUpdate(Ghost ghost, Player player)
    {
        ghost.AddToCombinedAIString("Leaf_MoveToRandomPosition");

        //Deduct current duration value by delta time
        duration -= Time.deltaTime;

        //Set correct visuals for ghost
        ghost.ghostVisuals.SetSpriteSet(SpriteSet.Chasing);

        //Set already selcted random position to move to
        ghost.SetTargetBoardPosition(randomPosition);

        //Move ghost to desired position
        ghost.Move();

        //After a desired duration, we will return success to allow parent node to select another way to chase.
        if (duration <= 0.0f)
        {
            duration = kMaxDuration;
            ResetRandomPosition();

            return Status.SUCCESS;
        }

        //Otherwise, keep running this leaf node
        else
        {
            return Status.RUNNING;
        }
    }

    //------------------------------------------------------------------------------
}
