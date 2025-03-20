using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Models
{
    public class BoardInfo
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string FilePath { get; private set; }




        public BoardInfo(string name, string filePath, string description = "")
        {
            Name = name;
            FilePath = filePath;
            Description = description;
        }
    }
}
