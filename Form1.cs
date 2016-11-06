using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        List<Employee> employees = new List<Employee>();
        DataTable xDataTable = new DataTable();
        OpenFileDialog fl = new OpenFileDialog();
        public int month;
        public int year;
        public Form1()
        {
            InitializeComponent();
        }

        public void TryThis(string fileName)
        {
            // luodaan oStreamReader. millä luetaan tiedostoa.
            StreamReader oStreamReader = new StreamReader(File.OpenRead(fileName));
            DataTable oDataTable = null;
            int RowCount = 0;
            string[] ColumnNames = null;
            string[] oStreamDataValues = null;

            // käytetään while looppia tietojen lukemiseen kunnes loppu tulee vastaan.
            while (!oStreamReader.EndOfStream)
            {
                String oStreamRowData = oStreamReader.ReadLine().Trim();
                if (oStreamRowData.Length > 0)
                {
                    oStreamDataValues = oStreamRowData.Split(',');

                    // Koska eka rivi sisältää vaan kolumnien nimet se luetaan vaan kerran.
                    // If lause tulee vain kerran olemaan totta.
                    if (RowCount == 0)
                    {
                        RowCount = 1;
                        ColumnNames = oStreamRowData.Split(',');
                        oDataTable = new DataTable();


                        // käytetään foreach:ia looppaamaan kaikki kolumnien nimet
                        foreach (string csvcolumn in ColumnNames)
                        {
                            DataColumn oDataColumn = new DataColumn(csvcolumn.ToUpper(), typeof(string));
                            // asetetaan vakioarvo uudelle kolumnille
                            oDataColumn.DefaultValue = string.Empty;
                            // lisätään kolumni taulukkoon
                            oDataTable.Columns.Add(oDataColumn);
                        }
                    }
                    else
                    {   
                        // luodaan uusi datarivi oDataTableen.     
                        DataRow oDataRow = oDataTable.NewRow();
                       
                        // käydään kolumnien nime läpi for loopilla.
                        for (int i = 0; i < ColumnNames.Length; i++)
                        {
                            oDataRow[ColumnNames[i]] = oStreamDataValues[i] == null ? string.Empty : oStreamDataValues[i].ToString();

                            if (i == 1) // varsinaisesti ei merkitystä onko 0 vai 1 tms. Olennaista on, että tekee vain kerran rivin aikana.
                            {    
                                // jos samalla id:llä olevaa työntekijää ei ole niin luodaan sellainen.    
                                if (!employees.Any(x => x.personId == Convert.ToInt32(oStreamDataValues[1])))             
                                {
                                    // otetaan ensimmäiseltä datariviltä talteen kuukausi ja vuosi.
                                    if (month == 0)
                                    {
                                        string traverse = oStreamDataValues[2].ToString();
                                        string lastYear = traverse;
                                        lastYear = lastYear.Substring(lastYear.Length - 4, 4);
                                        traverse = traverse.Remove(traverse.Length - 5, 5);
                                        traverse = traverse.Substring(traverse.Length - 1, 1);
                                        year = Convert.ToInt32(lastYear);
                                        month = Convert.ToInt32(traverse);
                                        textMonth.Text = ("Monthly wages for "+ month + "/"+year);   
                                    }

                                    // lisätään työntekijä
                                     employees.Add(new Employee(oStreamDataValues[0].ToString(), Convert.ToInt32(oStreamDataValues[1])));                             
                                      Console.WriteLine("Employee created");
                                    textBox1.Text = (employees.Count.ToString() + " employees found");
                                    string startTime = oStreamDataValues[3].ToString();
                                    string endTime = oStreamDataValues[4].ToString();
                                   double startDouble= convertTimeToDouble(startTime);
                                   double endDouble = convertTimeToDouble(endTime);
                                    // katsotaan mikä id vastaa listan järjestystä
                                    var zx = employees.FindIndex(y => y.personId == Convert.ToInt32(oStreamDataValues[1]));
                                    employees[zx].addHours(startDouble, endDouble);
                                                                        
                                }
                                else
                                {
                                    // lisätään olemassa olevaan työntekijään arvoja
                                    string startTime = oStreamDataValues[3].ToString();
                                    string endTime = oStreamDataValues[4].ToString();
                                    double startDouble = convertTimeToDouble(startTime);
                                    double endDouble = convertTimeToDouble(endTime);
                                    // katsotaan mikä id vastaa listan järjestystä
                                    var zx = employees.FindIndex(y => y.personId == Convert.ToInt32(oStreamDataValues[1]));
                                    employees[zx].addHours(startDouble, endDouble);
                                }         
                             }                           
                        }
                        //Lisätään rivi taulukkoon
                        //adding the newly created row with data to the oDataTable       
                        oDataTable.Rows.Add(oDataRow);
                    }
                }
            }
            // lasketaan lopuksi kokonaispalkat
            for (int i = 0; i < employees.Count; i++)
            {
                employees[i].calculateTotal();
            }
            btnCalc.Enabled = true;
            // suljetaan oStreamReader olio
            oStreamReader.Close();
            // vapautetaan olion käyttämät resurssit
            oStreamReader.Dispose();

            // looppaa kaikki rivit datatablessa
            foreach (DataRow oDataRow in oDataTable.Rows)
            {
                string RowValues = string.Empty;

                // looppaa kaikki columnit riveissä
                foreach (string csvcolumn in ColumnNames)
                {
                    //ketjuttaa arvot esittelykäyttöön
                    RowValues += csvcolumn + "=" + oDataRow[csvcolumn].ToString() + ";  ";
                }
                //näyttää lopputuloksen konsolissa.
                Console.WriteLine(RowValues);
            }
            // päivittää datata
            dataGridView1.DataSource = oDataTable;
        }
        // rakentaa uuden DataTablen, jossa on employee olioiden tiedot.
        public void updateTable()
        {
            xDataTable.Clear();  
            xDataTable.Columns.Add("Name");
            xDataTable.Columns.Add("ID");
            xDataTable.Columns.Add("Work hours");
            xDataTable.Columns.Add("Basic wage");
            xDataTable.Columns.Add("Overtime compensation");
            xDataTable.Columns.Add("Evening compensation");
            xDataTable.Columns.Add("Total wage");

            // käy loopilla läpi  työntekijät riveinä ja lisää datatableen
            // arvoissa korvaraan , .:llä, jotta ulos viety muoto olisi selkeämpi.
            for (int i = 0; i < employees.Count; i++)
            {
                DataRow xRow = xDataTable.NewRow();    
                xRow["Name"] = employees[i].personName;
                xRow["ID"] = employees[i].personId;
                xRow["Work hours"] = employees[i].workHours.ToString().Replace(@",", @".");
                xRow["Basic wage"] = employees[i].monthlyWage.ToString().Replace(@",", @".");
                xRow["Overtime compensation"] = employees[i].overtimeComp.ToString().Replace(@",", @".");
                xRow["Evening compensation"] = employees[i].eveningComp.ToString().Replace(@",", @".");
                xRow["Total wage"] = employees[i].totalWage.ToString().Replace(@",", @".");
                xDataTable.Rows.Add(xRow);
            }
            // tekee ui muutokset.
            dataGridView1.DataSource = xDataTable;
            textBox1.Text = ("Report ready");
            btnCalc.Enabled = false;
        }
        // tällä viedään datatable csv-muodossa pois
        public void exportTable()
        {
            StringBuilder sb = new StringBuilder();
            IEnumerable<string> columnNames = xDataTable.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));
            foreach (DataRow row in xDataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            DirectoryInfo di = new DirectoryInfo(fl.FileName); 
            String fileName = Path.Combine(di.Parent.FullName, "salary" + month + year + ".csv");
            File.WriteAllText(fileName, sb.ToString());
            textBox1.Text = ("CSV file created");
            btnExport.Enabled = false;

         
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                // Etsitään luettava tiedosto. Huom. jos tiedoston kolumnit eivät ole samat kuin esimerkissä, voi tiedoston avata, muttei 
                // muuttaa halutun laiseksi.
                using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "CSV|*csv", ValidateNames = true, Multiselect = false })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                        textFileLoc.Text = ofd.FileName;
                    fl.FileName = ofd.FileName;

                    TryThis(ofd.FileName);
                    btnOpen.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);     
            }
        }

        // muutetaan aika stringistä doubleksi ja korjataan desimaalit
      public double convertTimeToDouble(string time)
        {
            double timeConverted = 5;
            // korvataan : ,lla
            time = time.Replace(@":", @",");

            // muunnetaan vartit desimaaleiksi.
          if ( time.Substring(2)== "15" || time.Substring(3) == "15")
            {
                time= time.Replace(@"15", @"25");
            }
          else if (time.Substring(2) == "30" || time.Substring(3) == "30")
            {
                time = time.Replace(@"30", @"50");
        }
          else if (time.Substring(2) == "45" || time.Substring(3) == "45")
            {
                time = time.Replace(@"45", @"75");            
            }

          // muunetaan doubleksi
            timeConverted = double.Parse(time);
            // palautetaan arvo
            return timeConverted;
                    }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            updateTable();
            btnExport.Enabled = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            exportTable();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}