using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP.NET_Exam.Data;
using ASP.NET_Exam.Data.Schema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ASP.NET_Exam.Controllers;

[Authorize]
public class SchedulesController : Controller {
    private readonly ApplicationDataContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public SchedulesController(ApplicationDataContext context,
        UserManager<ApplicationUser> userManager) {
        _context = context;
        _userManager = userManager;
    }

    // GET: Schedules
    public async Task<IActionResult> Index() {
        var applicationDataContext = _context.Schedules!
            .OrderByDescending(s => s.StartDateTime)
            .Where(s => s.StartDateTime > DateTime.Now)
            .Include(s => s.Hall)
            .Include(s => s.Trainer);
        return View(await applicationDataContext.ToListAsync());
    }

    // GET: Schedules/Details/5
    public async Task<IActionResult> Details(string id) {
        if (id == null || _context.Schedules == null)
            return NotFound();

        var schedule = await _context.Schedules
            .Include(s => s.Hall)
            .Include(s => s.Trainer)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (schedule == null || (!User.IsInRole("Manager") && schedule.StartDateTime <= DateTime.Now))
            return NotFound();

        return View(schedule);
    }

    // GET: Schedules/Create
    [Authorize(Roles = "Manager")]
    public IActionResult Create() {
        ViewData["HallId"] = new SelectList(_context.Set<Hall>(), "Id", "Name");
        ViewData["TrainerId"] = new SelectList(_userManager.GetUsersInRoleAsync("Trainer").Result, "Id", "UserName");
        return View();
    }

    // POST: Schedules/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = "Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("MaxGroupSize,StartDateTime,TrainerId,HallId")]
        Schedule schedule) {
        if (ModelState.IsValid) {
            var user = await _context.Users.FindAsync(schedule.TrainerId);
            if (user is not null && await _userManager.IsInRoleAsync(user, "Trainer")) {
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        ViewData["HallId"] = new SelectList(_context.Set<Hall>(), "Id", "Name", schedule.HallId);
        ViewData["TrainerId"] = new SelectList(await _userManager.GetUsersInRoleAsync("Trainer"), "Id", "UserName",
            schedule.TrainerId);

        return View(schedule);
    }

    // GET: Schedules/Edit/5
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Edit(string id) {
        if (id == null || _context.Schedules == null)
            return NotFound();

        var schedule = await _context.Schedules.FindAsync(id);
        if (schedule == null)
            return NotFound();

        ViewData["HallId"] = new SelectList(_context.Set<Hall>(), "Id", "Name", schedule.HallId);
        ViewData["TrainerId"] = new SelectList(await _userManager.GetUsersInRoleAsync("Trainer"), "Id", "UserName",
            schedule.TrainerId);
        return View(schedule);
    }

    // POST: Schedules/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = "Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id,
        [Bind("Id,MaxGroupSize,GroupSize,StartDateTime,TrainerId,HallId")]
        Schedule schedule) {
        if (id != schedule.Id)
            return NotFound();

        if (ModelState.IsValid) {
            try {
                var user = await _context.Users.FindAsync(schedule.TrainerId);
                if (user is null || !await _userManager.IsInRoleAsync(user, "Trainer"))
                    return NotFound();

                _context.Update(schedule);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ScheduleExists(schedule.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["HallId"] = new SelectList(_context.Set<Hall>(), "Id", "Name", schedule.HallId);
        ViewData["TrainerId"] = new SelectList(await _userManager.GetUsersInRoleAsync("Trainer"), "Id", "UserName",
            schedule.TrainerId);
        return View(schedule);
    }

    // GET: Schedules/Delete/5
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Delete(string id) {
        if (id == null || _context.Schedules == null)
            return NotFound();

        var schedule = await _context.Schedules
            .Include(s => s.Hall)
            .Include(s => s.Trainer)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (schedule == null)
            return NotFound();

        return View(schedule);
    }

    // POST: Schedules/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteConfirmed(string id) {
        if (_context.Schedules == null)
            return Problem("Entity set 'ApplicationDataContext.Schedules'  is null.");

        var schedule = await _context.Schedules.FindAsync(id);

        if (!User.IsInRole("TopManager"))
            if (await IsUserHaveAccessToHall(schedule!.HallId))
                return Unauthorized();

        if (schedule != null) _context.Schedules.Remove(schedule);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> IsUserHaveAccessToHall(string hallId) {
        var hall = await _context.Halls!.FindAsync(hallId);
        var user = await _userManager.GetUserAsync(User);

        return hall!.ManagerId == user!.Id;
    }

    private bool ScheduleExists(string id) {
        return (_context.Schedules?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}