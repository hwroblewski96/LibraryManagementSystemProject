using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;

[Authorize]
public class BookCopiesController : Controller
{
    private readonly LibraryDbContext _context;

    public BookCopiesController(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var bookCopies = _context.BookCopies.Include(b => b.Book);
        return View(await bookCopies.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var bookCopy = await _context.BookCopies
            .Include(b => b.Book)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (bookCopy == null)
        {
            return NotFound();
        }

        return View(bookCopy);
    }

    public IActionResult Create()
    {
        ViewData["BookId"] = new SelectList(_context.Books, "Id", "Tytul");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,BookId,Dostepny")] BookCopy bookCopy)
    {
        if (ModelState.IsValid)
        {
            bookCopy.Dostepny = true;

            _context.Add(bookCopy);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        ViewData["BookId"] = new SelectList(_context.Books, "Id", "Tytul", bookCopy.BookId);
        return View(bookCopy);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var bookCopy = await _context.BookCopies.FindAsync(id);

        if (bookCopy == null)
        {
            return NotFound();
        }

        ViewData["BookId"] = new SelectList(_context.Books, "Id", "Tytul", bookCopy.BookId);

        return View(bookCopy);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,BookId,Dostepny")] BookCopy bookCopy)
    {
        if (id != bookCopy.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(bookCopy);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookCopyExists(bookCopy.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["BookId"] = new SelectList(_context.Books, "Id", "Tytul", bookCopy.BookId);

        return View(bookCopy);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var bookCopy = await _context.BookCopies
            .Include(b => b.Book)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (bookCopy == null)
        {
            return NotFound();
        }

        return View(bookCopy);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var bookCopy = await _context.BookCopies.FindAsync(id);

        if (bookCopy != null)
        {
            _context.BookCopies.Remove(bookCopy);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool BookCopyExists(int? id)
    {
        return _context.BookCopies.Any(e => e.Id == id);
    }
}