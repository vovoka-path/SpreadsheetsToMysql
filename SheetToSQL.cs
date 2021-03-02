using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace SpreadsheetsToMysql
{
    class SheetToSQL
    {
        public static string nameNewTable = "customers10";
        public static int amountRow = 0;


        // Get new empty structured sqlTable.
        public static DataTable CreateTable(string createTableString)
        {
            DB db = new DB();
            db.OpenConnection();

            string sql = $@"CREATE TABLE `{nameNewTable}` (
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

            MySqlCommand command = new MySqlCommand(sql); // Create a Command.

            command.Connection = DB.myConnect; // Set connection for command.

            // Execute SQL request: create table SQL.
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                Environment.Exit(0);
            }

            MySqlDataAdapter adapter = new MySqlDataAdapter();
            DataSet ds = new DataSet();
            DataTable emptyTable = new DataTable();

            // Get names column from new table SQL.
            sql = $"SELECT * FROM `{nameNewTable}`";
            command.CommandText = sql;
            adapter.SelectCommand = command;
            adapter.Fill(ds);
            adapter.Fill(emptyTable);

            db.CloseConnection();

            return emptyTable;
        }


        public static DataTable ValuesToTable(IList<IList<Object>> values)
        {
            // Get and checking myConnectionString.
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

            // rowSheet -> rowSQL
            foreach (var rowSheet in values)
            {
                rowSQL = tableSQL.NewRow(); // Add new empty row.

                // ** Get cells as is **

                rowSQL["status"] = $"{rowSheet[0]}"; // Column 0
                rowSQL["photographer"] = rowSheet[1].ToString().ToUpper(); // Column 1
                rowSQL["package1"] = rowSheet[2].ToString().ToUpper(); // Column 2
                rowSQL["package2"] = rowSheet[3].ToString().ToUpper(); // Column 3
                rowSQL["name_client"] = rowSheet[5]; // Column 5
                rowSQL["itinerary"] = rowSheet[11]; // Column 11
                rowSQL["year"] = rowSheet[12]; // Column 12
                rowSQL["month"] = rowSheet[13]; // Column 13
                rowSQL["city"] = rowSheet[15]; // Column 15

                // ** Get and change cells only if exists **

                // Column 4: Formatting [shoot_date] 
                // to YYYY-MM-DD (use columns: 4, 12, 13).
                if ((string)rowSheet[4] != "")
                {
                    day = String.Format("{0:d2}", Convert.ToInt32(rowSheet[4].ToString()));
                    month = String.Format("{0:d2}", Convert.ToInt32(rowSheet[13].ToString()));
                    year = $"{rowSheet[12]}";

                    rowSQL["shoot_date"] = $"{year}-{month}-{day}";
                }

                // Column 6: Encrypt [email].
                if ($"{rowSheet[6]}" != "")
                    rowSQL["email"] = AesCrypt.EncryptStringToBytes_Aes($"{rowSheet[6]}", aesKey, aesIV); 

                // Column 7: Formatting and encrypt [phone] number.
                if ((string)rowSheet[7] != "")
                {
                    phone = "+" + String.Format("{0:+###########}", rowSheet[7]).Replace("+", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");

                    if (phone.IndexOf("8") == 1)
                    {
                        phone = phone.Remove(1, 1).Insert(1, "7"); // The fastest way ~ 500-550 mcs.
                    }

                    rowSQL["phone"] = AesCrypt.EncryptStringToBytes_Aes(phone, aesKey, aesIV);
                }

                // Column 8: Encrypt [message].
                if ($"{rowSheet[8]}" != "")
                    rowSQL["message"] = AesCrypt.EncryptStringToBytes_Aes($"{rowSheet[8]}", aesKey, aesIV);

                // Column 9: Check [request_date].
                requestDate = $"{rowSheet[9]}";

                if (requestDate != "" & requestDate.IndexOf("2") == 0) // If 'Y0YY.MM.DD'
                    rowSQL["request_date"] = rowSheet[9];

                //  Column 10: Encrypt [remark].
                if ($"{(string)rowSheet[10]}" != "")
                    rowSQL["remark"] = AesCrypt.EncryptStringToBytes_Aes($"{rowSheet[10]}", aesKey, aesIV);

                // Column 14: Formatting and Encrypt [instagram].
                if ((string)rowSheet[14] != "")
                {
                    // Getting only the login:
                    // - Delete link.
                    insta = String.Format("{0}", rowSheet[14]).Replace("https://www.instagram.com/", "").Replace("https://instagram.com/", "").Replace("/", "");
                    // - Delete '@' if as instagram username.
                    insta = insta.Replace("@", "");
                    // - Delete UTM tag if exist.
                    if (insta.IndexOf("?") > 1) 
                        insta = insta.Remove(insta.IndexOf("?"));

                    rowSQL["instagram"] = AesCrypt.EncryptStringToBytes_Aes(insta, aesKey, aesIV);
                }

                rowSQL["vector"] = aesIV;
                
                // Add filled new row to table.
                tableSQL.Rows.Add(rowSQL);

                // For checking data transfer.
                amountRow++;
            }

            return tableSQL;
        }


        public static void Write(DataTable tableSQL)
        {
            DB db = new DB();
            db.OpenConnection();

            string sql = $"SELECT * FROM `{nameNewTable}`";
            MySqlCommand command = new MySqlCommand(sql);

            command.Connection = DB.myConnect;
            command.ExecuteNonQuery();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            command.CommandText = sql;
            adapter.SelectCommand = command;

            MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);

            // Write all changes to mySQL.
            adapter.Update(tableSQL);

            db.CloseConnection();
        }

    }
}
