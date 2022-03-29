using System;
using System.Media;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Data;




namespace melodia
{

    public partial class Form1 : Form
    {
        SqlConnection polaczenie = new SqlConnection(@"Data source=LAPTOPIK\SQLEXPRESS; database=Muzyka;User id=marek;Password=jarek;");
        private string zmienna, podpowiedz;
        private int numerpiosenki, wynik, rekord,i=1;
        private int max_id = Globals.max_id;
        private bool zgadniety;
        
       
        

        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);
        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        Piosenka utwor = new Piosenka("loc");

        
        public Form1()
        {          
            InitializeComponent();

            waveOutGetVolume(IntPtr.Zero, out uint CurrVol);
            ushort CalcVol = (ushort)(CurrVol & 0x0000ffff);
            trackBar1.Value = CalcVol / (ushort.MaxValue / 10);
            obrazek();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            utwor.Odtworz();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            utwor.StopPiosenka();
        }
        

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                button4.Enabled = false;
            }
            else 
                button4.Enabled = true;
            

            zmienna = textBox1.Text;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (zgadniety == true)
            {
                MessageBox.Show("Ten utwór został już odgadnięty!");
                textBox1.Clear();
                
            }
            else { 
            if (utwor.sciezka == "loc")
            { 
                MessageBox.Show("Najpierw wylosuj utwór!");
                textBox1.Clear();


            }

                if (zmienna.Equals(utwor.tytul, StringComparison.OrdinalIgnoreCase) && utwor.sciezka != "loc")
                {
                label1.Text = "Dobrze";
                textBox1.Clear();
                pictureBox1.BackColor = System.Drawing.Color.Green;
                zgadniety = true;
                    wynik += 100;


                }
                else if (utwor.sciezka != "loc")
                {
                label1.Text = "Źle";
                textBox1.Clear();
                pictureBox1.BackColor = System.Drawing.Color.Red;
                zgadniety = false;
                    wynik = 0;
                }
                label5.Text = wynik.ToString();
                if (wynik>=rekord)
                {
                    rekord = wynik;
                    label7.Text = rekord.ToString();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool FirstClick = Globals.pierwszyklik;
            
           if( FirstClick == true)
            {
                podp();
                Globals.pierwszyklik = false;
            }
            LiczbaPiosenek(polaczenie);
            utwor.StopPiosenka();
            
            Random liczba = new Random();
            numerpiosenki = liczba.Next(1, max_id+1);
            LosujPiosenke(polaczenie);


            if (utwor.sciezka != "loc")
            {
                button1.Enabled = true;
                button2.Enabled = true;
            }
            label3.Text = utwor.wykonawca;
            button4.Enabled = false;
            zgadniety = false;
            pictureBox1.BackColor = System.Drawing.Color.White;
            label1.Text = "XXXXX";
        }



        private void textBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Return))
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                { }
                else
                    button4_Click(this, new EventArgs());
                e.Handled = true;       
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

            int NewVolume = (ushort.MaxValue / 10 * trackBar1.Value);

            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));

            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
            obrazek();

            

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
        private void obrazek ()
        {
            if (trackBar1.Value == 0)
                pictureBox2.Load(@"C:\Users\lenovo\source\repos\melodia\files\glosnik\glosnoscmute.png");
            if (trackBar1.Value >= 1 & trackBar1.Value <= 3)
                pictureBox2.Load(@"C:\Users\lenovo\source\repos\melodia\files\glosnik\glosnoscmin.png");
            if (trackBar1.Value >= 4 & trackBar1.Value <= 7)
                pictureBox2.Load(@"C:\Users\lenovo\source\repos\melodia\files\glosnik\glosnoscsrednia.png");
            if (trackBar1.Value >= 8 & trackBar1.Value <= 10)
                pictureBox2.Load(@"C:\Users\lenovo\source\repos\melodia\files\glosnik\glosnoscmax.png");
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                polaczenie.Open(); 
            }
            catch
            {
                DialogResult odp = MessageBox.Show("Wystąpił błąd przy próbie połączenia z bazą danych. Proszę ponowanie uruchomić program.", "Błąd", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                switch (odp)
                {
                    case DialogResult.Retry:
                        Application.Restart();
                        
                        break;
                    case DialogResult.Cancel:
                        Application.ExitThread();
                        break;
                }
                
            }
        }
        private void podp()
        {
            AutoCompleteStringCollection source = new AutoCompleteStringCollection();
            LiczbaPiosenek(polaczenie);
            for (i=1; i<max_id+2;i++)
            {
                odczyt(polaczenie);
            source.AddRange(new string[]
                            {
                        podpowiedz
                            }); 
                textBox1.AutoCompleteCustomSource = source;
                textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            }            
        }


        private void odczyt (SqlConnection polaczenie)
        {
            string zapytanie = "SELECT title FROM music WHERE id="+ i.ToString() ;
            SqlCommand command = new SqlCommand
            {
                Connection = polaczenie,
                CommandText = zapytanie
            };
            SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
            while (reader != null && reader.Read())
            {
                podpowiedz = reader.GetString(0);
            }
            reader.Close();
        }
        private void LiczbaPiosenek(SqlConnection polacznie)
        {
            string zapytanie = "SELECT COUNT(*) FROM music;";
            SqlCommand command = new SqlCommand
            {
                Connection = polaczenie,
                CommandText = zapytanie
            };
            SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult);
            while (reader != null && reader.Read())
            {
                max_id = reader.GetInt32(0);
            }
            reader.Close();
        }

        public void LosujPiosenke(SqlConnection polaczenie)
        {


            string zapytanie = "SELECT title, artist,sciezka FROM music WHERE id = " + numerpiosenki.ToString();
            SqlCommand command = new SqlCommand
            {
                Connection = polaczenie,
                CommandText = zapytanie
            };

            SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult);
           
            while (reader != null && reader.Read())
            {
                utwor.tytul = reader.GetString(0);
                utwor.wykonawca = reader.GetString(1);
                utwor.sciezka =  reader.GetString(2);
            }
            reader.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
                DialogResult odp = MessageBox.Show("Czy na pewno chcesz wyjść", "Wyjście", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                switch (odp)
                {
                    case DialogResult.Yes:
                        Application.ExitThread();
                        break;
                    case DialogResult.No:
                     e.Cancel= true;
                        break;
                }
            
        }
        private void button6_Click(object sender, EventArgs e)
        {
            Form2 opinia = new Form2();
            opinia.ShowDialog();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            LiczbaPiosenek(polaczenie);
            Globals.max_id = max_id;
            Form3 nowa = new Form3();
            nowa.ShowDialog();
        }
        public class Piosenka
        {
            public int id;
            public string tytul;
            public string sciezka;
            public string wykonawca;
            public Piosenka(string loc)
            {
                id = 0;
                tytul = "brak";
                sciezka = loc;
                wykonawca = "nieznany";
            }
            ~Piosenka()
            { }
            public void Odtworz()
            {
                SoundPlayer dzwiek = new SoundPlayer(sciezka);
                dzwiek.Play();

            }
            public void StopPiosenka()
            {
                SoundPlayer dzwiek = new SoundPlayer(sciezka);
                dzwiek.Stop();

            }

        }
    }
}
        
            

    

