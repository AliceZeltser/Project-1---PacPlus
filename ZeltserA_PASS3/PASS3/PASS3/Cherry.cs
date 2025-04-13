// Author: Alice Zeltser
// File Name: Cherry.cs
// Project Name: PASS3
// Creation Date: Jan. 20, 2024
// Modified Date: Jan. 21, 2024
// Description: The purpose of this program is handle cherries properities

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
    class Cherry
    {
        //store position with a getter and setter
        public int posX { get; set; }
        public int posY { get; set; }

        //store has cherry wwith a getter and setter
        public bool hasCherry { get; set; }

        //store image and rectangle of cheery
        private Texture2D img;
        private Rectangle rec;

        public Cherry(int posX, int posY, Texture2D img)
        {
            //set position
            this.posX = posX;
            this.posY = posY;

            //set image
            this.img = img;

            //set has cherry to true
            hasCherry = true;

            //set rectanlge
            rec = new Rectangle(posX, posY, TileMap.tileWidth, TileMap.tileHeight);
        }

        //Pre: spriteBach
        //Post: N/A
        //Description: draws cherry
        public void Draw(SpriteBatch spriteBatch)
        {
            //if player has cherry
            if (hasCherry)
            {
                //draw cherry
                spriteBatch.Draw(img, rec, Color.White);
            }
        }
    }
}
