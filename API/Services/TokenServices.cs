using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entites;
using API.Interface;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenServices : ITokenService
    {
        private SymmetricSecurityKey _key{get;}
        public TokenServices(IConfiguration config )
        {
            _key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            var claims=new List<Claim>{
                new Claim(JwtRegisteredClaimNames.NameId,user.UserName)
            };
            var creds=new SigningCredentials(_key,SecurityAlgorithms.HmacSha512);
            
            var tokenDescriptor=new SecurityTokenDescriptor{
                Subject=new ClaimsIdentity(claims),
                Expires=DateTime.Now.AddHours(5),
                SigningCredentials=creds
            };
            var TokenHandler=new JwtSecurityTokenHandler();
            var Token=TokenHandler.CreateToken(tokenDescriptor);
            return TokenHandler.WriteToken(Token);
        }
    }
}