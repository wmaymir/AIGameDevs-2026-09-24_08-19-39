using UnityEngine;

public class Leaf_MoveToPlayer : TreeNode_Base
{
    private const float kMaxDuration = 10.0f;
    private float       duration     = kMaxDuration;

    //------------------------------------------------------------------------------

    public Leaf_MoveToPlayer()
    {
        //Leaf - No children.
    }

    //------------------------------------------------------------------------------

    public override Status OnUpdate(Ghost ghost, Player player)
    {

        duration -= Time.deltaTime;

        ghost.ghostVisuals.SetSpriteSet(SpriteSet.Chasing);
        ghost.SetTargetBoardPosition(player.GetBoardPosition());
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