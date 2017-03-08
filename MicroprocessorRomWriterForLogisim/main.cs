using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicroprocessorRomWriterForLogisim
{
    

    public partial class main : Form
    {
        String[,] ValArray;
        int currentselcted, maxnumistr, maxnummic, istrnum, micrnum;
        public main(int istrnum, int maxmic)
        {
            InitializeComponent();

            this.istrnum = istrnum;
            this.micrnum = maxmic;

            maxnumistr = tobin(istrnum-1).Length;
            maxnummic = tobin(micrnum - 1).Length;

            for(int i = 0; i<istrnum; i++)
            {
                String istrName = format(tobin(i), maxnumistr);
                comboBox1.Items.Add(new istruction(istrName));
            }

            ValArray = new String[istrnum, micrnum];
            

            comboBox1.SelectedIndex = 0;
            currentselcted = 0;

            comboBox1.SelectedIndexChanged += new EventHandler(comboboxchanged);


            createedittext();
        }

        public void createedittext()
        {
            for (int i = 0; i < micrnum; i++)
            {
                TextBox box = new TextBox();
                Label lb = new Label();
                box.Tag = i;
                box.Name = "box" + i;
                box.TextChanged += new EventHandler(textchanged);
                lb.Text = format(tobin(i), maxnummic);
                lb.Tag = i;
                lb.Name = i.ToString();
                lb.MaximumSize = new Size(60, 60);
                Color alpha = Color.FromArgb(30, Color.Black);
                lb.ForeColor = alpha;
                panel1.Controls.Add(box);
                panel1.Controls.Add(lb);
                if (i < (micrnum / 2))
                {
                    box.Location = new Point(20, 5 + (i * 25));
                    lb.Location = new Point(130, 7 + (i * 25));

                }
                else
                {
                    box.Location = new Point(200, 17 + (i * 25 - (25 * micrnum / 2)));
                    lb.Location = new Point(310, 19 + (i * 25 - (25 * micrnum / 2)));

                }
            }
        }

        public void textchanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            System.Diagnostics.Debug.WriteLine("modifico la microistruzione " + txt.Tag + " del set di istruzioni " + currentselcted + " con il valore di " + txt.Text);

            var correct = iscorrect(txt.Text);
            if (correct.Item1)
            {
                editlabel(int.Parse(txt.Tag.ToString()), true, null);
                ValArray[currentselcted, (int)txt.Tag] = txt.Text;
            }
            else
            {
                editlabel(int.Parse(txt.Tag.ToString()), false, correct.Item2);
            }
        }


        public void editlabel(int num, bool validate, String msg)
        {
            foreach (Control ctrl in panel1.Controls)
            {
                if (ctrl is Label)
                {
                    if (ctrl.Tag.Equals(num))
                    {

                        if (validate)
                        {
                            ctrl.Text = format(tobin(num), maxnummic);
                            ctrl.ForeColor = System.Drawing.Color.Black;
                            toolTip1.SetToolTip(ctrl, null);

                        }
                        else
                        {
                            String text = String.Format("il valore \"{0}\" non è consentito", msg);
                            toolTip1.SetToolTip(ctrl, text);
                            ctrl.ForeColor = System.Drawing.Color.Red;
                        }

                    }
                }
            }
        }



        public Tuple<bool, String> iscorrect(String text)
        {
            string[] numbs = text.Split(null);
            foreach(String num in numbs)
            {
                int use;
                String numzer = num.Replace(" ", "");
                if (!int.TryParse(numzer, out use) && numzer != "")
                {
                    System.Diagnostics.Debug.WriteLine("non è un numero");
                    return Tuple.Create(false, numzer);
                }
            }

            return Tuple.Create(true, "");
        }

        public void comboboxchanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            currentselcted = cmb.SelectedIndex;
            int cnt = 0;
            foreach (Control txt in panel1.Controls)
            {
                if(txt is TextBox)
                {
                    txt.Text = ValArray[currentselcted, cnt];
                    cnt++;
                }
            }
        }

        public String tobin(int dec)
        {
            return Convert.ToString(dec, 2);
        }

        public String format(String num, int max)
        {
            int toadd = max - num.Length;
            while(toadd > 0){
                num = "0" + num;
                toadd--;
            }

            return num;
        }
        

        private void button1_Click_1(object sender, EventArgs e)
        {
            foreach(Control tx in panel1.Controls)
            {
                if(tx is TextBox)
                {
                    tx.Text = tx.Text + " " + AddBox.Text;
                }
            }
        }
        
        private void Import_rom(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                settings set = new settings(false);
                var resp = set.ShowDialog();
                if (resp == DialogResult.OK)
                {

                    string text = File.ReadAllText(openFileDialog1.FileName);
                    text = text.Replace("v2.0 raw", "");
                    text = text.Replace("\r", " ");
                    text = text.Replace("\n", " ");

                    
                    this.istrnum = set.istrnm;
                    this.micrnum = set.micronm;

                    maxnumistr = tobin(istrnum - 1).Length;
                    maxnummic = tobin(micrnum - 1).Length;

                    comboBox1.Items.Clear();

                    for (int i = 0; i < istrnum; i++)
                    {
                        String istrName = format(tobin(i), maxnumistr);
                        comboBox1.Items.Add(new istruction(istrName));
                    }

                    ValArray = new String[istrnum, micrnum];


                    currentselcted = 0;

                    foreach (Control ct in panel1.Controls)
                    {
                        panel1.Controls.Clear();
                    }

                    createedittext();

                    decompose(text);
                    
                }
                
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (Control tx in panel1.Controls)
            {
                if (tx is TextBox)
                {
                    tx.Text = tx.Text.Replace(RemoveBox.Text, "");
                }
            }
        }

        public String convertnumber(String origin)
        {
            #region tobinary
            string[] numbs = origin.Split(null);
            String ret = null;
            List<int> formattednumbers = new List<int>();
            foreach (String num in numbs)
            {
                String numzer = num.Replace(" ", "");
                if(numzer != "" && numzer != null)
                {
                    formattednumbers.Add(int.Parse(numzer));
                }
            }

            formattednumbers = formattednumbers.OrderByDescending(p => p).ToList();

            int maxval = formattednumbers[0];
            for(int i = maxval; i>=0; i--)
            {
                if (formattednumbers.Contains(i))
                {
                    ret = ret + "1";
                }
                else
                {
                    ret = ret + "0";
                }
            }
            #endregion
            System.Diagnostics.Debug.WriteLine(ret);

            String hex = Convert.ToInt32(ret, 2).ToString("X");




            return hex;             
        }

        public void decompose(String text)
        {
            List<String> instr = new List<string>();
            string[] instructions = text.Split(null);
            foreach (String instruction in instructions)
            {
                int use;
                String instructioncor = instruction.Replace(" ", "");
                System.Diagnostics.Debug.WriteLine(instructioncor);
                if(instructioncor != "")
                {
                    instr.Add(instructioncor);
                }
            }

            //riempo l'array e poi vado a triggerare il change della combobox

            int listcont = 0;
            for(int i = 0; i<istrnum; i++)
            {
                for(int j = 0; j <micrnum; j++)
                {
                    try
                    {
                        if (instr[listcont].Contains("*0"))
                        {
                            int jump = int.Parse(instr[listcont].Replace("*0", ""));
                            j = j + jump - 1; // 1 viene aggiunto dal ciclo
                        }
                        else
                        {
                            ValArray[i, j] = tonumbers(instr[listcont]);
                        }

                        listcont++;
                    }
                    catch (Exception e)
                    {

                    }
                    
                }
            }

            comboBox1.SelectedIndex = 0;

        }

        public String tonumbers(String text)
        {
            String res = "";
           string bin = String.Join(String.Empty,text.Select(
                            c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                          )
                        );

            int count = bin.Length;
            foreach(char ch in bin)
            {
                if (ch.Equals('1'))
                {
                    res = res + " " + count;
                }
                count--;
            }
            return res;
        }
        private void Esport_rom(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {           //saveFileDialog1.FileName
                String towrite = "v2.0 raw" + System.Environment.NewLine;
                for(int i = 0; i<istrnum; i++)
                {
                    int countzero = 0;
                    for(int v = 0; v< micrnum; v++)
                    {
                        if(ValArray[i,v] == null || ValArray[i, v] == "" || ValArray[i, v] == " ")
                        {
                            countzero++;
                        }
                        else
                        {
                            if (countzero != 0)
                            {
                                towrite = towrite + countzero + "*0 ";
                            }

                            System.Diagnostics.Debug.WriteLine(convertnumber(ValArray[i, v]));
                            towrite = towrite + convertnumber(ValArray[i, v]) + " ";
                        }

                        if(v.Equals(micrnum - 1) && countzero != 0)
                        {
                            towrite = towrite + countzero + "*0 ";
                        }
                    }
                }

                File.WriteAllText(saveFileDialog1.FileName, towrite);


            }
        }
        
    }
}
