using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Net.Http;

namespace Authentication
{
    public record ApiKeyRec(int Id, string Name, string Key, bool IsDeleted) {}
    public record ApiKey 
    {
        public static readonly string Table = "ApiKeys";
        static readonly string TableCreationQuery = 
            $"""
            CREATE TABLE IF NOT EXISTS {Table}(
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name     TEXT     NOT NULL,
            Key      TEXT     NOT NULL,
            IsDeleted BOOLEAN NOT NULL CHECK (IsDeleted IN (0, 1)) DEFAULT 0
            );
            """;
        int Id { get; set; }
        string Name { get; set; }
        string Key { get; set; }
        bool IsDeleted { get; set; }

        public ApiKey()
        {
            Id = -1;
            Name = "default";
            Key = "";
            IsDeleted = true;
        }
        
        public ApiKey(string name)
        {
            Id = -1;
            Name = name;
            Key = System.Guid.NewGuid().ToString();
            IsDeleted = false;
        }

        public ApiKey(int Id)
        {
            ApiKey result = new ApiKey(); 
            if(Exists(Id))
            {
                result = GetApiKey(Id);
            }

            Id = result.Id;
            Name = result.Name;
            Key = result.Key;
            IsDeleted = result.IsDeleted;
        }

        public ApiKey(int id, string name, string key, bool isDeleted)
        {
            Id = id;
            Name = name;
            Key = key;
            IsDeleted = isDeleted;
        }

        public ApiKey(ApiKey i)
        {
            Id = i.Id;
            Name = i.Name;
            Key = i.Key;
            IsDeleted = i.IsDeleted;
        }

        public ApiKey(ApiKeyRec i)
        {
            Id = i.Id;
            Name = i.Name;
            Key = i.Key;
            IsDeleted = i.IsDeleted;
        }

        public int GetId()
        {
            return this.Id;
        }

        public static bool Exists(int Id)
        {
            bool Result = false;
            string Query = 
            $"""
            SELECT Name
            FROM {Table}
            WHERE Id = $Id;
            """;

            using (var connection = new SqliteConnection(Constants.Conn))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = Query;
                command.Parameters.AddWithValue("$Id", Id);

                using (var reader = command.ExecuteReader())
                {
                    Result = true;
                }
            }

            return Result;
        }

        public static bool Exists(string Key)
        {
            bool Result = false;
            string Query = 
            $"""
            SELECT Name
            FROM {Table}
            WHERE Key = $Key;
            """;

            using (var connection = new SqliteConnection(Constants.Conn))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = Query;
                command.Parameters.AddWithValue("$Key", Key);

                using (var reader = command.ExecuteReader())
                {
                    Result = true;
                }
            }

