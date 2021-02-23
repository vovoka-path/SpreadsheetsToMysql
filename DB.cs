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

        public void OpenConnection(string myConnectionString)
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

        /*public static void Write(DataTable tableSQL)
        {
            
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataSet ds = new DataSet();
            DataTable table = new DataTable();
            string sql = $"SELECT * FROM `{SheetToSQL.newtableSQL}`";
            MySqlCommand command = new MySqlCommand(sql);
            command.Connection = myConnect; // Set connection for command.
            command.ExecuteNonQuery(); // execute SQL request: create table SQL

            // Get names column from new table SQL
            command.CommandText = sql;
            adapter.SelectCommand = command;
            //command.ExecuteNonQuery(); // execute SQL request
            adapter.Fill(ds);
            adapter.Fill(table);
            MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);

            adapter.Update(tableSQL); // write all changes to mySQL

            //-----------
            MySqlCommand command = new MySqlCommand(sql); // Create a Command

            command.Connection = DB.myConnect; // Set connection for command.
            command.ExecuteNonQuery(); // execute SQL request: create table SQL

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataSet ds = new DataSet();
            DataTable table = new DataTable();

            // Get names column from new table SQL
            sql = $"SELECT * FROM `{newtableSQL}`";
            command.CommandText = sql;
            adapter.SelectCommand = command;
            //command.ExecuteNonQuery(); // execute SQL request
            adapter.Fill(ds);
            adapter.Fill(tableSQL);

            db.CloseConnection();
        }*/
    }
}
