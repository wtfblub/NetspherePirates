namespace NetsphereExplorer.Views
{
    partial class MainView
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
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.miOpen = new System.Windows.Forms.MenuItem();
            this.tvFolders = new BlubLib.GUI.Controls.Extended.TreeViewEx();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.lvExplorer = new BlubLib.GUI.Controls.Extended.ListViewEx();
            this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.vistaMenu = new BlubLib.GUI.Components.VistaMenu(this.components);
            this.miRemoveFolder = new System.Windows.Forms.MenuItem();
            this.miAddToFolder = new System.Windows.Forms.MenuItem();
            this.miExtractFolder = new System.Windows.Forms.MenuItem();
            this.miExtractItems = new System.Windows.Forms.MenuItem();
            this.miRemoveItems = new System.Windows.Forms.MenuItem();
            this.miAddFiles = new System.Windows.Forms.MenuItem();
            this.cmFolders = new System.Windows.Forms.ContextMenu();
            this.cmExplorerSelection = new System.Windows.Forms.ContextMenu();
            this.cmExplorer = new System.Windows.Forms.ContextMenu();
            this.overlay = new NetsphereExplorer.Controls.Overlay();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miOpen});
            this.menuItem1.Text = "File";
            // 
            // miOpen
            // 
            this.vistaMenu.SetImage(this.miOpen, global::NetsphereExplorer.Properties.Resources.folder_open);
            this.miOpen.Index = 0;
            this.miOpen.Text = "Open...";
            // 
            // tvFolders
            // 
            this.tvFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFolders.FullRowSelect = true;
            this.tvFolders.HideSelection = false;
            this.tvFolders.ImageIndex = 0;
            this.tvFolders.ImageList = this.imageList;
            this.tvFolders.Location = new System.Drawing.Point(0, 0);
            this.tvFolders.Name = "tvFolders";
            this.tvFolders.SelectedImageIndex = 0;
            this.tvFolders.ShowLines = false;
            this.tvFolders.Size = new System.Drawing.Size(175, 357);
            this.tvFolders.TabIndex = 0;
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lvExplorer
            // 
            this.lvExplorer.AllowDrop = true;
            this.lvExplorer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chSize});
            this.lvExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvExplorer.FullRowSelect = true;
            this.lvExplorer.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvExplorer.HideSelection = false;
            this.lvExplorer.LargeImageList = this.imageList;
            this.lvExplorer.Location = new System.Drawing.Point(0, 0);
            this.lvExplorer.Name = "lvExplorer";
            this.lvExplorer.Size = new System.Drawing.Size(416, 357);
            this.lvExplorer.SmallImageList = this.imageList;
            this.lvExplorer.TabIndex = 1;
            this.lvExplorer.UseCompatibleStateImageBehavior = false;
            this.lvExplorer.View = System.Windows.Forms.View.Details;
            // 
            // chName
            // 
            this.chName.Text = "Name";
            this.chName.Width = 298;
            // 
            // chSize
            // 
            this.chSize.Text = "Size";
            this.chSize.Width = 91;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvFolders);
            this.splitContainer1.Panel1MinSize = 170;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvExplorer);
            this.splitContainer1.Size = new System.Drawing.Size(593, 357);
            this.splitContainer1.SplitterDistance = 175;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 0;
            // 
            // vistaMenu
            // 
            this.vistaMenu.ContainerControl = this;
            // 
            // miRemoveFolder
            // 
            this.vistaMenu.SetImage(this.miRemoveFolder, global::NetsphereExplorer.Properties.Resources.bin);
            this.miRemoveFolder.Index = 2;
            this.miRemoveFolder.Text = "Delete";
            // 
            // miAddToFolder
            // 
            this.vistaMenu.SetImage(this.miAddToFolder, global::NetsphereExplorer.Properties.Resources.document__plus);
            this.miAddToFolder.Index = 1;
            this.miAddToFolder.Text = "Add files...";
            // 
            // miExtractFolder
            // 
            this.vistaMenu.SetImage(this.miExtractFolder, global::NetsphereExplorer.Properties.Resources.folders);
            this.miExtractFolder.Index = 0;
            this.miExtractFolder.Text = "Extract to...";
            // 
            // miExtractItems
            // 
            this.vistaMenu.SetImage(this.miExtractItems, global::NetsphereExplorer.Properties.Resources.folders);
            this.miExtractItems.Index = 0;
            this.miExtractItems.Text = "Extract to...";
            // 
            // miRemoveItems
            // 
            this.vistaMenu.SetImage(this.miRemoveItems, global::NetsphereExplorer.Properties.Resources.bin);
            this.miRemoveItems.Index = 1;
            this.miRemoveItems.Text = "Delete";
            // 
            // miAddFiles
            // 
            this.vistaMenu.SetImage(this.miAddFiles, global::NetsphereExplorer.Properties.Resources.document__plus);
            this.miAddFiles.Index = 0;
            this.miAddFiles.Text = "Add files...";
            // 
            // cmFolders
            // 
            this.cmFolders.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miExtractFolder,
            this.miAddToFolder,
            this.miRemoveFolder});
            // 
            // cmExplorerSelection
            // 
            this.cmExplorerSelection.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miExtractItems,
            this.miRemoveItems});
            // 
            // cmExplorer
            // 
            this.cmExplorer.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miAddFiles});
            // 
            // overlay
            // 
            this.overlay.Owner = this;
            this.overlay.View = null;
            // 
            // MainView
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(593, 357);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Menu = this.mainMenu;
            this.MinimumSize = new System.Drawing.Size(590, 355);
            this.Name = "MainView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Netsphere Explorer";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.vistaMenu)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem miOpen;
        private Controls.Overlay overlay;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private BlubLib.GUI.Controls.Extended.TreeViewEx tvFolders;
        private BlubLib.GUI.Controls.Extended.ListViewEx lvExplorer;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chSize;
        private BlubLib.GUI.Components.VistaMenu vistaMenu;
        private System.Windows.Forms.ContextMenu cmFolders;
        private System.Windows.Forms.MenuItem miExtractFolder;
        private System.Windows.Forms.MenuItem miAddToFolder;
        private System.Windows.Forms.MenuItem miRemoveFolder;
        private System.Windows.Forms.ContextMenu cmExplorerSelection;
        private System.Windows.Forms.MenuItem miExtractItems;
        private System.Windows.Forms.MenuItem miRemoveItems;
        private System.Windows.Forms.ContextMenu cmExplorer;
        private System.Windows.Forms.MenuItem miAddFiles;
    }
}

