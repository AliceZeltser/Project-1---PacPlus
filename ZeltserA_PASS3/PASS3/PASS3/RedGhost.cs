// Author: Alice Zeltser
// File Name: RedGhost.cs
// Project Name: PASS3
// Creation Date: Dec. 25, 2023
// Modified Date: Jan. 21, 2024
// Description: Changes ghost to follow the red ghost movement behaviour

//Course concept: Here is uses queues for path finding in the Movement(Player player) method

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
    class RedGhost: Ghost
    {
        //store distance map
        private string[,] distanceMap = new string[31, 28];

        public RedGhost(int posX, int posY, int dirX, int dirY, Texture2D img, TileMap tilemap) : base(posX, posY, dirX, dirY, img, tilemap)
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

        //Pre: Player is not null
        //Post: N/A
        //Description: handles ghost movement
        public override void Movement(Player player)
        {
            //reset map
            ResetMap();

            //store a path queue to store next coodinates
            PathQueue nextCoordinateQueue = new PathQueue();

            //store current coodinates and current space
            List<int> currentCoordinates;
            string currentSpace;

            //set and store pacmancoords
            List<int> pacmanCoords = new List<int>() { player.GetPosY(), player.GetPosX() };

            //set distance map at ghost position to zero
            distanceMap[posY, posX] = "0";

            //enqueue the ghost's initial position
            nextCoordinateQueue.Enqueue(new List<int> { posY, posX });

            //while next coodinates is not empty
            while (!nextCoordinateQueue.IsEmpty())
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
            while (currentSpace != "0" && currentSpace != Game1.WALL_CHAR)
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
            }

            //check if player has power up
            if (player.hasPowerup)
            {
                //change directions
                dirX = dirX * -1;
                dirY = dirY * -1;
            }
        }

        //Pre: path queue, x and y is within distance map range and a string of current space
        //Post: N/A
        //Description: enqueue a list of coordinates if they are valid
        private void EnqueueIfValid(PathQueue queue, int x, int y, string currentSpace)
        {
            //Calls the Enqueue if vaild method with x and y, distance map and current space parameters
            queue.EnqueueIfValid(x, y, distanceMap, currentSpace);
        }
    }
}

