namespace CustomerApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // The connection string containing the address, credentials, and database name for your PostgreSQL instance.
        var connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=mysecretpassword";

        // Registers your Database Context (CustomerDb) to use the Npgsql provider for PostgreSQL.
        services.AddDbContext<CustomerDb>(options =>
            options.UseNpgsql(connectionString, x => x.MigrationsAssembly("Customer")));

        // Register the Logger: Whenever a class asks for IActivityLogger, give it MongoActivityLogger
        services.AddScoped<IActivityLogger, MongoActivityLogger>();

            // --- HANGFIRE CONFIGURATION ---
        // 1. Tell Hangfire to use your Postgres database to store job data
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

        // 2. Add the Hangfire Server (the background worker that actually runs the jobs)
        services.AddHangfireServer();

        // The default Redis port is 6379
        var redisConnection = "localhost:6379"; 

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "CustomerAPI_"; // Keeps your keys organized
        });

        // API Rate Limiting
        var redis = ConnectionMultiplexer.Connect(redisConnection);
        services.AddRateLimiter(options =>
        {
            options.AddRedisFixedWindowLimiter("fixed", opt =>
            {
                opt.ConnectionMultiplexerFactory = () => redis;
                opt.Window = TimeSpan.FromSeconds(10);
                opt.PermitLimit = 5; // Only 5 requests every 10 seconds
            });
        });

        // 1. Retrieves the secret string from appsettings.json to use for digital signatures.
        var secretKey = configuration["Jwt:Key"];

        // 2. Registers the Authentication service and sets JWT as the default scheme.
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // 3. Defines the strict "checklist" the server uses to decide if a token is valid.
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,           // Ensure the token came from YOUR server.
                    ValidateAudience = true,         // Ensure the token was meant for YOUR client app.
                    ValidateLifetime = true,         // Ensure the token hasn't expired.
                    ValidateIssuerSigningKey = true, // Ensure the digital "seal" hasn't been tampered with.
                    
                    ValidIssuer = "your-api",        // The expected name of your API.
                    ValidAudience = "your-client",   // The expected name of your frontend.
                    
                    // Converts your secret string into a mathematical key for verification.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
                };
            });

        // Register the MongoDB Activity Logger
        services.AddScoped<IActivityLogger, MongoActivityLogger>();

        ConfigureMongoMappings();

        return services;
    }

    private static void ConfigureMongoMappings()
    {
        // Only register if it hasn't been registered yet to avoid errors during integration tests
        if (!BsonClassMap.IsClassMapRegistered(typeof(CustomerActivity)))
        {
            BsonClassMap.RegisterClassMap<CustomerActivity>(cm => 
            {
                cm.AutoMap();
                
                // Maps the 'Id' property to the MongoDB _id and tells it to treat it as a string
                cm.MapIdMember(c => c.Id)
                  .SetIdGenerator(StringObjectIdGenerator.Instance)
                  .SetSerializer(new StringSerializer(BsonType.ObjectId));

                // Ensures the Guid is stored in a readable string format if desired
                cm.MapMember(c => c.CustomerId)
                  .SetSerializer(new GuidSerializer(BsonType.String));
            });
        }
    }
}