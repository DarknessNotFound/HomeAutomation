using Microsoft.Data.Sqlite;

namespace Logs
{
    public record ErrorRec(int Id, string File, string Function, string Message) {}
    public class Error
    {
        public static readonly string Table = "Errors";
        public int Id { get; set; }
        public string File { get; set;}        
        public string Function { get; set; }
        public string Message { get; set; }

        public Error()
        {
            Id = -1;
            File = "";
            Function = "";
            Message = "";
        }

        public Error(string file, string func, string msg)
        {
            Id = -1;
            File = file;
            Function = func;
            Message = msg;
        }

        public Error(int id, string file, string func, string msg)
        {
            Id = id;
            File = file;
            Function = func;
            Message = msg;
        }

        public void Insert(int Program)
        {
            string Query = 
                $"""
                INSERT INTO {Table} (Program, File, Function, Message)
                VALUES ($Program, $File, $Function, $Message)
                RETURNING Id;
                """;

            using (var connection = new SqliteConnection(Constants.Conn))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = Query;
                command.Parameters.AddWithValue("$Program", Program);
                command.Parameters.AddWithValue("$File", File);
                command.Parameters.AddWithValue("$Function", Function);
                command.Parameters.AddWithValue("$Message", Message);
                var data = command.ExecuteReader();
                while(data.Read())
                {
                    Id = data.GetInt32(0);
                }
            }
        }

        public static List<Error> GetAllErrors()
        {
            var Result = new List<Error>();
            string Query = $"SELECT * FROM {Table}";

            using (var connection = new SqliteConnection(Constants.Conn))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = Query;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Result.Add(new Error(
                            int.Parse(reader.GetString(0)),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetString(4)
                        ));
                    }
                }
            }
            return Result;
        }

        public ErrorRec ToRec()
        {
            return new ErrorRec(Id, File, Function, Message);
        }
    }
}