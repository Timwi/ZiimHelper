namespace GraphiteHelper
{
    partial class Mainform
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mnuContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miNewSingleArrow = new System.Windows.Forms.ToolStripMenuItem();
            this.miNewDoubleArrow = new System.Windows.Forms.ToolStripMenuItem();
            this.miDeleteArrow = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveUpInList = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveDownInList = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveRight = new System.Windows.Forms.ToolStripMenuItem();
            this.miRename = new System.Windows.Forms.ToolStripMenuItem();
            this.miRotateClockwise = new System.Windows.Forms.ToolStripMenuItem();
            this.miRotateCounterclockwise = new System.Windows.Forms.ToolStripMenuItem();
            this.miIncreaseDistance = new System.Windows.Forms.ToolStripMenuItem();
            this.miDecreaseDistance = new System.Windows.Forms.ToolStripMenuItem();
            this.miIncrease2ndDistance = new System.Windows.Forms.ToolStripMenuItem();
            this.miDecrease2ndDistance = new System.Windows.Forms.ToolStripMenuItem();
            this.miPointTo = new System.Windows.Forms.ToolStripMenuItem();
            this.miPoint2ndTo = new System.Windows.Forms.ToolStripMenuItem();
            this.miReflow = new System.Windows.Forms.ToolStripMenuItem();
            this.miSortByCoordinate = new System.Windows.Forms.ToolStripMenuItem();
            this.miSortByName = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.miSave = new System.Windows.Forms.ToolStripMenuItem();
            this.miUnusedMnemonics = new System.Windows.Forms.ToolStripMenuItem();
            this.splitMain = new RT.Util.Controls.SplitContainerEx();
            this.ctList = new RT.Util.Controls.ListBoxEx();
            this.ctImage = new RT.Util.Controls.DoubleBufferedPanel();
            this.miRevert = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuContext
            // 
            this.mnuContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNewSingleArrow,
            this.miNewDoubleArrow,
            this.miDeleteArrow,
            this.miMoveUpInList,
            this.miMoveDownInList,
            this.miMoveUp,
            this.miMoveDown,
            this.miMoveLeft,
            this.miMoveRight,
            this.miRename,
            this.miRotateClockwise,
            this.miRotateCounterclockwise,
            this.miIncreaseDistance,
            this.miDecreaseDistance,
            this.miIncrease2ndDistance,
            this.miDecrease2ndDistance,
            this.miPointTo,
            this.miPoint2ndTo,
            this.miReflow,
            this.miSortByCoordinate,
            this.miSortByName,
            this.miSep1,
            this.miSave,
            this.miRevert,
            this.miUnusedMnemonics});
            this.mnuContext.Name = "mnuContext";
            this.mnuContext.Size = new System.Drawing.Size(243, 560);
            // 
            // miNewSingleArrow
            // 
            this.miNewSingleArrow.Name = "miNewSingleArrow";
            this.miNewSingleArrow.ShortcutKeys = System.Windows.Forms.Keys.Insert;
            this.miNewSingleArrow.Size = new System.Drawing.Size(242, 22);
            this.miNewSingleArrow.Text = "New sin&gle arrow";
            this.miNewSingleArrow.Click += new System.EventHandler(this.newArrow);
            // 
            // miNewDoubleArrow
            // 
            this.miNewDoubleArrow.Name = "miNewDoubleArrow";
            this.miNewDoubleArrow.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Insert)));
            this.miNewDoubleArrow.Size = new System.Drawing.Size(242, 22);
            this.miNewDoubleArrow.Text = "New dou&ble arrow";
            this.miNewDoubleArrow.Click += new System.EventHandler(this.newArrow);
            // 
            // miDeleteArrow
            // 
            this.miDeleteArrow.Name = "miDeleteArrow";
            this.miDeleteArrow.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.miDeleteArrow.Size = new System.Drawing.Size(242, 22);
            this.miDeleteArrow.Text = "Delete &arrow";
            this.miDeleteArrow.Click += new System.EventHandler(this.deleteArrow);
            // 
            // miMoveUpInList
            // 
            this.miMoveUpInList.Name = "miMoveUpInList";
            this.miMoveUpInList.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
                        | System.Windows.Forms.Keys.Up)));
            this.miMoveUpInList.Size = new System.Drawing.Size(242, 22);
            this.miMoveUpInList.Text = "Move &up in list";
            this.miMoveUpInList.Click += new System.EventHandler(this.moveUpInList);
            // 
            // miMoveDownInList
            // 
            this.miMoveDownInList.Name = "miMoveDownInList";
            this.miMoveDownInList.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
                        | System.Windows.Forms.Keys.Down)));
            this.miMoveDownInList.Size = new System.Drawing.Size(242, 22);
            this.miMoveDownInList.Text = "Move dow&n in list";
            this.miMoveDownInList.Click += new System.EventHandler(this.moveDownInList);
            // 
            // miMoveUp
            // 
            this.miMoveUp.Name = "miMoveUp";
            this.miMoveUp.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Up)));
            this.miMoveUp.Size = new System.Drawing.Size(242, 22);
            this.miMoveUp.Text = "&Move up";
            this.miMoveUp.Click += new System.EventHandler(this.moveArrow);
            // 
            // miMoveDown
            // 
            this.miMoveDown.Name = "miMoveDown";
            this.miMoveDown.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Down)));
            this.miMoveDown.Size = new System.Drawing.Size(242, 22);
            this.miMoveDown.Text = "Move &down";
            this.miMoveDown.Click += new System.EventHandler(this.moveArrow);
            // 
            // miMoveLeft
            // 
            this.miMoveLeft.Name = "miMoveLeft";
            this.miMoveLeft.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Left)));
            this.miMoveLeft.Size = new System.Drawing.Size(242, 22);
            this.miMoveLeft.Text = "Move &left";
            this.miMoveLeft.Click += new System.EventHandler(this.moveArrow);
            // 
            // miMoveRight
            // 
            this.miMoveRight.Name = "miMoveRight";
            this.miMoveRight.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Right)));
            this.miMoveRight.Size = new System.Drawing.Size(242, 22);
            this.miMoveRight.Text = "Move &right";
            this.miMoveRight.Click += new System.EventHandler(this.moveArrow);
            // 
            // miRename
            // 
            this.miRename.Name = "miRename";
            this.miRename.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Return)));
            this.miRename.Size = new System.Drawing.Size(242, 22);
            this.miRename.Text = "C&hange name...";
            this.miRename.Click += new System.EventHandler(this.rename);
            // 
            // miRotateClockwise
            // 
            this.miRotateClockwise.Name = "miRotateClockwise";
            this.miRotateClockwise.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Right)));
            this.miRotateClockwise.Size = new System.Drawing.Size(242, 22);
            this.miRotateClockwise.Text = "Rotate cloc&kwise";
            this.miRotateClockwise.Click += new System.EventHandler(this.rotate);
            // 
            // miRotateCounterclockwise
            // 
            this.miRotateCounterclockwise.Name = "miRotateCounterclockwise";
            this.miRotateCounterclockwise.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Left)));
            this.miRotateCounterclockwise.Size = new System.Drawing.Size(242, 22);
            this.miRotateCounterclockwise.Text = "Rotate counter-clock&wise";
            this.miRotateCounterclockwise.Click += new System.EventHandler(this.rotate);
            // 
            // miIncreaseDistance
            // 
            this.miIncreaseDistance.Name = "miIncreaseDistance";
            this.miIncreaseDistance.ShortcutKeyDisplayString = "Ctrl+=";
            this.miIncreaseDistance.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemplus)));
            this.miIncreaseDistance.Size = new System.Drawing.Size(242, 22);
            this.miIncreaseDistance.Text = "&Increase distance";
            this.miIncreaseDistance.Click += new System.EventHandler(this.changeDistance);
            // 
            // miDecreaseDistance
            // 
            this.miDecreaseDistance.Name = "miDecreaseDistance";
            this.miDecreaseDistance.ShortcutKeyDisplayString = "Ctrl+-";
            this.miDecreaseDistance.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.OemMinus)));
            this.miDecreaseDistance.Size = new System.Drawing.Size(242, 22);
            this.miDecreaseDistance.Text = "D&ecrease distance";
            this.miDecreaseDistance.Click += new System.EventHandler(this.changeDistance);
            // 
            // miIncrease2ndDistance
            // 
            this.miIncrease2ndDistance.Name = "miIncrease2ndDistance";
            this.miIncrease2ndDistance.ShortcutKeyDisplayString = "Alt+=";
            this.miIncrease2ndDistance.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Oemplus)));
            this.miIncrease2ndDistance.Size = new System.Drawing.Size(242, 22);
            this.miIncrease2ndDistance.Text = "Increase &2nd distance";
            this.miIncrease2ndDistance.Click += new System.EventHandler(this.changeDistance);
            // 
            // miDecrease2ndDistance
            // 
            this.miDecrease2ndDistance.Name = "miDecrease2ndDistance";
            this.miDecrease2ndDistance.ShortcutKeyDisplayString = "Alt+-";
            this.miDecrease2ndDistance.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.OemMinus)));
            this.miDecrease2ndDistance.Size = new System.Drawing.Size(242, 22);
            this.miDecrease2ndDistance.Text = "Decrease 2nd dis&tance";
            this.miDecrease2ndDistance.Click += new System.EventHandler(this.changeDistance);
            // 
            // miPointTo
            // 
            this.miPointTo.Name = "miPointTo";
            this.miPointTo.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.miPointTo.Size = new System.Drawing.Size(242, 22);
            this.miPointTo.Text = "&Point to...";
            this.miPointTo.Click += new System.EventHandler(this.pointTo);
            // 
            // miPoint2ndTo
            // 
            this.miPoint2ndTo.Name = "miPoint2ndTo";
            this.miPoint2ndTo.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.P)));
            this.miPoint2ndTo.Size = new System.Drawing.Size(242, 22);
            this.miPoint2ndTo.Text = "P&oint 2nd to...";
            this.miPoint2ndTo.Click += new System.EventHandler(this.pointTo);
            // 
            // miReflow
            // 
            this.miReflow.Name = "miReflow";
            this.miReflow.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.miReflow.Size = new System.Drawing.Size(242, 22);
            this.miReflow.Text = "Re&flow";
            this.miReflow.Click += new System.EventHandler(this.reflow);
            // 
            // miSortByCoordinate
            // 
            this.miSortByCoordinate.Name = "miSortByCoordinate";
            this.miSortByCoordinate.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.miSortByCoordinate.Size = new System.Drawing.Size(242, 22);
            this.miSortByCoordinate.Text = "Sort by &co-ordinate";
            this.miSortByCoordinate.Click += new System.EventHandler(this.sort);
            // 
            // miSortByName
            // 
            this.miSortByName.Name = "miSortByName";
            this.miSortByName.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.miSortByName.Size = new System.Drawing.Size(242, 22);
            this.miSortByName.Text = "Sort b&y name";
            this.miSortByName.Click += new System.EventHandler(this.sort);
            // 
            // miSep1
            // 
            this.miSep1.Name = "miSep1";
            this.miSep1.Size = new System.Drawing.Size(239, 6);
            // 
            // miSave
            // 
            this.miSave.Name = "miSave";
            this.miSave.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.miSave.Size = new System.Drawing.Size(242, 22);
            this.miSave.Text = "&Save";
            this.miSave.Click += new System.EventHandler(this.save);
            // 
            // miUnusedMnemonics
            // 
            this.miUnusedMnemonics.Name = "miUnusedMnemonics";
            this.miUnusedMnemonics.Size = new System.Drawing.Size(242, 22);
            this.miUnusedMnemonics.Text = "jqxz";
            this.miUnusedMnemonics.Visible = false;
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            this.splitMain.PaintSplitter = true;
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.ctList);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.ctImage);
            this.splitMain.Size = new System.Drawing.Size(716, 679);
            this.splitMain.SplitterDistance = 197;
            this.splitMain.TabIndex = 0;
            // 
            // ctList
            // 
            this.ctList.ContextMenuStrip = this.mnuContext;
            this.ctList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctList.FormattingEnabled = true;
            this.ctList.IntegralHeight = false;
            this.ctList.Location = new System.Drawing.Point(0, 0);
            this.ctList.Name = "ctList";
            this.ctList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.ctList.Size = new System.Drawing.Size(197, 679);
            this.ctList.TabIndex = 0;
            this.ctList.SelectedIndexChanged += new System.EventHandler(this.selectedIndexChanged);
            // 
            // ctImage
            // 
            this.ctImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctImage.Font = new System.Drawing.Font("Cambria", 10F);
            this.ctImage.Location = new System.Drawing.Point(0, 0);
            this.ctImage.Name = "ctImage";
            this.ctImage.Size = new System.Drawing.Size(515, 679);
            this.ctImage.TabIndex = 0;
            this.ctImage.PaintBuffer += new System.Windows.Forms.PaintEventHandler(this.paintBuffer);
            this.ctImage.Paint += new System.Windows.Forms.PaintEventHandler(this.paint);
            this.ctImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imageMouseDown);
            this.ctImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.imageMouseMove);
            this.ctImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.imageMouseUp);
            // 
            // revertToolStripMenuItem
            // 
            this.miRevert.Name = "revertToolStripMenuItem";
            this.miRevert.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.miRevert.Size = new System.Drawing.Size(242, 22);
            this.miRevert.Text = "Re&vert";
            this.miRevert.Click += new System.EventHandler(this.revert);
            // 
            // Mainform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 679);
            this.Controls.Add(this.splitMain);
            this.Name = "Mainform";
            this.Text = "Graphite Helper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.save);
            this.mnuContext.ResumeLayout(false);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private RT.Util.Controls.SplitContainerEx splitMain;
        private RT.Util.Controls.ListBoxEx ctList;
        private RT.Util.Controls.DoubleBufferedPanel ctImage;
        private System.Windows.Forms.ContextMenuStrip mnuContext;
        private System.Windows.Forms.ToolStripMenuItem miNewSingleArrow;
        private System.Windows.Forms.ToolStripMenuItem miDeleteArrow;
        private System.Windows.Forms.ToolStripMenuItem miNewDoubleArrow;
        private System.Windows.Forms.ToolStripMenuItem miMoveUpInList;
        private System.Windows.Forms.ToolStripMenuItem miMoveDownInList;
        private System.Windows.Forms.ToolStripMenuItem miRename;
        private System.Windows.Forms.ToolStripMenuItem miRotateClockwise;
        private System.Windows.Forms.ToolStripMenuItem miRotateCounterclockwise;
        private System.Windows.Forms.ToolStripMenuItem miReflow;
        private System.Windows.Forms.ToolStripMenuItem miIncreaseDistance;
        private System.Windows.Forms.ToolStripMenuItem miDecreaseDistance;
        private System.Windows.Forms.ToolStripMenuItem miIncrease2ndDistance;
        private System.Windows.Forms.ToolStripMenuItem miDecrease2ndDistance;
        private System.Windows.Forms.ToolStripMenuItem miUnusedMnemonics;
        private System.Windows.Forms.ToolStripMenuItem miPointTo;
        private System.Windows.Forms.ToolStripMenuItem miPoint2ndTo;
        private System.Windows.Forms.ToolStripSeparator miSep1;
        private System.Windows.Forms.ToolStripMenuItem miSave;
        private System.Windows.Forms.ToolStripMenuItem miMoveUp;
        private System.Windows.Forms.ToolStripMenuItem miMoveDown;
        private System.Windows.Forms.ToolStripMenuItem miMoveLeft;
        private System.Windows.Forms.ToolStripMenuItem miMoveRight;
        private System.Windows.Forms.ToolStripMenuItem miSortByCoordinate;
        private System.Windows.Forms.ToolStripMenuItem miSortByName;
        private System.Windows.Forms.ToolStripMenuItem miRevert;
    }
}

