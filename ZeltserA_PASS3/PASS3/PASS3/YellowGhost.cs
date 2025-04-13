// Author: Alice Zeltser
// File Name: YellowGhost.cs
// Project Name: PASS3
// Creation Date: Dec. 18, 2023
// Modified Date: Jan. 21, 2024
// Description: Changes ghost to follow the yellow ghost movement behaviour

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
    class YellowGhost : Ghost
    {
        public YellowGhost(int posX, int posY, int dirX, int dirY, Texture2D img, TileMap tilemap) : base(posX, posY, dirX, dirY, img, tilemap)
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

        //Pre: Player is not null
        //Post: N/A
        //Description: handles ghost movement
        public override void Movement(Player player)
        {
            //store random value
            Random rng = new Random();

            //if ghost collided with wall
            while (map[posY + dirY, posX + dirX] == Game1.WALL_CHAR)
            {
                //set num to a random number between 1 to 4
                int num = rng.Next(1, 5);

                //performe appropriate  operation based on num value
                if (num == 1)
                {
                    //change direction right
                    dirX = 1;
                    dirY = 0;
                }
                else if (num == 2)
                {
                    //change direction left
                    dirX = -1;
                    dirY = 0;
                }
                else if (num == 3)
                {
                    //change direction down
                    dirY = 1;
                    dirX = 0;
                }
                else
                {
                    //change direction up
                    dirY = -1;
                    dirX = 0;
                }
            }

            //call update positions method
            UpdatePositions();
        }
    }
}
