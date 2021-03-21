using System;
using System.ComponentModel;
using System.Data.Odbc;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Тик таймера = 1сек
        int time = 0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            time++;
        }

        // Начать работу
        private void Button1_Click(object sender, EventArgs e)
        {
            string[] bases = new string[] { "pmib6702.pdb1", "pmib6702.pdb2", "pmib6702.pdb3" };
            Random rnd = new Random(); // Создание объекта для генерации чисел
            find_keys(); // Выбираем со всех таблиц все ключи
            timer.Start(); // Запускаем таймер
            for (; ; )
            {
                int command = rnd.Next(1, 4);// Выбираем случайную транзакцию           
                string randbase = bases[rnd.Next(0, 3)]; // Выбираем случайную базу
                int count;

                //string randbase = "pmib6702.pdb1";
                //int command = 1;

                // Выполнение случайной транзакции
                switch (command)
                {
                    // Вставка
                    case 1:
                        Insert(randbase);

                        string str_number = "";
                        string date_time = "";
                        string query_n = "Select n_det, date_oper from " + randbase + " where oid = (select max(oid) from " + randbase + ")";
                        // Выбираю внесенную строку
                        using (OdbcConnection connection = new OdbcConnection("Dsn=PostgreSQL16"))
                        {
                            OdbcCommand cm_n = new OdbcCommand(query_n);
                            cm_n.Connection = connection;
                            connection.Open();
                            OdbcDataReader reader = cm_n.ExecuteReader();

                            while (reader.Read())
                            {
                                str_number = reader.GetString(0);
                                date_time = reader.GetString(1);
                            }
                            reader.Close();
                            // Соединение автоматически закрывается при использовании блока using
                        }
                        if (randbase == "pmib6702.pdb1" || randbase == "pmib6702.pdb2")
                        {
                            // Вставка в DataGridView (журнал)
                            dataGridView1.Rows.Add();
                            count = dataGridView1.RowCount - 1;
                            dataGridView1[0, count].Value = DateTime.Now.ToString();  // Дата и время операции
                            dataGridView1[1, count].Value = "Вставка в " + randbase.Remove(0, 9);
                            dataGridView1[2, count].Value = str_number + " Крепление Новосибирск Вставка в " + randbase.Remove(0, 9);
                        }
                        else
                        {
                            // Вставка в DataGridView (журнал)
                            dataGridView2.Rows.Add();
                            count = dataGridView2.RowCount - 1;
                            dataGridView2[0, count].Value = DateTime.Now.ToString();  // Дата и время операции
                            dataGridView2[1, count].Value = "Вставка в " + randbase.Remove(0, 9);
                            dataGridView2[2, count].Value = str_number + " Крепление Новосибирск Вставка в " + randbase.Remove(0, 9);
                        }

                        content_log(randbase); // Вносим таблицы в журнал содержимого
                        break;

                    // Удаление
                    case 2:
                        string str = "";
                        date_time = "";
                        string query = "Select * from " + randbase + " where oid = (select max(oid) from " + randbase + ")";
                        // Получаем удаляемую строку из таблицы
                        using (OdbcConnection connection = new OdbcConnection("Dsn=PostgreSQL16"))
                        {
                            OdbcCommand cm = new OdbcCommand(query, connection);
                            connection.Open();
                            OdbcDataReader reader = cm.ExecuteReader();

                            while (reader.Read())
                            {
                                str = reader.GetString(0) + "  " + reader.GetString(1) + reader.GetString(2) + reader.GetString(3) + reader.GetString(4);
                                date_time = reader.GetString(4);
                            }
                            reader.Close();
                        }
                        if (randbase == "pmib6702.pdb1" || randbase == "pmib6702.pdb2")
                        {
                            // Вставка в DataGridView (журнал)
                            dataGridView1.Rows.Add();
                            count = dataGridView1.RowCount - 1;
                            dataGridView1[0, count].Value = DateTime.Now.ToString();  // Дата и время операции
                            dataGridView1[1, count].Value = "Удаление из " + randbase.Remove(0, 9);
                            str = Regex.Replace(str, @"\s+", " "); // Удаляю лишние пробелы в строке
                            dataGridView1[2, count].Value = str;
                        }
                        else
                        {
                            // Вставка в DataGridView (журнал)
                            dataGridView2.Rows.Add();
                            count = dataGridView2.RowCount - 1;
                            dataGridView2[0, count].Value = DateTime.Now.ToString();  // Дата и время операции
                            dataGridView2[1, count].Value = "Удаление из " + randbase.Remove(0, 9);
                            str = Regex.Replace(str, @"\s+", " "); // Удаляю лишние пробелы в строке
                            dataGridView2[2, count].Value = str;
                        }

                        Delete(randbase); // Удаляем строку из таблицы
                        content_log(randbase); // Вносим таблицы в журнал содержимого
                        break;

                    // Изменение
                    case 3:
                        string str1 = "";
                        date_time = "";
                        string query1 = "Select * from " + randbase + " where oid = (select min(oid) from " + randbase + ")";
                        // Получаем старую строку из таблицы
                        using (OdbcConnection connection = new OdbcConnection("Dsn=PostgreSQL16"))
                        {
                            OdbcCommand cm = new OdbcCommand(query1, connection);
                            connection.Open();
                            OdbcDataReader reader = cm.ExecuteReader();

                            while (reader.Read())
                                str1 = reader.GetString(0) + "  " + reader.GetString(1) + reader.GetString(2) + reader.GetString(3) + reader.GetString(4);
                            reader.Close();
                        }
                        if (randbase == "pmib6702.pdb1" || randbase == "pmib6702.pdb2")
                        {
                            // Вставка в DataGridView (журнал)
                            dataGridView1.Rows.Add();
                            count = dataGridView1.RowCount - 1;
                            dataGridView1[1, count].Value = "Обновление в " + randbase.Remove(0, 9);
                            str1 = Regex.Replace(str1, @"\s+", " "); // Удаляю лишние пробелы в строке
                            dataGridView1[2, count].Value = str1;
                        }
                        else
                        {
                            // Вставка в DataGridView (журнал)
                            dataGridView2.Rows.Add();
                            count = dataGridView2.RowCount - 1;
                            dataGridView2[1, count].Value = "Обновление в " + randbase.Remove(0, 9);
                            str1 = Regex.Replace(str1, @"\s+", " "); // Удаляю лишние пробелы в строке
                            dataGridView2[2, count].Value = str1;
                        }

                        // Изменяем строку
                        Update(randbase);

                        string str2 = "";
                        string query2 = "Select * from " + randbase + " where oid = (select min(oid) from " + randbase + ")";
                        // Получаем новую строку из таблицы
                        using (OdbcConnection connection = new OdbcConnection("Dsn=PostgreSQL16"))
                        {
                            OdbcCommand cm = new OdbcCommand(query2, connection);
                            connection.Open();
                            OdbcDataReader reader = cm.ExecuteReader();

                            while (reader.Read())
                            {
                                str2 = reader.GetString(0) + "  " + reader.GetString(1) + reader.GetString(2) + reader.GetString(3) + reader.GetString(4);
                                date_time = reader.GetString(4);
                            }
                            reader.Close();
                        }
                        if (randbase == "pmib6702.pdb1" || randbase == "pmib6702.pdb2")
                        {
                            str2 = Regex.Replace(str2, @"\s+", " "); // Удаляю лишние пробелы в строке
                            count = dataGridView1.RowCount - 1;
                            dataGridView1[0, count].Value = DateTime.Now.ToString();  // Дата и время операции
                            dataGridView1[3, count].Value = str2;
                        }
                        else
                        {
                            str2 = Regex.Replace(str2, @"\s+", " "); // Удаляю лишние пробелы в строке
                            count = dataGridView2.RowCount - 1;
                            dataGridView2[0, count].Value = DateTime.Now.ToString();  // Дата и время операции
                            dataGridView2[3, count].Value = str2;
                        }

                        content_log(randbase); // Вносим таблицы в журнал содержимого
                        break;

                }

                MessageBox.Show("Транзакция");
                if (time >= 15)
                {
                    timer.Stop(); // Остановим таймер через 10 сек работы
                    break;
                }  
            }

            // Репликация данных (РД)
            Data_Replication();
            MessageBox.Show("Цикл ИРС и РД завершен");
        }

        bool[] keys = new bool[1000];
        // Выбираем со всех таблиц все ключи
        void find_keys()
        {
            for (int i = 0; i < 1000; i++)
                keys[i] = true;
            string query = "SELECT n_det FROM pmib6702.pdb1 UNION SELECT n_det FROM pmib6702.pdb2 UNION SELECT n_det FROM pmib6702.pdb3 UNION SELECT n_det FROM pmib6702.db_level1 UNION SELECT n_det FROM pmib6702.db_level2 ORDER BY n_det";
            using (OdbcConnection connection = new OdbcConnection("Dsn=PostgreSQL16"))
            {
                int ikey = 0;
                OdbcCommand cm = new OdbcCommand(query);
                cm.Connection = connection;
                connection.Open();
                OdbcDataReader reader = cm.ExecuteReader();
                while (reader.Read())
                {
                    ikey = Convert.ToInt32(reader.GetString(0));
                    keys[ikey] = false;
                }
                reader.Close();
                // Соединение автоматически закрывается при использовании блока using
            }
        }

        // Поиск уникального ключа со всех таблиц
        int union_key()
        {
            bool find = false;
            int j = 1;
            int num = 1;

            while (find == false && j < 1000)
            {
                if (keys[j] == true)
                {
                    find = true;
                    keys[j] = false;
                    num = j;
                }
                j++;
            }
            return num;
        }

        // Вставка строки в таблицу
        void Insert(string randbase)
        {
            int num = union_key(); // ищем уникальный ключ
            string query = "Insert into " + randbase + "(n_det,name,town,type_oper,date_oper) values(" + num + ", 'Крепление', 'Новосибирск', 'Вставка в " + randbase.Remove(0, 9) + "', date_trunc('second', current_timestamp) AT TIME ZONE '-7 UTC')";
            OdbcCommand cm = new OdbcCommand(query);

            using (OdbcConnection connection = new OdbcConnection("Dsn=PostgreSQL16"))
            {
                cm.Connection = connection;
                connection.Open();
                cm.ExecuteNonQuery();
                // Соединение автоматически закрывается при использовании блока using
            }
        }

        // Удаление строки из таблицы
        void Delete(string randbase)
        {
            string query = "Delete from " + randbase + " WHERE oid = (select max(oid) from " + randbase + ")";
            OdbcCommand cm = new OdbcCommand(query);

            using (OdbcConnection connection = new OdbcConnection("Dsn=PostgreSQL16"))
            {
                cm.Connection = connection;
                connection.Open();
                cm.ExecuteNonQuery();
                // Соединение автоматически закрывается при использовании блока using
            }
        }

        // Изменение строки в таблице
        void Update(string randbase)
        {
            string query = "Update " + randbase + " set town = 'Новосибирск', type_oper = 'Обновление в " + randbase.Remove(0, 9) + "', date_oper = date_trunc('second', current_timestamp) AT TIME ZONE '-7 UTC' WHERE oid = (select min(oid) from " + randbase + ")";
            OdbcCommand cm = new OdbcCommand(query);

            using (OdbcConnection connection = new OdbcConnection("Dsn=PostgreSQL16"))
            {
                cm.Connection = connection;
                connection.Open();
                cm.ExecuteNonQuery();
                // Соединение автоматически закрывается при использовании блока using
            }
        }

        // Журнал седержимого
        void content_log(string randbase)
        {
            string str3 = "";
            string query3 = "Select * from " + randbase;
            // Получаем всю таблицу
            using (OdbcConnection connection = new OdbcConnection("Dsn=PostgreSQL16"))
            {
                OdbcCommand cm = new OdbcCommand(query3, connection);
                connection.Open();
                OdbcDataReader reader = cm.ExecuteReader();
                using (StreamWriter sw = new StreamWriter(@"Logs/content_log.txt", true))
                {
                    sw.Write("{0}\n", randbase.Remove(0, 9));
                    sw.Close();
                }
                while (reader.Read())
                {
                    str3 = reader.GetString(0) + "  " + reader.GetString(1) + reader.GetString(2) + reader.GetString(3) + reader.GetString(4);
                    // Фиксируем в ЖУРНАЛЕ СОДЕРЖИМОГО
                    using (StreamWriter sw = new StreamWriter(@"Logs/content_log.txt", true))
                    {
                        str3 = Regex.Replace(str3, @"\s+", " "); // Удаляю лишние пробелы в строке
                        sw.Write("{0}\n", str3);
                        sw.Close();
                    }
                }
                reader.Close();
            }
            using (StreamWriter sw = new StreamWriter(@"Logs/content_log.txt", true))
            {
                sw.Write("*****************************************************************\n\n");
                sw.Close();
            }
        }

        // Репликация данных (РД)
        void Data_Replication()
        {
            // Блокируем ПБД
            string query_lock = "Lock table pmib6702.pdb1, pmib6702.pdb2, pmib6702.pdb3 in exclusive mode";
            using (OdbcConnection connection = new OdbcConnection("Dsn=PostgreSQL16"))
            {
                OdbcCommand cm = new OdbcCommand(query_lock, connection);
                connection.Open();
                cm.ExecuteNonQueryAsync();

                /// **** Репликация данных (РД) ****

                // Проверка на коллизии pbd1 и pbd2. Разрешение в сторону раннего обновления           
                for (int k = 0; k < dataGridView1.RowCount; k++)
                {
                    for (int j = k + 1; j < dataGridView1.RowCount; j++)
                    {
                        // Сравниваю уникальный ID строки. Если разные таблицы работают с одной и той же строкой, то это коллизия
                        string[] number1 = dataGridView1[2, k].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] number2 = dataGridView1[2, j].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        // Выясняю, разные ли это таблицы. Если таблица одна и та же, то все хорошо
                        string[] base1 = dataGridView1[1, k].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] base2 = dataGridView1[1, j].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (number1[0] == number2[0] && base1[2] != base2[2]) // Если это коллизия
                        {
                            string[] time1 = dataGridView1[0, k].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            string[] time2 = dataGridView1[0, j].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            // Если первая таблица начала работать со строкой раньше, то оставляю ее, а другую удаляю
                            if (Convert.ToDateTime(time1[1]) <= Convert.ToDateTime(time2[1]))
                            {
                                dataGridView1.Rows.RemoveAt(j);
                                j--;
                            }
                            // Иначе удаляю вторую
                            else
                            {
                                dataGridView1.Rows.RemoveAt(k);
                                j--;
                            }
                        }
                    }
                }
                dataGridView1.Refresh();
                dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

                // Занесение данных в db_level1 (согласование репликатора)
                for (int k = 0; k < dataGridView1.RowCount; k++)
                {
                    string[] operation = dataGridView1[1, k].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] id = dataGridView1[2, k].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] time = dataGridView1[0, k].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (operation[0] == "Вставка")
                    {
                        int num = union_key(); // ищем уникальный ключ
                        string query = "Insert into pmib6702.db_level1 (n_det,name,town,type_oper,date_oper) values(" + num + ",'Крепление', 'Новосибирск', 'Вставка в " + operation[2] + "', '" + Convert.ToDateTime(time[1]).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        OdbcCommand cm1 = new OdbcCommand(query);
                        using (OdbcConnection connection1 = new OdbcConnection("Dsn=PostgreSQL16"))
                        {
                            cm1.Connection = connection1;
                            connection1.Open();
                            cm1.ExecuteNonQuery();
                            // Соединение автоматически закрывается при использовании блока using
                        }
                    }
                    if (operation[0] == "Удаление")
                    {
                        string query = "Delete from pmib6702.db_level1 WHERE n_det = " + id[0];
                        OdbcCommand cm2 = new OdbcCommand(query);

                        using (OdbcConnection connection2 = new OdbcConnection("Dsn=PostgreSQL16"))
                        {
                            cm2.Connection = connection2;
                            connection2.Open();
                            cm2.ExecuteNonQuery();
                            // Соединение автоматически закрывается при использовании блока using
                        }
                    }
                    if (operation[0] == "Обновление")
                    {
                        string query = "Update pmib6702.db_level1 set town = 'Новосибирск', type_oper = 'Обновление в " + operation[2] + "', date_oper = '" + Convert.ToDateTime(time[1]).ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE n_det = " + id[0];
                        OdbcCommand cm3 = new OdbcCommand(query);

                        using (OdbcConnection connection3 = new OdbcConnection("Dsn=PostgreSQL16"))
                        {
                            cm3.Connection = connection3;
                            connection3.Open();
                            cm3.ExecuteNonQuery();
                            // Соединение автоматически закрывается при использовании блока using
                        }
                    }
                }

                int count;
                // Переношу транзакции pdb1 и pdb2 без коллизий к pdb3. В объединенной таблице снова отслеживаю транзакции     
                // Вставка pdb1,pdb2 в datagrid
                for (int k = 0; k < dataGridView1.RowCount; k++)
                {
                    dataGridView3.Rows.Add();
                    count = dataGridView3.RowCount - 1;
                    dataGridView3[0, count].Value = dataGridView1[0, k].Value;  // Дата и время операции
                    dataGridView3[1, count].Value = dataGridView1[1, k].Value;
                    dataGridView3[2, count].Value = dataGridView1[2, k].Value;
                    if (dataGridView1[3, k].Value != null)
                        dataGridView3[3, count].Value = dataGridView1[3, k].Value;
                }

                dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);
                // Вставка pdb3 в datagrid
                for (int k = 0; k < dataGridView2.RowCount; k++)
                {
                    dataGridView3.Rows.Add();
                    count = dataGridView3.RowCount - 1;
                    dataGridView3[0, count].Value = dataGridView2[0, k].Value;  // Дата и время операции
                    dataGridView3[1, count].Value = dataGridView2[1, k].Value;
                    dataGridView3[2, count].Value = dataGridView2[2, k].Value;
                    if (dataGridView2[3, k].Value != null)
                        dataGridView3[3, count].Value = dataGridView2[3, k].Value;
                }

                // Проверка на коллизии в объединении pdb1,pdb2,pdb3. Разрешение в сторону раннего обновления           
                for (int k = 0; k < dataGridView3.RowCount; k++)
                {
                    for (int j = k + 1; j < dataGridView3.RowCount; j++)
                    {
                        // Сравниваю уникальный ID строки. Если разные таблицы работают с одной и той же строкой, то это коллизия
                        string[] number1 = dataGridView3[2, k].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] number2 = dataGridView3[2, j].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        // Выясняю, разные ли это таблицы. Если таблица одна и та же, то все хорошо
                        string[] base1 = dataGridView3[1, k].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] base2 = dataGridView3[1, j].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (number1[0] == number2[0] && base1[2] != base2[2])
                        {
                            string[] time1 = dataGridView3[0, k].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            string[] time2 = dataGridView3[0, j].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            // Если первая таблица начала работать со строкой раньше, то оставляю ее, а другую удаляю
                            if (Convert.ToDateTime(time1[1]) <= Convert.ToDateTime(time2[1]))
                            {
                                dataGridView3.Rows.RemoveAt(j);
                                j--;
                            }
                            // Иначе удаляю вторую
                            else
                            {
                                dataGridView3.Rows.RemoveAt(k);
                                j--;
                            }
                        }
                    }
                }
                dataGridView3.Refresh();
                dataGridView3.Sort(dataGridView3.Columns[0], ListSortDirection.Ascending);

                // Занесение данных в db_level2 (согласование репликатора)
                for (int k = 0; k < dataGridView3.RowCount; k++)
                {
                    string[] operation = dataGridView3[1, k].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] id = dataGridView3[2, k].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] time = dataGridView3[0, k].Value.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (operation[0] == "Вставка")
                    {
                        int num = union_key(); // ищем уникальный ключ
                        string query = "Insert into pmib6702.db_level2 (n_det,name,town,type_oper,date_oper) values(" + num + ",'Крепление', 'Новосибирск', 'Вставка в " + operation[2] + "', '" + Convert.ToDateTime(time[1]).ToString("yyyy-MM-dd HH:mm:ss") + "')";
                        OdbcCommand cm1 = new OdbcCommand(query);
                        using (OdbcConnection connection1 = new OdbcConnection("Dsn=PostgreSQL16"))
                        {
                            cm1.Connection = connection1;
                            connection1.Open();
                            cm1.ExecuteNonQuery();
                            // Соединение автоматически закрывается при использовании блока using
                        }
                    }
                    if (operation[0] == "Удаление")
                    {
                        string query = "Delete from pmib6702.db_level2 WHERE n_det = " + id[0];
                        OdbcCommand cm2 = new OdbcCommand(query);

                        using (OdbcConnection connection2 = new OdbcConnection("Dsn=PostgreSQL16"))
                        {
                            cm2.Connection = connection2;
                            connection2.Open();
                            cm2.ExecuteNonQuery();
                            // Соединение автоматически закрывается при использовании блока using
                        }
                    }
                    if (operation[0] == "Обновление")
                    {
                        string query = "Update pmib6702.db_level2 set town = 'Новосибирск', type_oper = 'Обновление в " + operation[2] + "', date_oper = '" + Convert.ToDateTime(time[1]).ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE n_det = " + id[0];
                        OdbcCommand cm3 = new OdbcCommand(query);

                        using (OdbcConnection connection3 = new OdbcConnection("Dsn=PostgreSQL16"))
                        {
                            cm3.Connection = connection3;
                            connection3.Open();
                            cm3.ExecuteNonQuery();
                            // Соединение автоматически закрывается при использовании блока using
                        }
                    }
                }

                // Соединение автоматически закрывается при использовании блока using
            }
        }
    }
}
