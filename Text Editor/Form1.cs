using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

namespace Text_Editor {
    public partial class Form1 : Form {
        private int row { get; set; }
        private int col { get; set; }
        private string filepath { get; set; }
        private string filename { get; set; }
        private string textBuffer { get; set; }
        private string currTimeDate { get; set; }
        private string currState { get; set; }

        public Form1() {
            InitializeComponent();

            // Show the title.
            this.Text = "untitled.txt - Text Editor";
            filename = "untitled.txt";

            // Initial text font and time.
            richTextBox1.Font = new Font("新細明體", 12, FontStyle.Regular);
            currTimeDate = DateTime.Today.ToLongDateString();

            // Initial the row and col numbers.
            row = 0;
            col = 0;

            // Initial timer and progress bar.
            timer_pb.Interval = 500;
            toolStripProgressBar1.Maximum = 100;
            toolStripProgressBar1.Value = 0;
        }

        private void Form1_Load(object sender, EventArgs e) {
            exitToolStripMenuItem_Click(sender, e);
        }

        /* Open file */
        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            // Set the OpenFileDialog.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select file";
            openFileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = false;

            // Open file.
            if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileName.Length > 0) {
                try {
                    // Get the file info.
                    FileInfo info = new FileInfo(openFileDialog.FileName);
                    filepath = info.DirectoryName;
                    filename = info.Name;

                    // Get the permission of the file on localhost.
                    FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.AllAccess, filepath);
                    permission.AllLocalFiles = FileIOPermissionAccess.Read;
                    try {
                        permission.Demand();
                        currState = "load";
                        timer_pb.Enabled = true;
                    }
                    catch (SecurityException ex) {
                        MessageBox.Show(ex.Message, "Text Editor - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Show the file path on the window title.
                    this.Text = filepath + "\\" + filename + " - Text Editor";
                }
                catch (IOException ex) {
                    MessageBox.Show("Error: Could not read the file." + ex, "Text Editor - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /* Save file */
        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveFileDialog savaFileDialog = new SaveFileDialog();
            savaFileDialog.Title = "Save";
            savaFileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            savaFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            savaFileDialog.FilterIndex = 1;
            savaFileDialog.RestoreDirectory = true;
            savaFileDialog.OverwritePrompt = true;

            if (filename == "untitled.txt") {   // New file saving.
                if (savaFileDialog.ShowDialog() == DialogResult.OK && savaFileDialog.FileName.Length > 0) {
                    // Get the file info.
                    FileInfo info = new FileInfo(savaFileDialog.FileName);
                    filepath = info.DirectoryName;
                    filename = info.Name;

                    // Get the permission of the file on localhost.
                    FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.AllAccess, filepath);
                    permission.AllLocalFiles = FileIOPermissionAccess.Read;
                    try {
                        permission.Demand();
                        currState = "save";
                        timer_pb.Enabled = true;
                    }
                    catch (SecurityException ex) {
                        MessageBox.Show(ex.Message, "Text Editor - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else {  // File existed saving.
                // Get the permission of the file on localhost.
                FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.AllAccess, filepath);
                permission.AllLocalFiles = FileIOPermissionAccess.Read;
                try {   // Saving.
                    permission.Demand();
                    currState = "save";
                    timer_pb.Enabled = true;
                }
                catch (SecurityException ex) {
                    MessageBox.Show(ex.Message, "Text Editor - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Show the saved file path on the window title.
            this.Text = filepath + "\\" + filename + " - Text Editor";
        }

        /* Save as file */
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveFileDialog savaFileDialog = new SaveFileDialog();
            savaFileDialog.Title = "Save as";
            savaFileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            savaFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            savaFileDialog.FilterIndex = 2;
            savaFileDialog.RestoreDirectory = true;
            savaFileDialog.OverwritePrompt = true;

            // Save as file.
            if (savaFileDialog.ShowDialog() == DialogResult.OK && savaFileDialog.FileName.Length > 0) {
                // Get the file info.
                FileInfo info = new FileInfo(savaFileDialog.FileName);
                filepath = info.DirectoryName;
                filename = info.Name;

                // Get the permission of the file on localhost.
                FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.AllAccess, filepath);
                permission.AllLocalFiles = FileIOPermissionAccess.Read;
                try {
                    permission.Demand();
                    currState = "save";
                    timer_pb.Enabled = true;
                }
                catch (SecurityException ex) {
                    MessageBox.Show(ex.Message, "Text Editor - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Show the saved file path on the window title.
            this.Text = filepath + "\\" + filename + " - Text Editor";
        }

        /* Create new file */
        private void newToolStripMenuItem_Click(object sender, EventArgs e) {
            if (this.Text[this.Text.Length - 1] == '*') {
                DialogResult isSave = MessageBox.Show("Sure to exit without saving?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                if (isSave == DialogResult.Yes) {
                    this.Close();
                }
                else if (isSave == DialogResult.No) {
                    saveToolStripMenuItem_Click(sender, e);
                }
            }

            // Clear the current richTextBox.
            richTextBox1.Text = "";

            // Show the title.
            this.Text = "untitled.txt - Text Editor";
            filename = "untitled.txt";
        }

        /* Exit */
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            if (this.Text[this.Text.Length - 1] == '*') {
                DialogResult isSave = MessageBox.Show("Sure to exit without saving?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (isSave == DialogResult.Yes) {
                    this.Close();
                }
                else if (isSave == DialogResult.No) {
                    saveToolStripMenuItem_Click(sender, e);
                }
            }
        }

        // Changed the color of the selected text.
        private void colorToolStripMenuItem_Click(object sender, EventArgs e) {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() != DialogResult.Cancel) {
                richTextBox1.SelectionColor = colorDialog.Color;
            }
        }

        // Changed the font of the selected text.
        private void fontToolStripMenuItem1_Click(object sender, EventArgs e) {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() != DialogResult.Cancel) {
                richTextBox1.SelectionFont = fontDialog.Font;
            }
        }

        // Cut the text.
        private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
            richTextBox1.Cut();
        }

        // Copy the text.
        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            richTextBox1.Copy();
        }

        // Paste the cut or copied text.
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
            richTextBox1.Paste();
        }

        // Select all text.
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) {
            richTextBox1.SelectAll();
        }

        // Time and Date.
        private void timeAndDateToolStripMenuItem_Click(object sender, EventArgs e) {
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectedText = currTimeDate + DateTime.Now.ToLongTimeString();
        }

        // Changed the current content of file.
        private void richTextBox1_TextChanged(object sender, EventArgs e) {
            // Mark whether the update the text.
            if (this.Text[this.Text.Length - 1] != '*') {
                this.Text += '*';
            }

            row = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart) + 1;
            col = richTextBox1.SelectionStart - richTextBox1.GetFirstCharIndexFromLine(row - 1) + 1;

            label_row.Text = Convert.ToString(row);
            label_col.Text = Convert.ToString(col);
        }

