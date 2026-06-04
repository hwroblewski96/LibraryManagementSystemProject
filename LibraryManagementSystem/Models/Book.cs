namespace LibraryManagementSystem.Models;

public class Book
{
    public int Id { get; set; }

    public string Tytul { get; set; } = string.Empty;

    public string Autor { get; set; } = string.Empty;

    public string ISBN { get; set; } = string.Empty;

    public string Opis { get; set; } = string.Empty;

    public int RokWydania { get; set; }

    // Wszystkie egzemplarze tej książki
    public ICollection<BookCopy> Egzemplarze { get; set; } = new List<BookCopy>();
}