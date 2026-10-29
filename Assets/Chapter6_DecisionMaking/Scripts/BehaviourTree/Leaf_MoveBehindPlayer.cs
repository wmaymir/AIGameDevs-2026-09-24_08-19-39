using UnityEngine;

public class Leaf_MoveBehindPlayer : TreeNode_Base
{
    private const float kMaxDuration = 10.0f;
    private float       duration     = kMaxDuration;

    //------------------------------------------------------------------------------

    public Leaf_MoveBehindPlayer()
    {
        //Leaf - No children.
    }

    //------------------------------------------------------------------------------

    public override Status OnUpdate(Ghost ghost, Player player)
    {
        //This is for visual debug output.
        ghost.AddToCombinedAIString("Leaf_MoveBehindPlayer");

        duration -= Time.deltaTime;

        ghost.ghostVisuals.SetSpriteSet(SpriteSet.Chasing);

        Vector2Int positionBehindPlayer = new Vector2Int();

        switch (player.GetDirection())
        {

            case Direction.Left:
                positionBehindPlayer = player.GetBoardPosition() + new Vector2Int(1, 0);
                break;

            case Direction.Right:
                positionBehindPlayer = player.GetBoardPosition() + new Vector2Int(-1, 0);
                break;

            case Direction.Up:
                positionBehindPlayer = player.GetBoardPosition() + new Vector2Int(0, 1);
                break;

            case Direction.Down:
                positionBehindPlayer = player.GetBoardPosition() + new Vector2Int(0, -1);
                break;
        }

        if (GameWorld.IsCellAccessible(positionBehindPlayer))
        {

            ghost.SetTargetBoardPosition(positionBehindPlayer);
        }
        else
        {

            ghost.SetTargetBoardPosition(player.GetBoardPosition());
        }
        ghost.Move();

        if (duration <= 0.0f)
        {

            duration = kMaxDuration;
            return Status.SUCCESS;
        }
        else
        {

            return Status.RUNNING;
        }
    }

    //------------------------------------------------------------------------------
}
