using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RESTfulAPI.Models;
using RESTfulAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RESTfulAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly String secretKey;

        public MemberController(IConfiguration config)
        {
            secretKey = config.GetSection("Settings").GetSection("SecretKey").ToString();
        }

        #region Log In
        [HttpPost]
        [Route("LogIn")]
        public async Task<IActionResult> LogIn(MemberService newMemberService)
        {
            using (RestdatabaseContext context = new RestdatabaseContext())
            {
                var members = context.Members
                    .Where(member => member.Username == newMemberService.Username)
                    .ToList();

                var memberDAL = members.FirstOrDefault(member =>
                    HashVerifier(newMemberService.Password.Trim(), member.Password.Trim()));

                if (memberDAL != null && UsernameComparison(newMemberService.Username.Trim(), memberDAL.Username.Trim()))
                {
                    return Ok(CreateToken(memberDAL.Id));
                }

                return NotFound();
            }
        }

        private Boolean HashVerifier(String plainPassword, String hashedPassword)
        {
            Boolean validPassword = BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);

            return validPassword;
        }

        private String CreateToken(String ID)
        {
            var keyBytes = Encoding.ASCII.GetBytes(secretKey);
            var claims = new ClaimsIdentity();

            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, ID));

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claims,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            String createdToken = tokenHandler.WriteToken(tokenConfig);

            return createdToken;
        }

        private Boolean PasswordComparison(String input, String source)
        {

            if (String.Equals(input, source, StringComparison.CurrentCulture))
            {

                return true;
            }

            return false;
        }

        private Boolean UsernameComparison(String input, String source)
        {
            if (String.Equals(input, source, StringComparison.CurrentCulture))
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Sign Up
        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp(MemberService newMember)
        {
            using (RestdatabaseContext context = new RestdatabaseContext())
            {
                var memberDAL = context.Members.FirstOrDefault(member => member.Username == newMember.Username
                                                                         && member.Password == newMember.Password);
                if (memberDAL != null)
                {
                    return BadRequest();
                }

                newMember.Password = Hash(newMember.Password);

                context.Members.Add(MappingSignUp(newMember));
                await context.SaveChangesAsync();

                return Created();
            }
        }

        private String Hash(String plainPassword)
        {
            String hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            return hashedPassword;
        }

        private Member MappingSignUp(MemberService memberService)
        {
            var newMember = new Member()
            {

                Id = memberService.ID,
                Username = memberService.Username,
                Password = memberService.Password
            };

            return newMember;
        }
        #endregion
    }
}