        private void richTextBox1_EnabledChanged(object sender, EventArgs e) {
            label_status.Text = "Editing";
        }

        // Timer.
        private void timer_pb_Tick(object sender, EventArgs e) {
            if (toolStripProgressBar1.Value < toolStripProgressBar1.Maximum) {
                toolStripProgressBar1.Value += 25;
                richTextBox1.Enabled = false;
                if (currState == "save") {
                    label_status.Text = "Saving：" + (toolStripProgressBar1.Value).ToString("#") + "%";
                }
                else if (currState == "load") {
                    label_status.Text = "Loading：" + (toolStripProgressBar1.Value).ToString("#") + "%";
                }
            }
            else {
                timer_pb.Enabled = false;
                label_status.Text = "Finish";

                DialogResult result;
                if (currState == "load") {
                    loadFile();
                    result = MessageBox.Show("Load successful.", "Load", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (result == DialogResult.OK) {
                        toolStripProgressBar1.Value = 0;
                        richTextBox1.Enabled = true;
                    }
                }
                else if (currState == "save") {
                    saveFile();
                    result = MessageBox.Show("Save successful.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (result == DialogResult.OK) {
                        toolStripProgressBar1.Value = 0;
                        richTextBox1.Enabled = true;
                    }
                }
            }
        }

        // Save file.
        private void loadFile() {
            richTextBox1.LoadFile(filepath + "\\" + filename, RichTextBoxStreamType.RichText);
        }

        // Load file.
        private void saveFile() {
            richTextBox1.SaveFile(filepath + "\\" + filename, RichTextBoxStreamType.RichText);
        }
    }
}
