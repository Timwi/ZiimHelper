namespace ZiimHelper
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
            this.miNewSingleArrow = new System.Windows.Forms.ToolStripMenuItem();
            this.miNewDoubleArrow = new System.Windows.Forms.ToolStripMenuItem();
            this.miDeleteArrow = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveUpInList = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveDownInList = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveRight = new System.Windows.Forms.ToolStripMenuItem();
            this.miRotateClockwise = new System.Windows.Forms.ToolStripMenuItem();
            this.miRotateCounterclockwise = new System.Windows.Forms.ToolStripMenuItem();
            this.miIncreaseDistance = new System.Windows.Forms.ToolStripMenuItem();
            this.miDecreaseDistance = new System.Windows.Forms.ToolStripMenuItem();
            this.miIncrease2ndDistance = new System.Windows.Forms.ToolStripMenuItem();
            this.miDecrease2ndDistance = new System.Windows.Forms.ToolStripMenuItem();
            this.miPointTo = new System.Windows.Forms.ToolStripMenuItem();
            this.miPoint2ndTo = new System.Windows.Forms.ToolStripMenuItem();
            this.miMark = new System.Windows.Forms.ToolStripMenuItem();
            this.miReflow = new System.Windows.Forms.ToolStripMenuItem();
            this.miSortByCoordinate = new System.Windows.Forms.ToolStripMenuItem();
            this.miAdjustDistances = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.miCopySource = new System.Windows.Forms.ToolStripMenuItem();
            this.miCopyImage = new System.Windows.Forms.ToolStripMenuItem();
            this.miCopyImageByFont = new System.Windows.Forms.ToolStripMenuItem();
            this.miCopyImageByWidth = new System.Windows.Forms.ToolStripMenuItem();
            this.miCopyImageByHeight = new System.Windows.Forms.ToolStripMenuItem();
            this.miSave = new System.Windows.Forms.ToolStripMenuItem();
            this.miRevert = new System.Windows.Forms.ToolStripMenuItem();
            this.splitMain = new RT.Util.Controls.SplitContainerEx();
            this.ctList = new RT.Util.Controls.ListBoxEx();
            this.ctImage = new RT.Util.Controls.DoubleBufferedPanel();
            this.ctMenu = new System.Windows.Forms.MenuStrip();
            this.mnuProgram = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuArrow = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.miSep3 = new System.Windows.Forms.ToolStripSeparator();
            this.miSep4 = new System.Windows.Forms.ToolStripSeparator();
            this.miAnnotate = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuList = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.miGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.miConnectionLines = new System.Windows.Forms.ToolStripMenuItem();
            this.miInstructions = new System.Windows.Forms.ToolStripMenuItem();
            this.miAnnotations = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize) (this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.ctMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // miNewSingleArrow
            // 
            this.miNewSingleArrow.Name = "miNewSingleArrow";
            this.miNewSingleArrow.ShortcutKeys = System.Windows.Forms.Keys.Insert;
            this.miNewSingleArrow.Size = new System.Drawing.Size(266, 22);
            this.miNewSingleArrow.Text = "New &single arrow";
            this.miNewSingleArrow.Click += new System.EventHandler(this.newArrow);
            // 
            // miNewDoubleArrow
            // 
            this.miNewDoubleArrow.Name = "miNewDoubleArrow";
            this.miNewDoubleArrow.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Insert)));
            this.miNewDoubleArrow.Size = new System.Drawing.Size(266, 22);
            this.miNewDoubleArrow.Text = "New dou&ble arrow";
            this.miNewDoubleArrow.Click += new System.EventHandler(this.newArrow);
            // 
            // miDeleteArrow
            // 
            this.miDeleteArrow.Name = "miDeleteArrow";
            this.miDeleteArrow.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.miDeleteArrow.Size = new System.Drawing.Size(266, 22);
            this.miDeleteArrow.Text = "Delete &arrow";
            this.miDeleteArrow.Click += new System.EventHandler(this.deleteArrow);
            // 
            // miMoveUpInList
            // 
            this.miMoveUpInList.Name = "miMoveUpInList";
            this.miMoveUpInList.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
                        | System.Windows.Forms.Keys.Up)));
            this.miMoveUpInList.Size = new System.Drawing.Size(267, 22);
            this.miMoveUpInList.Text = "Move arrow &up in list";
            this.miMoveUpInList.Click += new System.EventHandler(this.moveUpInList);
            // 
            // miMoveDownInList
            // 
            this.miMoveDownInList.Name = "miMoveDownInList";
            this.miMoveDownInList.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
                        | System.Windows.Forms.Keys.Down)));
            this.miMoveDownInList.Size = new System.Drawing.Size(267, 22);
            this.miMoveDownInList.Text = "Move arrow &down in list";
            this.miMoveDownInList.Click += new System.EventHandler(this.moveDownInList);
            // 
            // miMoveUp
            // 
            this.miMoveUp.Name = "miMoveUp";
            this.miMoveUp.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Up)));
            this.miMoveUp.Size = new System.Drawing.Size(266, 22);
            this.miMoveUp.Text = "Move &up";
            this.miMoveUp.Click += new System.EventHandler(this.moveArrow);
            // 
            // miMoveDown
            // 
            this.miMoveDown.Name = "miMoveDown";
            this.miMoveDown.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Down)));
            this.miMoveDown.Size = new System.Drawing.Size(266, 22);
            this.miMoveDown.Text = "Mo&ve down";
            this.miMoveDown.Click += new System.EventHandler(this.moveArrow);
            // 
            // miMoveLeft
            // 
            this.miMoveLeft.Name = "miMoveLeft";
            this.miMoveLeft.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Left)));
            this.miMoveLeft.Size = new System.Drawing.Size(266, 22);
            this.miMoveLeft.Text = "Move &left";
            this.miMoveLeft.Click += new System.EventHandler(this.moveArrow);
            // 
            // miMoveRight
            // 
            this.miMoveRight.Name = "miMoveRight";
            this.miMoveRight.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Right)));
            this.miMoveRight.Size = new System.Drawing.Size(266, 22);
            this.miMoveRight.Text = "Move rig&ht";
            this.miMoveRight.Click += new System.EventHandler(this.moveArrow);
            // 
            // miRotateClockwise
            // 
            this.miRotateClockwise.Name = "miRotateClockwise";
            this.miRotateClockwise.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Right)));
            this.miRotateClockwise.Size = new System.Drawing.Size(266, 22);
            this.miRotateClockwise.Text = "Rotate cloc&kwise";
            this.miRotateClockwise.Click += new System.EventHandler(this.rotate);
            // 
            // miRotateCounterclockwise
            // 
            this.miRotateCounterclockwise.Name = "miRotateCounterclockwise";
            this.miRotateCounterclockwise.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Left)));
            this.miRotateCounterclockwise.Size = new System.Drawing.Size(266, 22);
            this.miRotateCounterclockwise.Text = "Rotate counter-clock&wise";
            this.miRotateCounterclockwise.Click += new System.EventHandler(this.rotate);
            // 
            // miIncreaseDistance
            // 
            this.miIncreaseDistance.Name = "miIncreaseDistance";
            this.miIncreaseDistance.ShortcutKeyDisplayString = "Ctrl+=";
            this.miIncreaseDistance.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemplus)));
            this.miIncreaseDistance.Size = new System.Drawing.Size(266, 22);
            this.miIncreaseDistance.Text = "&Increase distance";
            this.miIncreaseDistance.Click += new System.EventHandler(this.changeDistance);
            // 
            // miDecreaseDistance
            // 
            this.miDecreaseDistance.Name = "miDecreaseDistance";
            this.miDecreaseDistance.ShortcutKeyDisplayString = "Ctrl+-";
            this.miDecreaseDistance.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.OemMinus)));
            this.miDecreaseDistance.Size = new System.Drawing.Size(266, 22);
            this.miDecreaseDistance.Text = "D&ecrease distance";
            this.miDecreaseDistance.Click += new System.EventHandler(this.changeDistance);
            // 
            // miIncrease2ndDistance
            // 
            this.miIncrease2ndDistance.Name = "miIncrease2ndDistance";
            this.miIncrease2ndDistance.ShortcutKeyDisplayString = "Alt+=";
            this.miIncrease2ndDistance.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Oemplus)));
            this.miIncrease2ndDistance.Size = new System.Drawing.Size(266, 22);
            this.miIncrease2ndDistance.Text = "Increase &2nd distance";
            this.miIncrease2ndDistance.Click += new System.EventHandler(this.changeDistance);
            // 
            // miDecrease2ndDistance
            // 
            this.miDecrease2ndDistance.Name = "miDecrease2ndDistance";
            this.miDecrease2ndDistance.ShortcutKeyDisplayString = "Alt+-";
            this.miDecrease2ndDistance.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.OemMinus)));
            this.miDecrease2ndDistance.Size = new System.Drawing.Size(266, 22);
            this.miDecrease2ndDistance.Text = "Decrease 2nd dis&tance";
            this.miDecrease2ndDistance.Click += new System.EventHandler(this.changeDistance);
            // 
            // miPointTo
            // 
            this.miPointTo.Name = "miPointTo";
            this.miPointTo.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.miPointTo.Size = new System.Drawing.Size(266, 22);
            this.miPointTo.Text = "&Point to...";
            this.miPointTo.Click += new System.EventHandler(this.pointTo);
            // 
            // miPoint2ndTo
            // 
            this.miPoint2ndTo.Name = "miPoint2ndTo";
            this.miPoint2ndTo.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.P)));
            this.miPoint2ndTo.Size = new System.Drawing.Size(266, 22);
            this.miPoint2ndTo.Text = "P&oint 2nd to...";
            this.miPoint2ndTo.Click += new System.EventHandler(this.pointTo);
            // 
            // miMark
            // 
            this.miMark.Name = "miMark";
            this.miMark.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.miMark.Size = new System.Drawing.Size(266, 22);
            this.miMark.Text = "Toggle &mark";
            this.miMark.Click += new System.EventHandler(this.toggleMark);
            // 
            // miReflow
            // 
            this.miReflow.Name = "miReflow";
            this.miReflow.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.miReflow.Size = new System.Drawing.Size(307, 22);
            this.miReflow.Text = "&Reflow";
            this.miReflow.Click += new System.EventHandler(this.reflow);
            // 
            // miSortByCoordinate
            // 
            this.miSortByCoordinate.Name = "miSortByCoordinate";
            this.miSortByCoordinate.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.miSortByCoordinate.Size = new System.Drawing.Size(267, 22);
            this.miSortByCoordinate.Text = "Sor&t by co-ordinate";
            this.miSortByCoordinate.Click += new System.EventHandler(this.sort);
            // 
            // miAdjustDistances
            // 
            this.miAdjustDistances.Name = "miAdjustDistances";
            this.miAdjustDistances.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.miAdjustDistances.Size = new System.Drawing.Size(266, 22);
            this.miAdjustDistances.Text = "Adjust &distance values to actual";
            this.miAdjustDistances.Click += new System.EventHandler(this.adjustDistances);
            // 
            // miSep1
            // 
            this.miSep1.Name = "miSep1";
            this.miSep1.Size = new System.Drawing.Size(304, 6);
            // 
            // miCopySource
            // 
            this.miCopySource.Name = "miCopySource";
            this.miCopySource.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.C)));
            this.miCopySource.Size = new System.Drawing.Size(307, 22);
            this.miCopySource.Text = "Copy program to clipboard as te&xt";
            this.miCopySource.Click += new System.EventHandler(this.copySource);
            // 
            // miCopyImage
            // 
            this.miCopyImage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCopyImageByFont,
            this.miCopyImageByWidth,
            this.miCopyImageByHeight});
            this.miCopyImage.Name = "miCopyImage";
            this.miCopyImage.Size = new System.Drawing.Size(307, 22);
            this.miCopyImage.Text = "&Copy program to clipboard as image";
            // 
            // miCopyImageByFont
            // 
            this.miCopyImageByFont.Name = "miCopyImageByFont";
            this.miCopyImageByFont.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.C)));
            this.miCopyImageByFont.Size = new System.Drawing.Size(250, 22);
            this.miCopyImageByFont.Text = "... using fixed &font size";
            this.miCopyImageByFont.Click += new System.EventHandler(this.copyImage);
            // 
            // miCopyImageByWidth
            // 
            this.miCopyImageByWidth.Name = "miCopyImageByWidth";
            this.miCopyImageByWidth.Size = new System.Drawing.Size(250, 22);
            this.miCopyImageByWidth.Text = "... using fixed bitmap &width";
            this.miCopyImageByWidth.Click += new System.EventHandler(this.copyImage);
            // 
            // miCopyImageByHeight
            // 
            this.miCopyImageByHeight.Name = "miCopyImageByHeight";
            this.miCopyImageByHeight.Size = new System.Drawing.Size(250, 22);
            this.miCopyImageByHeight.Text = "... using fixed bitmap &height";
            this.miCopyImageByHeight.Click += new System.EventHandler(this.copyImage);
            // 
            // miSave
            // 
            this.miSave.Name = "miSave";
            this.miSave.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.miSave.Size = new System.Drawing.Size(307, 22);
            this.miSave.Text = "&Save";
            this.miSave.Click += new System.EventHandler(this.save);
            // 
            // miRevert
            // 
            this.miRevert.Name = "miRevert";
            this.miRevert.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.miRevert.Size = new System.Drawing.Size(307, 22);
            this.miRevert.Text = "Re&vert";
            this.miRevert.Click += new System.EventHandler(this.revert);
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 24);
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
            this.splitMain.Size = new System.Drawing.Size(716, 655);
            this.splitMain.SplitterDistance = 197;
            this.splitMain.TabIndex = 0;
            // 
            // ctList
            // 
            this.ctList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctList.FormattingEnabled = true;
            this.ctList.IntegralHeight = false;
            this.ctList.Location = new System.Drawing.Point(0, 0);
            this.ctList.Name = "ctList";
            this.ctList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.ctList.Size = new System.Drawing.Size(197, 655);
            this.ctList.TabIndex = 0;
            this.ctList.SelectedIndexChanged += new System.EventHandler(this.selectedIndexChanged);
            // 
            // ctImage
            // 
            this.ctImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctImage.Font = new System.Drawing.Font("Cambria", 7F);
            this.ctImage.Location = new System.Drawing.Point(0, 0);
            this.ctImage.Name = "ctImage";
            this.ctImage.Size = new System.Drawing.Size(515, 655);
            this.ctImage.TabIndex = 0;
            this.ctImage.PaintBuffer += new System.Windows.Forms.PaintEventHandler(this.paintBuffer);
            this.ctImage.Paint += new System.Windows.Forms.PaintEventHandler(this.paint);
            this.ctImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imageMouseDown);
            this.ctImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.imageMouseMove);
            this.ctImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.imageMouseUp);
            // 
            // ctMenu
            // 
            this.ctMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuProgram,
            this.mnuArrow,
            this.mnuList,
            this.mnuView});
            this.ctMenu.Location = new System.Drawing.Point(0, 0);
            this.ctMenu.Name = "ctMenu";
            this.ctMenu.Size = new System.Drawing.Size(716, 24);
            this.ctMenu.TabIndex = 1;
            this.ctMenu.Text = "menuStrip1";
            // 
            // mnuProgram
            // 
            this.mnuProgram.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miReflow,
            this.miSep5,
            this.miRevert,
            this.miSave,
            this.miSep1,
            this.miCopySource,
            this.miCopyImage});
            this.mnuProgram.Name = "mnuProgram";
            this.mnuProgram.Size = new System.Drawing.Size(59, 20);
            this.mnuProgram.Text = "&Program";
            // 
            // miSep5
            // 
            this.miSep5.Name = "miSep5";
            this.miSep5.Size = new System.Drawing.Size(304, 6);
            // 
            // mnuArrow
            // 
            this.mnuArrow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNewSingleArrow,
            this.miNewDoubleArrow,
            this.miDeleteArrow,
            this.miSep2,
            this.miMoveUp,
            this.miMoveDown,
            this.miMoveLeft,
            this.miMoveRight,
            this.miRotateClockwise,
            this.miRotateCounterclockwise,
            this.miSep3,
            this.miIncreaseDistance,
            this.miDecreaseDistance,
            this.miIncrease2ndDistance,
            this.miDecrease2ndDistance,
            this.miAdjustDistances,
            this.miSep4,
            this.miPointTo,
            this.miPoint2ndTo,
            this.miMark,
            this.miAnnotate});
            this.mnuArrow.Name = "mnuArrow";
            this.mnuArrow.Size = new System.Drawing.Size(48, 20);
            this.mnuArrow.Text = "&Arrow";
            // 
            // miSep2
            // 
            this.miSep2.Name = "miSep2";
            this.miSep2.Size = new System.Drawing.Size(263, 6);
            // 
            // miSep3
            // 
            this.miSep3.Name = "miSep3";
            this.miSep3.Size = new System.Drawing.Size(263, 6);
            // 
            // miSep4
            // 
            this.miSep4.Name = "miSep4";
            this.miSep4.Size = new System.Drawing.Size(263, 6);
            // 
            // miAnnotate
            // 
            this.miAnnotate.Name = "miAnnotate";
            this.miAnnotate.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Return)));
            this.miAnnotate.Size = new System.Drawing.Size(266, 22);
            this.miAnnotate.Text = "A&nnotate...";
            this.miAnnotate.Click += new System.EventHandler(this.annotate);
            // 
            // mnuList
            // 
            this.mnuList.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miMoveUpInList,
            this.miMoveDownInList,
            this.miSortByCoordinate});
            this.mnuList.Name = "mnuList";
            this.mnuList.Size = new System.Drawing.Size(35, 20);
            this.mnuList.Text = "&List";
            // 
            // mnuView
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miGrid,
            this.miConnectionLines,
            this.miInstructions,
            this.miAnnotations});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(41, 20);
            this.mnuView.Text = "&View";
            // 
            // miGrid
            // 
            this.miGrid.Name = "miGrid";
            this.miGrid.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.miGrid.Size = new System.Drawing.Size(189, 22);
            this.miGrid.Text = "&Grid";
            this.miGrid.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // miConnectionLines
            // 
            this.miConnectionLines.Name = "miConnectionLines";
            this.miConnectionLines.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.miConnectionLines.Size = new System.Drawing.Size(189, 22);
            this.miConnectionLines.Text = "&Connection lines";
            this.miConnectionLines.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // miInstructions
            // 
            this.miInstructions.Name = "miInstructions";
            this.miInstructions.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.miInstructions.Size = new System.Drawing.Size(189, 22);
            this.miInstructions.Text = "&Instructions";
            this.miInstructions.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // miAnnotations
            // 
            this.miAnnotations.Name = "miAnnotations";
            this.miAnnotations.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.miAnnotations.Size = new System.Drawing.Size(189, 22);
            this.miAnnotations.Text = "&Annotations";
            this.miAnnotations.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // Mainform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 679);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.ctMenu);
            this.MainMenuStrip = this.ctMenu;
            this.Name = "Mainform";
            this.Text = "Ziim Helper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.save);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.ctMenu.ResumeLayout(false);
            this.ctMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RT.Util.Controls.SplitContainerEx splitMain;
        private RT.Util.Controls.ListBoxEx ctList;
        private RT.Util.Controls.DoubleBufferedPanel ctImage;
        private System.Windows.Forms.ToolStripMenuItem miNewSingleArrow;
        private System.Windows.Forms.ToolStripMenuItem miDeleteArrow;
        private System.Windows.Forms.ToolStripMenuItem miNewDoubleArrow;
        private System.Windows.Forms.ToolStripMenuItem miMoveUpInList;
        private System.Windows.Forms.ToolStripMenuItem miMoveDownInList;
        private System.Windows.Forms.ToolStripMenuItem miRotateClockwise;
        private System.Windows.Forms.ToolStripMenuItem miRotateCounterclockwise;
        private System.Windows.Forms.ToolStripMenuItem miReflow;
        private System.Windows.Forms.ToolStripMenuItem miIncreaseDistance;
        private System.Windows.Forms.ToolStripMenuItem miDecreaseDistance;
        private System.Windows.Forms.ToolStripMenuItem miIncrease2ndDistance;
        private System.Windows.Forms.ToolStripMenuItem miDecrease2ndDistance;
        private System.Windows.Forms.ToolStripMenuItem miPointTo;
        private System.Windows.Forms.ToolStripMenuItem miPoint2ndTo;
        private System.Windows.Forms.ToolStripSeparator miSep1;
        private System.Windows.Forms.ToolStripMenuItem miSave;
        private System.Windows.Forms.ToolStripMenuItem miMoveUp;
        private System.Windows.Forms.ToolStripMenuItem miMoveDown;
        private System.Windows.Forms.ToolStripMenuItem miMoveLeft;
        private System.Windows.Forms.ToolStripMenuItem miMoveRight;
        private System.Windows.Forms.ToolStripMenuItem miSortByCoordinate;
        private System.Windows.Forms.ToolStripMenuItem miRevert;
        private System.Windows.Forms.ToolStripMenuItem miAdjustDistances;
        private System.Windows.Forms.ToolStripMenuItem miMark;
        private System.Windows.Forms.ToolStripMenuItem miCopySource;
        private System.Windows.Forms.ToolStripMenuItem miCopyImage;
        private System.Windows.Forms.MenuStrip ctMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuArrow;
        private System.Windows.Forms.ToolStripMenuItem mnuList;
        private System.Windows.Forms.ToolStripMenuItem mnuProgram;
        private System.Windows.Forms.ToolStripSeparator miSep2;
        private System.Windows.Forms.ToolStripSeparator miSep3;
        private System.Windows.Forms.ToolStripSeparator miSep4;
        private System.Windows.Forms.ToolStripSeparator miSep5;
        private System.Windows.Forms.ToolStripMenuItem miAnnotate;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem miGrid;
        private System.Windows.Forms.ToolStripMenuItem miConnectionLines;
        private System.Windows.Forms.ToolStripMenuItem miInstructions;
        private System.Windows.Forms.ToolStripMenuItem miAnnotations;
        private System.Windows.Forms.ToolStripMenuItem miCopyImageByWidth;
        private System.Windows.Forms.ToolStripMenuItem miCopyImageByHeight;
        private System.Windows.Forms.ToolStripMenuItem miCopyImageByFont;
    }
}

