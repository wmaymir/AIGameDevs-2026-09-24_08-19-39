using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TreeNode_Base
{
    protected List<TreeNode_Base> childrenNodes = new List<TreeNode_Base>();

    //------------------------------------------------------------------------------

    public TreeNode_Base()
    {
        //Debug.Log("TreeNode_Base - Constructor() - Override required");
    }

    //------------------------------------------------------------------------------


    //a virtual function is special in that it can be optionally overridden in a derived class
    //the runtime will decide which version of it to run based on the actual type of the object at runtime
    //it stores the tree with built in hidden pointers
    public virtual Status OnUpdate(Ghost ghost, Player player)
    {
        if (ghost)
        {
            ghost.SetCurrentAIString("TreeNode_Base");
        }

        Debug.Log("TreeNode_Base - OnUpdate() - Override required");
        return Status.RUNNING;
    }

    //------------------------------------------------------------------------------
}
