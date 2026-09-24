/*
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
        numberOfInputs = qtyInputs;
        numberOfOutputs = qtyOutputs;
        numberOfHiddenLayers = qtyHiddenLayers;
        numberOfNeuronsPerHiddenLayer = qtyNeuronsPerHiddenLayer;

        //Create the network.
        if (numberOfHiddenLayers > 0)
        {
            //Create the first hidden layer - Takes a set number of inputs.
            layers.Add(new Layer(numberOfNeuronsPerHiddenLayer, numberOfInputs));

            //Create subsequent hidden layers - Takes the number of neurons per hidden layer as inputs and outputs.
            for (int i = 0; i < numberOfHiddenLayers - 1; ++i)
            {
                layers.Add(new Layer(numberOfNeuronsPerHiddenLayer, numberOfNeuronsPerHiddenLayer));
            }

            //Create the last hidden layer - Takes the number of neurons per hidden layer as inputs and outputs required number of outputs.
            layers.Add(new Layer(numberOfOutputs, numberOfNeuronsPerHiddenLayer));
        }
        else
        {
            //Otherwise just create a single output layer.
            layers.Add(new Layer(numberOfOutputs, numberOfInputs));
        }
    }

    //---------------------------------------------------------------------------

    public void Update(List<float> inputs, ref List<float> outputs)
    {
        //Work through each layer. +1 to include output layer. inputs = input layer.
        for (int i = 0; i < numberOfHiddenLayers + 1; ++i)
        {
            if (i > 0)
            {
                inputs.Clear();

                //The inputs to the next layer are the outputs from the previous layer.
                for (int index = 0; index < outputs.Count; index++)
                {
                    inputs.Add(outputs[index]);
                }
            }
            outputs.Clear();

            //For each neuron, calculate the input*weight.
            //Total goes through the activation function.
            for (int j = 0; j < layers[i].GetNumberOfNeurons(); j++)
            {
                //Debug.Log("Neuron: " + j + " ");
                float netInput = 0.0f;
                int inputCount = layers[i].GetNeurons()[j].numberOfInputs;

                for (int k = 0; k < inputCount; ++k)
                {
                    //Sum the inputs*weights.
                    netInput += layers[i].GetNeurons()[j].weights[k] * inputs[k];
                }

                //Debug.Log("netInput: " + netInput + " ");

                //Don't forget the BIAS.
                netInput += layers[i].GetNeurons()[j].GetBias();

                //Debug.Log("netInput (BIAS): " + netInput + " ");

                //Store the outputs as we go.
                outputs.Add(Sigmoid(netInput));

                //Debug.Log("output: " + outputs[outputs.Count-1] + " ");
            }
        }
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
*/