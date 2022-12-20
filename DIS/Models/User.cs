using DIS.Models;

namespace DIS.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public uint PassportSeries { get; set; }
        public uint PassportNumber { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public int? RoleId { get; set; }
        public Role Role { get; set; }
    }
}