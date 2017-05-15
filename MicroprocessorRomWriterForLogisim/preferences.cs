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
    public partial class preferences : Form
    {
        public preferences()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = (Properties.Settings.Default.Formato.Equals("Hex")) ? 1 : 0;
            textBox1.Text = Properties.Settings.Default.NumIstruzioni.ToString();
            textBox2.Text = Properties.Settings.Default.NumMicroIstruzioni.ToString();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void preferences_Load(object sender, EventArgs e)
        {

        }

        private void annulla(object sender, EventArgs e)
        {
            this.Close();
        }

        private void save(object sender, EventArgs e)
        {
            Properties.Settings.Default.Formato = comboBox1.SelectedIndex.Equals(1) ? "Hex" : "Dec";
            Properties.Settings.Default.NumIstruzioni = int.Parse(textBox1.Text);
            Properties.Settings.Default.NumMicroIstruzioni = int.Parse(textBox2.Text);
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
