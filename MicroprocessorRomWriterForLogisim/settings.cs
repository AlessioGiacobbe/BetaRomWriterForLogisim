using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicroprocessorRomWriterForLogisim
{
    public partial class settings : Form
    {
        public int istrnm { get; set; }
        public int micronm { get; set; }

        bool? firstrun;
        public settings()
        {
            InitializeComponent();
        }

        public settings(bool firstrun)
        {
            this.firstrun = firstrun;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(IstrNum.Text != null && MicroText.Text != null)
            {
                try
                {
                    this.istrnm = int.Parse(IstrNum.Text);
                    this.micronm = int.Parse(MicroText.Text); 
                    if(firstrun != null)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        this.Hide();
                        main m = new main(int.Parse(IstrNum.Text), int.Parse(MicroText.Text));
                        m.Closed += (s, args) => this.Close();
                        m.Show();
                    }


                    
                }catch(Exception ex)
                {

                }
               
            }
            
        }

        private void settings_Load(object sender, EventArgs e)
        {
            IstrNum.Text = Properties.Settings.Default.NumIstruzioni.ToString();
            MicroText.Text = Properties.Settings.Default.NumMicroIstruzioni.ToString();


        }
    }
}
