namespace poligonEditor
{
    partial class mainWindow
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainPictureBox = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addAPointMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.movaAPointMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveAnEdgeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveAPoligonMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAPointMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.methodOfDrawingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.defaultMethodOfDrawingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bresenhamsAlgorithmMethodOfDrawingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPictureBox
            // 
            this.mainPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPictureBox.Location = new System.Drawing.Point(0, 0);
            this.mainPictureBox.Name = "mainPictureBox";
            this.mainPictureBox.Size = new System.Drawing.Size(800, 450);
            this.mainPictureBox.TabIndex = 3;
            this.mainPictureBox.TabStop = false;
            this.mainPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.clickOnPictureBox);
            this.mainPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMoveOverCanvas);
            this.mainPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.endOfClickOnPictureBox);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAPointMenuItem,
            this.movaAPointMenuItem,
            this.moveAnEdgeMenuItem,
            this.moveAPoligonMenuItem,
            this.deleteAPointMenuItem,
            this.methodOfDrawingToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(181, 158);
            // 
            // addAPointMenuItem
            // 
            this.addAPointMenuItem.Checked = true;
            this.addAPointMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.addAPointMenuItem.Name = "addAPointMenuItem";
            this.addAPointMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addAPointMenuItem.Text = "Add a point";
            this.addAPointMenuItem.Click += new System.EventHandler(this.addAPointMenuItem_Click);
            // 
            // movaAPointMenuItem
            // 
            this.movaAPointMenuItem.Name = "movaAPointMenuItem";
            this.movaAPointMenuItem.Size = new System.Drawing.Size(180, 22);
            this.movaAPointMenuItem.Text = "Move a point";
            this.movaAPointMenuItem.Click += new System.EventHandler(this.movaAPointMenuItem_Click);
            // 
            // moveAnEdgeMenuItem
            // 
            this.moveAnEdgeMenuItem.Name = "moveAnEdgeMenuItem";
            this.moveAnEdgeMenuItem.Size = new System.Drawing.Size(180, 22);
            this.moveAnEdgeMenuItem.Text = "Move an edge";
            this.moveAnEdgeMenuItem.Click += new System.EventHandler(this.moveAnEdgeMenuItem_Click);
            // 
            // moveAPoligonMenuItem
            // 
            this.moveAPoligonMenuItem.Name = "moveAPoligonMenuItem";
            this.moveAPoligonMenuItem.Size = new System.Drawing.Size(180, 22);
            this.moveAPoligonMenuItem.Text = "Move a poligon";
            this.moveAPoligonMenuItem.Click += new System.EventHandler(this.moveAPoligonMenuItem_Click);
            // 
            // deleteAPointMenuItem
            // 
            this.deleteAPointMenuItem.Name = "deleteAPointMenuItem";
            this.deleteAPointMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deleteAPointMenuItem.Text = "Delete a point";
            this.deleteAPointMenuItem.Click += new System.EventHandler(this.deleteAPointMenuItem_Click);
            // 
            // methodOfDrawingToolStripMenuItem
            // 
            this.methodOfDrawingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultMethodOfDrawingMenuItem,
            this.bresenhamsAlgorithmMethodOfDrawingMenuItem});
            this.methodOfDrawingToolStripMenuItem.Name = "methodOfDrawingToolStripMenuItem";
            this.methodOfDrawingToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.methodOfDrawingToolStripMenuItem.Text = "Method of drawing";
            // 
            // defaultMethodOfDrawingMenuItem
            // 
            this.defaultMethodOfDrawingMenuItem.Checked = true;
            this.defaultMethodOfDrawingMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.defaultMethodOfDrawingMenuItem.Name = "defaultMethodOfDrawingMenuItem";
            this.defaultMethodOfDrawingMenuItem.Size = new System.Drawing.Size(198, 22);
            this.defaultMethodOfDrawingMenuItem.Text = "Default";
            this.defaultMethodOfDrawingMenuItem.Click += new System.EventHandler(this.defaultMethodOfDrawingMenuItem_Click);
            // 
            // bresenhamsAlgorithmMethodOfDrawingMenuItem
            // 
            this.bresenhamsAlgorithmMethodOfDrawingMenuItem.Name = "bresenhamsAlgorithmMethodOfDrawingMenuItem";
            this.bresenhamsAlgorithmMethodOfDrawingMenuItem.Size = new System.Drawing.Size(198, 22);
            this.bresenhamsAlgorithmMethodOfDrawingMenuItem.Text = "Bresenham\'s Algorithm";
            this.bresenhamsAlgorithmMethodOfDrawingMenuItem.Click += new System.EventHandler(this.bresenhamsAlgorithmMethodOfDrawingMenuItem_Click);
            // 
            // mainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.mainPictureBox);
            this.Name = "mainWindow";
            this.Text = "poligonEditor";
            this.Resize += new System.EventHandler(this.mainWindow_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.mainPictureBox)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox mainPictureBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addAPointMenuItem;
        private System.Windows.Forms.ToolStripMenuItem movaAPointMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveAnEdgeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveAPoligonMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteAPointMenuItem;
        private System.Windows.Forms.ToolStripMenuItem methodOfDrawingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultMethodOfDrawingMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bresenhamsAlgorithmMethodOfDrawingMenuItem;
    }
}

