using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public class Loan
{
    public int Id { get; set; }

    [Display(Name = "Egzemplarz")]
    public int BookCopyId { get; set; }

    public BookCopy? BookCopy { get; set; }

    [Display(Name = "Imię i nazwisko wypożyczającego")]
    public string Czytelnik { get; set; } = string.Empty;

    [Display(Name = "Data wypożyczenia")]
    public DateTime DataWypozyczenia { get; set; } = DateTime.Now;

    [Display(Name = "Termin zwrotu")]
    public DateTime TerminZwrotu { get; set; } = DateTime.Now.AddDays(14);

    [Display(Name = "Data zwrotu")]
    public DateTime? DataZwrotu { get; set; }
}