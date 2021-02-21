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
//using SpreadsheetsToMysql.CreateGoogleSheetsAPIservice;

namespace SpreadsheetsToMysql
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //ublic static MainWindow form1; // переменная, которая будет содержать ссылку на форму MainWindow
        internal DB db = new DB();
        private string myConnectionString;
        //DB Db { get => db; set => db = value; }

        public MainWindow()
        {
            InitializeComponent();

            string path = @"accessSQL.txt";

            myConnectionString = SqlStringFromFile(path);
            CheckConnectionString(myConnectionString);

            // --------- in worked --------
            /*private ValuesFromSheet(string nameSheet, string range)
            {
                GetCredential.main() // get: var service

                return IList < IList < Object >> values;
            }

            private DataTable Create(string sql)
            {
                DB db = new DB();
                return DataTable table; // empty DB
            }

            private ValuesToTable(var values, DataTable table)
            {
                GetAesKeys();
                DB.OpenConnection();
                return DataTable table;
            }*/


        }

        private void Button_ConnectMySQL_Click(object sender, RoutedEventArgs e) // del
        {
            if (File.Exists("credentials.json"))
            {
                var service = CreateGoogleSheetsAPIservice.GetService();
                processStatus.Text = $@"var service : {service}";
            }
            else
            {
                processStatus.Text = $@"File credentials.json not found!";
            }
            /*ConnecttoMySQL.Content = "Connection now";
            processStatus.Text = $@"Connection to mySQL: processing now";

            db.OpenConnection(myConnectionString);

            processStatus.Text = $@"Connection to mySQL: {db.myConnect.State}";*/
        }

        private void Button_SpreadsheetstoMySQL_Click(object sender, RoutedEventArgs e) // click to transfer
        {
            db.CloseConnection();

            processStatus.Text = $@"Connection to mySQL: {db.myConnect.State}";
        }

        private string SqlStringFromFile(string path) // get string from file
        {
            string myConnectionString;

            if (!File.Exists(path))
            {
                myConnectionString = "Username=fJuQo6tTlF;Database=fJuQo6tTlF;Password=vpI95KC9UA;Server=remotemysql.com";
                sqlStatus.Text = "File accessSQL.txt not found!";
            }
            else
            {
                string[] readAccessSQL = File.ReadAllLines(path);
                myConnectionString = readAccessSQL[0];

                if (!(myConnectionString == ""))
                    sqlStatus.Text = "File accessSQL.txt found.";
                else
                    sqlStatus.Text = "File accessSQL.txt found but empty!"; // not correct
            }

            return myConnectionString;
        }

        private void CheckConnectionString(string connectionString) // check string
        {
            if (connectionString.Contains("server=") 
                & connectionString.Contains("username=")
                & connectionString.Contains("password=")
                & connectionString.Contains("database="))
            {
                processStatus.Text = $"File accessSQL.txt is OK.";
            }
            else
                processStatus.Text = $"File accessSQL.txt requires conversion to the correct format!";
        }
    }
}
