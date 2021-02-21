using MySql.Data.MySqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
//using SpreadsheetsToMysql.MainWindow.xaml;

namespace SpreadsheetsToMysql
{
    class DB
    {
        public MySqlConnection myConnect = new MySqlConnection();

        public  void OpenConnection(string myConnectionString)
        {
            MainWindow mainForm = new MainWindow();
            mainForm.processStatus.Text = $@"Connection to mySQL: processing now";

            if (myConnect.State == System.Data.ConnectionState.Closed)
            {
                myConnect.ConnectionString = myConnectionString;

                try
                {
                    myConnect.Open();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
        
        public void CloseConnection()
        {
            if (myConnect.State == System.Data.ConnectionState.Open)
                myConnect.Close();
        }
    }
}
