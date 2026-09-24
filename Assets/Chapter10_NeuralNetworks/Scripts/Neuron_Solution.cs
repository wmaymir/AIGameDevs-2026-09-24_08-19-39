/*
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
		//Store the parameters in member variables.
		numberOfInputs = numOfInputs;
		fBias = bias;

		for (int i = 0; i < numOfInputs; ++i)
		{
			weights.Add(Random.Range(-1.0f, 1.0f));
		}
	}

	//--------------------------------------------------------------------------------------------------

	public float GetBias()
	{
		return fBias;
	}

	//--------------------------------------------------------------------------------------------------
}
*/