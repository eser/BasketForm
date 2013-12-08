// -----------------------------------------------------------------------
// <copyright file="frmMain.cs" company="-">
// Copyright (c) 2013 larukedi (eser@sent.com). All rights reserved.
// </copyright>
// <author>larukedi (http://github.com/larukedi/)</author>
// -----------------------------------------------------------------------

//// This program is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 3 of the License, or
//// (at your option) any later version.
//// 
//// This program is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//// GNU General Public License for more details.
////
//// You should have received a copy of the GNU General Public License
//// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace BasketForm
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using BasketForm.Abstraction;
    
    public partial class frmMain : Form
    {
        public List<Folder> Folders { get; set; }
        private string[] contextMenuFiles;
        private string contextTargetDirectory;
        private bool allowQuit = false;

        public frmMain()
        {
            this.InitializeComponent();

            this.Folders = new List<Folder>();

            if (ProgramLogic.Instance.Config.Folders != null)
            {
                this.Folders.AddRange(ProgramLogic.Instance.Config.Folders);
            }

            int finalX = this.FoldersCreate();

            Rectangle x = Screen.GetWorkingArea(this);
            this.Width = finalX;
            this.SetDesktopLocation(x.Right - this.Width - 5, x.Top + 5);
        }

        public void FoldersClear() {
            foreach (Folder folder in this.Folders) {
                if (folder.FormButton != null) {
                    this.Controls.Remove(folder.FormButton);
                }
            }
        }

        public int FoldersCreate()
        {
            const int spacing = 12;
            int x = spacing;

            foreach (Folder folder in this.Folders) {
                int width = 90 * folder.TileSize;

                folder.FormButton = new Button() {
                    AllowDrop = true,
                    Text = folder.Title,
                    Tag = folder,
                    Size = new Size(width, 78),
                    Location = new Point(x, 28),
                    Image = global::BasketForm.Properties.Resources.Folder.ToBitmap(),
                    ImageAlign = ContentAlignment.MiddleCenter,
                    TextImageRelation = TextImageRelation.ImageAboveText,
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left // | AnchorStyles.Right
                };

                x += width + spacing;

                folder.FormButton.DragEnter += this.button_DragEnter;
                folder.FormButton.DragDrop += this.button_DragDrop;
                folder.FormButton.Click += this.button_Click;

                this.Controls.Add(folder.FormButton);
            }

            return x + spacing;
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

                if (contextMenuStrip1.Items.Count > 0)
                {
                    Control senderControl = sender as Control;
                    contextMenuStrip1.Show(senderControl, 0, senderControl.Height);

                    return;
                }
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

        private void toolStripQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.allowQuit && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }
    }
}
