using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct connectFourNode
{
    public int[,] boardStateStruct;
    public float ni; // number of times this path has been checked to a terminal node
    public float t; // total value
    public float vi;// average value
    float c;// exploration parameter
    float N; // root node number of iterations
    public int currentPlayer; //1 is red 2 is blue
    int heightOfBoardAi;
    int lengthOfBoardAi;
    int moveCountAi;
    bool DidWinAi(int currentPlayer)
    {
        for(int x = 0; x < lengthOfBoardAi - 3; x++)
        {
            for (int y = 0; y < heightOfBoardAi; y++)
            {
                if (boardStateStruct[x, y] == currentPlayer && boardStateStruct[x + 1, y] == currentPlayer && boardStateStruct[x + 2, y] == currentPlayer && boardStateStruct[x + 3, y] == currentPlayer)
                {
                    return true;
                }
            }
        }
        for (int x = 0; x < lengthOfBoardAi; x++)
        {
            for (int y = 0; y < heightOfBoardAi - 3; y++)
            {
                if (boardStateStruct[x, y] == currentPlayer && boardStateStruct[x, y + 1] == currentPlayer && boardStateStruct[x, y + 2] == currentPlayer && boardStateStruct[x, y + 3] == currentPlayer)
                {
                    return true;
                }
            }
        }
        for (int x = 0; x < lengthOfBoardAi - 3; x++)
        {
            for (int y = 0; y < heightOfBoardAi - 3; y++)
            {
                if ((boardStateStruct[x, y + 3] == currentPlayer && boardStateStruct[x + 1, y + 2] == currentPlayer && boardStateStruct[x + 2, y + 1] == currentPlayer && boardStateStruct[x + 3, y] == currentPlayer) || (boardStateStruct[x, y] == currentPlayer && boardStateStruct[x + 1, y + 1] == currentPlayer
                    && boardStateStruct[x + 2, y + 2] ==  currentPlayer && boardStateStruct[x + 3, y + 3] == currentPlayer))
                {
                    return true;
                }
            }
        }
        return false;
    }


    public void RolloutAndCheckWin()
    {
        Stack<connectFourNode> path = new Stack<connectFourNode>();
        path.Push(this); // Add node to stack

        int currentPlayerRollout = currentPlayer;
        int moveCountRollout = 0;
        bool gameEnded = false;

        while (!gameEnded && moveCountRollout < 42) // Perform rollout until terminal node or draw
        {
            List<connectFourNode> childNodes = Expand();
            int randomIndex = Random.Range(0, childNodes.Count);
            connectFourNode randomChildNode = childNodes[randomIndex];
            currentPlayerRollout = currentPlayerRollout == 1 ? 2 : 1; // Switch player
            moveCountRollout++;

            // Check if the game has ended
            gameEnded = randomChildNode.DidWinAi(1) || randomChildNode.DidWinAi(2) || moveCountRollout == 42;

            path.Push(randomChildNode); // Push the chosen child node onto the path stack
        }

        // Determine the result of the rollout
        float result = 0.5f; // Draw
        if (path.Peek().DidWinAi(1))
        {
            result = currentPlayer == 1 ? 1 : 0;  
        }
        else if (path.Peek().DidWinAi(2))
        {
            result = currentPlayer == 2 ? 1 : 0; 
        }

        // Backpropagate the result
        while (path.Count > 0)
        {
            connectFourNode current = path.Pop();
            current.ni++;
            current.t += result;
            current.vi = current.t / current.ni;
        }
    }

    public connectFourNode(int[,] currentBoardState, float n)
    {
        heightOfBoardAi = 6;
        lengthOfBoardAi = 7;
        boardStateStruct = currentBoardState;
        ni = 0;
        t = 0;
        vi = 0;
        c = 2;
        N = n;
        currentPlayer = 2; // 2 is blue 1 is red
        moveCountAi = 0;
    }

    public List<connectFourNode> Expand()
    {
        List<connectFourNode> childNodes = new List<connectFourNode>();

        for (int col = 0; col < 7; col++)
        {
            if (IsValidMove(col))
            {
                connectFourNode newNode = new connectFourNode(boardStateStruct, N);
                newNode.boardStateStruct = (int[,])boardStateStruct.Clone(); // Copy the current board state
                MakeMove(newNode.boardStateStruct, col);
                childNodes.Add(newNode);
            }
        }
        currentPlayer = currentPlayer == 1 ? 2 : 1; // sets player to other player after expanding
        return childNodes;
    }

    private bool IsValidMove(int row)
    {
        for (int col = 0; col < boardStateStruct.GetLength(1); col++)
        {
            if (boardStateStruct[row, col] == 0)
            {
                return true;
            }
        }
        return false;
    }

    private void MakeMove(int[,] board, int row)
    {
        for (int col = 0; col < 7; col++) // Change loop condition to match the board width (7 columns)
        {
            if (board[row, col] == 0)
            {
                board[row, col] = currentPlayer;
                break;
            }
        }
    }

    // UCB1 
    public float UCB1(float N)
    {
        if (ni == 0)
        {
            return float.MaxValue; // Return an infinite value for unexplored nodes
        }
        else
        {
            float exploitation = vi;
            float exploration = c * Mathf.Sqrt(Mathf.Log(N) / ni);
            return exploitation + exploration;
        }
    }


}


