namespace Logs
{
    public record ErrorRec(int Id, string File, string Function, string Message) {}
    public class Error
    {
        //[DbColumn(IsIdentity =true, IsPrimary =true)]
        public int Id { get; set; }
        
        //[DbColumn]
        public string File { get; set;}
        
        //[DbColumn]
        public string Function { get; set; }
        
        //[DbColumn]
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

        public bool Upsert()
        {
            return true;
        }

        public ErrorRec ToRec()
        {
            return new ErrorRec(Id, File, Function, Message);
        }
    }
}