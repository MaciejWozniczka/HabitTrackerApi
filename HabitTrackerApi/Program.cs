var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

var services = builder.Services;

services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(cfg =>
    {
        cfg.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = configuration["Authentication:Issuer"],
            ValidAudience = configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:SecretKey"]))
        };
    });

services.AddAuthorization(o =>
{
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
    defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
    o.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
});

IdentityModelEventSource.ShowPII = false;
services.AddScoped<IUserService, UserService>();

services.AddCors(o => o.AddPolicy("default", builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

services.AddIdentity<User, IdentityRole>(cfg =>
{
    cfg.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<DataContext>();

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddMediatR(typeof(Program));
services.AddValidatorsFromAssembly(typeof(Program).Assembly);

services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

services.AddScoped<IGarminService, GarminService>();
services.AddScoped<IHabitService, HabitService>();
services.AddScoped<IUserService, UserService>();

services.Configure<TokenOption>(configuration.GetSection("Authentication"));

services.AddControllers();

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MobileApp", Version = "v1" });
    c.EnableAnnotations();

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {{ jwtSecurityScheme, Array.Empty<string>() }});
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("default");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HabitTracker v1"));

var modules = new IModule[]
{
    new HabitTrackerApi.Module()
};

foreach (var module in modules)
{
    await module.Run(app.Services);
}

app.Run();

public partial class Program { }