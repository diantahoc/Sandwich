using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace Sandwich
{
    public partial class FullImageViewer : Form
    {

        private GenericPost _gp;
     
        bool has_loaded = false;
        int progress = 0;
        string formatted_size;

        public FullImageViewer(GenericPost gp)
        {
            InitializeComponent();
            this.Icon = Icon.FromHandle(Properties.Resources.image.GetHicon());
            _gp = gp;
            this.Text = gp.file.filename + "." + gp.file.ext + " - " + BoardInfo.GetBoardTitle(gp.board) ; 
            init();
        }

        private void init() 
        {
            formatted_size = Common.format_size_string(_gp.file.size);
            _gp.file.FullFileLoaded += new PostFile.ImageLoadedEvent(file_FullFileLoaded);
            _gp.file.FileProgressChanged += new Common.ProgressChanged(file_FileProgressChanged);
            _gp.file.BeginFullImageLoading();
        }

        private void load_image(Image b, Size s) 
        {
            PictureBoxWPF pbw = (PictureBoxWPF)(this.elementHost1.Child);
            pbw.SetImage(_gp.file.ext, b);
            pbw.SetSize(s);
        }

        void file_FileProgressChanged(int p)
        {
            progress = p;
        }

        void file_FullFileLoaded(PostFile sender)
        {
            try
            {
                load_image(_gp.file.FullImage, new Size(_gp.file.width, _gp.file.height));

                has_loaded = true;
                try
                {
                    this.Icon = Icon.FromHandle(((Bitmap)_gp.file.FullImage).GetHicon());
                }
                catch (Exception)
                {
                    this.Icon = BoardInfo.get_board_def_icon(_gp.board);
                }
            }
            catch (Exception ex)
            {
                this.Text = "Cannot load image " + ex.Message;
            }
         
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (!has_loaded) { return; }
            SaveFileDialog sf = new SaveFileDialog();
            sf.AddExtension = true;
            sf.Filter = String.Format("{0} files|*.{0}", _gp.file.ext);
            sf.CheckPathExists = true;
            sf.FileName = _gp.file.filename;
            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                save_image(sf.FileName);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (!has_loaded) { return; }
            string desktop_path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string file_path = Path.Combine(desktop_path, _gp.file.filename + "." + _gp.file.ext);
            save_image(file_path);
        }

        private void FullImageViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            ((PictureBoxWPF)(this.elementHost1.Child)).Clear();

            _gp.file.DestroyFullImage();

            this.Dispose();
        }

        private void show_message(string message, bool is_error) 
        {
            timer1.Start();
            if (is_error)
            {
                status_label.ForeColor = Color.Red;
            }
            else 
            {
                status_label.ForeColor = SystemColors.ControlText;
            }
            status_label.Text = message;
            panel1.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            panel1.Hide();
            timer1.Stop();
        }
     
        private void ui_Tick(object sender, EventArgs e)
        {
            if (progress > 100)
            {
                progress = 100;
            }
                toolStripProgressBar1.Value = progress;
                double downloaded = Convert.ToDouble(_gp.file.size) * (Convert.ToDouble(progress) /100d);

                progress_label.Text = progress.ToString() + "% " + Common.format_size_string(Convert.ToInt32(downloaded)) + " of " + formatted_size;
                if (progress == 100) { ui.Stop(); }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (!has_loaded) { return; }

            string desktop_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), _gp.board);

            Core.check_dir(desktop_path);

            string file_path = Path.Combine(desktop_path, _gp.file.filename + "." + _gp.file.ext);

            save_image(file_path);
        }

        private void save_image(string path) 
        {
            try
            {
                if (FileSystem.FileExists(path))
                {
                    throw new Exception("File already exist!");
                }
                
                File.WriteAllBytes(path, _gp.file.FullImageData);
                show_message(String.Format("{0} saved sucessfully!", _gp.file.filename + "." + _gp.file.ext), false);
            }
            catch (Exception ex)
            {
                show_message(ex.Message, true);
            }
        }

    }
}
