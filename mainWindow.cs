﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.LinkLabel;

namespace poligonEditor
{
    public partial class mainWindow : Form
    {
        // Actual object we are going draw on, like a "canvas"
        private Bitmap drawArea;

        // List of poligons we have on the screen
        List<components.Poligon> poli = new List<components.Poligon>();

        components.Poligon tmpPoli = null;

        public mainWindow()
        {
            InitializeComponent();

            // We create and attache our "canvas" to pictureBox
            drawArea = new Bitmap(mainPictureBox.Size.Width, mainPictureBox.Size.Height);
            mainPictureBox.Image = drawArea;

            drawOnPictureBox();
        }

        // When we resize our window we need to adapt our bitmap size
        private void mainWindow_Resize(object sender, EventArgs e)
        {
            if (!(drawArea is null)) drawArea.Dispose();
            drawArea = new Bitmap(mainPictureBox.Size.Width, mainPictureBox.Size.Height);
            mainPictureBox.Image = drawArea;
            drawOnPictureBox();
        }

        // Method that calls to draw specific vertixces and lines
        private void drawOnPictureBox()
        {
            // Fill bitmap with white background
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.Clear(Color.White);
            }

            if (!(tmpPoli is null)) tmpPoli.Draw(drawArea);

            foreach(components.Poligon p in poli)
            {
                p.Draw(drawArea);
            }
            
            mainPictureBox.Refresh();
        }

        private void clickOnPictureBox(object sender, MouseEventArgs e)
        {
            // Deside which button was pressed
            switch (e.Button) { 
                case MouseButtons.Left:
                    if(tmpPoli is null)
                    {
                        tmpPoli = new components.Poligon(new components.Point(e.X, e.Y));
                    }
                    else
                    {
                        tmpPoli.addNewPoint(new components.Point(e.X, e.Y));
                        tmpPoli.Draw(drawArea);
                        mainPictureBox.Refresh();
                        if (tmpPoli.isPoligonComplet())
                        {
                            poli.Add(tmpPoli);
                            tmpPoli = null;
                        }
                    }
                    break;
                case MouseButtons.Right:
                    break;
                case MouseButtons.Middle:
                    break;
                default:
                    throw new ArgumentException("Event argument contained invalid Button propperty");
            }
        }

        private void mouseMoveOverCanvas(object sender, MouseEventArgs e)
        {
            if (tmpPoli is null) return;

            drawOnPictureBox();

            tmpPoli.DrawIncompleteLine(drawArea, e.X, e.Y);

            mainPictureBox.Refresh();
        }
    }
}
