// Author: Alice Zeltser
// File Name: BlueGhost.cs
// Project Name: PASS3
// Creation Date: Dec. 18, 2023
// Modified Date: Jan. 21, 2024
// Description: Changes ghost to follow the blue ghost movement behaviour

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
    class BlueGhost: Ghost
    {
        public BlueGhost(int posX, int posY, int dirX, int dirY, Texture2D img, TileMap tilemap) : base(posX, posY, dirX, dirY, img, tilemap)
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
            //check if ghost collided wiwth wall
            if (map[posY - 1, posX] == Game1.WALL_CHAR)
            {
                //move ghost down
                dirY = 1;
            }
            else if (map[posY + 1, posX] == Game1.WALL_CHAR)
            {
                //move ghost up
                dirY = -1;
            }

            //Call update positions method
            UpdatePositions();
        }
    }
}

