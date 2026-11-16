using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpGet("sessao")]
        public IActionResult Sessao()
        {
            var usuario = HttpContext.User;
            return Ok(new { nome = usuario.Identity?.Name });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] string nome, [FromForm] string senha)
        {
            var nomeValido = new[] { "Mateus", "Pietro" }.FirstOrDefault(administrador =>
                string.Equals(administrador, nome.Trim(), StringComparison.OrdinalIgnoreCase));
            if (nomeValido is null || senha != "1234")
                return LocalRedirect("/login.html?erro=1");
            var identidade = new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, nomeValido)],
                CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identidade));
            return LocalRedirect("/admin.html");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/login.html");
        }
    }
}
