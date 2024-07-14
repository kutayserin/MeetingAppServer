using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public string Password { get; set; }
    public string ProfileImageUrl { get; set; }
}