using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ResimKucult
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Olaylar

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            label5.Visible = false;
            comboBox1.SelectedIndex = 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DosyaGoster(checkBox1.Checked);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            KlasorGoster(checkBox1.Checked);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Islem();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.ReadOnly = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.ReadOnly = !checkBox3.Checked;
            textBox2.ReadOnly = !checkBox3.Checked || checkBox2.Checked;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (((TextBox)(sender)).Text.IndexOf('q') >= 0)
            {
                ((TextBox)(sender)).Text = string.Empty;
            }
            else
            {
                if (((TextBox)(sender)).Text != string.Empty && ((TextBox)(sender)).Text != "q")
                {
                    if (((TextBox)(sender)).Name == "textBox3" && Convert.ToInt32(((TextBox)(sender)).Text) > 100)
                    {
                        ((TextBox)(sender)).Text = "100";
                    }
                }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
                e.KeyChar = 'q';
        }

        #endregion

        #region Methodlar

        private void Islem()
        {
            Thread thr = new Thread(new ThreadStart(KucultveKaydet));
            thr.Start();
        }
        private void DosyaGoster(bool hemenyap)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Desteklenen Resim Dosyaları|*.jpeg;*.jpg;*.bmp;*.png;*.gif;*.tiff;*.tif|JPG Dosyaları|*.jpeg;*.jpg|BMP Dosyaları|*.bmp|PNG Dosyaları|*.png|GIF Dosyaları|*.gif|TIFF Dosyaları|*.tiff;*.tif|Bütün Dosyalar|*.*";
            ofd.SupportMultiDottedExtensions = true;
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < ofd.FileNames.Length; i++)
                {
                    listBox1.Items.Add(ofd.FileNames[i]);
                }
            }

            if (hemenyap)
                Islem();
        }

        private void KlasorGoster(bool hemenyap)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string klasor = fbd.SelectedPath;

                string[] dosyalar = Directory.GetFiles(fbd.SelectedPath);
                for (int i = 0; i < dosyalar.Length; i++)
                {
                    listBox1.Items.Add(dosyalar[i]);
                }
            }

            if (hemenyap)
                Islem();
        }

        private void KucultveKaydet()
        {
            foreach (Control Ctrl in this.Controls) ((Control)Ctrl).Enabled = false;
            progressBar1.Maximum = listBox1.Items.Count;
            progressBar1.Value = 0;
            label5.Enabled = true;
            label5.Visible = true;

            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                string resimAdi = listBox1.GetItemText(listBox1.Items[i]);
                using (Bitmap img = new Bitmap(resimAdi))
                {
                    double aspectRatio = Convert.ToDouble(img.Height) / img.Width;
                    int en = 0, boy = 0;
                    if (img.Width > img.Height)
                    {
                        en = Convert.ToInt32(textBox1.Text);
                        boy = Convert.ToInt32(aspectRatio * en);
                    }
                    else
                    {
                        boy = Convert.ToInt32(textBox1.Text);
                        en = Convert.ToInt32(boy / aspectRatio);
                    }
                    if (!checkBox2.Checked)
                        boy = Convert.ToInt32(textBox2.Text);
                    Size newsize = new Size(en, boy);
                    Bitmap thumbimg = new Bitmap(img, newsize);
                    string newname = resimAdi + "_k";

                    ImageCodecInfo ici = GetEncoderInfo(img.RawFormat);
                    EncoderParameter encp = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Convert.ToInt64(textBox3.Text.Trim()));
                    EncoderParameter encp1 = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, Convert.ToInt64(comboBox1.SelectedItem.ToString()));
                    EncoderParameters encps = new EncoderParameters(2);
                    encps.Param[0] = encp;
                    encps.Param[1] = encp1;

                    thumbimg.Save(resimAdi.Substring(0, resimAdi.LastIndexOf(".")) + "_k" + resimAdi.Substring(resimAdi.LastIndexOf(".")), ici, encps);
                    progressBar1.Value = i;
                }
            }

            listBox1.Items.Clear();
            label5.Visible = false;
            progressBar1.Value = 0;
            foreach (Control Ctrl in this.Controls) ((Control)Ctrl).Enabled = true;
        }

        private ImageCodecInfo GetEncoderInfo(ImageFormat imageformat)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].FormatID == imageformat.Guid)
                    return codecs[i];
            return null;
        }

        #endregion
    }
}
