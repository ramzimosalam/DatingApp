using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Interface;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplictionServices(this IServiceCollection services,IConfiguration _config){

            services.AddScoped<ITokenService,TokenServices>();
            services.AddDbContext<DataContext>(options=>{
                options.UseSqlServer(_config.GetConnectionString("DEFAULT_CONNECTION"));
            });
            return services;
        }
    }
}