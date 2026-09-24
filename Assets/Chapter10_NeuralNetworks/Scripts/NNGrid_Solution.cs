/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NNGrid : MonoBehaviour
{
    //Number of rows and columns - Square grid.
    const int kSnakeBoardDimensions = 16;

    const float kStartingMovementDelay = 0.5f;
    public float AiSpeedDelay = 0.0f;
    const int kMinUpdateDelay = 50;
    const int kDelayReductionAmount = 10;
    const int kTimeAllowedAfterAPickup = 10;	//Seconds											

    //Evolutionary Neural Network.
    const int kNumberOfNNSnakes = 50;
    const int kNumberOfInputNodes_Snake = 12;
    const int kNumberOfHiddenNodes_Snake = 4;
    const int kNumberOfOutputNodes_Snake = 4;
    const int kNumberOfWeights_Snake = (kNumberOfInputNodes_Snake * kNumberOfHiddenNodes_Snake) + (kNumberOfHiddenNodes_Snake * kNumberOfOutputNodes_Snake);

    const float kCrossoverRate_Snake = 30.0f;            
    const float kMutationRate_Snake = 0.1f;			            

    //Fitness scores.
    const float kScoreModifierWeight = 1.0f;
    const float kScoreForMovement = 0.0001f;

    //--------------------------------------------------------------------------------------

    public float cellSize = 0.0f;
    public float halfCellSize = 0.0f;

    [SerializeField] private GameObject cellPrefab = null;
    [SerializeField] private Transform cellParent = null;
    [SerializeField] private Transform topLeftCorner = null;

    [SerializeField] private GameObject pausedIndicator = null;
    [SerializeField] private Text generationText = null;
    [SerializeField] private Text snakeText = null;

    //Array of what is actually in each cell.
    private NNTileType[,] gridLayout;

    //This is an array of references to images to allow us to change the colour.
    public NNCell[,] gridVisuals;

    private GridPosition pickupPosition;
    private List<GridPosition> snake = new List<GridPosition>();

    private MovementDirection currentMovementDirection = MovementDirection.Left;
    private MovementDirection proposedMovementDirection = MovementDirection.Left;
    private float timeUntilMove = 0.0f;
    private float movementDelay;

    private bool paused = false;
    private float levelTime = 0.0f;

    //--------------------------------------------------------------------------------------------------
    // AI variables.
    //--------------------------------------------------------------------------------------------------
    List<Network> neuralNetworks = new List<Network>(kNumberOfNNSnakes);
    bool bAIOn;

    float[][] chromosomes = new float[kNumberOfNNSnakes][];//[kNumberOfWeights_Snake];
    float[][] selectedChromosomes = new float[kNumberOfNNSnakes][]; //[kNumberOfWeights_Snake];
	int currentChromosome;
    float[] fitnessValues = new float[kNumberOfNNSnakes];

    bool bAllowMutation;
    int generation;

    //--------------------------------------------------------------------------------------

    void Start()
    {
        halfCellSize = cellSize / 2.0f;
        CreateBoard();

        SetUpChromosomes();
        SetUpNeuralNetwork();
        for (int i = 0; i < kNumberOfNNSnakes; i++)
        {
            fitnessValues[i] = 0;
        }
        bAllowMutation = true;

        ResetGrid();
        ResetSnake();
        ResetPickup();

        currentChromosome = 0;
        snakeText.text = "Snake: " + currentChromosome;
        generation = 1;
        generationText.text = "Generation: " + generation;

        //Start game in frozen state.
        paused = true;
        if(pausedIndicator != null)
        {
            pausedIndicator.SetActive(true);
        }
    }

    //--------------------------------------------------------------------------------------

    public void StartUserGame()
    {
        //With AI off.
        bAIOn = false;
        movementDelay = movementDelay = kStartingMovementDelay;
    }

    //--------------------------------------------------------------------------------------

    public void StartAiGame()
    {
        //With AI on.
        bAIOn = true;// false;
        movementDelay = AiSpeedDelay;
    }

    //--------------------------------------------------------------------------------------

    public void Quit()
    {
        SceneManager.LoadScene("Chapter10_NeuralNetworks");
    }

    //--------------------------------------------------------------------------------------

    private void Update()
    {
        //Always update the virtual joypad.
        VirtualJoypad.SetJoypadState();

        //Flip paused state when fire is pressed.
        if (VirtualJoypad.Fire())
        {
            paused = !paused;

            //Turn the pause indicator on/off.
            if (pausedIndicator != null)
            {
                pausedIndicator.SetActive(paused);
            }
        }

        //If not paused, update the game.
        if (!paused)
        {
            UpdateGrid();

            //Update Ai snake or user control snake?
            if (bAIOn)
            {
                UpdateSnakeAI();
            }
            else
            {
                UpdateSnake();
            }
        }
    }

    //--------------------------------------------------------------------------------------

    void CreateBoard()
    {
        if (cellPrefab != null && cellParent != null && topLeftCorner != null)
        {
            //Chess board is always 8x8
            gridLayout = new NNTileType[kSnakeBoardDimensions, kSnakeBoardDimensions];
            gridVisuals = new NNCell[kSnakeBoardDimensions, kSnakeBoardDimensions];

            //Set all cells to be null.
            for (int iRow = 0; iRow < kSnakeBoardDimensions; iRow++)
            {
                for (int iCol = 0; iCol < kSnakeBoardDimensions; iCol++)
                {
                    Vector3 startPosition = topLeftCorner.position + new Vector3((iCol * cellSize) + halfCellSize, (iRow * -cellSize) - halfCellSize, 0.0f);
                    GameObject newCell = Instantiate(cellPrefab, startPosition, Quaternion.identity, cellParent);

                    NNCell cellImage = newCell.GetComponent<NNCell>();
                    if (cellImage != null)
                    {
                        gridVisuals[iRow, iCol] = cellImage;
                    }

                    //Internal grid data.
                    gridLayout[iRow, iCol] = NNTileType.Empty;
                }
            }
        }
    }

    //--------------------------------------------------------------------------------------

    void SetCell(int iRow, int iCol, NNTileType type)
    {
        if (iRow >= 0 && iRow < kSnakeBoardDimensions && iCol >= 0 && iCol < kSnakeBoardDimensions)
        {
            if (type != gridLayout[iRow, iCol])
            {
                //Set the new type.
                gridLayout[iRow, iCol] = type;

                //Set the appropriate visuals.
                switch (type)
                {
                    case NNTileType.Blocked:gridVisuals[iRow, iCol].cellVisual.color = Color.black;     break;
                    case NNTileType.Empty:  gridVisuals[iRow, iCol].cellVisual.color = Color.white;     break;
                    case NNTileType.PickUp: gridVisuals[iRow, iCol].cellVisual.color = Color.grey;      break;
                    case NNTileType.Head:   gridVisuals[iRow, iCol].cellVisual.color = Color.red;       break;
                    case NNTileType.Body:   gridVisuals[iRow, iCol].cellVisual.color = Color.blue;      break;
                    case NNTileType.Tail:   gridVisuals[iRow, iCol].cellVisual.color = Color.green;     break;
                }
            }
        }
    }

    //--------------------------------------------------------------------------------------

    void ResetGrid()
    {
        for (int iRow = 0; iRow < kSnakeBoardDimensions; iRow++)
        {
            for (int iCol = 0; iCol < kSnakeBoardDimensions; iCol++)
            {
                //Set appropriate cell type.
                if (iRow == 0 || iCol == 0 || iRow == kSnakeBoardDimensions - 1 || iCol == kSnakeBoardDimensions - 1)
                {
                    //Add a border around the outside.
                    SetCell(iRow, iCol, NNTileType.Blocked);
                }
                else
                {
                    //Empty inside the grid.
                    SetCell(iRow, iCol, NNTileType.Empty);
                }
            }
        }
    }

    //--------------------------------------------------------------------------------------

    void ResetSnake()
    {
        snake.Clear();

        //Snake always starts with 2 elements at the centre of the map.
        snake.Add(new GridPosition(kSnakeBoardDimensions / 2, kSnakeBoardDimensions / 2));
        snake.Add(new GridPosition(snake[0].row, snake[0].column+1));

        //Starts moving left.
        currentMovementDirection = MovementDirection.Left;
        proposedMovementDirection = MovementDirection.Left;

        //Clear the inputs from the virtual joypad.
        VirtualJoypad.ResetJoypadState();
    }

    //--------------------------------------------------------------------------------------

    void ResetPickup()
    {
        int row, col;
        bool placedPickupOnBody = false;
        do
        {
            placedPickupOnBody = false;

            row = Random.Range(1, kSnakeBoardDimensions - 2);
            col = Random.Range(1, kSnakeBoardDimensions - 2);

            for (int i = 0; i < snake.Count; i++)
            {
                if (row == snake[i].row && col == snake[i].column)
                {
                    placedPickupOnBody = true;
                    break;
                }
            }

        } while (gridLayout[row, col] != NNTileType.Empty || placedPickupOnBody);

        //Set this cell as containing the pickup.
        SetCell(row, col, NNTileType.PickUp);

        //Remove the old pick up.
        SetCell(pickupPosition.row, pickupPosition.column, NNTileType.Empty);

        //Position used in NN to determine where aboouts the pckup is without searching the whole grid.
        pickupPosition.row = row;
        pickupPosition.column = col;

        //Ai - Allow an amount of time after a pickup before quiting this snake run through.
        levelTime = kTimeAllowedAfterAPickup;
    }

    //--------------------------------------------------------------------------------------------------

    void UpdateGrid()
    {
        //---------------------------------------------------------------------------------------------
        //Draw the grid.
        ResetGrid();

        //---------------------------------------------------------------------------------------------
        //Draw the snake.
        if (snake.Count > 0)
        {
            //First the head.
            SetCell(snake[0].row, snake[0].column, NNTileType.Head);

            //Incorporate snake body.
            for (int i = 1; i < snake.Count - 1; i++)
            {
                SetCell(snake[i].row, snake[i].column, NNTileType.Body);
            }

            //Draw snake tail.
            SetCell(snake[snake.Count - 1].row, snake[snake.Count - 1].column, NNTileType.Tail);
        }

        //---------------------------------------------------------------------------------------------
        //Draw the pickup.
        SetCell(pickupPosition.row, pickupPosition.column, NNTileType.PickUp);
    }
    //--------------------------------------------------------------------------------------------------

    void UpdateSnake()
    {
        //React to the VIRTUAL JOYPAD input.
        if (VirtualJoypad.UpArrow())         { proposedMovementDirection = MovementDirection.Up; }
        else if (VirtualJoypad.DownArrow())  { proposedMovementDirection = MovementDirection.Down; }
        else if (VirtualJoypad.LeftArrow())  { proposedMovementDirection = MovementDirection.Left; }
        else if (VirtualJoypad.RightArrow()) { proposedMovementDirection = MovementDirection.Right; }

        DoSnakeMovement();
    }

    //--------------------------------------------------------------------------------------------------

    void DoSnakeMovement()
    {
        timeUntilMove -= Time.deltaTime;
        if (timeUntilMove <= 0.0f)
        {
            //Rest the delay.
            timeUntilMove = movementDelay;

            //Do not allow invalid movements - such as moving down when currently moving up.
            if (currentMovementDirection == MovementDirection.Down && proposedMovementDirection == MovementDirection.Up)
            {
                proposedMovementDirection = MovementDirection.Down;
            }
            else if (currentMovementDirection == MovementDirection.Up && proposedMovementDirection == MovementDirection.Down)
            {
                proposedMovementDirection = MovementDirection.Up;
            }
            else if (currentMovementDirection == MovementDirection.Right && proposedMovementDirection == MovementDirection.Left)
            {
                proposedMovementDirection = MovementDirection.Right;
            }
            else if (currentMovementDirection == MovementDirection.Left && proposedMovementDirection == MovementDirection.Right)
            {
                proposedMovementDirection = MovementDirection.Left;
            }

            currentMovementDirection = proposedMovementDirection;

            //Do the actual movement.
            GridPosition headPos = snake[0];
            switch (currentMovementDirection)
            {
                case MovementDirection.Up:
                    snake.Insert(0, new GridPosition(headPos.row - 1, headPos.column));
                    break;

                case MovementDirection.Down:
                    snake.Insert(0, new GridPosition(headPos.row + 1, headPos.column));
                    break;

                case MovementDirection.Left:
                    snake.Insert(0, new GridPosition(headPos.row, headPos.column - 1));
                    break;

                case MovementDirection.Right:
                    snake.Insert(0, new GridPosition(headPos.row, headPos.column + 1));
                    break;

                default:
                    break;
            }

            //If AI is playing, we give it a bonus kScoreForMovement points of fitness for each movement.
            if (bAIOn)
            {
                fitnessValues[currentChromosome] += kScoreForMovement;
            }

            //Remove the final element in the snake array.
            snake.RemoveAt(snake.Count - 1);
        }

        //We don't want it to go on forever, so have a level time cutoff.
        if (levelTime <= 0)
        {
            ResetGame();
        }

        //--------------------------------------------------------------------------------------------------
        //Collisions
        //--------------------------------------------------------------------------------------------------
        if (snake.Count > 0)
        {
            switch (gridLayout[snake[0].row, snake[0].column])
            {
                case NNTileType.Head: //Head cannot collide with self.
                    break;
                case NNTileType.Blocked:
                case NNTileType.Body:
                case NNTileType.Tail:
                    ResetGame();
                    break;

                case NNTileType.PickUp:
                    //Single-body increment rule - Traditional ruleset.
                    AddToSnakeBody(1);

                    if (bAIOn)
                    {
                        fitnessValues[currentChromosome] += kScoreModifierWeight;
                    }

                    ResetPickup();
                    //IncreaseSpeed();
                    break;

                default:
                    break;
            }
        }
    }

    //--------------------------------------------------------------------------------------------------

    void ResetGame()
    {
        ResetGrid();
        ResetSnake();
        ResetPickup();

        if (bAIOn)
        {
            movementDelay = AiSpeedDelay;
            currentChromosome++;
            if (currentChromosome >= kNumberOfNNSnakes)
            {
                EvolveSolutions();

                //Reset values.
                currentChromosome = 0;
                for (int i = 0; i < kNumberOfNNSnakes; i++)
                {
                    fitnessValues[i] = 0.0f;
                }

                generation++;
                generationText.text = "Generation: " + generation;
                snakeText.text = "Snake: " + currentChromosome;
            }
            else
            {
                snakeText.text = "Snake: " + currentChromosome;
            }
        }
        else
        {
            movementDelay = kStartingMovementDelay;
            paused = true;
            //Turn the pause indicator on/off.
            if (pausedIndicator != null)
            {
                pausedIndicator.SetActive(paused);
            }
        }
    }

    //--------------------------------------------------------------------------------------------------

    void AddToSnakeBody(int numberOfSegments)
    {
        for (int i = 0; i < numberOfSegments; i++)
        {
            snake.Add(new GridPosition(snake[snake.Count - 1].row, snake[snake.Count - 1].column));
        }
    }

    //--------------------------------------------------------------------------------------------------

    void IncreaseSpeed()
    {
        movementDelay = Mathf.Max(kMinUpdateDelay, movementDelay - kDelayReductionAmount);
    }

    //--------------------------------------------------------------------------------------------------
    // AI functions.
    //--------------------------------------------------------------------------------------------------
    void SetUpNeuralNetwork()
    {
        for (int i = 0; i < kNumberOfNNSnakes; i++)
        {
            neuralNetworks.Add(new Network(kNumberOfInputNodes_Snake, kNumberOfOutputNodes_Snake, 1, kNumberOfHiddenNodes_Snake));
        }
    }

    //--------------------------------------------------------------------------------------------------

    void UpdateSnakeAI()
    {
        levelTime -= Time.deltaTime;

        List<float> inputs = new List<float>();

        //Get the details of the snake head position.

        //Inputs	- 4 directions
        inputs.Add(GetNNInputForPosition(MovementDirection.Up));
        inputs.Add(GetNNInputForPosition(MovementDirection.Down));
        inputs.Add(GetNNInputForPosition(MovementDirection.Left));
        inputs.Add(GetNNInputForPosition(MovementDirection.Right));

        //			- Distance to pickup in 4 directions.
        inputs.Add(GetNNInputForPickup(MovementDirection.Up));
        inputs.Add(GetNNInputForPickup(MovementDirection.Down));
        inputs.Add(GetNNInputForPickup(MovementDirection.Left));
        inputs.Add(GetNNInputForPickup(MovementDirection.Right));

        //			- Collidable position in each direction.
        inputs.Add(GetNNInputForCollision(MovementDirection.Up, NNTileType.Blocked));
        inputs.Add(GetNNInputForCollision(MovementDirection.Down, NNTileType.Blocked));
        inputs.Add(GetNNInputForCollision(MovementDirection.Left, NNTileType.Blocked));
        inputs.Add(GetNNInputForCollision(MovementDirection.Right, NNTileType.Blocked));

        List<float> outputs = new List<float>();
        neuralNetworks[currentChromosome].Update(inputs, ref outputs);

        //Should be 4 outputs.
        if ((outputs[(int)MovementDirection.Up] > outputs[(int)MovementDirection.Down]) && (outputs[(int)MovementDirection.Up] > outputs[(int)MovementDirection.Left]) && (outputs[(int)MovementDirection.Up] > outputs[(int)MovementDirection.Right]))
        {
            proposedMovementDirection = MovementDirection.Up;
        }
        else if ((outputs[(int)MovementDirection.Down] > outputs[(int)MovementDirection.Up]) && (outputs[(int)MovementDirection.Down] > outputs[(int)MovementDirection.Left]) && (outputs[(int)MovementDirection.Down] > outputs[(int)MovementDirection.Right]))
        {
            proposedMovementDirection = MovementDirection.Down;
        }
        else if ((outputs[(int)MovementDirection.Left] > outputs[(int)MovementDirection.Up]) && (outputs[(int)MovementDirection.Left] > outputs[(int)MovementDirection.Down]) && (outputs[(int)MovementDirection.Left] > outputs[(int)MovementDirection.Right]))
        {
            proposedMovementDirection = MovementDirection.Left;
        }
        else
        {
            proposedMovementDirection = MovementDirection.Right;
        }

        DoSnakeMovement();
    }

    //--------------------------------------------------------------------------------------------------

    float GetNNInputForPosition(MovementDirection dir)
    {
        NNTileType tileTypeToCheck = NNTileType.Empty;

        switch (dir)
        {
            case MovementDirection.Up:
                tileTypeToCheck = gridLayout[snake[0].row, snake[0].column - 1];
            break;

            case MovementDirection.Down:
                tileTypeToCheck = gridLayout[snake[0].row, snake[0].column + 1];
            break;

            case MovementDirection.Left:
                tileTypeToCheck = gridLayout[snake[0].row - 1, snake[0].column];
            break;

            case MovementDirection.Right:
                tileTypeToCheck = gridLayout[snake[0].row + 1, snake[0].column];
            break;

            default:
            break;
        }

        if (tileTypeToCheck == NNTileType.Empty || tileTypeToCheck == NNTileType.PickUp)
        {
            return 1.0f;
        }

        return 0.0f;
    }

    //--------------------------------------------------------------------------------------------------

    float GetNNInputForPickup(MovementDirection dir)
    {
        float pickUpPosCol = (float)pickupPosition.column;
        float pickUpPosRow = (float)pickupPosition.row;

        float headPosCol = (float)snake[0].column;
        float headPosRow = (float)snake[0].row;

        switch (dir)
        {
            case MovementDirection.Up:
                if (pickUpPosRow <= headPosRow)
                {
                    return (1.0f - (headPosRow - pickUpPosRow) / (float)kSnakeBoardDimensions);
                }
            break;

            case MovementDirection.Down:
                if (pickUpPosRow >= headPosRow)
                {
                    return (1.0f - ((pickUpPosRow - headPosRow) / (float)kSnakeBoardDimensions));
                }
            break;

            case MovementDirection.Left:
                if (pickUpPosCol <= headPosCol)
                {
                    return (1.0f - (headPosCol - pickUpPosCol) / (float)kSnakeBoardDimensions);
                }
            break;

            case MovementDirection.Right:
                if (pickUpPosCol >= headPosCol)
                {
                    return (1.0f - ((pickUpPosCol - headPosCol) / (float)kSnakeBoardDimensions));
                }
            break;

            default:
            break;
        }

        //The pickup is not located in the desired direction, so return zero.
        return 0.0f;
    }

    //--------------------------------------------------------------------------------------------------

    float GetNNInputForCollision(MovementDirection dir, NNTileType typeOfCollision)
    {
        //Simply return 0.0 if collision in the next cell down to 1.0 if wall is far away.
        bool collided = false;
        int i = 0;

        switch (dir)
        {
            case MovementDirection.Up:
                while (!collided)
                {
                    i++;
                    if (gridLayout[snake[0].column, snake[0].row - i] == typeOfCollision)
                    {
                        collided = true;
                    }
                    else
                    {
                        for (int j = 1; j < snake.Count; j++)
                        {
                            if (snake[0].column == snake[j].column && snake[0].row - i == snake[j].row)
                            {
                                collided = true;
                                break;
                            }
                        }
                    }
                }
            break;

            case MovementDirection.Down:
                while (!collided)
                {
                    i++;
                    if (gridLayout[snake[0].column, snake[0].row + i] == typeOfCollision)
                    {
                        collided = true;
                    }
                    else
                    {
                        for (int j = 1; j < snake.Count; j++)
                        {
                            if (snake[0].column == snake[j].column && snake[0].row + i == snake[j].row)
                            {
                                collided = true;
                                break;
                            }
                        }
                    }
                }
            break;

            case MovementDirection.Left:
                while (!collided)
                {
                    i++;
                    if (gridLayout[snake[0].column - i, snake[0].row] == typeOfCollision)
                    {
                        collided = true;
                    }
                    else
                    {
                        for (int j = 1; j < snake.Count; j++)
                        {
                            if (snake[0].column - i == snake[j].column && snake[0].row == snake[j].row)
                            {
                                collided = true;
                                break;
                            }
                        }
                    }
                }
            break;

            case MovementDirection.Right:
                while (!collided)
                {
                    i++;
                    if (gridLayout[snake[0].column + i, snake[0].row] == typeOfCollision)
                    {
                        collided = true;
                    }
                    else
                    {
                        for (int j = 1; j < snake.Count; j++)
                        {
                            if (snake[0].column + i == snake[j].column && snake[0].row == snake[j].row)
                            {
                                collided = true;
                                break;
                            }
                        }
                    }
                }
            break;

            default:
            break;
        }

        //Return the 1.0f - 0.0f value.
        float maxDistance = kSnakeBoardDimensions - 2;
        return 1.0f - ((float)i / maxDistance);
    }

    //--------------------------------------------------------------------------------------------------
    // GA functions.
    //--------------------------------------------------------------------------------------------------
    void SetUpChromosomes()
    {
        for (int i = 0; i < kNumberOfNNSnakes; i++)
        {
            chromosomes[i] = new float[kNumberOfWeights_Snake];
            selectedChromosomes[i] = new float[kNumberOfWeights_Snake];
        }
    }

    //--------------------------------------------------------------------------------------------------

    void ClearChromosomes()
    {
        for (int i = 0; i < kNumberOfNNSnakes; i++)
        {
            for (int weight = 0; weight < kNumberOfWeights_Snake; weight++)
            {
                chromosomes[i][weight] = 0.0f;
            }
        }
    }

    //--------------------------------------------------------------------------------------------------

    void EvolveSolutions()
    {
        //Store weights from Networks in our local array, ready for breeding.
        for (int i = 0; i < kNumberOfNNSnakes; i++)
        {
            List<float> weights = neuralNetworks[i].GetWeights();

            for (int j = 0; j < kNumberOfWeights_Snake; j++)
            {
                chromosomes[i][j] = weights[j];
            }
        }

        //------------------------------------------------------------------------------------
        double highestFitness = 0;
        int highestIndex = -1;

        //Find the best chromosome.
        for (int currentChild = 0; currentChild < kNumberOfNNSnakes; currentChild++)
        {
            if (fitnessValues[currentChild] > highestFitness)
            {
                highestFitness = fitnessValues[currentChild];
                highestIndex = currentChild;
            }
        }
        if (highestIndex != -1)
        {
            Debug.Log("Best: " + highestIndex + " @ " + highestFitness);
        }

        //------------------------------------------------------------------------------------

        //Debug.Log("Selection");
        Selection();

        //Debug.Log("Crossover");
        Crossover();

        //Debug.Log("Selection - Elitist");
        //SelectionElitist();

        if (bAllowMutation)
        {
           // Debug.Log("Mutation");
            Mutation();
        }

        //Set the new weights.
        for (int i = 0; i < kNumberOfNNSnakes; i++)
        {
            float[] weights = chromosomes[i];
            neuralNetworks[i].SetWeights(weights);
        }
    }

    //--------------------------------------------------------------------------------------------------

    void Selection()
    {
        //Tournament Selection.
        int tournamentNumber = 10;
        int highestIndex;

        //Tournament selection.
        for (int currentChild = 0; currentChild < kNumberOfNNSnakes; currentChild++)
        {
            //Select 2 random indexes.
            highestIndex = Random.Range(0, kNumberOfNNSnakes);

            //Loop through the number of combatants.
            for (int i = 0; i < tournamentNumber; i++)
            {
                int index2 = Random.Range(0, kNumberOfNNSnakes);

                if (fitnessValues[index2] > fitnessValues[highestIndex])
                {
                    highestIndex = index2;
                }
            }

            //Store the winning chromosome.
            for (int weight = 0; weight < kNumberOfWeights_Snake; weight++)
            {
                selectedChromosomes[currentChild][weight] = chromosomes[highestIndex][weight];
            }
        }
    }

    //--------------------------------------------------------------------------------------------------

    void SelectionElitist()
    {
        //Elite Selection.
        double highestFitness = 0;
        int highestIndex = 0;

        //Set up a temporary elite chromosome.
        List<float> eliteChromosome = new List<float>();
        for (int i = 0; i < kNumberOfWeights_Snake; i++)
        {
            eliteChromosome.Add(0.0f);
        }

        //Find the best chromosome.
        for (int currentChild = 0; currentChild < kNumberOfNNSnakes; currentChild++)
        {
            if (fitnessValues[currentChild] > highestFitness)
            {
                highestFitness = fitnessValues[currentChild];
                highestIndex = currentChild;
            }
        }
        Debug.Log("Best: " + highestIndex + " @ " + highestFitness);

        //Store the winning chromosome in all children
        for (int weight = 0; weight < kNumberOfWeights_Snake; weight++)
        {
            eliteChromosome[weight] = chromosomes[highestIndex][weight];
        }

        //Clear the values from the current generation of chromosomes.
        ClearChromosomes();

        for (int currentSnake = 0; currentSnake < kNumberOfNNSnakes; currentSnake++)
        {
            for (int weight = 0; weight < kNumberOfWeights_Snake; weight++)
            {
                //SWAP
                chromosomes[currentSnake][weight] = eliteChromosome[weight];
            }
        }

        //Check for mutation on every element in every Chromosome.
        for (int currentSnake = 0; currentSnake < kNumberOfNNSnakes; currentSnake++)
        {
            for (int weight = 0; weight < kNumberOfWeights_Snake; weight++)
            {
                if (Random.Range(0.0f, 100.0f) < kMutationRate_Snake)
                {
                    //Randomly generate a new weight.
                    chromosomes[currentSnake][weight] = Random.Range(-1.0f, 1.0f);

                    //Debug.Log("Mutation occurred");
                }
            }
        }
    }

    //--------------------------------------------------------------------------------------------------

    void Crossover()
    {
        //Clear all information in the last generation of Chromosomes.
        ClearChromosomes();

        //Multi-point crossover.
        for (int currentSnake = 0; currentSnake < kNumberOfNNSnakes; currentSnake += 2)
        {
            for (int weight = 0; weight < kNumberOfWeights_Snake; weight += 2)
            {
                if (Random.Range(0, 100) < kCrossoverRate_Snake)
                {
                    //SWAP
                    //Chromosome 1.
                    chromosomes[currentSnake][weight]         = selectedChromosomes[currentSnake][weight];
                    chromosomes[currentSnake][weight + 1]     = selectedChromosomes[currentSnake + 1][weight + 1];
                    //Chromosome 2.
                    chromosomes[currentSnake + 1][weight]     = selectedChromosomes[currentSnake + 1][weight];
                    chromosomes[currentSnake + 1][weight + 1] = selectedChromosomes[currentSnake][weight + 1];
                }
                else
                {
                    //STICK
                    //Chromosome 1.
                    chromosomes[currentSnake][weight]         = selectedChromosomes[currentSnake][weight];
                    chromosomes[currentSnake][weight + 1]     = selectedChromosomes[currentSnake][weight + 1];
                    //Chromosome 2.
                    chromosomes[currentSnake + 1][weight]     = selectedChromosomes[currentSnake + 1][weight];
                    chromosomes[currentSnake + 1][weight + 1] = selectedChromosomes[currentSnake + 1][weight + 1];
                }
            }
        }
    }

    //--------------------------------------------------------------------------------------------------

    void Mutation()
    {
        //Check for mutation on every element in every Chromosome.
        for (int currentSnake = 0; currentSnake < kNumberOfNNSnakes; currentSnake++)
        {
            for (int weight = 0; weight < kNumberOfWeights_Snake; weight++)
            {
                if (Random.Range(0.0f, 100.0f) < kMutationRate_Snake)
                {
                    //Randomly generate a new weight.
                    chromosomes[currentSnake][weight] = Random.Range(-1.0f, 1.0f);

                    //Debug.Log"Mutation occurred");
                }
            }
        }
    }

    //--------------------------------------------------------------------------------------------------

}
*/