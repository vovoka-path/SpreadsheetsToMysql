using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace SpreadsheetsToMysql
{
    class SheetToSQL
    {
        //private static string myConnectionString;

        static MainWindow mainForm = new MainWindow();

        public static string newtableSQL = "customers10";

        private void Main()
        {
            /*string path = @"accessSQL.txt";

            myConnectionString = SqlStringFromFile(path);
            CheckConnectionString(myConnectionString);*/
            //password = "password";
        }

        public static DataTable CreateTable(string myConnectionString) // get new empty structured DB
        {
            DB db = new DB();
            db.OpenConnection(myConnectionString);

            //MySqlCommand command = new MySqlCommand("SELECT * FROM `clients`"); // Create a Command

            // sql = $"DROP TABLE {newtableSQL}";
            //command.ExecuteNonQuery(); // execute SQL request: delete table SQL

            
            string sql = $@"CREATE TABLE `{newtableSQL}` (
                             `id` int(11) NOT NULL AUTO_INCREMENT,
                             `request_date` date NOT NULL COMMENT 'Дата заявки',
                             `city` set('Прага','Рим','Венеция','Париж') NOT NULL COMMENT 'Город',
                             `photographer` char(40) NOT NULL COMMENT 'Фотограф',
                             `status` enum('Клиент задал вопрос','Клиент отправил заявку','Передано фотографу','Передано обработчику','Фотографии обработаны','Фотографии отправлены клиенту','Клиент подтвердил получение фотографий','Получен отзыв','Отмена съемки') NOT NULL COMMENT 'Статус заказа',
                             `name_client` char(50) NOT NULL COMMENT 'Имя клиента',
                             `shoot_date` date NOT NULL COMMENT 'Дата съемки (факт.)',
                             `itinerary` char(10) NOT NULL COMMENT 'Маршрут',
                             `package1` char(15) NOT NULL COMMENT 'Пакет',
                             `package2` char(15) DEFAULT NULL COMMENT 'Доп. пакет',
                             `promocode` char(15) DEFAULT NULL COMMENT 'Промокод',
                             `message` blob NOT NULL COMMENT 'Сообщение',
                             `email` blob NOT NULL COMMENT 'Почта',
                             `phone` blob NOT NULL COMMENT 'Телефон',
                             `remark` blob NOT NULL COMMENT 'Примечание',
                             `cause` text NOT NULL COMMENT 'Причина отмены',
                             `review` text NOT NULL COMMENT 'Отзыв клиента',
                             `month` enum('01','02','03','04','05','06','07','08','09','10','11','12') NOT NULL COMMENT 'Месяц (номер ХХ)',
                             `year` year(4) NOT NULL COMMENT 'Год',
                             `instagram` blob NOT NULL COMMENT 'Инстаграм',
                             `time_contact` text NOT NULL COMMENT 'Когда связаться',
                             `vector` blob NOT NULL COMMENT 'aesIV',
                             UNIQUE KEY `1` (`id`)
                            ) ENGINE=MyISAM AUTO_INCREMENT = 0 DEFAULT CHARSET = utf8";

            MySqlCommand command = new MySqlCommand(sql); // Create a Command

            command.Connection = DB.myConnect; // Set connection for command.
            command.ExecuteNonQuery(); // execute SQL request: create table SQL

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataSet ds = new DataSet();
            DataTable tableSQL = new DataTable();

            // Get names column from new table SQL
            sql = $"SELECT * FROM `{newtableSQL}`";
            command.CommandText = sql;
            adapter.SelectCommand = command;
            //command.ExecuteNonQuery(); // execute SQL request
            adapter.Fill(ds);
            adapter.Fill(tableSQL);

            db.CloseConnection();

            return tableSQL; // empty DB
        }

        public static DataTable ValuesToTable(IList<IList<Object>> values)
        {
            string password = "password";
            byte[] aesKey = AesCrypt.GetAesKey(password);
            byte[] aesIV = AesCrypt.GetAesIV();

            string day = "";
            string month = "";
            string year = "";

            string insta = "";
            string phone = "";
            string requestDate = "";

            int amountRow = 0; // check complete data transfer

            string myConnectionString = SqlStringFromFile();
            CheckConnectionString(myConnectionString);

            DataTable tableSQL = CreateTable(myConnectionString);
            DataRow rowSQL;

            foreach (var rowSheet in values) // tableSheet -> tableSQL
            {
                rowSQL = tableSQL.NewRow();

                rowSQL["status"] = $"{rowSheet[0]}"; // column 
                rowSQL["photographer"] = rowSheet[1].ToString().ToUpper(); // column 1
                rowSQL["package1"] = rowSheet[2].ToString().ToUpper(); // column 2
                rowSQL["package2"] = rowSheet[3].ToString().ToUpper(); // column 3
                rowSQL["name_client"] = rowSheet[5]; // column 5
                rowSQL["itinerary"] = rowSheet[11]; // column 11
                rowSQL["year"] = rowSheet[12]; // column 12
                rowSQL["month"] = rowSheet[13]; // column 13
                rowSQL["city"] = rowSheet[15]; // column 15
                //aesIVHEXstr = Convert.ToBase64String(aesIV);
                rowSQL["vector"] = aesIV; // byte[] -> hex aesIV for store in SQL

                // - Get value only if exists:

                // formatting to YYYY-MM-DD
                if ((string)rowSheet[4] != "")
                {
                    day = String.Format("{0:d2}", Convert.ToInt32(rowSheet[4].ToString()));
                    month = String.Format("{0:d2}", Convert.ToInt32(rowSheet[13].ToString()));
                    year = $"{rowSheet[12]}";

                    rowSQL["shoot_date"] = $"{year}-{month}-{day}"; // column 4
                }

                // Encrypt email
                if ($"{rowSheet[6]}" != "")
                    rowSQL["email"] = AesCrypt.EncryptStringToBytes_Aes($"{rowSheet[6]}", aesKey, aesIV); // column 6

                // formatting and Encrypt PHONE number
                if ((string)rowSheet[7] != "")
                {
                    phone = "+" + String.Format("{0:+###########}", rowSheet[7]).Replace("+", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");

                    if (phone.IndexOf("8") == 1)
                    {
                        phone = phone.Remove(1, 1).Insert(1, "7"); // самая быстрая ~ 500-550 mcs
                    }

                    rowSQL["phone"] = AesCrypt.EncryptStringToBytes_Aes(phone, aesKey, aesIV); // column 7
                }

                // encrypt message
                if ($"{rowSheet[8]}" != "")
                    rowSQL["message"] = AesCrypt.EncryptStringToBytes_Aes($"{rowSheet[8]}", aesKey, aesIV); // column 8

                // check request_date
                requestDate = $"{rowSheet[9]}";

                if (requestDate != "" & requestDate.IndexOf("2") == 0) // only 20YY.MM.DD
                    rowSQL["request_date"] = rowSheet[9]; // column 9

                // encrypt remark
                if ($"{(string)rowSheet[10]}" != "")
                    rowSQL["remark"] = AesCrypt.EncryptStringToBytes_Aes($"{rowSheet[10]}", aesKey, aesIV); // column 10

                // formatting and Encrypt Instagram
                if ((string)rowSheet[14] != "")
                {
                    insta = String.Format("{0}", rowSheet[14]).Replace("https://www.instagram.com/", "").Replace("https://instagram.com/", "").Replace("/", "").Replace("@", "");

                    if (insta.IndexOf("?") > 1) // check exist "?utm" 
                        insta = insta.Remove(insta.IndexOf("?")); // delete "?utm"

                    rowSQL["instagram"] = AesCrypt.EncryptStringToBytes_Aes(insta, aesKey, aesIV); // column 14
                }

                tableSQL.Rows.Add(rowSQL); // add new row
                amountRow++;
            }

            return tableSQL;
        }

        public static void Write(DataTable tableSQL)
        {
            DB db = new DB();

            string myConnectionString = SqlStringFromFile();
            CheckConnectionString(myConnectionString);

            db.OpenConnection(myConnectionString);

            string sql = $"SELECT * FROM `{newtableSQL}`";
            MySqlCommand command = new MySqlCommand(sql); // Create a Command

            command.Connection = DB.myConnect; // Set connection for command.
            command.ExecuteNonQuery(); // execute SQL request: create table SQL

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            command.CommandText = sql;
            adapter.SelectCommand = command;

            MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);

            adapter.Update(tableSQL); // write all changes to mySQL

            db.CloseConnection();
        }

            private static string SqlStringFromFile() // get string from file
        {
            string path = @"accessSQL.txt"; // parameter in
            string myConnectionString;

            if (!File.Exists(path))
            {
                myConnectionString = "Username=fJuQo6tTlF;Database=fJuQo6tTlF;Password=vpI95KC9UA;Server=remotemysql.com";
                
                mainForm.sqlStatus.Text = "File accessSQL.txt not found!";
            }
            else
            {
                string[] readAccessSQL = File.ReadAllLines(path);
                myConnectionString = readAccessSQL[0];

                if (!(myConnectionString == ""))
                    mainForm.sqlStatus.Text = "File accessSQL.txt found.";
                else
                    mainForm.sqlStatus.Text = "File accessSQL.txt found but empty!"; // not correct
            }

            return myConnectionString;
        }

        private static void CheckConnectionString(string connectionString) // check string
        {
            if (connectionString.Contains("server=")
                & connectionString.Contains("username=")
                & connectionString.Contains("password=")
                & connectionString.Contains("database="))
            {
                mainForm.processStatus.Text = $"File accessSQL.txt is OK.";
            }
            else
                mainForm.processStatus.Text = $"File accessSQL.txt requires conversion to the correct format!";
        }

        /*private static FormatDate(object day)
        {
           // ----
        }*/
    }
}
