using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Data;
//using SpreadsheetsToMysql.SheetToSQL;
//using SpreadsheetsToMysql.CreateGoogleSheetsAPIservice;

namespace SpreadsheetsToMysql
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }


        // Click the big button to transfer.
        private void Button_SpreadsheetstoMySQL_Click(object sender, RoutedEventArgs e)
        {
            // Get data from Google spreadsheet.
            string spreadsheetId = GetIDfromLink(Link.Text);
            IList<IList<Object>> values = GoogleSheetsAPI.GetSheet(spreadsheetId);

            // Convert data for mySQL.
            DataTable tableSQL = SheetToSQL.ValuesToTable(values);

            // Write new table to mySQL DB.
            SheetToSQL.Write(tableSQL);

            processStatus.Text = 
                $@"> Finished! {SheetToSQL.amountRow} rows transfered to table '{SheetToSQL.nameNewTable}' to your mySQL!";
        }


        private string GetIDfromLink(string link)
        {
            string[] words = link.Split(new char[] { '/' });
            string id = "";

            // Get ID in link after 5th '/'.
            if (words.Length < 6)
            {
                MessageBox.Show("Enter the correct link to the spreadsheets!");
            }
            else
            {
                id = words[5];
                // id like "1ctHfDIc_yYcT4BBUzTTGJDMM5WHWJh-lFij9cSJNcHg"
            }

            return id;
        }

       
        // Debugging.
        private void Button_ConnectMySQL_Click(object sender, RoutedEventArgs e) // del
        {
            //-----------------------------------------
            /*string spreadsheetId = "1-Wx7IoOCkWj051EaCAV44j4d_Ibl5usYUO8d9iL2Z80";
            IList<IList<Object>> values = GoogleSheetsAPI.GetSheet(spreadsheetId);

            sqlStatus.Text = $"values.Count (обнаружено кол-во строк для экспорта) =  + {values.Count}";

            DataTable tableSQL = SheetToSQL.ValuesToTable(values);

            SheetToSQL.Write(tableSQL);
            processStatus.Text = $@"tableSQL.Row[0] : {tableSQL.Rows[125][12]};  {tableSQL.Rows[125][13]}; {tableSQL.Rows[125][2]}; {tableSQL.Rows[125][3]};";*/
            //-----------------------------------------
            //myConnectionString = SqlStringFromFile(path);


            /*DataTable table = SheetToSQL.CreateTable(myConnectionString);

            sqlStatus.Text = $"{table} = {table.Columns}";*/


            // ----------------------------------------
            /*byte[] key = AesCrypt.GetAesIV(); // need check on ""

            Encoding utf8 = Encoding.GetEncoding("windows-1251");

            string qq = utf8.GetString(key);
            sqlStatus.Text = $"{qq} = {qq.Length}";*/

            // ----------------------------------------
            /* if (File.Exists("credentials.json"))
             {
                 var service = CreateGoogleSheetsAPIservice.GetService();
                 processStatus.Text = $@"var service : {service}";
             }
             else
             {
                 processStatus.Text = $@"File credentials.json not found!";
             }*/

            // ---------------------------------------
            /*ConnecttoMySQL.Content = "Connection now";
            processStatus.Text = $@"Connection to mySQL: processing now";

            db.OpenConnection(myConnectionString);

            processStatus.Text = $@"Connection to mySQL: {db.myConnect.State}";*/
        }

    }
}
