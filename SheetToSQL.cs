using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetsToMysql
{
    class SheetToSQL
    {
        private void Main()
        {
        

        }
        public static DataTable CreateTable(string myConnectionString) // get new empty structured DB
        {
            DB db = new DB();
            db.OpenConnection(myConnectionString);

            //MySqlCommand command = new MySqlCommand("SELECT * FROM `clients`"); // Create a Command

            // sql = $"DROP TABLE {newtableSQL}";
            //command.ExecuteNonQuery(); // execute SQL request: delete table SQL

            string newtableSQL = "customers10";
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

            command.Connection = db.myConnect; // Set connection for command.
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
            adapter.Fill(table);

            return table; // empty DB
        }
    }
}
