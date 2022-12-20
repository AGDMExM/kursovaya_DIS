using System.ComponentModel.DataAnnotations;

namespace DIS.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Не указано ФИО")]
        public string FIO { get; set; }

        [Required(ErrorMessage = "Не указана серия паспорта")]
        public uint PassportSeries { get; set; }

        [Required(ErrorMessage = "Не указан номер паспорта")]
        public uint PassportNumber { get; set; }

        [Required(ErrorMessage = "Не указан логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; }
    }
}
