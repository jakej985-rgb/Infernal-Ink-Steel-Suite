namespace InfernalInkSteelSuite.Domain
{
    public class Client
    {
        public int Id { get; set; }           // client_id
        public string Name { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Notes { get; set; } = "";
        public int Visits { get; set; }
    }
}
