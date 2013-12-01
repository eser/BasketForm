using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BasketForm.Abstraction;

namespace BasketForm
{
    public partial class frmMain : Form
    {
        public List<Folder> Folders { get; set; }
        private string[] contextMenuFiles;
        private string contextTargetDirectory;

        public frmMain()
        {
            this.InitializeComponent();

            this.Folders = new List<Folder>();

            this.Folders.Add(new Folder() { Title = "Music", PhysicalPath = @"Q:\Music", TileSize = 1, DisplaySubfolderTree = true });
            this.Folders.Add(new Folder() { Title = "Webroot", PhysicalPath = @"Q:\webroot", TileSize = 1, DisplaySubfolderTree = false });

            this.FoldersCreate();

            Rectangle x = Screen.GetWorkingArea(this);
            this.SetDesktopLocation(x.Right - this.Width - 5, x.Top + 5);
        }

        public void FoldersClear() {
            foreach (Folder folder in this.Folders) {
                if (folder.FormButton != null) {
                    this.Controls.Remove(folder.FormButton);
                }
            }
        }

        public void FoldersCreate()
        {
            int createdCount = 0;

            foreach (Folder folder in this.Folders) {
                folder.FormButton = new Button() {
                    AllowDrop = true,
                    Text = folder.Title,
                    Tag = folder,
                    Size = new Size(130, 118),
                    Location = new Point(12 + (12 + 130) * createdCount++, 28)
                };

                folder.FormButton.DragEnter += this.button_DragEnter;
                folder.FormButton.DragDrop += this.button_DragDrop;
                folder.FormButton.Click += this.button_Click;

                this.Controls.Add(folder.FormButton);
            }
        }

        public void FoldersReset()
        {
            this.FoldersClear();
            this.FoldersCreate();
        }

        public Folder GetFolderFromControl(object sender)
        {
            Control control = sender as Control;
            if (control == null)
            {
                return null;
            }

            return control.Tag as Folder;
        }

        private void button_Click(object sender, EventArgs e)
        {
            Folder folder = this.GetFolderFromControl(sender);
            if (folder == null)
            {
                return;
            }

            Process.Start(folder.PhysicalPath);
        }

        private void button_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void button_DragDrop(object sender, DragEventArgs e)
        {
            Folder folder = this.GetFolderFromControl(sender);
            if (folder == null)
            {
                return;
            }

            this.contextMenuFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            this.contextTargetDirectory = folder.PhysicalPath;

            if (folder.DisplaySubfolderTree)
            {
                contextMenuStrip1.Items.Clear();

                foreach (string directory in Directory.EnumerateDirectories(folder.PhysicalPath, "*.*", SearchOption.TopDirectoryOnly))
                {
                    ToolStripMenuItem item = new ToolStripMenuItem()
                    {
                        Text = Path.GetFileName(directory),
                        Tag = directory
                    };

                    item.Click += this.item_Click;
                    contextMenuStrip1.Items.Add(item);
                }

                Control senderControl = sender as Control;
                contextMenuStrip1.Show(senderControl, 0, senderControl.Height);

                return;
            }

            this.MoveFiles(this.contextMenuFiles, this.contextTargetDirectory);
        }

        private void item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null)
            {
                return;
            }

            this.MoveFiles(this.contextMenuFiles, (string)item.Tag);
        }

        private void MoveFiles(string[] files, string targetPath)
        {
            foreach (string file in files)
            {
            retryPoint:
                try
                {
                    File.Move(file, Path.Combine(targetPath, Path.GetFileName(file)));
                }
                catch (Exception ex)
                {
                    DialogResult response = MessageBox.Show(ex.GetType().Name + ": " + ex.Message, this.Text, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);

                    if (response == DialogResult.Abort)
                    {
                        return;
                    }
                    else if (response == DialogResult.Retry)
                    {
                        goto retryPoint;
                    }
                    // else is ignore, so ignore it.
                }
            }
        }
    }
}
