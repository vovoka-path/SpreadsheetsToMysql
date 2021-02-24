using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static SpreadsheetsToMysql.DB;

namespace SpreadsheetsToMysql
{
    class SheetToSQL
    {
        public static string newtableSQL = "customers10";


        public static DataTable CreateTable(string createTableString) // get new empty structured sqlTable
        {
            DB db = new DB();
            db.OpenConnection();

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
            DataTable emptyTable = new DataTable();

            // Get names column from new table SQL
            sql = $"SELECT * FROM `{newtableSQL}`";
            command.CommandText = sql;
            adapter.SelectCommand = command;
            //command.ExecuteNonQuery(); // execute SQL request
            adapter.Fill(ds);
            adapter.Fill(emptyTable);

            db.CloseConnection();

            return emptyTable; // empty DB
        }

        public static DataTable ValuesToTable(IList<IList<Object>> values)
        {
            // Get and checking myConnectionString
            string myConnectionString = DB.AccessStringFromFile();

            DataTable tableSQL = CreateTable(myConnectionString);
            DataRow rowSQL;

            string password = "password";
            byte[] aesKey = AesCrypt.GetAesKey(password);
            byte[] aesIV = AesCrypt.GetAesIV();

            string day = "";
            string month = "";
            string year = "";
            string insta = "";
            string phone = "";
            string requestDate = "";
            int amountRow = 0;


            foreach (var rowSheet in values) // rowSheet -> rowSQL
            {
                rowSQL = tableSQL.NewRow(); // add empty row to fill

                // - Get cells as is -

                rowSQL["status"] = $"{rowSheet[0]}"; // column 
                rowSQL["photographer"] = rowSheet[1].ToString().ToUpper(); // column 1
                rowSQL["package1"] = rowSheet[2].ToString().ToUpper(); // column 2
                rowSQL["package2"] = rowSheet[3].ToString().ToUpper(); // column 3
                rowSQL["name_client"] = rowSheet[5]; // column 5
                rowSQL["itinerary"] = rowSheet[11]; // column 11
                rowSQL["year"] = rowSheet[12]; // column 12
                rowSQL["month"] = rowSheet[13]; // column 13
                rowSQL["city"] = rowSheet[15]; // column 15

                // - Get and change cells only if exists -

                // column 4: Formatting [shoot_date] to YYYY-MM-DD (use column 4, 12, 13)
                if ((string)rowSheet[4] != "")
                {
                    day = String.Format("{0:d2}", Convert.ToInt32(rowSheet[4].ToString()));
                    month = String.Format("{0:d2}", Convert.ToInt32(rowSheet[13].ToString()));
                    year = $"{rowSheet[12]}";

                    rowSQL["shoot_date"] = $"{year}-{month}-{day}";
                }

                // column 6: Encrypt [email]
                if ($"{rowSheet[6]}" != "")
                    rowSQL["email"] = AesCrypt.EncryptStringToBytes_Aes($"{rowSheet[6]}", aesKey, aesIV); 

                // column 7: Formatting and encrypt [phone] number
                if ((string)rowSheet[7] != "")
                {
                    phone = "+" + String.Format("{0:+###########}", rowSheet[7]).Replace("+", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");

                    if (phone.IndexOf("8") == 1)
                    {
                        phone = phone.Remove(1, 1).Insert(1, "7"); // the fastest ~ 500-550 mcs
                    }

                    rowSQL["phone"] = AesCrypt.EncryptStringToBytes_Aes(phone, aesKey, aesIV);
                }

                // column 8: Encrypt [message]
                if ($"{rowSheet[8]}" != "")
                    rowSQL["message"] = AesCrypt.EncryptStringToBytes_Aes($"{rowSheet[8]}", aesKey, aesIV);

                // column 9: Check [request_date]
                requestDate = $"{rowSheet[9]}";

                if (requestDate != "" & requestDate.IndexOf("2") == 0) // and if Y<0>YY.MM.DD for exclude wrong data
                    rowSQL["request_date"] = rowSheet[9];

                //  column 10: Encrypt [remark]
                if ($"{(string)rowSheet[10]}" != "")
                    rowSQL["remark"] = AesCrypt.EncryptStringToBytes_Aes($"{rowSheet[10]}", aesKey, aesIV);

                // column 14: Formatting and Encrypt [instagram]
                if ((string)rowSheet[14] != "")
                {
                    // Getting only the login
                    insta = String.Format("{0}", rowSheet[14]).Replace("https://www.instagram.com/", "").Replace("https://instagram.com/", "").Replace("/", ""); // del link
                    insta = insta.Replace("@", ""); // del '@' if as username in instagram

                    if (insta.IndexOf("?") > 1) // '?utm...' exist? 
                        insta = insta.Remove(insta.IndexOf("?")); // del "?utm"

                    rowSQL["instagram"] = AesCrypt.EncryptStringToBytes_Aes(insta, aesKey, aesIV);
                }

                rowSQL["vector"] = aesIV;

                tableSQL.Rows.Add(rowSQL); // add filled row to table
                amountRow++; // for checking complete data transfer
            }

            return tableSQL;
        }

        public static void Write(DataTable tableSQL)
        {
            DB db = new DB();
            db.OpenConnection();

            


            string sql = $"SELECT * FROM `{newtableSQL}`";
            MySqlCommand command = new MySqlCommand(sql); // Create a Command

            command.Connection = DB.myConnect; // Set connection for command.
            command.ExecuteNonQuery(); // Execute SQL request: create table SQL

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            command.CommandText = sql;
            adapter.SelectCommand = command;

            MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);

            adapter.Update(tableSQL); // write all changes to mySQL

            db.CloseConnection();
        }
    }
}
