// Author: Alice Zeltser
// File Name: Ghost.cs
// Project Name: PASS3
// Creation Date: Dec. 15, 2023
// Modified Date: Jan. 21, 2024
// Description: The purpose of this program is to handles player collisons, movement, drawing

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
    class Ghost
    {
        //store position x and y and with a getter and a setter
        public int posX { get; set; }
        public int posY { get; set; }

        //store ghost directions
        protected int dirX;
        protected int dirY;

        //store starting directions
        private int startDirX;
        private int startDirY;

        //store ghost starting position
        private int startPosX;
        private int startPosY;

        //store ghost animation
        protected Animation ghost;

        //store ghost position
        private Vector2 pos;

        //stores timer, timeToNext node and max time to next node
        protected Timer timer;
        public float timeToNextNode;
        public const float MAX_TIME_TO_NODE = 1.25f;

        //store map
        protected string[,] map = new string[31, 28];

        public Ghost(int posX, int posY, int dirX, int dirY, Texture2D img, TileMap tilemap)
        {
            //set position and direction
            this.posX = posX;
            this.posY = posY;
            this.dirX = dirX;
            this.dirY = dirY;

            //set starting positions and directions
            startPosX = posX;
            startPosY = posY;
            startDirX = dirX;
            startDirY = dirY;

            //set position
            pos = new Vector2(posX, posY);

            //set ghost
            ghost = new Animation(img, 8, 1, 8, 1, 1, Animation.ANIMATE_FOREVER, 1000, pos, 1, true);

            //set and activate timer
            timer = new Timer(Timer.INFINITE_TIMER, true);
            timer.Activate();
        }

        //Pre: gameTime, tileMap map and player 
        //Post: N/A
        //Description: updates ghost movement 
        public virtual void Update(GameTime gameTime, TileMap tilemap, Player player)
        {
            //update timer
            timer.Update(gameTime.ElapsedGameTime.TotalSeconds);

            //update ghost animation
            ghost.Update(gameTime);

            //increment time to next node by the time passed
            timeToNextNode += (float)(timer.GetTimePassed());

            //set map to current tilemap map
            string [,] map = tilemap.GetMap();

            //if position + direction is a floor tile
            if (map[posY + dirY, posX + dirX] != Game1.WALL_CHAR)
            {
                //if time to next node is grearer than max time to next node
                if (timeToNextNode > MAX_TIME_TO_NODE)
                {
                    //increment posX and posY baed on the direction values
                    posX += dirX;
                    posY += dirY;

                    //reset timer, set time to next node to zero
                    timer.ResetTimer(true);
                    timeToNextNode = 0;
                }
            }
            else
            {
                //reset timer, set time to next node to zero
                timer.ResetTimer(true);
                timeToNextNode = 0;
            }

            //Call update positions method
            UpdatePositions();
        }

        //Pre: N/A
        //Post: N/A
        //Description: updates ghost positions
        public virtual void UpdatePositions()
        {
            //sets new position
            int newPosX = (int)((posX * 32)) + (int)(timeToNextNode / MAX_TIME_TO_NODE * 32f * dirX);
            int newPosY = (int)((posY * 32)) + (int)(timeToNextNode / MAX_TIME_TO_NODE * 32f * dirY);

            //move the ghost based on the new positions
            ghost.TranslateTo(newPosX, newPosY);
        }

        //Pre: N/A
        //Post: N/A
        //Description: resets ghost position and directions to starting position and direction
        public void ResetToStartingPosition()
        {
            //set position to starting postion
            posX = startPosX;
            posY = startPosY;

            //set direction to starting direction
            dirX = startDirX;
            dirY = startDirY;
        }

        //Pre: Player is not null
        //Post: N/A
        //Description: handles ghost movement
        public virtual void Movement(Player player)
        {
        }

        //Pre: spriteBatch
        //Post: N/A
        //Description: draws ghost
        public void Draw(SpriteBatch spriteBatch, Player player)
        {
            if (player.hasPowerup)
            {
                //draw ghost
                ghost.Draw(spriteBatch, Color.PowderBlue, Animation.FLIP_NONE);
            }
            else
            {
                //draw ghost
                ghost.Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
            }
        }
    }
}
