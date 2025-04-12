namespace contact_app.DTO
{
    public class Response
    {
        public string Status { get; set; } = "success";
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }

        public Response() { }

        public Response(string status, string message, string? token = null) 
        {  
            Status = status; 
            Message = message; 
            Token = token;
        }
    }
}
