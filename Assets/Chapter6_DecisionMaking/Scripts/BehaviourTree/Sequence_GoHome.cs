using UnityEngine;

public class Sequence_GoHome : TreeNode_Base
{
    //------------------------------------------------------------------------------

    public Sequence_GoHome()
    {
        //Add relevant steps to sequence
        childrenNodes.Add(new Leaf_MoveToHome());
        childrenNodes.Add(new Leaf_ExitHome());
    }

    //------------------------------------------------------------------------------

    public override Status OnUpdate(Ghost ghost, Player player)
    {
        ghost.AddToCombinedAIString("Sequence_GoHome");

        //Keep track of whether a node is running or not
        bool childIsRunning = false;

        //Loop through all children nodes until all have returned successfully or one has failed
        for (int i = 0; i < childrenNodes.Count; i++)
        {
            //For each child node, call OnUpdate() function.
            switch (childrenNodes[i].OnUpdate(ghost, player))
            {
                //Return info to calling node in Behaviour Tree
                case Status.FAILURE:
                    return Status.FAILURE;
                    break;
                //Exit switch statement and iterate loop again
                case Status.SUCCESS:
                    continue;
                    break;
                //If still running, set appropriate flag and exit for loop
                case Status.RUNNING:
                    i = childrenNodes.Count;
                    childIsRunning = true;
                    continue;
                    break;
                //Any other scenario returns SUCCESS
                default:
                    break;
            }
        }
        //Determine if one the child nodes is still running, and if so set current status to RUNNING. If not, SUCCESS. Return to calling parent node.
        return childIsRunning ? Status.RUNNING : Status.SUCCESS;
    }

    //------------------------------------------------------------------------------
}