public class gameManager : MonoBehaviour
{

    public string debugStartMessege;

    public GameObject redChip;
    public GameObject blueChip;
    public int moveCount;
    int AiMove;
    public int heightOfBoard = 6;
    public int lengthOfBoard = 7;
    public GameObject[] spawnLocations;
    public bool redChipTurn = true;
    int[,] boardState;
    float N = 0; // Initialize N 

    void Start()
    {
        Debug.Log("helloWorld!");
        Debug.Log("Tacocat spelled backwards is tacocat");
        moveCount = 0;
        boardState = new int[lengthOfBoard, heightOfBoard];

    }

    int AiPlayerMove()
    {
        int randomNumber = Random.Range(0, 7); // random move
        AiMove = randomNumber;
        Debug.Log(boardState);
        // Monte carlo Tree Search Alg or MiniMax to choose move
        //********************************************************************

        //STEP1
        // initiate stuff to begin search with the new state
        int[,] boardStateAlg = boardState; // copy of the current board state
        int moveCountAlg = moveCount; // whenmovecount = 42 board is full

        //foreach (int element in boardState)
        // {
        //      Debug.Log("Array contents: " + string.Join(", ", element));
        // }


        //STEP2
        //expand the initial state one layer
        connectFourNode node = new connectFourNode(boardState, N); // root node
        connectFourNode bestNode = node; // Declare outside the loop
                                         // Continue until a leaf node is found
                                         // everthing inside this needs to loop for two seconds before breaking
                                         // Record the start time
        float startTime = Time.time;
        while (true)
        {
            // Check if two seconds have passed
            if (Time.time - startTime >= 2)
            {
                break;
            }

            // Call the Expand method on the node instance
            List<connectFourNode> childNodes = node.Expand();

            //STEP3
            //use UCB1 repeatedly to choose which child node to travel to until you find a unexplored node
            //and then do a rollout

            // Now you can work with the list of child nodes
            // Initialize variables to keep track of the best child node and its UCB1 value
            // Call the Expand method on the node instance

            // Now you can work with the list of child nodes
            float bestUCB1Value = float.MinValue;
            foreach (var childNode in childNodes)
            {
                // Calculate UCB1 value for the child node
                float ucb1Value = childNode.UCB1(N);

                // Log the UCB1 value for this child node
                Debug.Log("UCB1 value for child node: " + ucb1Value);

                // Update best node if this child has a higher UCB1 value
                if (ucb1Value > bestUCB1Value)
                {
                    bestUCB1Value = ucb1Value;
                    bestNode = childNode;
                }

                // print child boardstates to log
                Debug.Log("Child Node:");
                for (int i = 0; i < childNode.boardStateStruct.GetLength(0); i++)
                {
                    string rowString = "";
                    for (int j = 0; j < childNode.boardStateStruct.GetLength(1); j++)
                    {
                        rowString += childNode.boardStateStruct[i, j] + " ";
                    }
                    Debug.Log(rowString);
                }
                Debug.Log("----------------------");
            }

            // Check if the best node is a leaf node
            if (bestNode.ni == 0)
            {
                Debug.Log("I found a leaf node that needs a ROLLOUT!!!");
                Debug.Log("rollout about to happen");
                bestNode.RolloutAndCheckWin();
                Debug.Log("rollout has occured");
                // Exit the loop if it's a leaf node
            }
            Debug.Log("1");
            Debug.Log("2");
            //yield return new WaitForSeconds(0.5f);
            break;
        }

        //STEP3
        //do a rollout
        //Assign a value of 1 for a win, 0.5 for a draw, and 0 for a loss. These values represent the outcome of the game from the perspective of the player who made the last move.
        // Backpropagate the results of the simulated playout up the tree. Update the visit counts and average score of each node based on the results of the playouts.


        //STEP5
        // After running the simulation phase for a certain amount of time select the move that leads to the node with the highest win rate as the best move.

        //********************************************************************
        return AiMove;
    }

