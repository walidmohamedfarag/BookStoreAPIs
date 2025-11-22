using BookStoreAPIs.DataAccess;

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
        }
    }
}
