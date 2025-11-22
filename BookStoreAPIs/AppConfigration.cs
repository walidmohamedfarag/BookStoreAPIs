
namespace BookStoreAPIs
{
    public static class AppConfigration
    {
        public static void Configure(this IServiceCollection services , string connection)
        {
            services.AddDbContext<ApplicationDBContext>(option =>
            {
                option.UseSqlServer(connection);
            });
            services.AddScoped<IReposatory<Book> , Reposatory<Book>>();
            services.AddScoped<IReposatory<Author> , Reposatory<Author>>();
            services.AddScoped<IReposatory<Category> , Reposatory<Category>>();
        }
    }
}
