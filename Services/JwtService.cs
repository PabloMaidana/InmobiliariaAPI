using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Inmobiliaria.Api.Services;
public class JwtOptions {
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public string Key { get; set; } = "";
}
public class JwtService {
    private readonly JwtOptions _opt;
    public JwtService(IOptions<JwtOptions> opt) { _opt = opt.Value; }

    public string CreateToken(int idPropietario, string email) {
        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, idPropietario.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_opt.Issuer, _opt.Audience, claims,
            expires: DateTime.UtcNow.AddHours(8), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}