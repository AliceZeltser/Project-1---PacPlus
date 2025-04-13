// Author: Alice Zeltser
// File Name: OrangeGhost.cs
// Project Name: PASS3
// Creation Date: Dec. 25, 2023
// Modified Date: Jan. 21, 2024
// Description: Changes ghost to follow the orange ghost movement behaviour

using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace PASS3
{
    class OrangeGhost : Ghost
    {
        //store distance map
        private string[,] distanceMap = new string[31, 28];

        //store random number generator
        private static Random rng = new Random();

        public OrangeGhost(int posX, int posY, int dirX, int dirY, Texture2D img, TileMap tilemap) : base(posX, posY, dirX, dirY, img, tilemap)
        {
            //set position
            this.posX = posX;
            this.posY = posY;

            //set direction
            this.dirX = dirX;
            this.dirY = dirY;

            //set map
            map = tilemap.GetMap();
        }

        //Pre: N/A
        //Post: N/A
        //Description: resets map
        private void ResetMap()
        {
            //for each index in map
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    //ignore cherries
                    if (map[i, j] == Game1.CHERRY_CHAR)
                    {
                        distanceMap[i, j] = Game1.FLOOR_CHAR;
                    }
                    else
                    {
                        //set distance map to equal map
                        distanceMap[i, j] = map[i, j];
                    }
                }
            }
        }

        //Pre: N/A
        //Post: N/A
        //Description: gets a path five tiles away for pacman for path finding
        private Vector2 MoveFiveAway(Player player)
        {
            //store next
            int next;

            //strore next coordinate queue, current coordinates, current space
            Queue<List<int>> nextCoordinateQueue = new Queue<List<int>>();
            List<int> currentCoordinates;
            string currentSpace = "0";

            //store ghost coodinates
            List<int> ghostcoords = new List<int>() { posY, posX };

            //store possible direction x and y 
            List<int> possibleDirectionsX = new List<int>();
            List<int> possibleDirectionsY = new List<int>();

            //set player position to zero on distance map
            distanceMap[player.GetPosY(), player.GetPosX()] = "0";

            //enqueue the player's initial position
            nextCoordinateQueue.Enqueue(new List<int> { player.GetPosY(), player.GetPosX() });

            //while current space is not five
            while (currentSpace != "5")
            {
                //set current coordinates to the coodinates from dequeuing
                currentCoordinates = nextCoordinateQueue.Dequeue();

                //if current coordinates equals to ghosts coodinates break from while loop
                if (currentCoordinates[0] == ghostcoords[0] && currentCoordinates[1] == ghostcoords[1])
                {
                    break;
                }

                //set curret space to the value at current coordinates on distance map
                currentSpace = distanceMap[currentCoordinates[0], currentCoordinates[1]];

                //Call EnqueueIfVaild method with the parameters of next coodinates queue, position x and y and current space for each possible direction
                EnqueueIfValid(nextCoordinateQueue, currentCoordinates[0], currentCoordinates[1] + 1, currentSpace);
                EnqueueIfValid(nextCoordinateQueue, currentCoordinates[0], currentCoordinates[1] - 1, currentSpace);
                EnqueueIfValid(nextCoordinateQueue, currentCoordinates[0] + 1, currentCoordinates[1], currentSpace);
                EnqueueIfValid(nextCoordinateQueue, currentCoordinates[0] - 1, currentCoordinates[1], currentSpace);
            }

            //for each index in distance map
            for (int i = 0; i < distanceMap.GetLength(0); i++)
            {
                for (int j = 0; j < distanceMap.GetLength(1); j++)
                {
                    //if index equals to five
                    if (distanceMap[i, j] == "5")
                    {
                        //add possible x and y direction
                        possibleDirectionsX.Add(j);
                        possibleDirectionsY.Add(i);
                    }
                }
            }

            //if possible directions x count is not equal to zero
            if (possibleDirectionsX.Count != 0)
            {
                //generates a new number between 0 and possible dirctions count 
                next = rng.Next(1, possibleDirectionsX.Count());

                //call reset map method
                ResetMap();

                //return path five away from pacman
                return new Vector2(possibleDirectionsX[next - 1], possibleDirectionsY[next - 1]);
            }
            else
            {
                //call reset map method
                ResetMap();

                //return pacman's position
                return new Vector2(player.GetPosY(), player.GetPosX());
            }
        }

        //Pre: a queue of lists of integeres, x and y integers need be within distance map size and a string of current space
        //Post: N/A
        //Description: enqueue a list of coordinates if they are valid
        public override void Movement(Player player)
        {
            //reset map
            ResetMap();

            //store a queue to store next coodinates, store current coordinates and current space
            Queue<List<int>> nextCoordinateQueue = new Queue<List<int>>();
            List<int> currentCoordinates;
            string currentSpace;

            //store the target to path find
            Vector2 goTo = MoveFiveAway(player);

            //store target coodinates
            int playerX = (int)goTo.X;
            int playerY = (int)goTo.Y;

            //store target coodinates
            List<int> pacmanCoords = new List<int>() { playerY, playerX };

            //set ghost position on the map to zero
            distanceMap[posY, posX] = "0";

            //enqueue the ghost's initial position
            nextCoordinateQueue.Enqueue(new List<int> { posY, posX });

            //while next coodinates is not empty
            while (nextCoordinateQueue.Count > 0)
            {
                //set current coordinates to the coodinates from dequeuing
                currentCoordinates = nextCoordinateQueue.Dequeue();

                //if current coordinates equals to pacman coodinates break from while loop
                if (currentCoordinates[0] == pacmanCoords[0] && currentCoordinates[1] == pacmanCoords[1])
                {
                    break;
                }

                //set curret space to the value at current coordinates on distance map
                currentSpace = distanceMap[currentCoordinates[0], currentCoordinates[1]];
                 
                //Call EnqueueIfVaild method with the parameters of next coodinates queue, position x and y and current space for each possible direction
                EnqueueIfValid(nextCoordinateQueue, currentCoordinates[0], currentCoordinates[1] + 1, currentSpace);
                EnqueueIfValid(nextCoordinateQueue, currentCoordinates[0], currentCoordinates[1] - 1, currentSpace);
                EnqueueIfValid(nextCoordinateQueue, currentCoordinates[0] + 1, currentCoordinates[1], currentSpace);
                EnqueueIfValid(nextCoordinateQueue, currentCoordinates[0] - 1, currentCoordinates[1], currentSpace);
            }

            //set current, next and previous coodinates to pacman coordinates
            currentCoordinates = pacmanCoords;
            List<int> nextCoordinates = pacmanCoords;
            List<int> previousCoordinates = pacmanCoords;

            //set current space to the space where pacman is 
            currentSpace = distanceMap[pacmanCoords[0], pacmanCoords[1]];

            //continue looping while current space is not zero and current space is not a wall tile
            while (currentSpace != "0" && currentSpace != Game1.WALL_CHAR && currentSpace != "g")
            {
                //set next spaces to be the spaces for each possible direction
                string nextSpace1 = distanceMap[currentCoordinates[0], currentCoordinates[1] + 1];
                string nextSpace2 = distanceMap[currentCoordinates[0], currentCoordinates[1] - 1];
                string nextSpace3 = distanceMap[currentCoordinates[0] + 1, currentCoordinates[1]];
                string nextSpace4 = distanceMap[currentCoordinates[0] - 1, currentCoordinates[1]];

                //set wanted space to current space minus one
                string wantedSpace = (int.Parse(currentSpace) - 1).ToString();

                //Perform the appropriate  operation based on wantedspace value
                if (nextSpace1 == wantedSpace)
                {
                    //set next coodinates to one right of current coodinate and decerement current space by one
                    nextCoordinates = new List<int> { currentCoordinates[0], currentCoordinates[1] + 1 };
                    currentSpace = (int.Parse(currentSpace) - 1).ToString();
                }
                else if (nextSpace2 == wantedSpace)
                {
                    //set next coodinates to one left of current coodinate and decerement current space by one
                    nextCoordinates = new List<int> { currentCoordinates[0], currentCoordinates[1] - 1 };
                    currentSpace = (int.Parse(currentSpace) - 1).ToString();
                }
                else if (nextSpace3 == wantedSpace)
                {
                    //set next coodinates to one down of current coodinate and decerement current space by one
                    nextCoordinates = new List<int> { currentCoordinates[0] + 1, currentCoordinates[1] };
                    currentSpace = (int.Parse(currentSpace) - 1).ToString();
                }
                else if (nextSpace4 == wantedSpace)
                {
                    //set next coodinates to one up of current coodinate and decerement current space by one
                    nextCoordinates = new List<int> { currentCoordinates[0] - 1, currentCoordinates[1] };
                    currentSpace = (int.Parse(currentSpace) - 1).ToString();
                }
                else
                {
                    break;
                }

                //set previous coodinates to current coodinates and current coodinates to nect coodinates
                previousCoordinates = currentCoordinates;
                currentCoordinates = nextCoordinates;

                //set new directions
                dirX = previousCoordinates[1] - currentCoordinates[1];
                dirY = previousCoordinates[0] - currentCoordinates[0];

                //check if player has power up
                if (player.hasPowerup)
                {
                    //change directions
                    dirX = dirX * -1;
                    dirY = dirY * -1;
                }
            }
        }

        //Pre: queue, x and y is within distance map range and a string of current space
        //Post: N/A
        //Description: enqueue a list of coordinates if they are valid
        private void EnqueueIfValid(Queue<List<int>> queue, int x, int y, string currentSpace)
        {
            // Check if x and y are within the bounds of distanceMap
            if (x >= 0 && x < distanceMap.GetLength(0) && y >= 0 && y < distanceMap.GetLength(1))
            {
                // Check if distance map at (x, y) is a floor tile
                if (distanceMap[x, y] == Game1.FLOOR_CHAR)
                {
                    // Create a list of coordinates and enqueue it
                    List<int> coords = new List<int> { x, y };
                    queue.Enqueue(coords);

                    // Update the distance map at the specified position with the incremented current space value
                    distanceMap[x, y] = (int.Parse(currentSpace) + 1).ToString();
                }
            }
        }
    }
}

