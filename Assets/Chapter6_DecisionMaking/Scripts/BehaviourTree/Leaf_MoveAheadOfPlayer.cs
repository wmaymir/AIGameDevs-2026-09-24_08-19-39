using UnityEngine;

public class Leaf_MoveAheadOfPlayer : TreeNode_Base
{
    private const float kMaxDuration = 10.0f;
    private float       duration     = kMaxDuration;

    //------------------------------------------------------------------------------

    public Leaf_MoveAheadOfPlayer()
    {
        //Leaf - No children.
    }

    //------------------------------------------------------------------------------

    public override Status OnUpdate(Ghost ghost, Player player)
    {
        duration -= Time.deltaTime;

        ghost.ghostVisuals.SetSpriteSet(SpriteSet.Chasing);

        Vector2Int positionAheadOfPlayer = new Vector2Int();
        
        switch (player.GetDirection())
        {

            case Direction.Left:
                positionAheadOfPlayer = player.GetBoardPosition() + new Vector2Int(-1, 0);
                break;

            case Direction.Right:
                positionAheadOfPlayer = player.GetBoardPosition() + new Vector2Int(1, 0);
                break;

            case Direction.Up:
                positionAheadOfPlayer = player.GetBoardPosition() + new Vector2Int(0, -1);
                break;

            case Direction.Down:
                positionAheadOfPlayer = player.GetBoardPosition() + new Vector2Int(0, 1);
                break;
        }

        if (GameWorld.IsCellAccessible(positionAheadOfPlayer))
        {

            ghost.SetTargetBoardPosition(positionAheadOfPlayer);
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