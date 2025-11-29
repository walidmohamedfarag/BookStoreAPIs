
using BookStoreAPIs.Utility;
using BookStoreAPIs.Utility.DBInitializer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.RegularExpressions;

namespace BookStoreAPIs
{
    public static class AppConfigration
    {
        public static void Configure(this IServiceCollection services, string connection)
        {
            services.AddDbContext<ApplicationDBContext>(option =>
            {
                option.UseSqlServer(connection);
            });
            services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.User.RequireUniqueEmail = true;
                option.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<ApplicationDBContext>()
            .AddDefaultTokenProviders();
            services.AddTransient<IEmailSender, Emailsender>();
            services.AddScoped<IReposatory<Book>, Reposatory<Book>>();
            services.AddScoped<IReposatory<Author>, Reposatory<Author>>();
            services.AddScoped<IReposatory<Category>, Reposatory<Category>>();
            services.AddScoped<IDBInitializer, DBInitializer>();
            services.AddScoped<IReposatory<OTPUser>, Reposatory<OTPUser>>();
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "https://localhost:7139",
                        ValidAudience = "https://localhost:7139",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ghkjjhkjkhjhjkjkjhjkhjhkkjkhjhnnmnmjkdsdsdsds"))
                    };
                });
        }
        public static string TrimMoreThanOneSpace(this string word)
        {
            word = Regex.Replace(word, @"\s+", " ");
            return word;
        }
    }
}
