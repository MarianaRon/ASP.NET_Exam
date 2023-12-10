using ASP.NET_Exam.Data;
using ASP.NET_Exam.Data.Schema;
using ASP.NET_Exam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_Exam.Controllers;

[Authorize]
public class UsersController : Controller {
    private readonly ApplicationDataContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(ApplicationDataContext context,
        UserManager<ApplicationUser> userManager
    ) {
        _context = context;
        _userManager = userManager;
    }

    // GET: Specializations
    public async Task<IActionResult> Index() {
        if (_context.Users == null) return NotFound();

        var users = User.IsInRole("Manager")
            ? _context.Users.ToList()
            : _userManager.GetUsersInRoleAsync("Trainer").Result.ToList();

        var usersDetails = users.Select(user => new UserDetailsModel { Id = user.Id, UserName = user.UserName })
            .ToList();
        return View(usersDetails);
    }

    // GET: Specializations/Details/5
    public async Task<IActionResult> Details(string id) {
        if (id == null || _context.Users == null) return NotFound();

        var user = await _context.Users.FindAsync(id);

        if (user == null) return NotFound();

        if (!User.IsInRole("Manager") && !await _userManager.IsInRoleAsync(user, "Trainer")) return NotFound();

        var userFormModel = new UserDetailsModel {
            Id = user.Id,
            UserName = user.UserName
        };

        return View(userFormModel);
    }

    // GET: Specializations/Details/5
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> ExtendedDetails(string id) {
        if (id == null || !_context.Users.Any()) return NotFound();

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        var userSpecializations = _context.ApplicationUserSpecifications!.Where(aus => aus.UserId == user.Id)
            .Include(aus => aus.Specialization).Select(aus => aus.Specialization).ToList();
        var roleNames = _userManager.GetRolesAsync(user).Result.ToList();

        var userModel = new UserExtendedDetailsModel {
            Id = user.Id,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            UserName = user.UserName!,
            BirthDate = user.BirthDate,
            Gender = user.Gender,
            RoleNames = roleNames,
            SpecializationIdStrings = null!,
            Specializations = userSpecializations!
        };

        if (userModel == null) return NotFound();

        return View(userModel);
    }

    // GET: Specializations/Create
    [Authorize(Roles = "Manager")]
    public IActionResult Create() {
        ViewData["SpecializationId"] = new SelectList(_context.Set<Specialization>().ToList(), "Id", "Name");
        ViewData["RoleName"] = new SelectList(_context.Roles.Select(r => r.Name).ToList());

        return View();
    }

