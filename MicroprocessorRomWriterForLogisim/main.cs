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
        String[,] ValArray1; // Array per la prima ROM
        int currentselcted, maxnumistr, maxnummic, istrnum, micrnum;
        bool attivo64 = false;
        bool attivo32 = false;


        // Costruttore della classe principale
        public main(int istrnum, int maxmic)
        {
            InitializeComponent();

            this.istrnum = 256;
            this.micrnum = 16;

            ValArray1 = new String[istrnum, micrnum];

            maxnumistr = (Properties.Settings.Default.Formato.Equals("Hex")) ? tohex(istrnum - 1).Length : tobin(istrnum - 1).Length;
            maxnummic = (Properties.Settings.Default.Formato.Equals("Hex")) ? tohex(istrnum - 1).Length : tobin(micrnum - 1).Length;
            // Crea una lista di istruzioni per le combobox
            for (int i = 0; i < istrnum; i++)
            {
                String istrName = (Properties.Settings.Default.Formato.Equals("Hex")) ? format(tohex(i), maxnumistr) : format(tobin(i), maxnumistr);
                comboBox1.Items.Add(new istruction(istrName));
                comboBox2.Items.Add(new istruction(istrName));
            }

            currentselcted = 0;
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox1.SelectedIndexChanged += new EventHandler(comboboxchanged);

            createedittext();
        }

        // Creazione dei campi di testo per l'editing
        public void createedittext()
        {
            for (int i = 0; i < micrnum; i++)
            {
                TextBox box = new TextBox();
                Label lb = new Label();
                box.Tag = i;
                box.Name = "Box" + i;
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


        // Gestione delle modifiche al testo
        public void textchanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            System.Diagnostics.Debug.WriteLine("Modifico la microistruzione " + txt.Tag + " del set di istruzioni " + currentselcted + " con il valore di " + txt.Text);

            var correct = iscorrect(txt.Text);
            if (correct.Item1)
            {
                editlabel(int.Parse(txt.Tag.ToString()), true, null);
                ValArray1[currentselcted, (int)txt.Tag] = txt.Text;
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
                            String text = String.Format("Il valore \"{0}\" non è consentito", msg);
                            toolTip1.SetToolTip(ctrl, text);
                            ctrl.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
            }
        }

        // Funzione per verificare la correttezza del testo
        public Tuple<bool, String> iscorrect(String text)
        {
            string[] numbs = text.Split(null);
            foreach (String num in numbs)
            {
                int use;
                String numzer = num.Replace(" ", "");
                if (!int.TryParse(numzer, out use) && numzer != "")
                {
                    System.Diagnostics.Debug.WriteLine("Non è un numero");
                    return Tuple.Create(false, numzer);
                }
            }

            return Tuple.Create(true, "");
        }

        // Gestione del cambiamento di selezione nella combobox
        public void comboboxchanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            currentselcted = cmb.SelectedIndex;
            int cnt = 0;
            foreach (Control txt in panel1.Controls)
            {
                if (txt is TextBox)
                {
                    txt.Text = ValArray1[currentselcted, cnt];
                    cnt++;
                }
            }
        }

        // Funzioni di conversione e formattazione
        public String tobin(int dec)
        {
            return Convert.ToString(dec, 2);
        }

        public String format(String num, int max)
        {
            int toadd = max - num.Length;
            while (toadd > 0)
            {
                num = "0" + num;
                toadd--;
            }

            return num;
        }

        //Convertire a esadecimale un decimale
        public String tohex(int dec)
        {
            return Convert.ToString(dec, 16).ToUpper();
        }

        public string analyze(string text)
        {
            String rtn = "";
            foreach (String tx in text.Split(null))
            {
                if (tx.Contains("*"))
                {

                    var tmp = tx.Replace("*0", "");
                    for (int i = 0; i < int.Parse(tmp); i++)
                    {
                        rtn = rtn + "0" + " ";

                    }

                }
                else
                {
                    rtn = rtn + tx + " ";
                }
            }
            return rtn;
        }

        // Funzione per importare le ROM
        private void Import_rom(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "All files (*.*)|*.*|(*.low)|*.low";
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

                    text = analyze(text);

                    // Salva i valori correnti di istrnum e micrnum
                    int oldIstrNum = this.istrnum;
                    int oldMicrNum = this.micrnum;

                    // Crea un array temporaneo per salvare i valori esistenti
                    String[,] tempArray = null;

                    // Se ci sono già dati nell'array, salviamoli
                    if (ValArray1 != null)
                    {
                        tempArray = new String[oldIstrNum, oldMicrNum];
                        for (int i = 0; i < oldIstrNum; i++)
                        {
                            for (int j = 0; j < oldMicrNum; j++)
                            {
                                tempArray[i, j] = ValArray1[i, j];
                            }
                        }
                    }

                    // Aggiorna le dimensioni in base alle nuove impostazioni
                    this.istrnum = set.istrnm;
                    this.micrnum = set.micronm;

                    maxnumistr = (Properties.Settings.Default.Formato.Equals("Hex")) ? tohex(istrnum - 1).Length : tobin(istrnum - 1).Length;
                    maxnummic = (Properties.Settings.Default.Formato.Equals("Hex")) ? tohex(istrnum - 1).Length : tobin(micrnum - 1).Length;

                    comboBox1.Items.Clear();
                    comboBox2.Items.Clear();

                    for (int i = 0; i < istrnum; i++)
                    {
                        String istrName = (Properties.Settings.Default.Formato.Equals("Hex")) ? format(tohex(i), maxnumistr) : format(tobin(i), maxnumistr);
                        comboBox1.Items.Add(new istruction(istrName));
                        comboBox2.Items.Add(new istruction(istrName));
                    }

                    // Crea un nuovo array con le nuove dimensioni
                    String[,] newArray = new String[istrnum, micrnum];

                    // Crea un array temporaneo per i nuovi dati importati
                    String[,] importedArray = new String[istrnum, micrnum];

                    // Pulisci i controlli del pannello
                    foreach (Control ct in panel1.Controls)
                    {
                        panel1.Controls.Clear();
                    }


                    createedittext();
                    decomposeToArray(text, importedArray);


                    if (tempArray != null)
                    {
                        int minIstrNum = Math.Min(oldIstrNum, istrnum);
                        int minMicrNum = Math.Min(oldMicrNum, micrnum);

                        for (int i = 0; i < minIstrNum; i++)
                        {
                            for (int j = 0; j < minMicrNum; j++)
                            {
                                newArray[i, j] = tempArray[i, j];
                            }
                        }
                    }

                    for (int i = 0; i < istrnum; i++)
                    {
                        for (int j = 0; j < micrnum; j++)
                        {
                            if (importedArray[i, j] != null && importedArray[i, j] != "")
                            {
                                if (newArray[i, j] == null || newArray[i, j] == "")
                                {
                                    newArray[i, j] = importedArray[i, j];
                                }
                                else
                                {
                                    newArray[i, j] = MergeValues(newArray[i, j], importedArray[i, j]);
                                }
                            }
                        }
                    }

                    ValArray1 = newArray;

                    if (comboBox1.Items.Count > 0)
                    {
                        comboBox1.SelectedIndex = 0;
                    }

                    attivo64 = true;
                    attivo32 = false;

                    MessageBox.Show("Importazione e unione completate con successo.", "Fatto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Funzione per esportare le ROM
        private void Export_Combined(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.low)|*.low";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                bool needsTwoFiles = false;

                // Check if we need to create two files by checking if any numbers are > 31
                for (int i = 0; i < istrnum; i++)
                {
                    for (int v = 0; v < micrnum; v++)
                    {
                        if (ValArray1[i, v] != null && ValArray1[i, v] != "" && ValArray1[i, v] != " ")
                        {
                            string[] numbs = ValArray1[i, v].Split(null);
                            foreach (String num in numbs)
                            {
                                String numzer = num.Replace(" ", "");
                                if (numzer != "" && numzer != null)
                                {
                                    int numValue = int.Parse(numzer);
                                    if (numValue > 31)
                                    {
                                        needsTwoFiles = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (needsTwoFiles) break;
                    }
                    if (needsTwoFiles) break;
                }

                // Create the first file (numbers <= 31)
                String towrite1 = "v2.0 raw" + System.Environment.NewLine;
                for (int i = 0; i < istrnum; i++)
                {
                    int countzero = 0;
                    for (int v = 0; v < micrnum; v++)
                    {
                        if (ValArray1[i, v] == null || ValArray1[i, v] == "" || ValArray1[i, v] == " ")
                        {
                            countzero++;
                        }
                        else
                        {
                            string[] numbs = ValArray1[i, v].Split(null);
                            List<int> under32 = new List<int>();

                            foreach (String num in numbs)
                            {
                                String numzer = num.Replace(" ", "");
                                if (numzer != "" && numzer != null)
                                {
                                    int numValue = int.Parse(numzer);
                                    if (numValue <= 31)
                                    {
                                        under32.Add(numValue);
                                    }
                                }
                            }

                            if (under32.Count > 0)
                            {
                                if (countzero != 0)
                                {
                                    towrite1 = towrite1 + countzero + "*0 ";
                                    countzero = 0;
                                }

                                string adjustedNumbers = string.Join(" ", under32);
                                towrite1 = towrite1 + convertnumber(adjustedNumbers) + " ";
                            }
                            else
                            {
                                countzero++;
                            }
                        }

                        if (v.Equals(micrnum - 1) && countzero != 0)
                        {
                            towrite1 = towrite1 + countzero + "*0 ";
                        }
                    }
                    towrite1 = towrite1 + System.Environment.NewLine;
                }

                File.WriteAllText(saveFileDialog1.FileName, towrite1);

                // Create the second file if needed (numbers > 31)
                if (needsTwoFiles)
                {
                    string filename2 = Path.GetFileNameWithoutExtension(saveFileDialog1.FileName) + ".high";
                    string filePath2 = Path.Combine(Path.GetDirectoryName(saveFileDialog1.FileName), filename2);

                    String towrite2 = "v2.0 raw" + System.Environment.NewLine;
                    for (int i = 0; i < istrnum; i++)
                    {
                        int countzero = 0;
                        for (int v = 0; v < micrnum; v++)
                        {
                            if (ValArray1[i, v] == null || ValArray1[i, v] == "" || ValArray1[i, v] == " ")
                            {
                                countzero++;
                            }
                            else
                            {
                                string[] numbs = ValArray1[i, v].Split(null);
                                List<int> over31 = new List<int>();

                                foreach (String num in numbs)
                                {
                                    String numzer = num.Replace(" ", "");
                                    if (numzer != "" && numzer != null)
                                    {
                                        int numValue = int.Parse(numzer);
                                        if (numValue > 31)
                                        {
                                            over31.Add(numValue - 32);
                                        }
                                    }
                                }

                                if (over31.Count > 0)
                                {
                                    if (countzero != 0)
                                    {
                                        towrite2 = towrite2 + countzero + "*0 ";
                                        countzero = 0;
                                    }

                                    string adjustedNumbers = string.Join(" ", over31);
                                    towrite2 = towrite2 + convertnumber(adjustedNumbers) + " ";
                                }
                                else
                                {
                                    countzero++;
                                }
                            }

                            if (v.Equals(micrnum - 1) && countzero != 0)
                            {
                                towrite2 = towrite2 + countzero + "*0 ";
                            }
                        }
                        towrite2 = towrite2 + System.Environment.NewLine;
                    }

                    File.WriteAllText(filePath2, towrite2);
                    MessageBox.Show("Export completed. Two files created due to values > 31.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Export completed successfully.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Funzione per processare i file importati
        private void ProcessImportedFiles(string text1)
        {
            string[] instructions1 = text1.Split(null);

            PopulateArray(ValArray1, instructions1, 0, 31);
        }

        private void PopulateArray(string[,] array, string[] instructions, int start, int end)
        {
            int listcont = 0;
            for (int i = start; i <= end; i++)
            {
                for (int j = 0; j < micrnum; j++)
                {
                    try
                    {
                        if (listcont < instructions.Length)
                        {
                            if (instructions[listcont].Contains("*0"))
                            {
                                int jump = int.Parse(instructions[listcont].Replace("*0", ""));
                                j = j + jump - 1;
                            }
                            else
                            {
                                array[i, j] = tonumbers(instructions[listcont]);
                            }
                            listcont++;
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Errore gestito");
                    }
                }
            }
        }



        // Funzioni di conversione dei numeri
        public String tonumbers(String text)
        {
            String res = "";
            string bin = String.Join(String.Empty, text.Select(
                            c => Convert.ToString(Convert.ToInt64(c.ToString(), 16), 2).PadLeft(4, '0')
                          )
                        );

            int count = bin.Length - 1;
            foreach (char ch in bin)
            {
                if (ch.Equals('1'))
                {
                    res = res + " " + count;
                }
                count--;
            }
            return res;
        }


        // Funzione per convertire una stringa di numeri separati da spazi in una rappresentazione esadecimale della stringa binaria equivalente
        public String convertnumber(String origin)
        {
            // Divide la stringa di input in singole parole usando Split(null)
            string[] numbs = origin.Split(null);

            // Stringa vuota per memorizzare la rappresentazione binaria
           string ret = null;

            // Lista per memorizzare i numeri formattati (interi)
            List<int> formattednumbers = new List<int>();

            // Ciclo per iterare sulle singole parole (numeri)
            foreach (String num in numbs)
            {
                // Rimuove gli spazi iniziali e finali dal numero
                String numzer = num.Replace(" ", "");

                // Controlla se la stringa del numero è vuota o null
                if (numzer != "" && numzer != null)
                {
                    // Aggiunge il numero intero convertito alla lista
                    formattednumbers.Add(int.Parse(numzer));
                }
            }

            // Ordina la lista di numeri in ordine decrescente
            formattednumbers = formattednumbers.OrderByDescending(p => p).ToList();

            // Individua il valore massimo nella lista ordinata
            int maxval = formattednumbers[0];

            // Stringa per memorizzare la rappresentazione binaria
            ret = "";

            // Ciclo per iterare su tutti i valori possibili (da maxval a 0)
            for (int i = maxval; i >= 0; i--)
            {
                // Controlla se il valore corrente è presente nella lista ordinata
                if (formattednumbers.Contains(i))
                {
                    // Aggiunge "1" alla stringa binaria se il valore è presente
                    ret = ret + "1";
                }
                else
                {
                    // Aggiunge "0" alla stringa binaria se il valore è assente
                    ret = ret + "0";
                }
            }

            // Converte la stringa binaria in esadecimale
            String hex = Convert.ToInt64(ret, 2).ToString("X");

            // Restituisce la rappresentazione esadecimale
            return hex;
        }

        // Funzioni per la gestione dei pulsanti
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(AddBox.Text))
            {        // Aggiungi il testo inserito nella TextBox AddBox alla lista delle istruzioni       
                     comboBox1.Items.Add(new istruction(AddBox.Text)); 
                comboBox2.Items.Add(new istruction(AddBox.Text));
                AddBox.Clear();    }
                else{        
                MessageBox.Show("Inserisci un valore valido.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error); 

            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            // Cancella tutti i valori di ValArray1
            for (int i = 0; i < istrnum; i++)
            {
                for (int j = 0; j < micrnum; j++)
                {
                    ValArray1[i, j] = null;
                }
            }

            // Cancella i controlli del pannello
            panel1.Controls.Clear();

            // Ricrea i campi di testo per l'editing
            createedittext();
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

        private void buttonImport64_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "All files (*.*)|*.*|(*.high)|*.high";
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

                        text = analyze(text);

                        // Salva i valori correnti di istrnum e micrnum
                        int oldIstrNum = this.istrnum;
                        int oldMicrNum = this.micrnum;

                        // Crea un array temporaneo per salvare i valori esistenti
                        String[,] tempArray = null;

                        // Se ci sono già dati nell'array, salviamoli
                        if (ValArray1 != null)
                        {
                            tempArray = new String[oldIstrNum, oldMicrNum];
                            for (int i = 0; i < oldIstrNum; i++)
                            {
                                for (int j = 0; j < oldMicrNum; j++)
                                {
                                    tempArray[i, j] = ValArray1[i, j];
                                }
                            }
                        }

                        // Aggiorna le dimensioni in base alle nuove impostazioni
                        this.istrnum = set.istrnm;
                        this.micrnum = set.micronm;

                        maxnumistr = (Properties.Settings.Default.Formato.Equals("Hex")) ? tohex(istrnum - 1).Length : tobin(istrnum - 1).Length;
                        maxnummic = (Properties.Settings.Default.Formato.Equals("Hex")) ? tohex(istrnum - 1).Length : tobin(micrnum - 1).Length;

                        comboBox1.Items.Clear();
                        comboBox2.Items.Clear();

                        for (int i = 0; i < istrnum; i++)
                        {
                            String istrName = (Properties.Settings.Default.Formato.Equals("Hex")) ? format(tohex(i), maxnumistr) : format(tobin(i), maxnumistr);
                            comboBox1.Items.Add(new istruction(istrName));
                            comboBox2.Items.Add(new istruction(istrName));
                        }

                        // Crea un nuovo array con le nuove dimensioni
                        String[,] newArray = new String[istrnum, micrnum];

                        // Crea un array temporaneo per i nuovi dati importati
                        String[,] importedArray = new String[istrnum, micrnum];

                        // Pulisci i controlli del pannello
                        foreach (Control ct in panel1.Controls)
                        {
                            panel1.Controls.Clear();
                        }


                        createedittext();
                        decomposeToArray64(text, importedArray);


                        if (tempArray != null)
                        {
                            int minIstrNum = Math.Min(oldIstrNum, istrnum);
                            int minMicrNum = Math.Min(oldMicrNum, micrnum);

                            for (int i = 0; i < minIstrNum; i++)
                            {
                                for (int j = 0; j < minMicrNum; j++)
                                {
                                    newArray[i, j] = tempArray[i, j];
                                }
                            }
                        }

                        for (int i = 0; i < istrnum; i++)
                        {
                            for (int j = 0; j < micrnum; j++)
                            {
                                if (importedArray[i, j] != null && importedArray[i, j] != "")
                                {
                                    if (newArray[i, j] == null || newArray[i, j] == "")
                                    {
                                        newArray[i, j] = importedArray[i, j];
                                    }
                                    else
                                    {
                                        newArray[i, j] = MergeValues(newArray[i, j], importedArray[i, j]);
                                    }
                                }
                            }
                        }

                        ValArray1 = newArray;

                        if (comboBox1.Items.Count > 0)
                        {
                            comboBox1.SelectedIndex = 0;
                        }

                        attivo64 = true;
                        attivo32 = false;

                        MessageBox.Show("Importazione e unione completate con successo.", "Fatto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
            }
        }

        private void decomposeToArray(String text, String[,] targetArray)
        {
            List<String> instr = new List<string>();
            string[] instructions = text.Split(null);
            foreach (String instruction in instructions)
            {
                String instructioncor = instruction.Replace(" ", "");
                System.Diagnostics.Debug.WriteLine(instructioncor);
                if (instructioncor != "")
                {
                    instr.Add(instructioncor);
                }
            }

            int listcont = 0;
            for (int i = 0; i < istrnum; i++)
            {
                for (int j = 0; j < micrnum; j++)
                {
                    try
                    {
                        if (listcont < instr.Count)
                        {
                            if (instr[listcont].Contains("*0"))
                            {
                                int jump = int.Parse(instr[listcont].Replace("*0", ""));
                                j = j + jump - 1;
                            }
                            else
                            {
                                string originalNumbers = tonumbers(instr[listcont]);
                                string[] numberArray = originalNumbers.Split(null);

                                string adjustedNumbers = "";
                                foreach (string numStr in numberArray)
                                {
                                    if (!string.IsNullOrWhiteSpace(numStr))
                                    {
                                        int num = int.Parse(numStr.Trim());
                                        adjustedNumbers += " " + (num);
                                    }
                                }

                                targetArray[i, j] = adjustedNumbers.Trim();
                            }
                            listcont++;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Errore gestito: " + e.Message);
                    }
                }
            }
        }

        private void decomposeToArray64(String text, String[,] targetArray)
        {
            List<String> instr = new List<string>();
            string[] instructions = text.Split(null);
            foreach (String instruction in instructions)
            {
                String instructioncor = instruction.Replace(" ", "");
                System.Diagnostics.Debug.WriteLine(instructioncor);
                if (instructioncor != "")
                {
                    instr.Add(instructioncor);
                }
            }

            int listcont = 0;
            for (int i = 0; i < istrnum; i++)
            {
                for (int j = 0; j < micrnum; j++)
                {
                    try
                    {
                        if (listcont < instr.Count)
                        {
                            if (instr[listcont].Contains("*0"))
                            {
                                int jump = int.Parse(instr[listcont].Replace("*0", ""));
                                j = j + jump - 1;
                            }
                            else
                            {
                                string originalNumbers = tonumbers(instr[listcont]);
                                string[] numberArray = originalNumbers.Split(null);

                                string adjustedNumbers = "";
                                foreach (string numStr in numberArray)
                                {
                                    if (!string.IsNullOrWhiteSpace(numStr))
                                    {
                                        int num = int.Parse(numStr.Trim()) + 32;
                                        adjustedNumbers += " " + (num);
                                    }
                                }

                                targetArray[i, j] = adjustedNumbers.Trim();
                            }
                            listcont++;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Errore gestito: " + e.Message);
                    }
                }
            }
        }

        private string MergeValues(string existingValue, string newValue)
        {
            if (string.IsNullOrEmpty(existingValue))
                return newValue;

            if (string.IsNullOrEmpty(newValue))
                return existingValue;

            HashSet<int> numbers = new HashSet<int>();

            string[] existingNumbers = existingValue.Split(null);
            foreach (string numStr in existingNumbers)
            {
                if (!string.IsNullOrWhiteSpace(numStr))
                {
                    numbers.Add(int.Parse(numStr.Trim()));
                }
            }

            string[] newNumbers = newValue.Split(null);
            foreach (string numStr in newNumbers)
            {
                if (!string.IsNullOrWhiteSpace(numStr))
                {
                    numbers.Add(int.Parse(numStr.Trim()));
                }
            }

            return string.Join(" ", numbers);
        }


        private void button5_Click(object sender, EventArgs e)
        {
            System.Console.WriteLine("Sostituisco le istruzioni di " + comboBox2.SelectedIndex + " con " + comboBox1.SelectedIndex + " micrnum " + micrnum);
            int toreplace = comboBox2.SelectedIndex;
            int tobereplace = comboBox1.SelectedIndex;

            for (int i = 0; i < micrnum; i++)
            {
                var temp = ValArray1[toreplace, i];
                ValArray1[toreplace, i] = ValArray1[tobereplace, i];
                ValArray1[tobereplace, i] = temp;
            }

            comboBox1.SelectedIndex = toreplace;
            comboBox1.SelectedIndex = tobereplace;

            MessageBox.Show("Sostituzione completata.", "Fatto", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (attivo32 == true)
            {
                MessageBox.Show("Non puoi esportare numeri minore di 32 con esporta64,usa esporta", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string text = File.ReadAllText(openFileDialog1.FileName);
                text = text.Replace("v2.0 raw", "");
                text = text.Replace("\r", " ");
                text = text.Replace("\n", " ");

                String[,] MergeArray = new String[istrnum, micrnum];

                List<String> instr = new List<string>();
                string[] instructions = text.Split(null);
                foreach (String instruction in instructions)
                {
                    String instructioncor = instruction.Replace(" ", "");
                    System.Diagnostics.Debug.WriteLine(instructioncor);
                    if (instructioncor != "")
                    {
                        instr.Add(instructioncor);
                    }
                }

                int listcont = 0;
                for (int i = 0; i < istrnum; i++)
                {
                    for (int j = 0; j < micrnum; j++)
                    {
                        try
                        {
                            if (listcont < instr.Count)
                            {
                                if (instr[listcont].Contains("*0"))
                                {
                                    int jump = int.Parse(instr[listcont].Replace("*0", ""));
                                    j = j + jump - 1;
                                }
                                else
                                {
                                    MergeArray[i, j] = tonumbers(instr[listcont]);
                                }
                                listcont++;
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Errore gestito");
                        }
                    }
                }

                for (int i = 0; i < istrnum; i++)
                {
                    for (int j = 0; j < micrnum; j++)
                    {
                        if (ValArray1[i, j] == null)
                        {
                            ValArray1[i, j] = MergeArray[i, j];
                        }
                    }
                }

                MessageBox.Show("Unione completata con successo.", "Fatto", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        

        public String convertnumber64(String origin)
        {

            // Divide la stringa di input in singole parole usando Split(null)
            string[] numbs = origin.Split(null);

            // Stringa vuota per memorizzare la rappresentazione binaria
            string ret = null;

            // Lista per memorizzare i numeri formattati (interi)
            List<int> formattednumbers = new List<int>();

            // Ciclo per iterare sulle singole parole (numeri)
            foreach (String num in numbs)
            {
                // Rimuove gli spazi iniziali e finali dal numero
                String numzer = num.Replace(" ", "");

                // Controlla se la stringa del numero è vuota o null
                if (numzer != "" && numzer != null)
                {
                    if(int.Parse(numzer) > 32)
                    {
                        formattednumbers.Add(int.Parse(numzer) - 32);
                    }
                    
                }
            }

            // Ordina la lista di numeri in ordine decrescente
            formattednumbers = formattednumbers.OrderByDescending(p => p).ToList();

            // Individua il valore massimo nella lista ordinata
            int maxval = formattednumbers[0];

            // Stringa per memorizzare la rappresentazione binaria
            ret = "";

            // Ciclo per iterare su tutti i valori possibili (da maxval a 0)
            for (int i = maxval; i >= 0; i--)
            {
                // Controlla se il valore corrente è presente nella lista ordinata
                if (formattednumbers.Contains(i))
                {
                    // Aggiunge "1" alla stringa binaria se il valore è presente
                    ret = ret + "1";
                }
                else
                {
                    // Aggiunge "0" alla stringa binaria se il valore è assente
                    ret = ret + "0";
                }
            }

            String hex = Convert.ToInt32(ret, 2).ToString("X");

           
            return hex;
        }
        

        private void button7_Click(object sender, EventArgs e)
        {
            preferences p = new preferences();
            p.ShowDialog();
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
                if (instructioncor != "")
                {
                    instr.Add(instructioncor);
                }
            }

            int listcont = 0;
            for (int i = 0; i < istrnum; i++)
            {
                for (int j = 0; j < micrnum; j++)
                {
                    try
                    {
                        if (listcont < instr.Count)
                        {
                            if (instr[listcont].Contains("*0"))
                            {
                                int jump = int.Parse(instr[listcont].Replace("*0", ""));
                                j = j + jump - 1;
                            }
                            else
                            {
                                ValArray1[i, j] = tonumbers(instr[listcont]);
                            }
                            listcont++;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Errore gestito");
                    }
                }
            }
        }
        public void createedittext64()
        {
            for (int i = 0; i < micrnum; i++)
            {
                TextBox box = new TextBox();
                Label lb = new Label();
                box.Tag = i;
                box.Name = "Box" + i;
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

        public void decompose64(String text)
        {
            List<String> instr = new List<string>();
            string[] instructions = text.Split(null);
            foreach (String instruction in instructions)
            {
                String instructioncor = instruction.Replace(" ", "");
                System.Diagnostics.Debug.WriteLine(instructioncor);
                if (instructioncor != "")
                {
                    instr.Add(instructioncor);
                }
            }

            int listcont = 0;
            for (int i = 0; i < istrnum; i++)
            {
                for (int j = 0; j < micrnum; j++)
                {
                    try
                    {
                        if (listcont < instr.Count)
                        {
                            if (instr[listcont].Contains("*0"))
                            {
                                int jump = int.Parse(instr[listcont].Replace("*0", ""));
                                j = j + jump - 1;
                            }
                            else
                            {

                                string originalNumbers = tonumbers(instr[listcont]);


                                string[] numberArray = originalNumbers.Split(null);

                                string adjustedNumbers = "";
                                foreach (string numStr in numberArray)
                                {
                                    if (!string.IsNullOrWhiteSpace(numStr))
                                    {

                                        int num = int.Parse(numStr.Trim());
                                        if (num >= 32)
                                        {
                                            break;              // Add an error message if numbersis greater than 32
                                        }
                                        adjustedNumbers += " " + (num + 32);
                                    }
                                }

                                ValArray1[i, j] = adjustedNumbers.Trim();
                            }
                            listcont++;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Errore gestito: " + e.Message);
                    }
                }
            }

            // Update the UI by triggering the combobox selection
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }
    }
}
 

