
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
            services.AddScoped<IReposatory<Book>, Reposatory<Book>>();
            services.AddScoped<IReposatory<Author>, Reposatory<Author>>();
            services.AddScoped<IReposatory<Category>, Reposatory<Category>>();
        } 
        public static string TrimMoreThanOneSpace(this string word)
        {
            word = Regex.Replace(word, @"\s+", " ");
            return word;
        }
    }
}
