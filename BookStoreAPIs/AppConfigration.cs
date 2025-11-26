
using BookStoreAPIs.Utility;
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
            services.AddScoped<IReposatory<OTPUser>, Reposatory<OTPUser>>();
        } 
        public static string TrimMoreThanOneSpace(this string word)
        {
            word = Regex.Replace(word, @"\s+", " ");
            return word;
        }
    }
}
