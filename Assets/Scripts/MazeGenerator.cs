using UnityEngine;
using System.Collections;

// found here https://vimeo.com/36076209

public class MazeGenerator : MonoBehaviour {
    public int GridSize = 1;

    public int MazeHeight = 11;
    public int MazeWidth = 11;
   
    private int[,] maze;

    public GameObject WallGo;
    public GameObject SpaceGo;

    private static System.Random _random = new System.Random();

    //Use this for initialization
    void Start()
    {
        //Run our Maze Generator
        maze = GenerateMaze(MazeHeight, MazeWidth);
        //Show A Visual Representation of the maze within Unity
        for (int i = 0; i < MazeHeight; i++)
            for(int j = 0; j < MazeWidth; j++)
            {
                // create a position for the block/wall
                Vector3 pos = new Vector3(i* GridSize, 0, j* GridSize);

                //Maze a wall if there is a 1 in the array
                if (maze[i,j] == 1)
                {  
                    //create the wall within the scene
                    GameObject wall = Instantiate(WallGo) as GameObject;
                    // move wall to correct position
                    if (wall != null)
                        wall.transform.position = pos;
                }
                else
                {
                    //create the wall within the scene
                    GameObject wall = Instantiate(SpaceGo) as GameObject;
                    // move wall to correct position
                    if (wall != null)
                        wall.transform.position = pos;
                }

                
            }

    }


    private int[,] GenerateMaze(int Height, int Width)
    {
        // Create our temporary Maze
        int[,] maze = new int[Height, Width];

        // Initialize all the cells to walls 'value = 1'
        for (int i = 0; i < Height; i++)
            for (int j = 0; j < Width; j++)
                maze[i, j] = 1;

        // Generate a new random seed
        System.Random rand = new System.Random();

        // Fine a random starting cell
        int r = rand.Next(Height);
        while (r % 2 == 0)
            r = rand.Next(Height);

        int c = rand.Next(Width);
        while (c % 2 == 0)
            c = rand.Next(Width);

        // Set our Start Cell to a path, 'value = 0'
        maze[r, c] = 0;

        //Create our maze using Depth-First Search Algorithm
        MazeDigger(maze, r, c);

        // Return the maze

        return maze;
    }//end GenerateMaze
	
    private void MazeDigger(int[,] maze, int r, int c)
    {

        /*  Digging Directions
        1 = North
        2 = South
        3 = East
        4 = West
        */

        int[] directions = new int[] { 1, 2, 3, 4 };
        Shuffle(directions);
        //Debug.Log("Directions: " + directions[0] + directions[1] + directions[2] + directions[3]);
        // Look in a direction two blocks ahead

        for (int i = 0; i < directions.Length; i++)
        {
            switch (directions[i])
            {
                case 1: // up/north
                    // check whether 2 cells up is out of maze 
                    if (r - 2 <= 0)
                        continue;
                    if(maze[r-2,c] != 0)
                    {
                        maze[r - 2, c] = 0; // set it to a pathway
                        maze[r - 1, c] = 0;
                        MazeDigger(maze, r - 2, c);
                    }
                    break;

                case 2: // Right/East
                    //Check 2 cells right is out of the maze
                    if (c + 2 >= MazeWidth - 1)
                        continue;
                    if(maze[r,c+2] != 0)
                    {
                        maze[r, c + 2] = 0;
                        maze[r, c + 1] = 0;
                        MazeDigger(maze, r, c + 2);
                    }
                    break;

                case 3: // down
                    // check whether 2 cells down is out of maze 
                    if (r + 2 >= MazeHeight -1 )
                        continue;
                    if (maze[r + 2, c] != 0)
                    {
                        maze[r + 2, c] = 0; // set it to a pathway
                        maze[r + 1, c] = 0;
                        MazeDigger(maze, r + 2, c);
                    }
                    break;

                case 4: // Left
                    //Check 2 cells left is out of the maze
                    if (c - 2 <= - 0)
                        continue;
                    if (maze[r, c - 2] != 0)
                    {
                        maze[r, c - 2] = 0;
                        maze[r, c - 1] = 0;
                        MazeDigger(maze, r, c - 2);
                    }
                    break;

            } // endswitch
        }

    }

    // http://www.dotnetperls.com/fish-yates-shuffle
    public static void Shuffle<T>(T[] array)
    {
        var random = _random;
        for(int i = array.Length; i > 1; i--)
        {
            // Pick random element to swap
            int j = random.Next(i);
            //swap
            T tmp = array[j];
            array[j] = array[i - 1];
            array[i - 1] = tmp;

        }
    }

   
	
}
