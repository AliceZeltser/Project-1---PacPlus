// Author: Alice Zeltser
// File Name: Coin.cs
// Project Name: PASS3
// Creation Date: Jan. 7, 2024
// Modified Date: Jan. 21, 2024
// Description: The purpose of this program is handle coins properities

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
    class Coin
    {
        //store  rectangle
        private Rectangle rec;

        //store has coin with a getter and setter
        public bool hasCoin { get; set; }

        public Coin(int x, int y, int width, int height)
        {
            //set coin rectangle
            rec = new Rectangle(x, y, width, height);

            //set has coin 
            hasCoin = true;
        }

        //Pre: spriteBach, texture2d is not null
        //Post: N/A
        //Description: draws coin
        public void Draw(SpriteBatch spriteBatch, Texture2D coinTexture)
        {
            //if has coin is true
            if (hasCoin)
            {
                //draw coin
                spriteBatch.Draw(coinTexture, rec, Color.White);
            }
        }
    }
}

