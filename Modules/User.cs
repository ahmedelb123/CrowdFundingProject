public class User
{
    public int id { get; set; }
    public string name { get; set; }
    public string surname { get; set; }
    public string email { get; set; }
    public string password_hash { get; set; }
    public bool isAdmin { get; set; } = false; 
    public DateTime createdAt { get; set; } 

    public User() { }

    public User(string name, string surname, string email, string password)
    {
        this.name = name;
        this.surname = surname;
        this.email = email;
        this.password_hash = password;
    }
}
