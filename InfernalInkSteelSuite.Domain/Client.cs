namespace InfernalInkSteelSuite.Domain
{
    public class Client
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string MiddleName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Notes { get; set; } = "";
        public int Visits { get; set; }

        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(MiddleName))
                {
                    return $"{FirstName} {LastName}";
                }
                else
                {
                    return $"{FirstName} {MiddleName} {LastName}";
                }
            }
        }
    }
}
