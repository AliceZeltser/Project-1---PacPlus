// Author: Alice Zeltser
// File Name: TileMap.cs
// Project Name: PASS3
// Creation Date: Dec. 12, 2023
// Modified Date: Jan. 21, 2024
// Description: The purpose of this program is to draw tiles and handle tiles

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
    class TileMap
    {
        //stores img, rec and blockrecs
        private Texture2D [] imgs = new Texture2D[6];
        private Rectangle[,] tileRecs = new Rectangle[31, 28];
        public string[,] tiles = new string[31, 28];

        //store coins and cherries objects
        public Coin[,] coins = new Coin[31, 28];
        public Cherry [,] cherries = new Cherry[31, 28];

        //store tile / grid size 
        public const int tileWidth = 32;
        public const int tileHeight = 32;

        //store tile constants
        public const int FLOOR = 0;
        public const int WALL = 1;
        public const int TOP_LEFT = 2;
        public const int TOP_RIGHT = 3;
        public const int BOTTOM_LEFT = 4;
        public const int BOTTOM_RIGHT = 5;
        public const int COIN = 6;
        public const int CHERRY = 7;

        public TileMap(string[,] tiles, Texture2D[] imgs, Rectangle[,] tileRecs)
        {
            ///set tile images tiles and tiles rectangle
            this.imgs = imgs;
            this.tiles = tiles;
            this.tileRecs = tileRecs;

            //for each tile in tiles
            for (int row = 0; row < tiles.GetLength(0); row++)
            {
                for (int col = 0; col < tiles.GetLength(1); col++)
                {
                    //set rectangle based on index
                    tileRecs[row, col] = new Rectangle(col * tileWidth, row * tileHeight, tileWidth, tileHeight);
                    
                    //check if tiles is a floor tile
                    if (tiles[row, col] == Game1.FLOOR_CHAR)
                    {
                        //set a new coin
                        coins[row, col] = new Coin((col * tileWidth) + (tileWidth / 4), (row * tileHeight) + (tileHeight / 4), tileWidth / 2, tileHeight / 2);
                    }
                    else if (tiles[row, col] == Game1.CHERRY_CHAR)
                    {
                        //set a new cherry
                        cherries[row, col] = new Cherry(col * tileWidth, row * tileHeight, imgs[7]);
                    }
                }
            }
        }

        //Pre: N/A
        //Post: a boolean
        //Description: checks if all of the coins were collected
        public bool AreAllCoinsCollected()
        {
            //for each index of coins
            for (int i = 0; i < coins.GetLength(0); i++)
            {
                for (int j = 0; j < coins.GetLength(1); j++)
                {
                    //if coin is not null and it has a coin
                    if (coins[i, j] != null && coins[i, j].hasCoin)
                    {
                        //return false
                        return false;
                    }
                }
            }
            //return true
            return true;
        }

        //Pre: tileX and tile Y are both positive integers and are within theh size of the 2d array
        //Post: coin 
        //Description: returns a coin based on positions given
        public Coin GetCoin(int tileX, int tileY)
        {
            //return coin
            return coins[tileY, tileX];
        }

        //Pre: tileX and tile Y are both positive integers and are within theh size of the 2d array
        //Post: coin 
        //Description: returns a cherry based on positions given
        public Cherry GetCherry(int tileX, int tileY)
        {
            //return cherry
            return cherries[tileY, tileX];
        }

        //Pre: N/A
        //Post: returns a 2d array of strings 
        //Description: returns amap
        public string[,] GetMap()
        {
            //return map
            return tiles;
        }

        //Pre: spriteBatch
        //Post:N/A
        //Description: draws map
        public void Draw(SpriteBatch spriteBatch)
        {
            //stores the hright and width of the tileRecs
            int height = tileRecs.GetLength(0);
            int width = tileRecs.GetLength(1);

            //for each index in tilesRec
            for (int i = 0; i < tileRecs.GetLength(0); i++)
            {
                for (int j = 0; j < tileRecs.GetLength(1); j++)
                {
                    //checks if tile is a wall tile
                    if (tiles[i, j] == Game1.WALL_CHAR)
                    {
                        //check which type of wall tile to draw
                        if (j < width - 1 && i < height - 1 && tiles[i, j + 1] == Game1.WALL_CHAR && tiles[i + 1, j] == Game1.WALL_CHAR && (i == 0 || tiles[i - 1, j] != Game1.WALL_CHAR
                             && (j == 0 || tiles[i, j - 1] != Game1.WALL_CHAR)))
                        {
                            //draw top left tile
                            spriteBatch.Draw(imgs[TOP_LEFT], tileRecs[i, j], Color.White);
                        }
                        else if (j > 0 && i < height - 1 && tiles[i, j - 1] == Game1.WALL_CHAR && tiles[i + 1, j] == Game1.WALL_CHAR && (i == 0 || tiles[i - 1, j] != Game1.WALL_CHAR
                             && (j == width - 1 || tiles[i, j + 1] != Game1.WALL_CHAR)))
                        {
                            //draw top right tile
                            spriteBatch.Draw(imgs[TOP_RIGHT], tileRecs[i, j], Color.White);
                        }
                        else if (j < width - 1 && i > 0 && tiles[i, j + 1] == Game1.WALL_CHAR && tiles[i - 1, j] == Game1.WALL_CHAR && (i == height - 1 || tiles[i + 1, j] != Game1.WALL_CHAR)
                                 && (j == 0 || tiles[i, j - 1] != Game1.WALL_CHAR))
                        {
                            //draw bottom left tile
                            spriteBatch.Draw(imgs[BOTTOM_LEFT], tileRecs[i, j], Color.White);
                        }
                        else if (j > 0 && i > 0 && tiles[i, j - 1] == Game1.WALL_CHAR && tiles[i - 1, j] == Game1.WALL_CHAR && (i == height - 1 || tiles[i + 1, j] != Game1.WALL_CHAR)
                                && (j == width - 1 || tiles[i, j + 1] != Game1.WALL_CHAR))
                        {
                            //draw bottom right tile
                            spriteBatch.Draw(imgs[BOTTOM_RIGHT], tileRecs[i, j], Color.White);
                        }
                        else
                        {
                            //draw wall tile
                            spriteBatch.Draw(imgs[WALL], tileRecs[i, j], Color.White);
                        }
                    }
                    else if (tiles[i, j] == Game1.CHERRY_CHAR)
                    {
                        //Draw floor tile
                        spriteBatch.Draw(imgs[FLOOR], tileRecs[i, j], Color.White);

                        //draw cherry
                        cherries[i, j].Draw(spriteBatch);
                    }
                    else
                    {
                        //Draw floor tile
                        spriteBatch.Draw(imgs[FLOOR], tileRecs[i, j], Color.White);

                        //draw coins
                        DrawCoins(spriteBatch, imgs[COIN]);
                    }
                }
            }
        }

        //Pre: spriteBatch, texture 2d is not null
        //Post:N/A
        //Description: draws coins
        public void DrawCoins(SpriteBatch spriteBatch, Texture2D coinTexture)
        {
            //for each index in coins
            for (int i = 0; i < coins.GetLength(0); i++)
            {
                for (int j = 0; j < coins.GetLength(1); j++)
                {
                    //check if coin is not null and that it has a coin
                    if (coins[i, j] != null && coins[i, j].hasCoin)
                    {
                        //draw coin
                        coins[i, j].Draw(spriteBatch, coinTexture);
                    }
                }
            }
        }
    }
}
