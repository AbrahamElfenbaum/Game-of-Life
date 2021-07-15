﻿using System;
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
    public partial class Form1 : Form
    {
        //Timer interval
        int interval = 100;

        //Width and Height of the universe

        // The universe array
        bool[,] universe = new bool[30, 30];
        bool[,] scratchPad = new bool[30, 30];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = interval; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            
            int neighbors;
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    //int count = CountNeighbor
                    neighbors = CountNeighborsFinite(x, y);

                    //apply rules to see if the cell should live or die in the next gen
                    if (universe[x, y] == true && neighbors < 2)
                        scratchPad[x, y] = false;
                    else if (universe[x, y] == true && neighbors > 3)
                        scratchPad[x, y] = false;
                    else if (universe[x, y] == true && (neighbors == 2 || neighbors == 3))
                        scratchPad[x, y] = true;
                    else if (universe[x, y] == false && neighbors == 3)
                        scratchPad[x, y] = true;
                }
            }
            //copy from scratchPad to universe
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++) 
                    scratchPad[x, y] = false;
            }

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            //Neighbor Count
            int neighbors;
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    neighbors = CountNeighborsFinite(x, y);
                    //Shows number of neighbors
                    if(neighbors > 0)
                    {
                        Font font = new Font("Arial", 8f);
                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Black, cellRect, sf);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
                float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[(int)x, (int)y] = !universe[(int)x, (int)y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        //Finite Neighbor Count
        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if ((xOffset == 0) && (yOffset == 0)) continue;
                    // if xCheck is less than 0 then continue
                    else if (xCheck < 0) continue;
                    // if yCheck is less than 0 then continue
                    else if (yCheck < 0) continue;
                    // if xCheck is greater than or equal too xLen then continue
                    else if (xCheck >= xLen) continue;
                    // if yCheck is greater than or equal too yLen then continue
                    else if (yCheck >= yLen) continue;

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        //Toroidal Neighbor Count
        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if ((xOffset == 0) && (yOffset == 0)) continue;
                    // if xCheck is less than 0 then set to xLen - 1
                    // if yCheck is less than 0 then set to yLen - 1
                    // if xCheck is greater than or equal too xLen then set to 0
                    // if yCheck is greater than or equal too yLen then set to 0

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void startToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void pauseToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void nextToolStripButton_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                    universe[x, y] = false;
            }
            graphicsPanel1.Invalidate();
        }

        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = graphicsPanel1.BackColor;
            if(DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = gridColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = cellColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options dlg = new Options();

            dlg.Interval = interval;
            dlg.Width = universe.GetLength(0);
            dlg.Height = universe.GetLength(1);

            if (DialogResult.OK == dlg.ShowDialog())
            {
                interval = dlg.Interval;
                universe = new bool[dlg.Width, dlg.Height];
                scratchPad = new bool[dlg.Width, dlg.Height];

                graphicsPanel1.Invalidate();
            }
        }
    }
}
