namespace ReqDBBrowser
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.tbQuickSearch = new System.Windows.Forms.ToolStripTextBox();
            this.btQuickSearch = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabelTraceRoot = new System.Windows.Forms.ToolStripLabel();
            this.imageListReqTree = new System.Windows.Forms.ImageList(this.components);
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.tabDetails = new System.Windows.Forms.TabControl();
            this.tabPageTable = new System.Windows.Forms.TabPage();
            this.tabPageGraph = new System.Windows.Forms.TabPage();
            this.webBrowserReqTraceGraph = new ReqDBBrowser.ZoomWebBrowser();
            this.tabPageTree = new System.Windows.Forms.TabPage();
            this.tabPageSearchResults = new System.Windows.Forms.TabPage();
            this.tabPageQuickSearchResults = new System.Windows.Forms.TabPage();
            this.menuStripMain.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.tabDetails.SuspendLayout();
            this.tabPageGraph.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(889, 24);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStripMain";
            // 
            // projectToolStripMenuItem
            // 
            this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            this.projectToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.projectToolStripMenuItem.Text = "Project";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(100, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripMain
            // 
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonRefresh,
            this.tbQuickSearch,
            this.btQuickSearch,
            this.toolStripSeparator2,
            this.toolStripLabel,
            this.toolStripLabelTraceRoot});
            this.toolStripMain.Location = new System.Drawing.Point(0, 24);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Size = new System.Drawing.Size(889, 25);
            this.toolStripMain.TabIndex = 1;
            this.toolStripMain.Text = "toolStripMain";
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRefresh.Image = global::ReqDBBrowser.Properties.Resources.refresh_sm1;
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRefresh.Text = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.ToolTipText = "Refresh";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // tbQuickSearch
            // 
            this.tbQuickSearch.Name = "tbQuickSearch";
            this.tbQuickSearch.Size = new System.Drawing.Size(100, 25);
            this.tbQuickSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbQuickSearch_KeyPress);
            // 
            // btQuickSearch
            // 
            this.btQuickSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btQuickSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btQuickSearch.Name = "btQuickSearch";
            this.btQuickSearch.Size = new System.Drawing.Size(80, 22);
            this.btQuickSearch.Text = "Quick Search";
            this.btQuickSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btQuickSearch.ToolTipText = "searches case insensitive through the Requirement Tag and the Requirement Name";
            this.btQuickSearch.Click += new System.EventHandler(this.btQuickSearch_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel
            // 
            this.toolStripLabel.Name = "toolStripLabel";
            this.toolStripLabel.Size = new System.Drawing.Size(0, 22);
            // 
            // toolStripLabelTraceRoot
            // 
            this.toolStripLabelTraceRoot.Name = "toolStripLabelTraceRoot";
            this.toolStripLabelTraceRoot.Size = new System.Drawing.Size(0, 22);
            // 
            // imageListReqTree
            // 
            this.imageListReqTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListReqTree.ImageStream")));
            this.imageListReqTree.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListReqTree.Images.SetKeyName(0, "Project.PNG");
            this.imageListReqTree.Images.SetKeyName(1, "folder_closed.png");
            this.imageListReqTree.Images.SetKeyName(2, "folder_open.png");
            this.imageListReqTree.Images.SetKeyName(3, "Requirement.PNG");
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 49);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.tabDetails);
            this.splitContainerMain.Size = new System.Drawing.Size(889, 429);
            this.splitContainerMain.SplitterDistance = 245;
            this.splitContainerMain.TabIndex = 2;
            // 
            // tabDetails
            // 
            this.tabDetails.Controls.Add(this.tabPageTable);
            this.tabDetails.Controls.Add(this.tabPageGraph);
            this.tabDetails.Controls.Add(this.tabPageTree);
            this.tabDetails.Controls.Add(this.tabPageSearchResults);
            this.tabDetails.Controls.Add(this.tabPageQuickSearchResults);
            this.tabDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabDetails.Location = new System.Drawing.Point(0, 0);
            this.tabDetails.Name = "tabDetails";
            this.tabDetails.SelectedIndex = 0;
            this.tabDetails.Size = new System.Drawing.Size(640, 429);
            this.tabDetails.TabIndex = 0;
            this.tabDetails.SizeChanged += new System.EventHandler(this.tabDetails_SizeChanged);
            // 
            // tabPageTable
            // 
            this.tabPageTable.Location = new System.Drawing.Point(4, 22);
            this.tabPageTable.Name = "tabPageTable";
            this.tabPageTable.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTable.Size = new System.Drawing.Size(632, 403);
            this.tabPageTable.TabIndex = 0;
            this.tabPageTable.Text = "Trace Table";
            this.tabPageTable.UseVisualStyleBackColor = true;
            // 
            // tabPageGraph
            // 
            this.tabPageGraph.Controls.Add(this.webBrowserReqTraceGraph);
            this.tabPageGraph.Location = new System.Drawing.Point(4, 22);
            this.tabPageGraph.Name = "tabPageGraph";
            this.tabPageGraph.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGraph.Size = new System.Drawing.Size(632, 403);
            this.tabPageGraph.TabIndex = 4;
            this.tabPageGraph.Text = "Trace Graph";
            this.tabPageGraph.UseVisualStyleBackColor = true;
            this.tabPageGraph.Leave += new System.EventHandler(this.tabPageGraph_Leave);
            this.tabPageGraph.Enter += new System.EventHandler(this.tabPageGraph_Enter);
            // 
            // webBrowserReqTraceGraph
            // 
            this.webBrowserReqTraceGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserReqTraceGraph.Location = new System.Drawing.Point(3, 3);
            this.webBrowserReqTraceGraph.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserReqTraceGraph.Name = "webBrowserReqTraceGraph";
            this.webBrowserReqTraceGraph.Size = new System.Drawing.Size(626, 397);
            this.webBrowserReqTraceGraph.TabIndex = 0;
            this.webBrowserReqTraceGraph.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(webBrowserReqTraceGraph_Navigating);
            // 
            // tabPageTree
            // 
            this.tabPageTree.AutoScroll = true;
            this.tabPageTree.Location = new System.Drawing.Point(4, 22);
            this.tabPageTree.Name = "tabPageTree";
            this.tabPageTree.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTree.Size = new System.Drawing.Size(632, 403);
            this.tabPageTree.TabIndex = 1;
            this.tabPageTree.Text = "Trace Graph (old)";
            this.tabPageTree.UseVisualStyleBackColor = true;
            this.tabPageTree.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPageTree_Paint);
            this.tabPageTree.Scroll += new System.Windows.Forms.ScrollEventHandler(this.tabPageTree_Scroll);
            this.tabPageTree.ClientSizeChanged += new System.EventHandler(this.tabPageTree_ClientSizeChanged);
            // 
            // tabPageSearchResults
            // 
            this.tabPageSearchResults.Location = new System.Drawing.Point(4, 22);
            this.tabPageSearchResults.Name = "tabPageSearchResults";
            this.tabPageSearchResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSearchResults.Size = new System.Drawing.Size(632, 403);
            this.tabPageSearchResults.TabIndex = 2;
            this.tabPageSearchResults.Text = "Search Results";
            this.tabPageSearchResults.UseVisualStyleBackColor = true;
            // 
            // tabPageQuickSearchResults
            // 
            this.tabPageQuickSearchResults.Location = new System.Drawing.Point(4, 22);
            this.tabPageQuickSearchResults.Name = "tabPageQuickSearchResults";
            this.tabPageQuickSearchResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageQuickSearchResults.Size = new System.Drawing.Size(632, 403);
            this.tabPageQuickSearchResults.TabIndex = 3;
            this.tabPageQuickSearchResults.Text = "Quick Search Results";
            this.tabPageQuickSearchResults.UseVisualStyleBackColor = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 478);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.menuStripMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "FormMain";
            this.Text = "ReqDBBrowser";
            this.Load += new System.EventHandler(this.formMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.tabDetails.ResumeLayout(false);
            this.tabPageGraph.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.TabControl tabDetails;
        private System.Windows.Forms.TabPage tabPageTable;
        private System.Windows.Forms.TabPage tabPageTree;
        private System.Windows.Forms.ImageList imageListReqTree;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel;
        private System.Windows.Forms.ToolStripLabel toolStripLabelTraceRoot;
        private System.Windows.Forms.TabPage tabPageSearchResults;
        private System.Windows.Forms.ToolStripTextBox tbQuickSearch;
        private System.Windows.Forms.ToolStripButton btQuickSearch;
        private System.Windows.Forms.TabPage tabPageQuickSearchResults;
        private System.Windows.Forms.TabPage tabPageGraph;
        private ZoomWebBrowser webBrowserReqTraceGraph;
    }
}