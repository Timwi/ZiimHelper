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
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miNew = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep4 = new System.Windows.Forms.ToolStripSeparator();
            this.miImport = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep3 = new System.Windows.Forms.ToolStripSeparator();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.miCut = new System.Windows.Forms.ToolStripMenuItem();
            this.miCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.miPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep6 = new System.Windows.Forms.ToolStripSeparator();
            this.miSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.miArrowsOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.miToggleInput = new System.Windows.Forms.ToolStripMenuItem();
            this.miAnnotate = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep5 = new System.Windows.Forms.ToolStripSeparator();
            this.miCloudsOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.miColor = new System.Windows.Forms.ToolStripMenuItem();
            this.miSetLabel = new System.Windows.Forms.ToolStripMenuItem();
            this.miBackToOuter = new System.Windows.Forms.ToolStripMenuItem();
            this.miEditMnemonics = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.miGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.miConnectionLines = new System.Windows.Forms.ToolStripMenuItem();
            this.miInstructions = new System.Windows.Forms.ToolStripMenuItem();
            this.miAnnotations = new System.Windows.Forms.ToolStripMenuItem();
            this.miOwnCloud = new System.Windows.Forms.ToolStripMenuItem();
            this.miInnerClouds = new System.Windows.Forms.ToolStripMenuItem();
            this.miCoordinates = new System.Windows.Forms.ToolStripMenuItem();
            this.miSep7 = new System.Windows.Forms.ToolStripSeparator();
            this.miResetZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.miZoomIn = new System.Windows.Forms.ToolStripMenuItem();
            this.miZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMode = new System.Windows.Forms.ToolStripMenuItem();
            this.miMoveSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.miDraw = new System.Windows.Forms.ToolStripMenuItem();
            this.miSetLabelPosition = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUnusedCtrlShortcuts = new System.Windows.Forms.ToolStripMenuItem();
            this.ctMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // miDeleteArrow
            // 
            this.miDeleteArrow.Name = "miDeleteArrow";
            this.miDeleteArrow.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.miDeleteArrow.Size = new System.Drawing.Size(258, 22);
            this.miDeleteArrow.Text = "&Delete";
            this.miDeleteArrow.Click += new System.EventHandler(this.deleteItem);
            // 
            // miMoveUp
            // 
            this.miMoveUp.Name = "miMoveUp";
            this.miMoveUp.ShortcutKeyDisplayString = "Up";
            this.miMoveUp.Size = new System.Drawing.Size(258, 22);
            this.miMoveUp.Text = "&Move up";
            this.miMoveUp.Click += new System.EventHandler(this.move);
            // 
            // miMoveDown
            // 
            this.miMoveDown.Name = "miMoveDown";
            this.miMoveDown.ShortcutKeyDisplayString = "Down";
            this.miMoveDown.Size = new System.Drawing.Size(258, 22);
            this.miMoveDown.Text = "Mo&ve down";
            this.miMoveDown.Click += new System.EventHandler(this.move);
            // 
            // miMoveLeft
            // 
            this.miMoveLeft.Name = "miMoveLeft";
            this.miMoveLeft.ShortcutKeyDisplayString = "Left";
            this.miMoveLeft.Size = new System.Drawing.Size(258, 22);
            this.miMoveLeft.Text = "Move le&ft";
            this.miMoveLeft.Click += new System.EventHandler(this.move);
            // 
            // miMoveRight
            // 
            this.miMoveRight.Name = "miMoveRight";
            this.miMoveRight.ShortcutKeyDisplayString = "Right";
            this.miMoveRight.Size = new System.Drawing.Size(258, 22);
            this.miMoveRight.Text = "Move rig&ht";
            this.miMoveRight.Click += new System.EventHandler(this.move);
            // 
            // miRotateClockwise
            // 
            this.miRotateClockwise.Name = "miRotateClockwise";
            this.miRotateClockwise.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Right)));
            this.miRotateClockwise.Size = new System.Drawing.Size(258, 22);
            this.miRotateClockwise.Text = "&Rotate clockwise";
            this.miRotateClockwise.Click += new System.EventHandler(this.rotate);
            // 
            // miRotateCounterclockwise
            // 
            this.miRotateCounterclockwise.Name = "miRotateCounterclockwise";
            this.miRotateCounterclockwise.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Left)));
            this.miRotateCounterclockwise.Size = new System.Drawing.Size(258, 22);
            this.miRotateCounterclockwise.Text = "Rotate counter-clock&wise";
            this.miRotateCounterclockwise.Click += new System.EventHandler(this.rotate);
            // 
            // miMark
            // 
            this.miMark.Name = "miMark";
            this.miMark.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.miMark.Size = new System.Drawing.Size(258, 22);
            this.miMark.Text = "Toggle mar&k";
            this.miMark.Click += new System.EventHandler(this.toggleMark);
            // 
            // miSep1
            // 
            this.miSep1.Name = "miSep1";
            this.miSep1.Size = new System.Drawing.Size(325, 6);
            // 
            // miCopySource
            // 
            this.miCopySource.Name = "miCopySource";
            this.miCopySource.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.miCopySource.Size = new System.Drawing.Size(328, 22);
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
            this.miCopyImage.Size = new System.Drawing.Size(328, 22);
            this.miCopyImage.Text = "&Copy program to clipboard as image";
            // 
            // miCopyImageByFont
            // 
            this.miCopyImageByFont.Name = "miCopyImageByFont";
            this.miCopyImageByFont.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.miCopyImageByFont.Size = new System.Drawing.Size(260, 22);
            this.miCopyImageByFont.Text = "... using fixed &font size";
            this.miCopyImageByFont.Click += new System.EventHandler(this.copyImage);
            // 
            // miCopyImageByWidth
            // 
            this.miCopyImageByWidth.Name = "miCopyImageByWidth";
            this.miCopyImageByWidth.Size = new System.Drawing.Size(260, 22);
            this.miCopyImageByWidth.Text = "... using fixed bitmap &width";
            this.miCopyImageByWidth.Click += new System.EventHandler(this.copyImage);
            // 
            // miCopyImageByHeight
            // 
            this.miCopyImageByHeight.Name = "miCopyImageByHeight";
            this.miCopyImageByHeight.Size = new System.Drawing.Size(260, 22);
            this.miCopyImageByHeight.Text = "... using fixed bitmap &height";
            this.miCopyImageByHeight.Click += new System.EventHandler(this.copyImage);
            // 
            // miSave
            // 
            this.miSave.Name = "miSave";
            this.miSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.miSave.Size = new System.Drawing.Size(328, 22);
            this.miSave.Text = "&Save";
            this.miSave.Click += new System.EventHandler(this.save);
            // 
            // miRevert
            // 
            this.miRevert.Name = "miRevert";
            this.miRevert.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.miRevert.Size = new System.Drawing.Size(328, 22);
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
            this.ctImage.DoubleClick += new System.EventHandler(this.editInnerCloud);
            this.ctImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseDown);
            this.ctImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove);
            this.ctImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mouseUp);
            // 
            // ctMenu
            // 
            this.ctMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuView,
            this.mnuMode,
            this.mnuUnusedCtrlShortcuts});
            this.ctMenu.Location = new System.Drawing.Point(0, 0);
            this.ctMenu.Name = "ctMenu";
            this.ctMenu.Size = new System.Drawing.Size(716, 24);
            this.ctMenu.TabIndex = 1;
            this.ctMenu.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(37, 20);
            this.mnuFile.Text = "&File";
            // 
            // miNew
            // 
            this.miNew.Name = "miNew";
            this.miNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.miNew.Size = new System.Drawing.Size(328, 22);
            this.miNew.Text = "&New";
            this.miNew.Click += new System.EventHandler(this.fileNew);
            // 
            // miOpen
            // 
            this.miOpen.Name = "miOpen";
            this.miOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.miOpen.Size = new System.Drawing.Size(328, 22);
            this.miOpen.Text = "&Open...";
            this.miOpen.Click += new System.EventHandler(this.open);
            // 
            // miSaveAs
            // 
            this.miSaveAs.Name = "miSaveAs";
            this.miSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.miSaveAs.Size = new System.Drawing.Size(328, 22);
            this.miSaveAs.Text = "Save &As...";
            this.miSaveAs.Click += new System.EventHandler(this.saveAs);
            // 
            // miSep4
            // 
            this.miSep4.Name = "miSep4";
            this.miSep4.Size = new System.Drawing.Size(325, 6);
            // 
            // miImport
            // 
            this.miImport.Name = "miImport";
            this.miImport.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.miImport.Size = new System.Drawing.Size(328, 22);
            this.miImport.Text = "&Import...";
            this.miImport.Click += new System.EventHandler(this.import);
            // 
            // miSep3
            // 
            this.miSep3.Name = "miSep3";
            this.miSep3.Size = new System.Drawing.Size(325, 6);
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(328, 22);
            this.miExit.Text = "E&xit";
            this.miExit.Click += new System.EventHandler(this.exit);
            // 
            // mnuEdit
            // 
            this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCut,
            this.miCopy,
            this.miPaste,
            this.miDeleteArrow,
            this.miSep6,
            this.miMoveUp,
            this.miMoveDown,
            this.miMoveLeft,
            this.miMoveRight,
            this.miRotateClockwise,
            this.miRotateCounterclockwise,
            this.miSep2,
            this.miArrowsOnly,
            this.miToggleInput,
            this.miMark,
            this.miAnnotate,
            this.miSep5,
            this.miCloudsOnly,
            this.miColor,
            this.miSetLabel,
            this.miBackToOuter,
            this.miEditMnemonics});
            this.mnuEdit.Name = "mnuEdit";
            this.mnuEdit.Size = new System.Drawing.Size(39, 20);
            this.mnuEdit.Text = "&Edit";
            // 
            // miCut
            // 
            this.miCut.Name = "miCut";
            this.miCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.miCut.Size = new System.Drawing.Size(258, 22);
            this.miCut.Text = "C&ut";
            this.miCut.Click += new System.EventHandler(this.cutOrCopy);
            // 
            // miCopy
            // 
            this.miCopy.Name = "miCopy";
            this.miCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.miCopy.Size = new System.Drawing.Size(258, 22);
            this.miCopy.Text = "&Copy";
            this.miCopy.Click += new System.EventHandler(this.cutOrCopy);
            // 
            // miPaste
            // 
            this.miPaste.Name = "miPaste";
            this.miPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.miPaste.Size = new System.Drawing.Size(258, 22);
            this.miPaste.Text = "&Paste";
            this.miPaste.Click += new System.EventHandler(this.paste);
            // 
            // miSep6
            // 
            this.miSep6.Name = "miSep6";
            this.miSep6.Size = new System.Drawing.Size(255, 6);
            // 
            // miSep2
            // 
            this.miSep2.Name = "miSep2";
            this.miSep2.Size = new System.Drawing.Size(255, 6);
            // 
            // miArrowsOnly
            // 
            this.miArrowsOnly.Enabled = false;
            this.miArrowsOnly.Name = "miArrowsOnly";
            this.miArrowsOnly.Size = new System.Drawing.Size(258, 22);
            this.miArrowsOnly.Text = "Arrows only";
            // 
            // miToggleInput
            // 
            this.miToggleInput.Name = "miToggleInput";
            this.miToggleInput.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.miToggleInput.Size = new System.Drawing.Size(258, 22);
            this.miToggleInput.Text = "Toggle &terminal";
            this.miToggleInput.Click += new System.EventHandler(this.toggleTerminal);
            // 
            // miAnnotate
            // 
            this.miAnnotate.Name = "miAnnotate";
            this.miAnnotate.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Return)));
            this.miAnnotate.Size = new System.Drawing.Size(258, 22);
            this.miAnnotate.Text = "&Annotate...";
            this.miAnnotate.Click += new System.EventHandler(this.annotate);
            // 
            // miSep5
            // 
            this.miSep5.Name = "miSep5";
            this.miSep5.Size = new System.Drawing.Size(255, 6);
            // 
            // miCloudsOnly
            // 
            this.miCloudsOnly.Enabled = false;
            this.miCloudsOnly.Name = "miCloudsOnly";
            this.miCloudsOnly.Size = new System.Drawing.Size(258, 22);
            this.miCloudsOnly.Text = "Current cloud only";
            // 
            // miColor
            // 
            this.miColor.Name = "miColor";
            this.miColor.Size = new System.Drawing.Size(258, 22);
            this.miColor.Text = "C&olor...";
            this.miColor.Click += new System.EventHandler(this.cloudColor);
            // 
            // miSetLabel
            // 
            this.miSetLabel.Name = "miSetLabel";
            this.miSetLabel.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.miSetLabel.Size = new System.Drawing.Size(258, 22);
            this.miSetLabel.Text = "Set &label...";
            this.miSetLabel.Click += new System.EventHandler(this.setLabel);
            // 
            // miBackToOuter
            // 
            this.miBackToOuter.Name = "miBackToOuter";
            this.miBackToOuter.ShortcutKeyDisplayString = "Backspace";
            this.miBackToOuter.Size = new System.Drawing.Size(258, 22);
            this.miBackToOuter.Text = "&Back to outer cloud";
            this.miBackToOuter.Click += new System.EventHandler(this.backToOuterCloud);
            // 
            // miEditMnemonics
            // 
            this.miEditMnemonics.Name = "miEditMnemonics";
            this.miEditMnemonics.Size = new System.Drawing.Size(258, 22);
            this.miEditMnemonics.Text = "egijnqsxyz";
            this.miEditMnemonics.Visible = false;
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
            this.miCoordinates,
            this.miSep7,
            this.miResetZoom,
            this.miZoomIn,
            this.miZoomOut});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(44, 20);
            this.mnuView.Text = "&View";
            // 
            // miGrid
            // 
            this.miGrid.Name = "miGrid";
            this.miGrid.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.miGrid.Size = new System.Drawing.Size(201, 22);
            this.miGrid.Text = "&Grid";
            this.miGrid.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // miConnectionLines
            // 
            this.miConnectionLines.Name = "miConnectionLines";
            this.miConnectionLines.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.J)));
            this.miConnectionLines.Size = new System.Drawing.Size(201, 22);
            this.miConnectionLines.Text = "Connection &lines";
            this.miConnectionLines.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // miInstructions
            // 
            this.miInstructions.Name = "miInstructions";
            this.miInstructions.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.miInstructions.Size = new System.Drawing.Size(201, 22);
            this.miInstructions.Text = "Instr&uctions";
            this.miInstructions.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // miAnnotations
            // 
            this.miAnnotations.Name = "miAnnotations";
            this.miAnnotations.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.miAnnotations.Size = new System.Drawing.Size(201, 22);
            this.miAnnotations.Text = "&Annotations";
            this.miAnnotations.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // miOwnCloud
            // 
            this.miOwnCloud.Name = "miOwnCloud";
            this.miOwnCloud.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.miOwnCloud.Size = new System.Drawing.Size(201, 22);
            this.miOwnCloud.Text = "O&wn cloud";
            this.miOwnCloud.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // miInnerClouds
            // 
            this.miInnerClouds.Name = "miInnerClouds";
            this.miInnerClouds.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.miInnerClouds.Size = new System.Drawing.Size(201, 22);
            this.miInnerClouds.Text = "Inner clou&ds";
            this.miInnerClouds.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // miCoordinates
            // 
            this.miCoordinates.Name = "miCoordinates";
            this.miCoordinates.Size = new System.Drawing.Size(201, 22);
            this.miCoordinates.Text = "&Coordinates";
            this.miCoordinates.Click += new System.EventHandler(this.toggleViewOption);
            // 
            // miSep7
            // 
            this.miSep7.Name = "miSep7";
            this.miSep7.Size = new System.Drawing.Size(198, 6);
            // 
            // miResetZoom
            // 
            this.miResetZoom.Name = "miResetZoom";
            this.miResetZoom.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D0)));
            this.miResetZoom.Size = new System.Drawing.Size(201, 22);
            this.miResetZoom.Text = "&Reset Zoom";
            this.miResetZoom.Click += new System.EventHandler(this.resetZoom);
            // 
            // miZoomIn
            // 
            this.miZoomIn.Name = "miZoomIn";
            this.miZoomIn.ShortcutKeyDisplayString = "Ctrl+=";
            this.miZoomIn.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemplus)));
            this.miZoomIn.Size = new System.Drawing.Size(201, 22);
            this.miZoomIn.Text = "&Zoom in";
            this.miZoomIn.Click += new System.EventHandler(this.zoomIn);
            // 
            // miZoomOut
            // 
            this.miZoomOut.Name = "miZoomOut";
            this.miZoomOut.ShortcutKeyDisplayString = "Ctrl+-";
            this.miZoomOut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.OemMinus)));
            this.miZoomOut.Size = new System.Drawing.Size(201, 22);
            this.miZoomOut.Text = "Zoom &out";
            this.miZoomOut.Click += new System.EventHandler(this.zoomOut);
            // 
            // mnuMode
            // 
            this.mnuMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miMoveSelect,
            this.miDraw,
            this.miSetLabelPosition});
            this.mnuMode.Name = "mnuMode";
            this.mnuMode.Size = new System.Drawing.Size(50, 20);
            this.mnuMode.Text = "&Mode";
            // 
            // miMoveSelect
            // 
            this.miMoveSelect.Name = "miMoveSelect";
            this.miMoveSelect.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.miMoveSelect.Size = new System.Drawing.Size(236, 22);
            this.miMoveSelect.Text = "&Move or Select";
            this.miMoveSelect.Click += new System.EventHandler(this.switchMode);
            // 
            // miDraw
            // 
            this.miDraw.Name = "miDraw";
            this.miDraw.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.miDraw.Size = new System.Drawing.Size(236, 22);
            this.miDraw.Text = "&Draw";
            this.miDraw.Click += new System.EventHandler(this.switchMode);
            // 
            // miSetLabelPosition
            // 
            this.miSetLabelPosition.Name = "miSetLabelPosition";
            this.miSetLabelPosition.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.L)));
            this.miSetLabelPosition.Size = new System.Drawing.Size(236, 22);
            this.miSetLabelPosition.Text = "Set &label position";
            this.miSetLabelPosition.Click += new System.EventHandler(this.switchMode);
            // 
            // mnuUnusedCtrlShortcuts
            // 
            this.mnuUnusedCtrlShortcuts.Name = "mnuUnusedCtrlShortcuts";
            this.mnuUnusedCtrlShortcuts.Size = new System.Drawing.Size(193, 20);
            this.mnuUnusedCtrlShortcuts.Text = "Unused Ctrl Shortcuts: befhpqryz";
            this.mnuUnusedCtrlShortcuts.Visible = false;
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
        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
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
        private System.Windows.Forms.ToolStripMenuItem miCoordinates;
        private System.Windows.Forms.ToolStripMenuItem mnuUnusedCtrlShortcuts;
        private System.Windows.Forms.ToolStripMenuItem miSetLabelPosition;
        private System.Windows.Forms.ToolStripMenuItem miArrowsOnly;
        private System.Windows.Forms.ToolStripSeparator miSep5;
        private System.Windows.Forms.ToolStripMenuItem miCloudsOnly;
        private System.Windows.Forms.ToolStripMenuItem miColor;
        private System.Windows.Forms.ToolStripMenuItem miSetLabel;
        private System.Windows.Forms.ToolStripMenuItem miBackToOuter;
        private System.Windows.Forms.ToolStripMenuItem miCut;
        private System.Windows.Forms.ToolStripMenuItem miCopy;
        private System.Windows.Forms.ToolStripMenuItem miPaste;
        private System.Windows.Forms.ToolStripSeparator miSep6;
        private System.Windows.Forms.ToolStripMenuItem miEditMnemonics;
        private System.Windows.Forms.ToolStripSeparator miSep7;
        private System.Windows.Forms.ToolStripMenuItem miResetZoom;
        private System.Windows.Forms.ToolStripMenuItem miZoomIn;
        private System.Windows.Forms.ToolStripMenuItem miZoomOut;
    }
}

