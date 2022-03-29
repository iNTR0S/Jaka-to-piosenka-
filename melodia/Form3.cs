using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;



namespace melodia
{

    public partial class Form3 : Form
    {
        string sciezka, tytul, wykonawca;
        SqlConnection polaczenie = new SqlConnection(@"Data source=LAPTOPIK\SQLEXPRESS; database=Muzyka;User id=marek;Password=jarek;");
        int max_id = Globals.max_id;
        private bool czyistnieje;
        public Form3()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox1.Text) == false && string.IsNullOrWhiteSpace(textBox2.Text) == false && string.IsNullOrWhiteSpace(textBox1.Text) == false && string.IsNullOrWhiteSpace(textBox3.Text) == false)
            {
                if (textBox3.Text.EndsWith(".wav"))
                {
                    if (textBox1.Text.Length < 50 && textBox2.Text.Length < 50)
                    {
                        tytul = textBox1.Text;
                        wykonawca = textBox2.Text;
                        sciezka = textBox3.Text;
                        CzyIstnieje(polaczenie);
                        if (czyistnieje == true)
                        {
                            MessageBox.Show("Ta piosenka już się znajduje bazie.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else if (czyistnieje == false)
                        {
                            DodajPiosenke(polaczenie);
                            MessageBox.Show("Piosenka została dodana do bazy.", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Close();
                        }
                    }
                    else if (textBox1.Text.Length >= 50)
                    {
                        MessageBox.Show("Tytuł piosenki może składać się maksymalnie z 50 znaków", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (textBox2.Text.Length >= 50)
                    {
                        MessageBox.Show("Nazwa wykonawcy może składać się maksymalnie z 50 znaków", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                    MessageBox.Show("Format pliku jest nieobsługiwany. Obsługiwany format to .wav", "Bład", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show("Żadne pole nie może być puste", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            try
            {
                polaczenie.Open();
            }
            catch (SqlException se)
            {
                MessageBox.Show("Wystąpił błąd przy próbie połączenia z bazą danych" + se.StackTrace);
            }
        }
        private void DodajPiosenke(SqlConnection polaczenie)
        {
            string polecenie = "INSERT INTO music(id, artist, title, sciezka) VALUES('" + (max_id + 1).ToString() + "','" + wykonawca + "','" + tytul + "','" + sciezka + "')";
            SqlCommand command = new SqlCommand
            {
                Connection = polaczenie,
                CommandText = polecenie
            };
            try
            {
                command.ExecuteNonQuery();
            }
            catch
            {
                MessageBox.Show("Dodawanie piosenki do bazy nie powiodło się", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void kopiujToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Copy();
        }

        private void wytnijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Cut();
        }

        private void wklejToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Paste();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            textBox2.Copy();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            textBox2.Cut();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            textBox2.Paste();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            textBox3.Copy();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            textBox3.Cut();
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            textBox3.Paste();
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.pierwszyklik = true;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            sciezka = openFileDialog1.FileName;
            textBox3.Text = sciezka;
        }
        private void CzyIstnieje(SqlConnection polaczenie)
        {
            string polecenie = "IF EXISTS (SELECT title FROM music WHERE sciezka ='" + sciezka + "')" + "SELECT 'true' ELSE SELECT 'false'";
            SqlCommand command = new SqlCommand
            {
                Connection = polaczenie,
                CommandText = polecenie
            };
            try
            {
                SqlDataReader czytnik = command.ExecuteReader(CommandBehavior.SingleResult);

                while (czytnik != null && czytnik.Read())
                {
                    string sprawdz = czytnik.GetString(0);
                    czyistnieje = Convert.ToBoolean(sprawdz);

                }
                czytnik.Close();
            }
            catch
            {
                MessageBox.Show("Dodawanie piosenki do bazy nie powiodło się", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