    public void SelectColumn(int column)
    {
        Debug.Log("game manager Column " + column);
        if (redChipTurn)
        {
            TakeTurn(column);
        }
        else
        {
            AiPlayerMove();
            TakeTurn(AiMove);
        }


    }

    public void TakeTurn(int column)
    {

        if (UpdateBoardState(column))
        {


            if (redChipTurn == true)
            {
                Instantiate(redChip, spawnLocations[column].transform.position, Quaternion.identity);
                redChipTurn = false;

                if (DidWin(1))
                {
                    Debug.Log("redplayer Wins!");
                    GameOver();//game over here
                }

            }
            else
            {
                //Instantiate(blueChip, spawnLocations[column].transform.position, Quaternion.identity);

                Instantiate(blueChip, spawnLocations[column].transform.position, Quaternion.identity);
                redChipTurn = true;

                if (DidWin(2))
                {
                    Debug.Log("blueplayer Wins!");
                    GameOver();// game over here
                }
            }
        }
    }


    bool UpdateBoardState(int column)
    {
        for (int row = 0; row < heightOfBoard; row++)
        {
            if (boardState[column, row] == 0)
            {
                if (redChipTurn == true)
                {
                    boardState[column, row] = 1;
                    Debug.Log("red chip instantiated correctly");
                    moveCount++;
                }
                else
                {
                    boardState[column, row] = 2;
                    Debug.Log("blue chip instantiated correctly");
                    moveCount++;
                }
                Debug.Log("Piece being spawned at(" + column + "," + row + ")");
                return true;
            }
        }
        Debug.Log("Column is Full");
        return false;
    }

    bool GameOver()
    {
        for (int x = 0; x < lengthOfBoard; x++)
        {
            for (int y = 0; y < heightOfBoard; y++)
            {
                boardState[x, y] = 1;
            }
        }
        return true;
    }

    bool DidWin(int playerNum)
    {
        for (int x = 0; x < lengthOfBoard - 3; x++)
        {
            for (int y = 0; y < heightOfBoard; y++)
            {
                if (boardState[x, y] == playerNum && boardState[x + 1, y] == playerNum && boardState[x + 2, y] == playerNum && boardState[x + 3, y] == playerNum)
                {
                    //GameOver();
                    return true;
                }
            }
        }
        for (int x = 0; x < lengthOfBoard; x++)
        {
            for (int y = 0; y < heightOfBoard - 3; y++)
            {
                if (boardState[x, y] == playerNum && boardState[x, y + 1] == playerNum && boardState[x, y + 2] == playerNum && boardState[x, y + 3] == playerNum)
                {
                    //GameOver(); // remove this later
                    return true;
                }
            }
        }
        for (int x = 0; x < lengthOfBoard - 3; x++)
        {
            for (int y = 0; y < heightOfBoard - 3; y++)
            {
                if ((boardState[x, y + 3] == playerNum && boardState[x + 1, y + 2] == playerNum && boardState[x + 2, y + 1] == playerNum && boardState[x + 3, y] == playerNum) || (boardState[x, y] == playerNum && boardState[x + 1, y + 1] == playerNum && boardState[x + 2, y + 2] == playerNum && boardState[x + 3, y + 3] == playerNum))
                {
                    //GameOver();// remove this later
                    return true;
                }
            }
        }
        return false;
    }

}