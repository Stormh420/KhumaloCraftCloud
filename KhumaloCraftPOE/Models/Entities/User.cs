using System.ComponentModel.DataAnnotations;

namespace KhumaloCraftPOE.Models.Entities
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        public required string DeliveryAddress { get; set; }
    }
}