            return Result;
        }

        public bool IsValid()
        {
            return true;
        }

        public static bool IsAdmin(string Key)
        {
            return Key == GetApiKey(1).Key;
        }

        public void Insert()
        {
            string Query = 
                $"""
                INSERT INTO {Table} (Name, Key)
                VALUES ($Name, $Key)
                RETURNING Id;
                """;

            using (var connection = new SqliteConnection(Constants.Conn))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = Query;
                command.Parameters.AddWithValue("$Name", Name);
                command.Parameters.AddWithValue("$Key", Key);
                var data = command.ExecuteReader();
                while(data.Read())
                {
                    Id = data.GetInt32(0);
                }
            }
        }

        public void Update()
        {
            string Query = 
            $"""
            UPDATE {Table}
            SET Name = $Name, 
                Key = $Key,
                IsDeleted = $IsDeleted
            WHERE Id = $Id;
            """;

            if(ApiKey.Exists(Id))
            {
                using (var connection = new SqliteConnection(Constants.Conn))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = Query;
                    command.Parameters.AddWithValue("$Id", Id);
                    command.Parameters.AddWithValue("$Name", Name);
                    command.Parameters.AddWithValue("$Key", Key);
                    command.Parameters.AddWithValue("$IsDeleted", IsDeleted ? 1 : 0);
                    var data = command.ExecuteReader();
                }
            }
            else
                this.Insert();
        }

        public static void Remove(int Id)
        {
            string Query = 
            $"""
            UPDATE {Table}
            SET IsDeleted = 1
            WHERE Id = $Id;
            """;

            using (var connection = new SqliteConnection(Constants.Conn))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = Query;
                command.Parameters.AddWithValue("$Id", Id);
                var data = command.ExecuteReader();
            }
        }

        public static ApiKey GetApiKey(int Id)
        {
            ApiKey Result = new ApiKey();
            string Query = $"SELECT * FROM {Table} WHERE Id = $Id;";

            using (var connection = new SqliteConnection(Constants.Conn))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = Query;
                command.Parameters.AddWithValue("$Id", Id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Result = new ApiKey(
                            int.Parse(reader.GetString(0)),
                            reader.GetString(1),
                            reader.GetString(2),
                            int.Parse(reader.GetString(3)) == 1
                        );
                    }
                }
            }
            Console.WriteLine("GetApiKey: " + Result.ToString());
            return Result;
        }

        public static List<ApiKey> GetAllApiKey(bool IncludeDelete = false)
        {
            List<ApiKey> Result = new List<ApiKey>();
            string Query;
            if(IncludeDelete)
                Query = $"SELECT * FROM {Table};";
            else
                Query = $"SELECT * FROM {Table} WHERE IsDeleted = 0;";

            using (var connection = new SqliteConnection(Constants.Conn))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = Query;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Result.Add(new ApiKey(
                            int.Parse(reader.GetString(0)),
                            reader.GetString(1),
                            reader.GetString(2),
                            int.Parse(reader.GetString(3)) == 1
                        ));
                    }
                }
            }
            return Result;
        } 

        public static void InitialSetup()
        {
            //Make the database.
            using (var connection = new SqliteConnection(Constants.Conn))
            {
                connection.Open();
                var command = connection.CreateCommand();
                    command.CommandText = TableCreationQuery;
                    command.ExecuteNonQuery();
            }

            bool AtLeastOneRow = true;

            //Get NumRows
            string NumRowsQuery = $"SELECT * FROM {Table} LIMIT 1;";
            using (var connection = new SqliteConnection(Constants.Conn))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = NumRowsQuery;

                object? result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    int count = Convert.ToInt32(result);
                    Console.WriteLine(result.ToString());
                    AtLeastOneRow = count > 0;
                }
                else
                {
                    AtLeastOneRow = false;
                }
            }

            //Insert the Initial Keys as needed
            ApiKey AdminKey = new ApiKey("Admin");
            if(AtLeastOneRow)
                AdminKey = new ApiKey(1);
            else
                AdminKey.Insert();

            Console.WriteLine("Admin Key: " + AdminKey.Key);
        }

        public static void PrintAllKeys()
        {

            var Keys = GetAllApiKey(true);
            
            Console.WriteLine("Printing all keys");
            Console.WriteLine("Id | Name | Key | IsDeleted");
            foreach(ApiKey key in Keys)
            {
                Console.WriteLine(key.ToString());
            }
            return;
        }

        public ApiKeyRec ToRec()
        {
            return new ApiKeyRec(Id, Name, Key, IsDeleted);
        }

        public override string ToString()
        {
            string s = " | "; //seperator.
            return Id.ToString() + s + Name + s + Key + s + IsDeleted.ToString();
        }
    }

    public static class ApiKeyEndpoints
    {
        public static void MapApiKeyEndpoints(this WebApplication app)
        {
            app.MapGet("/keys", AllKeys);
            app.MapGet("/keys/deleted", AllKeysIncludeDeleted);
            app.MapPost("/key/{Name}", NewKey);
            app.MapGet("/key/{id}", GetKey);
            app.MapDelete("/key/{id}", RemoveKey);
            app.MapPut("/key", UpdateKey);
        }

        public static async Task<IResult> AllKeys()
        {
            var result = await Task<List<ApiKeyRec>>.Run( () => ApiKey.GetAllApiKey().Select(x => x.ToRec()).ToArray());
            return Results.Ok(result);
        }

        public static async Task<IResult> AllKeysIncludeDeleted()
        {
            var result = await Task<List<ApiKeyRec>>.Run( () => ApiKey.GetAllApiKey(IncludeDelete: true).Select(x => x.ToRec()).ToArray());
            return Results.Ok(result);
        }

        public static async Task<IResult> NewKey(string Name)
        {
            if(string.IsNullOrEmpty(Name))
                return Results.NoContent();
            
            Console.WriteLine("Making a new key: " + Name);
            ApiKey NewKey = new ApiKey(Name);
            await Task.Run(() => NewKey.Insert());
            return Results.Ok(NewKey.ToRec());
        }

        public static async Task<IResult> RemoveKey(int Id)
        {
            if(ApiKey.Exists(Id))
            {
                await Task.Run(() => ApiKey.Remove(Id));
                return Results.Ok();
            }
            else
            {
                return Results.NotFound();
            }
        }

        public static async Task<IResult> GetKey(int Id)
        {
            if(ApiKey.Exists(Id))
            {
                ApiKey Key = await Task.Run(() => ApiKey.GetApiKey(Id));
                return Results.Ok(Key.ToRec());
            }
            else
            {
                return Results.NotFound();
            }
        }

        public static async Task<IResult> UpdateKey(ApiKeyRec RecFromRequest)
        {
            ApiKey KeyFromRequest = new ApiKey(RecFromRequest);
            Console.WriteLine(KeyFromRequest.ToString());
            if(KeyFromRequest.IsValid())
            {
                await Task.Run(() => KeyFromRequest.Update());
                ApiKey UpdatedKey = new ApiKey(KeyFromRequest.GetId());
                return Results.Ok(UpdatedKey.ToRec());
            }
            else
            {
                return Results.BadRequest();
            }
        }
    }

    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEY = "ApiKey";
        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(APIKEY, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key was not provided ");
                return;
            }

            var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();

            if (!ApiKey.Exists(extractedApiKey!))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client");
                return;
            }

            await _next(context);
        }
    }
}