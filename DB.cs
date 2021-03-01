using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
//using SpreadsheetsToMysql.MainWindow.xaml;

namespace SpreadsheetsToMysql
{
    class DB
    {

        public static MySqlConnection myConnect = new MySqlConnection();
        public static string accessConnectionString;


        public void OpenConnection()
        {
            accessConnectionString = AccessStringFromFile();

            if (myConnect.State == System.Data.ConnectionState.Closed)
            {
                myConnect.ConnectionString = accessConnectionString;

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


        public static string AccessStringFromFile()
        {
            string path = @"accessSQL.txt"; // parameter in

            if (!File.Exists(path))
            {
                MessageBox.Show("accessSQL.txt File in the application folder was not found! " +
                    "File must contain sql connection string to your mySQL Databaase. " +
                    "Required format: Username=<yourusername>;Database=<yournamebase>;" +
                    "Password=<yourpassword>;Server=<yourserver>");
            }
            else
            {
                string[] readAccessSQL = File.ReadAllLines(path);

                accessConnectionString = readAccessSQL[0];

                if (accessConnectionString == "")
                {
                    MessageBox.Show("File accessSQL.txt was found but empty!");
                }
                else
                {
                    CheckConnectionString(accessConnectionString);
                }
            }

            return accessConnectionString;
        }


        public static void CheckConnectionString(string myConnectionString)
        {
            if (myConnectionString.Contains("server=")
                & myConnectionString.Contains("username=")
                & myConnectionString.Contains("password=")
                & myConnectionString.Contains("database="))
            {
                MainWindow mainForm = new MainWindow();

                mainForm.processStatus.Text = $"File accessSQL.txt is OK.";
            }
            else
            {
                MessageBox.Show("File accessSQL.txt found but contains " +
                    "wrong format sql connection string. " +
                    "Correct format: Username=<yourusername>;Database=<yournamebase>;" +
                    "Password=<yourpassword>;Server=<yourserver>");
            }
        }

    }
}
