using Npgsql;
using HabrPost.Model.Struct;
using HabrPost.LogException;

namespace HabrPost.Controllers.DB
{
    class DBRequest
    {
        private readonly string dbUserName = "postgres";
        private readonly string dbUserPassword = "root";
        public static bool hasUser;
        public static bool subscribed;

        ///<summary>
        ///Выполнить "изменяющий" таблицу запрос
        ///</summary>
        public async void SQLInstructionSet(string sqlRequest)
        {
            try
            {
                using(var conn = new NpgsqlConnection(connectionString: $"Server=localhost;Port=5432;User Id={dbUserName};Password={dbUserPassword};Database=postgres;"))
                {
                    conn.Open();
                    using(var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = sqlRequest;
                        await cmd.ExecuteNonQueryAsync();
                    }
                    conn.Close();
                }
            }
            catch(Exception exception)
            {
                ExceptionLogger newLog = new ExceptionLogger();
                newLog.NewException(exception);
            }
            
        } 

        ///<summary>
        ///Поиск пользователя в бд
        ///</summary>
        public bool HasUserInDB(long chatId)
        {
            try
            {
                using(var conn = new NpgsqlConnection(connectionString: $"Server=localhost;Port=5432;User Id={dbUserName};Password={dbUserPassword};Database=postgres;"))
                {
                    conn.Open();
                    using(var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = $"SELECT * FROM users WHERE tg_id = {chatId}";
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if(reader.HasRows)
                        {
                            conn.Close();
                            return true;
                        }
                        else
                        {
                            conn.Close();
                            return false;
                        }
                    }
                }
            }
            catch(Exception exception)
            {
                ExceptionLogger newLog = new ExceptionLogger();
                newLog.NewException(exception);
            }
            return false;
        }

