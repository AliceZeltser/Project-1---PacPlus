// Author: Alice Zeltser
// File Name: PathQueue.cs
// Project Name: PASS3
// Creation Date: Jan. 20, 2023
// Modified Date: Jan. 21, 2024
// Description: The purpose of this program is to set a queue for the path

//Course concept: Queues in the PathQueue class - used for the path finding in red ghost
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS3
{
    class PathQueue
    {
        // Constant representing an invalid element in the queue
        public const int NO_ELEMENT = Int32.MinValue;

        //store the queue and its size
        private List<List<int>> queue;
        private int size;

        public PathQueue()
        {
            //set queue and size
            queue = new List<List<int>>();
            size = 0;
        }

        //Pre: a list of integers 
        //Post: N/A
        //Description: enqueues a list of integers to the queue
        public void Enqueue(List<int> path)
        {
            //add path to the list and increment size
            queue.Add(path);
            size++;
        }

        //Pre: N/A
        //Post: returns a list of integers
        //Description: dequeues the first item in the list
        public List<int> Dequeue()
        {
            //set result to null
            List<int> result = null;

            // Check if the queue is not empty
            if (!IsEmpty())
            {
                // Retrieve and remove the first item from the queue, decerement size by 1
                result = queue[0];
                queue.RemoveAt(0);
                size--;
            }

            // Return the dequeued list or an empty list if the queue was empty
            return result ?? new List<int>();
        }

        //Pre: N/A
        //Post: returns a bool
        //Description: checks if queue is empty
        public bool IsEmpty()
        {
            //return true if size is 0
            return size == 0;
        }

        //Pre: integer of x and y, string 2d array and a string of current space
        //Post: N/A
        //Description: enqueue a list of coordinates if they are valid
        public void EnqueueIfValid(int x, int y, string[,] distanceMap, string currentSpace)
        {
            // Check if the coordinates are within bounds and the space is valid
            if (x >= 0 && x < distanceMap.GetLength(0) && y >= 0 && y < distanceMap.GetLength(1) && distanceMap[x, y] == Game1.FLOOR_CHAR)
            {
                // Create a list representing the coordinates and enqueue it
                List<int> coords = new List<int> { x, y };
                Enqueue(coords);

                // Update the distanceMap with the incremented value
                distanceMap[x, y] = (int.Parse(currentSpace) + 1).ToString();
            }
        }
    }
}
