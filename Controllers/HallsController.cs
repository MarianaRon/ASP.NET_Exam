using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASP.NET_Exam.Data;
using ASP.NET_Exam.Data.Schema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASP.NET_Exam.Controllers;

[Authorize]
public class HallsController : Controller {
    private readonly ApplicationDataContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HallsController(ApplicationDataContext context,
        UserManager<ApplicationUser> userManager) {
        _context = context;
        _userManager = userManager;
    }

    // GET: Halls
    public async Task<IActionResult> Index() {
        return _context.Halls != null
            ? View(await _context.Halls.Include(h => h.Manager).ToListAsync())
            : Problem("Entity set 'ApplicationDataContext.Halls'  is null.");
    }

    // GET: Halls/Details/5
    public async Task<IActionResult> Details(string id) {
        if (id == null || _context.Halls == null) return NotFound();

        var hall = await _context.Halls.Include(h => h.Schedules)!
            .ThenInclude(t => t.Trainer)
            .Include(h => h.Manager)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (hall == null) return NotFound();

        return View(hall);
    }

    // GET: Halls/Create
    [Authorize(Roles = "TopManager")]
    public IActionResult Create() {
        ViewData["ManagerId"] = new SelectList(_context.Users, "Id", "UserName");

        return View();
    }

    // POST: Halls/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TopManager")]
    public async Task<IActionResult> Create([Bind("Name,ManagerId")] Hall hall) {
        if (ModelState.IsValid) {
            _context.Add(hall);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["ManagerId"] = new SelectList(_context.Users, "Id", "UserName");

        return View(hall);
    }

    // GET: Halls/Edit/5
    [Authorize(Roles = "TopManager")]
    public async Task<IActionResult> Edit(string id) {
        if (id == null || _context.Halls == null) return NotFound();

        var hall = await _context.Halls.FindAsync(id);
        if (hall == null) return NotFound();
        ViewData["ManagerId"] = new SelectList(_context.Users, "Id", "UserName");

        return View(hall);
    }

    // POST: Halls/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = "TopManager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,Name,ManagerId")] Hall hall) {
        if (id != hall.Id) return NotFound();

        if (ModelState.IsValid) {
            try {
                _context.Update(hall);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!HallExists(hall.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["ManagerId"] = new SelectList(_context.Users, "Id", "UserName");

        return View(hall);
    }

    // GET: Halls/Delete/5
    [Authorize(Roles = "TopManager")]
    public async Task<IActionResult> Delete(string id) {
        if (id == null || _context.Halls == null) return NotFound();

        var hall = await _context.Halls.Include(h => h.Manager)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (hall == null) return NotFound();

        return View(hall);
    }

    // POST: Halls/Delete/5
    [Authorize(Roles = "TopManager")]
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id) {
        if (_context.Halls == null)
            return Problem("Entity set 'ApplicationDataContext.Halls'  is null.");

        var hall = await _context.Halls.FindAsync(id);
        if (hall != null) _context.Halls.Remove(hall);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> IsUserHaveAccessToHall(string hallId) {
        var hall = await _context.Halls!.FindAsync(hallId);
        var user = await _userManager.GetUserAsync(User);

        return hall!.ManagerId == user!.Id;
    }

    private bool HallExists(string id) {
        return (_context.Halls?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}