        ///<summary>
        ///Получить список подписчиков по определенному фильтру
        ///</summary>
        public List<long> GetListSubs(string sqlRequest)
        {
            List<long> subs = new List<long>();
            try
            {
                using(var conn = new NpgsqlConnection(connectionString: $"Server=localhost;Port=5432;User Id={dbUserName};Password={dbUserPassword};Database=postgres;"))
                {
                    conn.Open();
                    using(var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = sqlRequest;
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                subs.Add(reader.GetInt64(0));
                            }
                            conn.Close();
                            return subs;
                        }
                        else
                        {
                            conn.Close();
                            return subs;
                        }
                    }
                }
            }
            catch(Exception exception)
            {
                ExceptionLogger newLog = new ExceptionLogger();
                newLog.NewException(exception);
            }
            return subs;
        }

        ///<summary>
        ///Получить список подписок
        ///</summary>
        public SubList[] GetSubscriptions()
        {
            string sqlRequest = "SELECT COUNT(*) FROM subscriptions";
            int count = 0;
            SubList[] subList = new SubList[0];
            try
            {
                using(var conn = new NpgsqlConnection(connectionString: $"Server=localhost;Port=5432;User Id={dbUserName};Password={dbUserPassword};Database=postgres;"))
                {
                    conn.Open();
                    using(var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = sqlRequest;
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if(reader.HasRows)
                        {
                            reader.Read();
                            count = reader.GetInt32(0);
                            subList = new SubList[count];
                            reader.Close();

                            cmd.CommandText = "SELECT * FROM subscriptions ORDER BY id";
                            reader = cmd.ExecuteReader();
                            int id, price;
                            string title, description;
                            for(int i = 0; i < count; i++)
                            {
                                reader.Read();
                                id = reader.GetInt32(0);
                                title = reader.GetValue(1).ToString();
                                description = reader.GetString(2);
                                price = reader.GetInt32(3);
                                subList[i].SetSubscription(id, title, description, price);
                            }
                            reader.Close();
                            return subList;
                        }
                    }
                    conn.Close();
                }
            }
            catch(Exception exception)
            {
                ExceptionLogger newLog = new ExceptionLogger();
                newLog.NewException(exception);
            }
            return subList;
        }

        // ///<summary>
        // ///Получить общее количество подписок
        // ///</summary>
        // public int GetSubscriptionsQuantity()
        // {
        //     string sqlRequest = "SELECT COUNT(*) FROM subscriptions";
        //     int count = 0;
        //     try
        //     {
        //         using(var conn = new NpgsqlConnection(connectionString: $"Server=localhost;Port=5432;User Id={dbUserName};Password={dbUserPassword};Database=postgres;"))
        //         {
        //             conn.Open();
        //             using(var cmd = new NpgsqlCommand())
        //             {
        //                 cmd.Connection = conn;
        //                 cmd.CommandText = sqlRequest;
        //                 NpgsqlDataReader reader = cmd.ExecuteReader();
        //                 if(reader.HasRows)
        //                 {
        //                     reader.Read();
        //                     conn.Close();
        //                     return count = reader.GetInt32(0);
        //                 }
        //                 else
        //                 {

        //                     conn.Close();
        //                     return 0;
        //                 }
        //             }
        //         }
        //     }
        //     catch(Exception exception)
        //     {
        //         ExceptionLogger newLog = new ExceptionLogger();
        //         newLog.NewException(exception);
        //     }
        //     return 0;
        // }

        ///<summary>
        ///Получить данные подписки по названию
        ///</summary>
        public SubList GetSubscriptionTitle(string title)
        {
            SubList subscriprion = new SubList();
            string sqlRequest = $"SELECT id, title, description, price FROM subscriptions WHERE title = '{title}'";
            try
            {
                using(var conn = new NpgsqlConnection(connectionString: $"Server=localhost;Port=5432;User Id={dbUserName};Password={dbUserPassword};Database=postgres;"))
                {
                    conn.Open();
                    using(var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = sqlRequest;
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if(reader.HasRows)
                        {
                            reader.Read();
                            subscriprion.id = reader.GetInt32(0);
                            subscriprion.title = reader.GetString(1);
                            subscriprion.description = reader.GetString(2);
                            subscriprion.price = reader.GetInt32(3);
                            reader.Close();
                        }
                    }
                    conn.Close();
                }
            }
            catch(Exception exception)
            {
                ExceptionLogger newLog = new ExceptionLogger();
                newLog.NewException(exception);
            }
            return subscriprion;
        }


        ///<summary>
        ///Проверка. Есть у пользователя подписка
        ///</summary>
        public bool HasUserSubscription(long chatId, int subId)
        {
            string sqlRequest = $"SELECT * FROM user_subscriptions WHERE tg_id = '{chatId}' AND subscription_id = '{subId}'";
            try
            {
                using(var conn = new NpgsqlConnection(connectionString: $"Server=localhost;Port=5432;User Id={dbUserName};Password={dbUserPassword};Database=postgres;"))
                {
                    conn.Open();
                    using(var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = sqlRequest;
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if(reader.HasRows)
                        {
                            conn.Close();
                            return true;
                        }
                        else
                        {
                            conn.Close();
                            return false;
                        }
                            
                    }
                }
            }
            catch(Exception exception)
            {
                ExceptionLogger newLog = new ExceptionLogger();
                newLog.NewException(exception);
            }
            return false;
        }

        ///<summary>
        ///Проверка. Имеет ли пользователь права администратора
        ///</summary>
        public bool AdminVerify(long chatId)
        {
            string sqlRequest = $"SELECT * FROM users WHERE tg_id = '{chatId}' AND is_admin = 'TRUE'";
            try
            {
                using(var conn = new NpgsqlConnection(connectionString: $"Server=localhost;Port=5432;User Id={dbUserName};Password={dbUserPassword};Database=postgres;"))
                {
                    conn.Open();
                    using(var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = sqlRequest;
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if(reader.HasRows)
                        {
                            conn.Close();
                            return true;
                        }
                        else
                        {
                            conn.Close();
                            return false;
                        }
                            
                    }
                }
            }
            catch(Exception exception)
            {
                ExceptionLogger newLog = new ExceptionLogger();
                newLog.NewException(exception);
            }
            return false;
        }

        ///<summary>
        ///Получение списка администоров
        ///</summary>
        public User[] GetAdmins()
        {
            //Получение количества админов
            string sqlRequest = "SELECT COUNT(*) FROM users WHERE is_admin = 'TRUE'";
            int count = 0;
            User[] userList = new User[0];
            try
            {
                using(var conn = new NpgsqlConnection(connectionString: $"Server=localhost;Port=5432;User Id={dbUserName};Password={dbUserPassword};Database=postgres;"))
                {
                    conn.Open();
                    using(var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = sqlRequest;
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if(reader.HasRows)
                        {
                            //Начинаем чтение результатов
                            reader.Read();
                            //Получаем количество совпадений
                            count = reader.GetInt32(0);
                            //Массив структур User
                            userList = new User[count];
                            //Заканчиваем чтение результатов
                            reader.Close();

                            //Получение данных об администраторах
                            cmd.CommandText = $"SELECT tg_id, first_name, username, is_admin FROM users WHERE is_admin = 'TRUE'";
                            //Выполнение запроса
                            reader = cmd.ExecuteReader();
                            long chatId;
                            string first_name, username;
                            bool is_admin;
                            for(int i = 0; i < count; i++)
                            {
                                //Начинаем чнение
                                reader.Read();
                                //Кешируем результат
                                chatId = reader.GetInt64(0);
                                first_name = reader.GetString(1);
                                username = reader.GetString(2);
                                is_admin = reader.GetBoolean(3);
                                //Сохраняем результат в массив
                                userList[i].SetUser(chatId, first_name, username, is_admin);
                            }
                            reader.Close();
                            return userList;
                        }
                    }
                    conn.Close();
                }
            }
            catch(Exception exception)
            {
                ExceptionLogger newLog = new ExceptionLogger();
                newLog.NewException(exception);
            }
            return userList;
        }

        ///<summary>
        ///Получение информации о пользователе
        ///</summary>
        public User GetUser(string userName)
        {
            //Получение количества админов
            string sqlRequest = $"SELECT * FROM users WHERE username ='{userName}'";
            User user = new User();
            try
            {
                using(var conn = new NpgsqlConnection(connectionString: $"Server=localhost;Port=5432;User Id={dbUserName};Password={dbUserPassword};Database=postgres;"))
                {
                    conn.Open();
                    using(var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = sqlRequest;
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if(reader.HasRows)
                        {
                            //Начинаем чтение результатов
                            reader.Read();
                            //Сохраняем результат
                            user.chatId = reader.GetInt64(0);
                            user.firstName = reader.GetString(1);
                            user.userName = reader.GetString(2);
                            user.is_admin = reader.GetBoolean(3);
                            reader.Close();
                            return user;
                        }
                    }
                    conn.Close();
                }
            }
            catch(Exception exception)
            {
                ExceptionLogger newLog = new ExceptionLogger();
                newLog.NewException(exception);
            }
            return user;
        }
    }
}