using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;

namespace melodia
{
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MailMessage wiadomosc = new MailMessage("przeslijopinie@interia.pl", "przeslijopinie@interia.pl", textBox2.Text, textBox1.Text);
            SmtpClient smtp = new SmtpClient("poczta.interia.pl", 587);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("przeslijopinie@interia.pl", "qwerty123");
            smtp.Timeout = 300000;
            smtp.EnableSsl = true;
            try
            {
                if(textBox1.Text.Length <= 300 && textBox2.Text.Length <= 30)
                { 
                smtp.Send(wiadomosc);
                MessageBox.Show("Opinia została przesłana");
                    Close();
                }
                else if (textBox1.Text.Length > 300)
                MessageBox.Show("Treść wiadomości przekracza limit 300 znaków.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (textBox2.Text.Length > 30)
                    MessageBox.Show("Podpis przekracza limit 30 znaków.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch 
            {
                MessageBox.Show("Przesyłanie opini nie powiodło się", "Niepowodzenie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
            }                     
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {   
            label2.Text = textBox1.Text.Length.ToString() + "/300";
            if (textBox1.Text.Length < 300 )
            {
                label2.ForeColor = Color.Black;
            }
            else if (textBox1.Text.Length >= 300)
                label2.ForeColor = Color.Red;              
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label3.Text = textBox2.Text.Length.ToString() + "/30";
            if (textBox2.Text.Length < 30)
            {
                label3.ForeColor = Color.Black;
            }
            else if (textBox2.Text.Length >= 30)
                label3.ForeColor = Color.Red;
        }

        private void kopiujToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Copy();
        }

        private void wytniijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Cut();
        }

        private void wklejToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Paste();
        }

        private void kopiujToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            textBox2.Copy();
        }

        private void wytnijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox2.Cut();
        }

        private void wklejToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            textBox2.Paste();
        }
    }
}
