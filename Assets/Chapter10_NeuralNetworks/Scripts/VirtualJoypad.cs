using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualJoypad
{
    private static bool upArrowPressed	    = false;
    private static bool downArrowPressed    = false;
    private static bool leftArrowPressed    = false;
    private static bool rightArrowPressed   = false;
    private static bool firePressed         = false;

    //--------------------------------------------------------------------------------------------------

    public static bool UpArrow()    { return upArrowPressed; }
    public static bool DownArrow()  { return downArrowPressed; }
    public static bool LeftArrow()  { return leftArrowPressed; }
    public static bool RightArrow() { return rightArrowPressed; }
    public static bool Fire()		{ return firePressed; }

    //--------------------------------------------------------------------------------------------------

    public static void ResetJoypadState()
    {
        upArrowPressed      = false;
        downArrowPressed    = false;
        leftArrowPressed    = false;
        rightArrowPressed   = false;
        firePressed         = false;
    }

	//--------------------------------------------------------------------------------------------------

	public static void SetJoypadState()
	{
        upArrowPressed      = Input.GetKeyDown(KeyCode.UpArrow);
        downArrowPressed    = Input.GetKeyDown(KeyCode.DownArrow);
        leftArrowPressed    = Input.GetKeyDown(KeyCode.LeftArrow);
        rightArrowPressed   = Input.GetKeyDown(KeyCode.RightArrow);
        firePressed         = Input.GetKeyDown(KeyCode.Space);
    }

	//--------------------------------------------------------------------------------------------------
}
