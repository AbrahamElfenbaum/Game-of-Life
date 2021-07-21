using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Seed : Form
    {
        public Seed()
        {
            InitializeComponent();
        }

        public int SeedValue
        {
            get { return (int)numericUpDownSeed.Value; }
            set { numericUpDownSeed.Value = value; }
        }
    }
}
