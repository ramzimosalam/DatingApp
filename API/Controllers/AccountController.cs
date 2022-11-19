using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using API.DTOs;
using API.Interface;

namespace API.Controllers
{
    public class AccountController : BaseAPIController
    {
        public DataContext Context { get; }
        private readonly ITokenService tokenService;
        public AccountController(DataContext context,ITokenService tokenService)
        {
            this.tokenService = tokenService;
            this.Context = context;
        }
        [HttpPost("Register")]
        public async Task<object> Register(RegisterDto registerDto){
            // string Name,string UserName,string Password
            if (await UserExits(registerDto.UserName)){
                return BadRequest("{\"status\":\"000\",\"message\":\"UserName Is Token\"}");
                // "{code:400,message:\"UserName Is Token \"}";
            }
            using var hmac=new HMACSHA512();
            var User=new AppUser{
                Name= registerDto.Name,
                UserName=registerDto.UserName.ToLower(),
                Password=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)) ,
                PasswordSalt=hmac.Key

            };
            var DataJsonSer = JsonSerializer.Serialize(User);
            // Context.Database.OpenConnection();
            Context.Database.SetCommandTimeout(100);
            var ProcdData=    new SqlParameter("@Data", DataJsonSer);
            var ProcdMethod=    new SqlParameter("@MethodName", "CreateUser");
            var ProcdResult=    new SqlParameter("@ResponseResult", SqlDbType.VarChar,1000){ Direction = ParameterDirection.Output } ;
            // Context.Database.SetCommandTimeout(1000);
            await  Context.Database.ExecuteSqlRawAsync("EXEC [usp_Create_Update_Users] @Data={0}, @MethodName ={1}, @ResponseResult={2} OUTPUT",ProcdData,ProcdMethod,ProcdResult );
            ObjectResult result;
            if(ProcdResult.Value.Equals("SUCCESSFUL_OPERATIONS")){
                result=StatusCode(200,"{\"status\":\"000\",\"message\":\"SUCCESSFUL_OPERATIONS\"}");
            }else{
               result=BadRequest("{\"status\":\"400\",\"message\":\"ERROR_OPERATIONS\"}");  
            }
            return new UserDTOs{
                UserName=registerDto.UserName,
                Token=tokenService.CreateToken(User)
            };
        }
        [HttpPost("Login")]
         public async Task<object> Login(LoginDto loginDto){

            AppUser user;
            try{
                user=await Context.Users.SingleOrDefaultAsync(user=>user.UserName==loginDto.UserName);
            }catch(Exception e){
                user=null;
            }
            
            if(user==null)return Unauthorized("Invaild UserName");
            using var hmac=new HMACSHA512(user.PasswordSalt);
            var User=new LoginDto{
                UserName=loginDto.UserName,
                Password=loginDto.Password,
                PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password))
                };
            var DataJsonSer = JsonSerializer.Serialize(User);
            // Context.Database.OpenConnection();
            Context.Database.SetCommandTimeout(100);
            var ProcdData=    new SqlParameter("@Data", DataJsonSer);
            var ProcdMethod=    new SqlParameter("@MethodName", "LoginUser");
            var ProcdResult=    new SqlParameter("@ResponseResult", SqlDbType.VarChar,1000){ Direction = ParameterDirection.Output } ;
            // Context.Database.SetCommandTimeout(1000);
            await  Context.Database.ExecuteSqlRawAsync("EXEC [usp_Create_Update_Users] @Data={0}, @MethodName ={1}, @ResponseResult={2} OUTPUT",ProcdData,ProcdMethod,ProcdResult );
            ObjectResult result;
            if(ProcdResult.Value.Equals("SUCCESSFUL_OPERATIONS")){
                result=StatusCode(200, "{\"status\":\"000\",\"message\":\"SUCCESSFUL_OPERATIONS\"}");
            }else{
               result=BadRequest("{\"status\":\"400\",\"message\":\"USERNAME_OR_PASSWORD_ERROR\"}");  
            }
            return new UserDTOs{
                UserName=user.UserName,
                Token=tokenService.CreateToken(user)
            };
         }
        private async Task<bool>UserExits(string UserName){
            return await Context.Users.AnyAsync(user=>user.UserName==UserName.ToLower());
        }
    }
}