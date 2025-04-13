// Author: Alice Zeltser
// File Name: Level.cs
// Project Name: PASS3
// Creation Date: Jan. 17, 2023
// Modified Date: Jan. 21, 2024
// Description: The purpose of this program is to set a tree for levels

//Course concept: Trees in the Level class - used for the level paths

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASS3
{
    class Level
    {
        //store map with a getter
        public TileMap map { get; }

        //store level's children
        private List<Level> children = new List<Level>();

        //store ghosts with a getter
        public List<Ghost> ghosts { get; }

        //store cherries with a getter
        public List<Cherry> cherries { get; }

        //store parentwith a getter
        public Level parent { get; set; }

        //store score with getter and setter
        public int score { get; set; }

        //store level name
        public int level { get; }

        public Level(TileMap map, List<Ghost> ghosts, int level)
        {
            //set map and ghosts
            this.map = map;
            this.ghosts = ghosts;
            this.level = level;
        }

        //Pre: level node
        //Post: N/A
        //Description: trys to add a child
        public void TryAddChild(Level node)
        {
            //adds node to chldren list
            children.Add(node);

            //Sets the parent node to the current instance 
            node.parent = this;
        }

        //Pre: N/A
        //Post: returns the list of levels
        //Description: returns the list of children nodes
        public List<Level> GetChildren()
        {
            return children;
        }
    }
}
