using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;

[Authorize(Roles = "Admin")]
public class LoansController : Controller
{
    private readonly LibraryDbContext _context;

    public LoansController(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var loans = _context.Loans
            .Include(l => l.BookCopy)
            .ThenInclude(bc => bc.Book);

        return View(await loans.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var loan = await _context.Loans
            .Include(l => l.BookCopy)
            .ThenInclude(bc => bc.Book)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (loan == null) return NotFound();

        return View(loan);
    }

    public IActionResult Create()
    {
        var dostepneEgzemplarze = _context.BookCopies
            .Include(bc => bc.Book)
            .Where(bc => bc.Dostepny)
            .Select(bc => new
            {
                bc.Id,
                Nazwa = bc.Book!.Tytul + " - egzemplarz ID: " + bc.Id
            })
            .ToList();

        ViewData["BookCopyId"] = new SelectList(dostepneEgzemplarze, "Id", "Nazwa");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BookCopyId,Czytelnik")] Loan loan)
    {
        var bookCopy = await _context.BookCopies.FindAsync(loan.BookCopyId);

        if (bookCopy == null || !bookCopy.Dostepny)
        {
            ModelState.AddModelError("", "Wybrany egzemplarz nie jest dostępny.");
        }

        if (ModelState.IsValid)
        {
            loan.DataWypozyczenia = DateTime.UtcNow;
            loan.TerminZwrotu = DateTime.UtcNow.AddDays(14);
            loan.DataZwrotu = null;

            bookCopy!.Dostepny = false;

            _context.Add(loan);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        var dostepneEgzemplarze = _context.BookCopies
            .Include(bc => bc.Book)
            .Where(bc => bc.Dostepny)
            .Select(bc => new
            {
                bc.Id,
                Nazwa = bc.Book!.Tytul + " - egzemplarz ID: " + bc.Id
            })
            .ToList();

        ViewData["BookCopyId"] = new SelectList(dostepneEgzemplarze, "Id", "Nazwa", loan.BookCopyId);

        return View(loan);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int id)
    {
        var loan = await _context.Loans
            .Include(l => l.BookCopy)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null)
        {
            return NotFound();
        }

        if (loan.DataZwrotu == null)
        {
            loan.DataZwrotu = DateTime.UtcNow;

            if (loan.BookCopy != null)
            {
                loan.BookCopy.Dostepny = true;
            }

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var loan = await _context.Loans.FindAsync(id);

        if (loan == null) return NotFound();

        ViewData["BookCopyId"] = new SelectList(_context.BookCopies, "Id", "Id", loan.BookCopyId);

        return View(loan);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,BookCopyId,Czytelnik,DataWypozyczenia,TerminZwrotu,DataZwrotu")] Loan loan)
    {
        if (id != loan.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(loan);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanExists(loan.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["BookCopyId"] = new SelectList(_context.BookCopies, "Id", "Id", loan.BookCopyId);

        return View(loan);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var loan = await _context.Loans
            .Include(l => l.BookCopy)
            .ThenInclude(bc => bc.Book)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (loan == null) return NotFound();

        return View(loan);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var loan = await _context.Loans
            .Include(l => l.BookCopy)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan != null)
        {
            if (loan.BookCopy != null && loan.DataZwrotu == null)
            {
                loan.BookCopy.Dostepny = true;
            }

            _context.Loans.Remove(loan);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool LoanExists(int? id)
    {
        return _context.Loans.Any(e => e.Id == id);
    }
}