using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Globalization;
using Microsoft.Win32;
using System.Threading;
using System.IO;

namespace TextFile
{
    public class fileinit
    {
        protected string strFileName;
        protected string strProgName;

        int filterIndex;
        const string strNewLine = "NewLine";
        const string strFileEncoding = "FileEncoding";        // For registry
        const string strFilterIndex = "FilterIndex";        // For registry
        const string strFilterSave =
            "Text Documents (*.txt)|*.txt|Web Pages (*.htm;*.html)|*.htm;*.html|All Files (*.*)|*.*";
        const string strFilterOpen =
            "Text Documents (*.txt)|*.txt|Web Pages (*.htm;*.html)|*.htm;*.html|Rich Text Format (*.rtf)|*.rtf|All Files (*.*)|*.*";
        const string strMruList = "MruList";
        List<string> mruList = new List<string>();
        RichTextBox richTextBox1 = new RichTextBox();
        Form thisfrom=new Form();
        public class myVar
        {
            
        }
        public void Form1(Form2 form2)
        {

            this.Form1 = form2;
        }
            
            public void file()
        {

            strProgName = "Notepad with File";
            MakeCaption();
        }

        protected string FileTitle()
        {
            return (strFileName != null && strFileName.Length > 1) ?
                Path.GetFileName(strFileName) : "Untitled";
        }
        protected bool OkToTrash()
        {
            if (!richTextBox1.Modified)
            {
                return true;
            }

            DialogResult dr = MessageBox.Show(string.Format("The_text_in_the_{0}_has_changed", FileTitle()) + ".\n\n" +
                    "Do_you_want_to_save_the_changes ?", strProgName,
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation);
            switch (dr)
            {
                case DialogResult.Yes:
                    return SaveFileDlg();

                case DialogResult.No:
                    return true;

                case DialogResult.Cancel:
                    return false;
            }
            return false;
        }
        public void open_click()
        {
            if (!OkToTrash())
                return;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = strFilterOpen;
            ofd.FilterIndex = filterIndex;

            if (filterIndex == 1)
                ofd.FileName = "*.txt";
            else if (filterIndex == 2)
                ofd.FileName = "*.htm;*.html";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadFile(ofd.FileName);
                filterIndex = ofd.FilterIndex;
            }
        }
        public void new_click()
        {
            if (!OkToTrash())
                return;

            richTextBox1.Clear();
            richTextBox1.ClearUndo();
            richTextBox1.Modified = false;

            strFileName = null;
            MakeCaption();
        }
        // Utility routines
        protected void LoadFile(string strFileName)
        {
            thisfrom.Cursor = Cursors.WaitCursor;
            if (strFileName.EndsWith(".rtf"))
            {
                try
                {
                    richTextBox1.LoadFile(strFileName, RichTextBoxStreamType.RichText);
                }
                catch (Exception exc)
                {
                    //logger.Error(exc);
                    thisfrom.Cursor = Cursors.Default;
                    MessageBox.Show(exc.Message, strProgName,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }
            }
            else
            {
                StreamReader sr;

                try
                {
                    sr = new StreamReader(strFileName, System.Text.Encoding.Default, true);
                }
                catch (Exception exc)
                {
                    //logger.Error(exc);
                    thisfrom.Cursor = Cursors.Default;
                    MessageBox.Show(exc.Message, strProgName,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Asterisk);
                    return;
                }
                richTextBox1.Text = sr.ReadToEnd();
                sr.Close();
            }

            //thisfrom.strFileName = strFileName;
             
            MakeCaption();

            richTextBox1.SelectionStart = 0;
            richTextBox1.SelectionLength = 0;
            richTextBox1.Modified = false;
            richTextBox1.ClearUndo();
            thisfrom.Cursor = Cursors.Default;
        }
        bool SaveFileDlg()
        {
            SaveFileDialog sfd = new SaveFileDialog();

            if (strFileName != null && strFileName.Length > 1)
            {
                sfd.InitialDirectory = Path.GetDirectoryName(strFileName);
                sfd.FileName = Path.GetFileName(strFileName);
            }
            else if (filterIndex == 1)
                sfd.FileName = "*.txt";
            else if (filterIndex == 2)
                sfd.FileName = "*.htm;*.html";

            sfd.Filter = strFilterSave;
            sfd.FilterIndex = filterIndex;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                strFileName = sfd.FileName;
                filterIndex = sfd.FilterIndex;
                SaveFile();
                MakeCaption();
                return true;
            }
            else
            {
                return false;       // Return values are for OkToTrash.
            }
        }
        protected void MakeCaption()
        {
            string Text;
            Text = FileTitle() + " - " + strProgName;
        }
        private void save_Click()
        {
            if (strFileName == null || strFileName.Length == 0)
                SaveFileDlg();
            else
                SaveFile();
        }

        private void saveAs_Click()
        {
            SaveFileDlg();
        }
        protected virtual void pageSetup_Click()
        {
            MessageBox.Show("Print not yet implemented!", strProgName);
        }

        protected virtual void preview_Click()
        {
            MessageBox.Show("Print not yet implemented!", strProgName);
        }

        protected virtual void print_Click()
        {
            MessageBox.Show("Print not yet implemented!", strProgName);
        }

        private void exit_Click()
        {
            if (OkToTrash())
            {
                thisfrom.Close();
                Application.Exit();
            }
        }

        void SaveFile()
        {
            thisfrom.Cursor = Cursors.WaitCursor;
            try
            {
                StreamWriter sw = new StreamWriter(strFileName, false, System.Text.Encoding.Default);

                sw.NewLine = strNewLine;
                sw.Write(richTextBox1.Text);
                sw.Close();

            }
            catch (Exception exc)
            {
                thisfrom.Cursor = Cursors.Default;
                MessageBox.Show(exc.Message, strProgName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
                return;
            }
            richTextBox1.Modified = false;
            thisfrom.Cursor = Cursors.Default;
        }

    }
}
