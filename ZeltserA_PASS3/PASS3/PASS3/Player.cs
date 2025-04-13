// Author: Alice Zeltser
// File Name: Player.cs
// Project Name: PASS3
// Creation Date: Dec. 13, 2023
// Modified Date: Jan. 21, 2024
// Description: The purpose of this program is to handles player collisons, movement, drawing

using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace PASS3
{
    class Player
    {
        //Store position annd start position 
        private int posX;
        private int posY;
        private int startPosX;
        private int startPosY;

        //stores directions
        private int dirX;
        private int dirY;

        //stores timer, timeToNext node and max time to next node
        private Timer timer;
        private float timeToNextNode;
        private const float MAX_TIME_TO_NODE = 1f;

        //Store state and animation textures
        private int state;
        private Animation[] anims;

        //Store won bool
        static bool won = false;

        //store directions constants
        public const int UP = 0;
        public const int RIGHT = 1;
        public const int DOWN = 2;
        public const int LEFT = 3;

        //store lives with a getter and setter
        public int lives { set; get; }

        //store score with a getter and setter
        public int score { get; set; }

        //store wanna be directions
        private int wannaBeX;
        private int wannaBeY;

        //store powerup timer, has power up with a getter and setter
        private Timer powerupTimer;
        public bool hasPowerup { get; set; }

        //store boolean to see if its time to play coin sound effect
        bool coinSoundeffect = false;

        public Player(int posX, int posY, int dirX, int dirY, Texture2D[] imgs)
        {
            //Sets position and direction
            this.posX = posX;
            this.posY = posY;
            this.dirX = dirX;
            this.dirY = dirY;

            //set starting positions
            startPosX = posX;
            startPosY = posY;

            //set wanna be directionss
            wannaBeX = 0;
            wannaBeY = 0;

            //set position
            Vector2 pos = new Vector2(posX * 32, posY * 32);

            //Stores animation
            anims = new Animation[4];

            //set animations
            anims[UP] = new Animation(imgs[UP], 1, 8, 8, 0, 0, Animation.ANIMATE_FOREVER, 500, pos, 1, true);
            anims[RIGHT] = new Animation(imgs[RIGHT], 8, 1, 8, 0, 0, Animation.ANIMATE_FOREVER, 500, pos, 1, true);
            anims[DOWN] = new Animation(imgs[DOWN], 1, 8, 8, 0, 0, Animation.ANIMATE_FOREVER, 500, pos, 1, true);
            anims[LEFT] = new Animation(imgs[LEFT], 8, 1, 8, 0, 0, Animation.ANIMATE_FOREVER, 500, pos, 1, true);
           
            //set score and lives
            score = 0;
            lives = 3;

            //set timer and activate it
            timer = new Timer(Timer.INFINITE_TIMER, true);
            timer.Activate();

            //set has powerup and powerup timer
            hasPowerup = false;
            powerupTimer = new Timer(Timer.INFINITE_TIMER, false);
        }

        //Pre: N/A
        //Post: returns posX value as an integer
        //Description: returns position x value
        public int GetPosX()
        {
            return posX;
        }

        //Pre: N/A
        //Post: returns posY value as an integer
        //Description: returns position y value
        public int GetPosY()
        {
            return posY;
        }

        //Pre: a boolean variable
        //Post: N/A
        //Description: sets a new value of won
        public void SetHasWon(bool hasWon)
        {
            //set won to the value at haswon
            won = hasWon;
        }

        //Pre: N/A
        //Post: return has won value as a bool
        //Description: returns value at has won
        public bool hasWon()
        {
            return won;
        }

        //Pre: gameTime, tileMap map and ghosts 
        //Post: N/A
        //Description: updates player movement and collisons detections
        public void Update(GameTime gameTime, TileMap tilemap, List<Ghost> ghosts)
        {
            //update timer
            timer.Update(gameTime.ElapsedGameTime.TotalSeconds);
            powerupTimer.Update(gameTime.ElapsedGameTime.TotalSeconds);

            //update animation based on current state
            anims[state].Update(gameTime);

            //increment timeToNextNode based on the time passed
            timeToNextNode += (float)(timer.GetTimePassed());

            //store and set map to current map
            string [,] map = tilemap.GetMap();

            //if position + direction is a floor tile
            if (map[posY + dirY, posX + dirX] == Game1.FLOOR_CHAR || map[posY + dirY, posX + dirX] == Game1.CHERRY_CHAR)
            {
                //if time to next node is greater than max time to next node
                if (timeToNextNode > MAX_TIME_TO_NODE)
                {
                    //increment posX and posY baed on the direction values
                    posX += dirX;
                    posY += dirY;

                    //if position of player is 0
                    if (posX == 0)
                    {
                        //Call the check coin collision method with tile map parameter
                        CheckItemCollision(tilemap);

                        //change to position x to the other side of the map
                        posX = map.GetLength(1) - 1;
                    }
                    else if (posX == map.GetLength(1) - 1)
                    {
                        //Call the check coin collision method with tile map parameter
                        CheckItemCollision(tilemap);

                        //change position x to zereo
                        posX = 0;
                    }

                    // Call the check coin collision method with tile map parameter
                    CheckItemCollision(tilemap);

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

            //check fi power up is timer is greater than five seconds
            if (powerupTimer.GetTimePassed() > 5)
            {
                //set power up to false
                hasPowerup = false;
            }

            //set collided ghost
            Ghost collidedGhost = IsCollied(ghosts);

            //check if collision occured between ghost and player if so reset positions 
            if (collidedGhost != null && !hasPowerup)
            {
                //decerement lives
                lives--;

                //Call the reset to starting position and the reset ghosts to starting position method with the ghosts list parameter
                ResetToStartingPosition();
                ResetGhostsToStartingPosition(ghosts);

                //play player died sound effect
                Game1.playerDiedSound.CreateInstance().Play();
            }
            else if (collidedGhost != null && hasPowerup)
            {
                //reset ghost that was collided with during power up
                collidedGhost.ResetToStartingPosition();
            }

            //Call update positions method with map parameter
            UpdatePositions(map);
        }

        //Pre: tile map is not null
        //Post: N/A
        //Description: checks if player collided with coin if so increment score and check if player won
        private void CheckItemCollision(TileMap tileMap)
        {
            //set tile positions to the player positions
            int tileX = posX;
            int tileY = posY;

            //set the coin obkect to the current coin in the current tiles
            Coin currentCoin = tileMap.GetCoin(tileX, tileY);
            Cherry currentCherry = tileMap.GetCherry(tileX, tileY);

            //check if coin is not null and if it has a coin
            if (currentCoin != null && currentCoin.hasCoin)
            {
                //increment score by 10 and set has coin to false
                score += 10;
                currentCoin.hasCoin = false;

                //play coin sound effect every other coin
                if (coinSoundeffect)
                {
                    //player pick up coin sound effect
                    Game1.playerEatingCoin.CreateInstance().Play();

                    //set coin sound effect to true
                    coinSoundeffect = false;
                }
                else
                {
                    //set coin sound effect to false
                    coinSoundeffect = true;
                }

                //check if player collected all the coins
                if (tileMap.AreAllCoinsCollected())
                {
                    //set won to true
                    won = true;
                }
            }

            //check if cherry is not null and if it has a cherry
            if (currentCherry != null && currentCherry.hasCherry)
            {
                //increment score by 100 and set has cherry to false
                score += 100;
                currentCherry.hasCherry = false;

                //player pick up cherry sound effect
                Game1.playerEatingCherry.CreateInstance().Play();

                //call collect cherry method
                CollectCherry();
            }
        }

        //Pre:N/A
        //Post: N/A
        //Description: turns on power up and power up timer 
        private void CollectCherry()
        {
            //set has powerup to true
            hasPowerup = true;

            //set power up timer
            powerupTimer = new Timer(Timer.INFINITE_TIMER, true);
        }

        //Pre: list of ghosts
        //Post: returns a boolean
        //Description: checks if player collided with the ghosts
        public Ghost IsCollied(List<Ghost> ghosts)
        {
            //for each ghosts in the list of ghosts
            for (int i = 0; i < ghosts.Count; i++)
            {
                //if player's position matches the position of the ghost
                if ((posX == ghosts[i].posX) && (posY == ghosts[i].posY))
                {
                    //return true
                    return ghosts[i];
                }
            }

            //return false
            return null;
        }

        //Pre: N/A
        //Post: N/A
        //Description: resets player to startig position
        public void ResetToStartingPosition()
        {
            //set positions to starting positions
            posX = startPosX;
            posY = startPosY;

            //set directions to zero
            wannaBeX = 0;
            wannaBeY = 0;
            dirX = 0;
            dirY = 0;
        }

        //Pre: N/A
        //Post: N/A
        //Description: resets ghost to starting position
        public void ResetGhostsToStartingPosition(List<Ghost> ghosts)
        {
            //for each ghost in the ghost list
            foreach (var ghost in ghosts)
            {
                //Call the ResetToStartingPosition method
                ghost.ResetToStartingPosition();
            }
        }

        //Pre: a string of 2d array
        //Post: N/A
        //Description: updates player positions
        public void UpdatePositions(string[,] map)
        {
            //if position + wanna be directions is a floor tile
            if (map[posY + wannaBeY, posX + wannaBeX] == Game1.FLOOR_CHAR || map[posY + wannaBeY, posX + wannaBeX] == Game1.CHERRY_CHAR)
            {
                //change direcions to wanna be directions
                dirX = wannaBeX;
                dirY = wannaBeY;
            }

            //set new position
            int newPosX = (int)((posX * 32)) + (int)(timeToNextNode / MAX_TIME_TO_NODE * 32f * dirX);
            int newPosY = (int)((posY * 32)) + (int)(timeToNextNode / MAX_TIME_TO_NODE * 32f * dirY);

            //move the player based on the new positions
            anims[state].TranslateTo(newPosX, newPosY);
        }

        //Pre:spriteBatch
        //Post: N/A
        //Description: draws player
        public void Draw(SpriteBatch spriteBatch)
        {
            // draw player
            anims[state].Draw(spriteBatch, Color.White, Animation.FLIP_NONE);
        }

        //Pre: userX, userY and state as an integer and a 2d array of a string
        //Post: N/A
        //Description: Reads user input of where to go
        public void Move(int userX, int userY, int state, string[,] map)
        {
            //sets wanna be direction based on the usere input direction
            wannaBeX = userX;
            wannaBeY = userY;

            //set state to state given
            this.state = state;

            //Call update positions method with the map parameter
            UpdatePositions(map);
        }
    }
}

