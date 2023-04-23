using Npgsql;

namespace HabrPost.Controllers.DB
{
    class DBRequest
    {
        private readonly string dbUserName = "postgres";
        private readonly string dbUserPassword = "root";
        public static bool hasUser;
        public static bool subscribed;
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
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            
        } 
        public void HasUserInDB(long chatId)
        {
            try
            {
                using(var conn = new NpgsqlConnection(connectionString: $"Server=localhost;Port=5432;User Id={dbUserName};Password={dbUserPassword};Database=postgres;"))
                {
                    conn.Open();
                    using(var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = $"SELECT subscribed FROM users WHERE tgid = {chatId}";
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        if(reader.HasRows)
                        {
                            hasUser = true;
                            while(reader.Read())
                            {
                                subscribed = reader.GetBoolean(0);
                            }
                        }
                        else
                        {
                            hasUser = false;
                        }
                    }
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public List<long> GetListSubs()
        {
            string sqlRequest = "SELECT tgid FROM users WHERE subscribed = TRUE";
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
                            return subs;
                        }
                        else
                        {
                            return subs;
                        }
                    }
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            return subs;
        }
    }
}