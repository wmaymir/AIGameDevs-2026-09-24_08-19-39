using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network
{
    int numberOfInputs;
    int numberOfOutputs;
    int numberOfHiddenLayers;
    int numberOfNeuronsPerHiddenLayer;

    List<Layer> layers = new List<Layer>();

    //---------------------------------------------------------------------------

    public Network(int qtyInputs, int qtyOutputs, int qtyHiddenLayers, int qtyNeuronsPerHiddenLayer)
	{
        //Delete Me.
    }

    //---------------------------------------------------------------------------

    public void Update(List<float> inputs, ref List<float> outputs)
    {
        //Delete Me.
    }

    //---------------------------------------------------------------------------

    public List<float> GetWeights()
    {
        List<float> weights = new List<float>();

        for (int i = 0; i < numberOfHiddenLayers + 1; i++)
        {
            for (int j = 0; j < layers[i].GetNumberOfNeurons(); j++)
            {
                for (int k = 0; k < layers[i].GetNeurons()[j].numberOfInputs; k++)
                {
                    weights.Add(layers[i].GetNeurons()[j].weights[k]);
                }
            }
        }

        return weights;
    }

    //---------------------------------------------------------------------------

    public void SetWeights(float[] weights)
    {
        int index = 0;

        for (int i = 0; i < numberOfHiddenLayers + 1; i++)
        {
            for (int j = 0; j < layers[i].GetNumberOfNeurons(); j++)
            {
                for (int k = 0; k < layers[i].GetNeurons()[j].numberOfInputs; k++)
                {
                    layers[i].SetWeight(j, k, weights[index]);
                    index++;
                }
            }
        }
    }

    //---------------------------------------------------------------------------

    float Sigmoid(float activation)
    {
        return 1 / (1 + Mathf.Exp(-activation));
    }

    //---------------------------------------------------------------------------

    float LinearStep(float activation)
    {
        if (activation > 0.0f)
        {
            return 1.0f;
        }
        else
        {
            return 0.0f;
        }
    }

    //---------------------------------------------------------------------------
}
