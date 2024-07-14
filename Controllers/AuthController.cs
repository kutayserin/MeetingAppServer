using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly string _uploadsFolder;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        if (!Directory.Exists(_uploadsFolder))
        {
            Directory.CreateDirectory(_uploadsFolder);
        }
    }
[HttpPost("register")]
public async Task<IActionResult> Register([FromForm] UserRegisterDto dto)
{
    if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
    {
        return BadRequest(new { message = "User with this email already exists." });
    }

    var user = new User
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Email = dto.Email,
        PhoneNumber = dto.PhoneNumber,
        Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
    };

    if (dto.ProfileImage != null)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{dto.ProfileImage.FileName}";
        var filePath = Path.Combine(_uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.ProfileImage.CopyToAsync(stream);
        }
        user.ProfileImageUrl = Path.Combine("uploads", uniqueFileName);
    }

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return Ok(new { message = "User registered successfully." });
}


    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
        {
            return Unauthorized("Invalid email or password.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }
}
