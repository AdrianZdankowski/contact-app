namespace contact_app.Entities
{
    public class User
    {
        public int id { get; set; } 
        public string email { get; set; }
        public byte[] passwordHash { get; set; }

        public byte[] passwordSalt { get; set; }
    }
}
