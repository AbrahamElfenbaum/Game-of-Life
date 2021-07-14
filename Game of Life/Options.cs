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
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        public int Interval
        { 
            get { return (int)numericUpDownInterval.Value; }
            set { numericUpDownInterval.Value = value; } 
        }
        public int Height
        {
            get { return (int)numericUpDownHeight.Value; }
            set { numericUpDownHeight.Value = value; }
        }
        public int Width
        {
            get { return (int)numericUpDownWidth.Value; }
            set { numericUpDownWidth.Value = value; }
        }
    }
}
