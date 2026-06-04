using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public class BookCopy
{
    public int Id { get; set; }

    [Display(Name = "Książka")]
    public int BookId { get; set; }

    [Display(Name = "Dostępny")]
    public bool Dostepny { get; set; } = true;

    public Book? Book { get; set; }
}