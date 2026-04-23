using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "User ID is required")]
    public string? user_id { get; set; } // Matches [user_id]

    [Required(ErrorMessage = "Full Name is required")]
    public string? user_name { get; set; } // Matches [user_name]

    [Required, EmailAddress]
    public string? user_email { get; set; } // Matches [user_email]

    [Required]
    [DataType(DataType.Password)]
    public string? user_pass { get; set; } // Matches [user_pass]
    [Required]
    [Compare("user_pass", ErrorMessage = "Passwords do not match")]
    public string? ConfirmPassword { get; set; }

    [Required]
    public string? Role { get; set; } // Matches [Role]

    // Default values for other columns
    public int? active { get; set; } = 1;
    public string? status { get; set; } = "Pending";
}