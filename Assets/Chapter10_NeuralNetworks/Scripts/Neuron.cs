using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron
{
	public int numberOfInputs;
	public List<float> weights = new List<float>();
	public float fBias = 1.0f;

	//--------------------------------------------------------------------------------------------------

	public Neuron(int numOfInputs, float bias)
	{
		//Delete Me.
	}

	//--------------------------------------------------------------------------------------------------

	public float GetBias()
	{
		return fBias;
	}

	//--------------------------------------------------------------------------------------------------
}
