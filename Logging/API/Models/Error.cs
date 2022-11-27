public class Error
{

    [DbColumn(IsIdentity =true, IsPrimary =true)]
    int Id { get; set; }
    
    [DbColumn]
    string File { get; set;}
    
    [DbColumn]
    string Function { get; set; }
    
    [DbColumn]
    string Message { get; set; }
}