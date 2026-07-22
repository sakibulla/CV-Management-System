using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CVManagementSystem.Models.Domain;

namespace CVManagementSystem.Controllers;

[Authorize]
public class DiagnosticsController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DiagnosticsController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> MyInfo()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Content("Not logged in");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();

        var info = $@"
<html>
<head><title>User Diagnostics</title></head>
<body style='font-family: monospace; padding: 20px;'>
    <h2>User Diagnostics</h2>
    <hr>
    <h3>User Information:</h3>
    <p><strong>Email:</strong> {user.Email}</p>
    <p><strong>User ID:</strong> {user.Id}</p>
    <p><strong>First Name:</strong> {user.FirstName}</p>
    <p><strong>Last Name:</strong> {user.LastName}</p>
    <p><strong>Is Blocked:</strong> {user.IsBlocked}</p>
    
    <hr>
    <h3>Roles ({roles.Count}):</h3>
    <ul>
        {string.Join("", roles.Select(r => $"<li><strong>{r}</strong></li>"))}
        {(roles.Count == 0 ? "<li><em>No roles assigned</em></li>" : "")}
    </ul>
    
    <hr>
    <h3>All Claims ({claims.Count}):</h3>
    <ul>
        {string.Join("", claims.Select(c => $"<li>{c}</li>"))}
    </ul>
    
    <hr>
    <h3>Authorization Checks:</h3>
    <p><strong>Is in 'Candidate' role:</strong> {User.IsInRole("Candidate")}</p>
    <p><strong>Is in 'Recruiter' role:</strong> {User.IsInRole("Recruiter")}</p>
    <p><strong>Is in 'Admin' role:</strong> {User.IsInRole("Admin")}</p>
    
    <hr>
    <p><a href='/'>Go to Home</a> | <a href='/QuickAdmin/MakeAdmin'>Quick Admin (if admin)</a></p>
</body>
</html>
";

        return Content(info, "text/html");
    }
}
