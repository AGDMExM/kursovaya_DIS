namespace DIS.Models
{
    public class Package
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status {  get; set; }
        public int IdSender { get; set; }
        public int IdRecipient { get; set; }
        public int IdSenderPostOffice { get; set; }
        public int IdRecipientPostOffice { get; set; }
    }
}
