using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP.NET_Exam.Data;
using ASP.NET_Exam.Data.Schema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ASP.NET_Exam.Controllers;

[Authorize]
public class ApplicationUserSchedulesController : Controller {
    private readonly ApplicationDataContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationUserSchedulesController(ApplicationDataContext context,
        UserManager<ApplicationUser> userManager) {
        _context = context;
        _userManager = userManager;
    }

    // GET: ApplicationUserSchedules
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Index() {
        var applicationDataContext = _context.ApplicationUserSchedules!
            .Include(a => a.Schedule)
            .Include(a => a.User);
        return View(await applicationDataContext.ToListAsync());
    }

    // GET: ApplicationUserSchedules/Details/5
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Details(string id) {
        if (id == null || _context.ApplicationUserSchedules == null) return NotFound();

        var applicationUserSchedule = await _context.ApplicationUserSchedules
            .Include(a => a.Schedule)
            .Include(a => a.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (applicationUserSchedule == null) return NotFound();

        return View(applicationUserSchedule);
    }

    // GET: ApplicationUserSchedules/Create
    [Authorize(Roles = "Manager")]
    public IActionResult Create() {
        ViewData["ScheduleId"] = new SelectList(_context.Schedules, "Id", "Name");
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
        return View();
    }

    // POST: ApplicationUserSchedules/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = "Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Id,UserId,ScheduleId")] ApplicationUserSchedule applicationUserSchedule) {
        if (ModelState.IsValid) {
            _context.Add(applicationUserSchedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["ScheduleId"] = new SelectList(_context.Schedules, "Id", "Name", applicationUserSchedule.ScheduleId);
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", applicationUserSchedule.UserId);
        return View(applicationUserSchedule);
    }

    [Authorize(Roles = "Client")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(string scheduleId) {
        var user = await _userManager.GetUserAsync(User);

        var schedule = await _context.Schedules!.FindAsync(scheduleId);

        if (user is not null && schedule is not null && User.IsInRole("Client") &&
            schedule.GroupSize < schedule.MaxGroupSize) {
            if (schedule.StartDateTime <= DateTime.Now)
                return NotFound();

            var applicationUserSchedule = new ApplicationUserSchedule {
                ScheduleId = scheduleId,
                UserId = user.Id,
                Id = Guid.NewGuid().ToString()
            };

            _context.Add(applicationUserSchedule);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), "Schedules", new { id = scheduleId });
    }

    // GET: ApplicationUserSchedules/Edit/5
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Edit(string id) {
        if (id == null || _context.ApplicationUserSchedules == null) return NotFound();

        var applicationUserSchedule = await _context.ApplicationUserSchedules.FindAsync(id);
        if (applicationUserSchedule == null) return NotFound();

        ViewData["ScheduleId"] = new SelectList(_context.Schedules, "Id", "Name", applicationUserSchedule.ScheduleId);
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", applicationUserSchedule.UserId);
        return View(applicationUserSchedule);
    }

    // POST: ApplicationUserSchedules/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Edit(string id,
        [Bind("Id,UserId,ScheduleId")] ApplicationUserSchedule applicationUserSchedule) {
        if (id != applicationUserSchedule.Id) return NotFound();

        if (ModelState.IsValid) {
            try {
                _context.Update(applicationUserSchedule);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ApplicationUserScheduleExists(applicationUserSchedule.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["ScheduleId"] = new SelectList(_context.Schedules, "Id", "Name", applicationUserSchedule.ScheduleId);
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", applicationUserSchedule.UserId);
        return View(applicationUserSchedule);
    }

    // GET: ApplicationUserSchedules/Delete/5
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Delete(string id) {
        if (id == null || _context.ApplicationUserSchedules == null) return NotFound();

        var applicationUserSchedule = await _context.ApplicationUserSchedules
            .Include(a => a.Schedule)
            .Include(a => a.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (applicationUserSchedule == null) return NotFound();

        return View(applicationUserSchedule);
    }

    [HttpPost]
    [ActionName("Unsubscribe")]
    [Authorize(Roles = "Client")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unsubscribe(string scheduleId) {
        if (_context.ApplicationUserSchedules == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        var applicationUserSchedule = await _context.ApplicationUserSchedules
            .Where(aus => aus.ScheduleId == scheduleId && aus.UserId == user.Id)
            .FirstOrDefaultAsync();

        if (applicationUserSchedule == null)
            return NotFound();

        _context.Remove(applicationUserSchedule);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), "Schedules", new { id = scheduleId });
    }

    // POST: ApplicationUserSchedules/Delete/5
    [Authorize(Roles = "Manager")]
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id) {
        if (_context.ApplicationUserSchedules == null)
            return Problem("Entity set 'ApplicationDataContext.ApplicationUserSchedules'  is null.");

        var applicationUserSchedule = await _context.ApplicationUserSchedules.FindAsync(id);
        if (applicationUserSchedule != null) _context.ApplicationUserSchedules.Remove(applicationUserSchedule);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ApplicationUserScheduleExists(string id) {
        return (_context.ApplicationUserSchedules?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}