    // POST: Specializations/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Create(
        [Bind(
            "Email, PhoneNumber, UserName, BirthDate, Gender, Password, ConfirmPassword, RoleNames, SpecializationIdStrings")]
        UserExtendedRegisterModel userDetailsModel) {
        if (!ModelState.IsValid) {
            ViewData["SpecializationId"] = new SelectList(_context.Set<Specialization>().ToList(), "Id", "Name");
            ViewData["RoleName"] = new SelectList(_context.Roles.Select(r => r.Name).ToList());
            return View(userDetailsModel);
        }

        var user = new ApplicationUser(userDetailsModel.UserName);

        await _userManager.SetEmailAsync(user, userDetailsModel.Email);

        user.PhoneNumber = userDetailsModel.PhoneNumber;
        user.BirthDate = userDetailsModel.BirthDate;
        user.Gender = userDetailsModel.Gender;
        var result = await _userManager.CreateAsync(user, userDetailsModel.Password);

        if (result.Succeeded) {
            await _userManager.AddToRolesAsync(user, userDetailsModel.RoleNames!);

            var userSpecializations = userDetailsModel.SpecializationIdStrings!.Select(specializationId =>
                    new ApplicationUserSpecialization
                        { Id = Guid.NewGuid().ToString(), UserId = user.Id, SpecializationId = specializationId })
                .ToList();

            await _context.ApplicationUserSpecifications?.AddRangeAsync(userSpecializations)!;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Specializations/Edit/5
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Edit(string id) {
        if (id == null || !_context.Users.Any()) return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var userSpecializationIds = _context.ApplicationUserSpecifications!.Where(aus => aus.UserId == user.Id)
            .Include(aus => aus.Specialization).Select(aus => aus.SpecializationId).ToList();
        var roleNames = _userManager.GetRolesAsync(user).Result.ToList();

        var userModel = new UserExtendedDetailsModel {
            Id = user.Id,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            UserName = user.UserName!,
            BirthDate = user.BirthDate,
            Gender = user.Gender,
            RoleNames = roleNames,
            SpecializationIdStrings = userSpecializationIds,
            Specializations = null
        };
        if (userModel == null) return NotFound();

        ViewData["SpecializationId"] = new SelectList(_context.Set<Specialization>().ToList(), "Id", "Name");
        ViewData["RoleName"] = new SelectList(_context.Roles.Select(r => r.Name).ToList());

        return View(userModel);
    }

    // POST: Specializations/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Edit(string id, [Bind(
            "Id, Email, PhoneNumber, UserName, BirthDate, Gender, Password, ConfirmPassword, RoleNames, SpecializationIdStrings")]
        UserExtendedDetailsModel userDetailsModel) {
        if (id != userDetailsModel.Id) return NotFound();

        if (ModelState.IsValid) {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _userManager.SetEmailAsync(user, userDetailsModel.Email);

            user.PhoneNumber = userDetailsModel.PhoneNumber;
            user.BirthDate = userDetailsModel.BirthDate;
            user.Gender = userDetailsModel.Gender;

            var oldUserRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, oldUserRoles);

            await _userManager.AddToRolesAsync(user, userDetailsModel.RoleNames!);

            var specializations = _context.ApplicationUserSpecifications!.Where(aus => aus.UserId == user.Id).ToList();
            _context.ApplicationUserSpecifications!.RemoveRange(specializations);

            var userSpecializations = userDetailsModel.SpecializationIdStrings!.Select(specializationId =>
                    new ApplicationUserSpecialization
                        { Id = Guid.NewGuid().ToString(), UserId = user.Id, SpecializationId = specializationId })
                .ToList();

            await _context.ApplicationUserSpecifications?.AddRangeAsync(userSpecializations)!;
            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["SpecializationId"] = new SelectList(_context.Set<Specialization>().ToList(), "Id", "Name");
        ViewData["RoleName"] = new SelectList(_context.Roles.Select(r => r.Name).ToList());

        return View(userDetailsModel);
    }

    // GET: Specializations/Delete/5
    [Authorize(Roles = "TopManager")]
    public async Task<IActionResult> Delete(string id) {
        if (id == null || !_context.Users.Any()) return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var userFormModel = new UserDetailsModel {
            Id = user.Id,
            UserName = user.UserName
        };
        if (userFormModel == null) return NotFound();

        return View(userFormModel);
    }

    // POST: Specializations/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TopManager")]
    public async Task<IActionResult> DeleteConfirmed(string id) {
        if (!_context.Users.Any())
            return Problem("Entity set 'ApplicationDataContext.Users' is clear.");

        var user = await _userManager.FindByIdAsync(id);
        if (user != null) {
            var userId = user.Id;
            await _userManager.DeleteAsync(user);

            var aUserSchedules =
                await _context.ApplicationUserSchedules!.Where(aus => aus.UserId == userId).ToListAsync();
            _context.ApplicationUserSchedules!.RemoveRange(aUserSchedules);
            var aUserSpecifications = await _context.ApplicationUserSpecifications!.Where(aus => aus.UserId == userId)
                .ToListAsync();
            _context.ApplicationUserSpecifications!.RemoveRange(aUserSpecifications);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool SpecializationExists(string id) {
        return (_context.Specializations?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}