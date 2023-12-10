using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASP.NET_Exam.Data;
using ASP.NET_Exam.Data.Schema;
using Microsoft.AspNetCore.Authorization;

namespace ASP.NET_Exam.Controllers;

[Authorize(Roles = "TopManager")]
public class SpecializationsController : Controller {
    private readonly ApplicationDataContext _context;

    public SpecializationsController(ApplicationDataContext context) {
        _context = context;
    }

    // GET: Specializations
    public async Task<IActionResult> Index() {
        return _context.Specializations != null
            ? View(await _context.Specializations.ToListAsync())
            : Problem("Entity set 'ApplicationDataContext.Specializations'  is null.");
    }

    // GET: Specializations/Details/5
    public async Task<IActionResult> Details(string id) {
        if (id == null || _context.Specializations == null) return NotFound();

        var specialization = await _context.Specializations
            .FirstOrDefaultAsync(m => m.Id == id);
        if (specialization == null) return NotFound();

        return View(specialization);
    }

    // GET: Specializations/Create
    public IActionResult Create() {
        return View();
    }

    // POST: Specializations/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name")] Specialization specialization) {
        if (ModelState.IsValid) {
            _context.Add(specialization);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(specialization);
    }

    // GET: Specializations/Edit/5
    public async Task<IActionResult> Edit(string id) {
        if (id == null || _context.Specializations == null) return NotFound();

        var specialization = await _context.Specializations.FindAsync(id);
        if (specialization == null) return NotFound();

        return View(specialization);
    }

    // POST: Specializations/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,Name")] Specialization specialization) {
        if (id != specialization.Id) return NotFound();

        if (ModelState.IsValid) {
            try {
                _context.Update(specialization);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!SpecializationExists(specialization.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(specialization);
    }

    // GET: Specializations/Delete/5
    public async Task<IActionResult> Delete(string id) {
        if (id == null || _context.Specializations == null) return NotFound();

        var specialization = await _context.Specializations
            .FirstOrDefaultAsync(m => m.Id == id);
        if (specialization == null) return NotFound();

        return View(specialization);
    }

    // POST: Specializations/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id) {
        if (_context.Specializations == null)
            return Problem("Entity set 'ApplicationDataContext.Specializations'  is null.");

        var specialization = await _context.Specializations.FindAsync(id);
        if (specialization != null) _context.Specializations.Remove(specialization);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SpecializationExists(string id) {
        return (_context.Specializations?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}