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
            this.miDeleteArrow = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveRight = new System.Windows.Forms.ToolStripMenuItem();
            this.miRotateClockwise = new System.Windows.Forms.ToolStripMenuItem();
            this.miRotateCounterclockwise = new System.Windows.Forms.ToolStripMenuItem();
            this.miMark = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.miCopySource = new System.Windows.Forms.ToolStripMenuItem();
            this.miCopyImage = new System.Windows.Forms.ToolStripMenuItem();
            this.miCopyImageByFont = new System.Windows.Forms.ToolStripMenuItem();
            this.miCopyImageByWidth = new System.Windows.Forms.ToolStripMenuItem();
            this.miCopyImageByHeight = new System.Windows.Forms.ToolStripMenuItem();
            this.miSave = new System.Windows.Forms.ToolStripMenuItem();
            this.miRevert = new System.Windows.Forms.ToolStripMenuItem();
            this.ctImage = new RT.Util.Controls.DoubleBufferedPanel();
            this.ctMenu = new System.Windows.Forms.MenuStrip();
            this.mnuProgram = new System.Windows.Forms.ToolStripMenuItem();
            this.miNew = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep4 = new System.Windows.Forms.ToolStripSeparator();
            this.miImport = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep3 = new System.Windows.Forms.ToolStripSeparator();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuArrow = new System.Windows.Forms.ToolStripMenuItem();
            this.miToggleInput = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.miAnnotate = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCloud = new System.Windows.Forms.ToolStripMenuItem();
            this.miColor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.miGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.miConnectionLines = new System.Windows.Forms.ToolStripMenuItem();
            this.miInstructions = new System.Windows.Forms.ToolStripMenuItem();
            this.miAnnotations = new System.Windows.Forms.ToolStripMenuItem();
            this.miOwnCloud = new System.Windows.Forms.ToolStripMenuItem();
            this.miInnerClouds = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMode = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.miDraw = new System.Windows.Forms.ToolStripMenuItem();
            this.miCoordinates = new System.Windows.Forms.ToolStripMenuItem();
            this.ctMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // miDeleteArrow
            // 
            this.miDeleteArrow.Name = "miDeleteArrow";
            this.miDeleteArrow.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.miDeleteArrow.Size = new System.Drawing.Size(242, 22);
            this.miDeleteArrow.Text = "&Delete arrow";
            this.miDeleteArrow.Click += new System.EventHandler(this.deleteArrow);
            // 
            // miMoveUp
            // 
            this.miMoveUp.Name = "miMoveUp";
            this.miMoveUp.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Up)));
            this.miMoveUp.Size = new System.Drawing.Size(242, 22);
            this.miMoveUp.Text = "Move &up";
            this.miMoveUp.Click += new System.EventHandler(this.move);
            // 
            // miMoveDown
            // 
            this.miMoveDown.Name = "miMoveDown";
            this.miMoveDown.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Down)));
            this.miMoveDown.Size = new System.Drawing.Size(242, 22);
            this.miMoveDown.Text = "Mo&ve down";
            this.miMoveDown.Click += new System.EventHandler(this.move);
            // 
            // miMoveLeft
            // 
            this.miMoveLeft.Name = "miMoveLeft";
            this.miMoveLeft.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Left)));
            this.miMoveLeft.Size = new System.Drawing.Size(242, 22);
            this.miMoveLeft.Text = "Move &left";
            this.miMoveLeft.Click += new System.EventHandler(this.move);
            // 
            // miMoveRight
            // 
            this.miMoveRight.Name = "miMoveRight";
            this.miMoveRight.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Right)));
            this.miMoveRight.Size = new System.Drawing.Size(242, 22);
            this.miMoveRight.Text = "Move rig&ht";
            this.miMoveRight.Click += new System.EventHandler(this.move);
            // 
            // miRotateClockwise
            // 
            this.miRotateClockwise.Name = "miRotateClockwise";
            this.miRotateClockwise.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Right)));
            this.miRotateClockwise.Size = new System.Drawing.Size(242, 22);
            this.miRotateClockwise.Text = "&Rotate clockwise";
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
            // miMark
            // 
            this.miMark.Name = "miMark";
            this.miMark.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.miMark.Size = new System.Drawing.Size(242, 22);
            this.miMark.Text = "Toggle mar&k";
            this.miMark.Click += new System.EventHandler(this.toggleMark);
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
            this.miCopySource.Text = "Copy &program to clipboard as text";
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
            // ctImage
            // 
            this.ctImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctImage.Font = new System.Drawing.Font("Cambria", 7F);
            this.ctImage.Location = new System.Drawing.Point(0, 24);
            this.ctImage.Name = "ctImage";
            this.ctImage.Size = new System.Drawing.Size(716, 655);
            this.ctImage.TabIndex = 0;
            this.ctImage.PaintBuffer += new System.Windows.Forms.PaintEventHandler(this.paintBuffer);
            this.ctImage.Paint += new System.Windows.Forms.PaintEventHandler(this.paint);
            this.ctImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseDown);
            this.ctImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove);
            this.ctImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mouseUp);
            // 
            // ctMenu
            // 
            this.ctMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuProgram,
            this.mnuArrow,
            this.mnuCloud,
            this.mnuView,
            this.mnuMode});
            this.ctMenu.Location = new System.Drawing.Point(0, 0);
            this.ctMenu.Name = "ctMenu";
            this.ctMenu.Size = new System.Drawing.Size(716, 24);
            this.ctMenu.TabIndex = 1;
            this.ctMenu.Text = "menuStrip1";
            // 
            // mnuProgram
            // 
            this.mnuProgram.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNew,
            this.miOpen,
            this.miSave,
            this.miSaveAs,
            this.miRevert,
            this.miSep4,
            this.miImport,
            this.miSep1,
            this.miCopySource,
            this.miCopyImage,
            this.miSep3,
            this.miExit});
            this.mnuProgram.Name = "mnuProgram";
            this.mnuProgram.Size = new System.Drawing.Size(59, 20);
            this.mnuProgram.Text = "&Program";
            // 
            // miNew
            // 
            this.miNew.Name = "miNew";
            this.miNew.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.miNew.Size = new System.Drawing.Size(307, 22);
            this.miNew.Text = "&New";
            this.miNew.Click += new System.EventHandler(this.fileNew);
            // 
            // miOpen
            // 
            this.miOpen.Name = "miOpen";
            this.miOpen.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.miOpen.Size = new System.Drawing.Size(307, 22);
            this.miOpen.Text = "&Open...";
            this.miOpen.Click += new System.EventHandler(this.open);
            // 
            // miSaveAs
            // 
            this.miSaveAs.Name = "miSaveAs";
            this.miSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys) (((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.S)));
            this.miSaveAs.Size = new System.Drawing.Size(307, 22);
            this.miSaveAs.Text = "Save &As...";
            this.miSaveAs.Click += new System.EventHandler(this.saveAs);
            // 
            // miSep4
            // 
            this.miSep4.Name = "miSep4";
            this.miSep4.Size = new System.Drawing.Size(304, 6);
            // 
            // miImport
            // 
            this.miImport.Name = "miImport";
            this.miImport.Size = new System.Drawing.Size(307, 22);
            this.miImport.Text = "&Import...";
            this.miImport.Click += new System.EventHandler(this.import);
            // 
            // miSep3
            // 
            this.miSep3.Name = "miSep3";
            this.miSep3.Size = new System.Drawing.Size(304, 6);
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(307, 22);
            this.miExit.Text = "E&xit";
            this.miExit.Click += new System.EventHandler(this.exit);
            // 
            // mnuArrow
            // 
            this.mnuArrow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miMoveUp,
            this.miMoveDown,
            this.miMoveLeft,
            this.miMoveRight,
            this.miRotateClockwise,
            this.miRotateCounterclockwise,
            this.miDeleteArrow,
            this.miToggleInput,
            this.miSep2,
            this.miMark,
            this.miAnnotate});
            this.mnuArrow.Name = "mnuArrow";
            this.mnuArrow.Size = new System.Drawing.Size(48, 20);
            this.mnuArrow.Text = "&Arrow";
            // 
            // miToggleInput
            // 
            this.miToggleInput.Name = "miToggleInput";
            this.miToggleInput.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.miToggleInput.Size = new System.Drawing.Size(242, 22);
            this.miToggleInput.Text = "Toggle &input";
            this.miToggleInput.Click += new System.EventHandler(this.toggleInput);
            // 
            // miSep2
            // 
            this.miSep2.Name = "miSep2";
            this.miSep2.Size = new System.Drawing.Size(239, 6);
            // 
            // miAnnotate
            // 
            this.miAnnotate.Name = "miAnnotate";
            this.miAnnotate.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Return)));
            this.miAnnotate.Size = new System.Drawing.Size(242, 22);
            this.miAnnotate.Text = "A&nnotate...";
            this.miAnnotate.Click += new System.EventHandler(this.annotate);
            // 
            // mnuCloud
            // 
            this.mnuCloud.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miColor});
            this.mnuCloud.Name = "mnuCloud";
            this.mnuCloud.Size = new System.Drawing.Size(46, 20);
            this.mnuCloud.Text = "&Cloud";
            // 
            // miColor
            // 
            this.miColor.Name = "miColor";
            this.miColor.Size = new System.Drawing.Size(152, 22);
            this.miColor.Text = "&Color...";
            this.miColor.Click += new System.EventHandler(this.cloudColor);
            // 
            // mnuView
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miGrid,
            this.miConnectionLines,
            this.miInstructions,
            this.miAnnotations,
            this.miOwnCloud,
            this.miInnerClouds,
            this.miCoordinates});
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
            this.miConnectionLines.Text = "Connection &lines";
            this.miConnectionLines.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // miInstructions
            // 
            this.miInstructions.Name = "miInstructions";
            this.miInstructions.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.miInstructions.Size = new System.Drawing.Size(189, 22);
            this.miInstructions.Text = "Ins&tructions";
            this.miInstructions.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // miAnnotations
            // 
            this.miAnnotations.Name = "miAnnotations";
            this.miAnnotations.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.miAnnotations.Size = new System.Drawing.Size(189, 22);
            this.miAnnotations.Text = "&Annotations";
            this.miAnnotations.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // miOwnCloud
            // 
            this.miOwnCloud.Name = "miOwnCloud";
            this.miOwnCloud.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.miOwnCloud.Size = new System.Drawing.Size(189, 22);
            this.miOwnCloud.Text = "O&wn cloud";
            this.miOwnCloud.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // miInnerClouds
            // 
            this.miInnerClouds.Name = "miInnerClouds";
            this.miInnerClouds.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.miInnerClouds.Size = new System.Drawing.Size(189, 22);
            this.miInnerClouds.Text = "Inner clou&ds";
            this.miInnerClouds.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // mnuMode
            // 
            this.mnuMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miMoveSelect,
            this.miDraw});
            this.mnuMode.Name = "mnuMode";
            this.mnuMode.Size = new System.Drawing.Size(45, 20);
            this.mnuMode.Text = "&Mode";
            // 
            // miMoveSelect
            // 
            this.miMoveSelect.Name = "miMoveSelect";
            this.miMoveSelect.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.miMoveSelect.Size = new System.Drawing.Size(185, 22);
            this.miMoveSelect.Text = "&Move or Select";
            this.miMoveSelect.Click += new System.EventHandler(this.switchMode);
            // 
            // miDraw
            // 
            this.miDraw.Name = "miDraw";
            this.miDraw.ShortcutKeys = ((System.Windows.Forms.Keys) ((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.miDraw.Size = new System.Drawing.Size(185, 22);
            this.miDraw.Text = "&Draw";
            this.miDraw.Click += new System.EventHandler(this.switchMode);
            // 
            // miCoordinates
            // 
            this.miCoordinates.Name = "miCoordinates";
            this.miCoordinates.Size = new System.Drawing.Size(189, 22);
            this.miCoordinates.Text = "&Coordinates";
            this.miCoordinates.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // Mainform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 679);
            this.Controls.Add(this.ctImage);
            this.Controls.Add(this.ctMenu);
            this.MainMenuStrip = this.ctMenu;
            this.Name = "Mainform";
            this.Text = "Ziim Helper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formClose);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyUp);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.previewKeyDown);
            this.ctMenu.ResumeLayout(false);
            this.ctMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RT.Util.Controls.DoubleBufferedPanel ctImage;
        private System.Windows.Forms.ToolStripMenuItem miDeleteArrow;
        private System.Windows.Forms.ToolStripMenuItem miRotateClockwise;
        private System.Windows.Forms.ToolStripMenuItem miRotateCounterclockwise;
        private System.Windows.Forms.ToolStripSeparator miSep1;
        private System.Windows.Forms.ToolStripMenuItem miSave;
        private System.Windows.Forms.ToolStripMenuItem miMoveUp;
        private System.Windows.Forms.ToolStripMenuItem miMoveDown;
        private System.Windows.Forms.ToolStripMenuItem miMoveLeft;
        private System.Windows.Forms.ToolStripMenuItem miMoveRight;
        private System.Windows.Forms.ToolStripMenuItem miRevert;
        private System.Windows.Forms.ToolStripMenuItem miMark;
        private System.Windows.Forms.ToolStripMenuItem miCopySource;
        private System.Windows.Forms.ToolStripMenuItem miCopyImage;
        private System.Windows.Forms.MenuStrip ctMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuArrow;
        private System.Windows.Forms.ToolStripMenuItem mnuProgram;
        private System.Windows.Forms.ToolStripSeparator miSep2;
        private System.Windows.Forms.ToolStripMenuItem miAnnotate;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem miGrid;
        private System.Windows.Forms.ToolStripMenuItem miConnectionLines;
        private System.Windows.Forms.ToolStripMenuItem miInstructions;
        private System.Windows.Forms.ToolStripMenuItem miAnnotations;
        private System.Windows.Forms.ToolStripMenuItem miCopyImageByWidth;
        private System.Windows.Forms.ToolStripMenuItem miCopyImageByHeight;
        private System.Windows.Forms.ToolStripMenuItem miCopyImageByFont;
        private System.Windows.Forms.ToolStripMenuItem mnuMode;
        private System.Windows.Forms.ToolStripMenuItem miMoveSelect;
        private System.Windows.Forms.ToolStripMenuItem miDraw;
        private System.Windows.Forms.ToolStripMenuItem miNew;
        private System.Windows.Forms.ToolStripMenuItem miOpen;
        private System.Windows.Forms.ToolStripMenuItem miSaveAs;
        private System.Windows.Forms.ToolStripSeparator miSep3;
        private System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.ToolStripSeparator miSep4;
        private System.Windows.Forms.ToolStripMenuItem miImport;
        private System.Windows.Forms.ToolStripMenuItem miInnerClouds;
        private System.Windows.Forms.ToolStripMenuItem miToggleInput;
        private System.Windows.Forms.ToolStripMenuItem miOwnCloud;
        private System.Windows.Forms.ToolStripMenuItem mnuCloud;
        private System.Windows.Forms.ToolStripMenuItem miColor;
        private System.Windows.Forms.ToolStripMenuItem miCoordinates;
    }
}

