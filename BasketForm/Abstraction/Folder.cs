using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasketForm.Abstraction
{
    public class Folder
    {
        public string Title { get; set; }
        public string PhysicalPath { get; set; }
        public int TileSize { get; set; }
        public bool DisplaySubfolderTree { get; set; }
        public Button FormButton { get; set; }
    }
}
