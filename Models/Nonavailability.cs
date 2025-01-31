using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Nonavailability
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date1 { get; set; }

    [Required]
    public DateTime Date2 { get; set; }

    // Foreign key reference to Account
    [Required]
    public int AccountId { get; set; }

    [ForeignKey("AccountId")]
    public Account Account { get; set; }
}
