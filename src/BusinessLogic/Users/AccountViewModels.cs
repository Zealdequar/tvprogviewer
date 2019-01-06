using System.ComponentModel.DataAnnotations;

namespace TVProgViewer.BusinessLogic.Users
{
    /// <summary>
    /// Используется для входа в систему
    /// </summary>
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить")]
        public bool RememberMe { get; set; }
    }
}
