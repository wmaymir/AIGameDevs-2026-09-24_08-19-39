using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer
{
	int numberOfNeurons;
	List<Neuron> neurons = new List<Neuron>();

	//---------------------------------------------------------------------------

	public Layer(int qtyOfNeurons, int qtyOfInputsPerNeuron)
	{
		//Delete Me.
	}

	//---------------------------------------------------------------------------

	public List<Neuron> GetNeurons()   
	{
		return neurons; 
	}

	//---------------------------------------------------------------------------

	public int GetNumberOfNeurons() 
	{ 
		return numberOfNeurons;
	}

	//---------------------------------------------------------------------------

	public void SetWeight(int neuronIndex, int weightIndex, float newValue) 
	{ 
		neurons[neuronIndex].weights[weightIndex] = newValue; 
	}

	//---------------------------------------------------------------------------

}
