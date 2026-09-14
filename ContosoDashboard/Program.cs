using Microsoft.EntityFrameworkCore;
using ContosoDashboard.Data;
using ContosoDashboard.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add authentication state provider for Blazor
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Mock Authentication (Cookie-based for training purposes)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Employee", policy => policy.RequireRole("Employee", "TeamLead", "ProjectManager", "Administrator"));
    options.AddPolicy("TeamLead", policy => policy.RequireRole("TeamLead", "ProjectManager", "Administrator"));
    options.AddPolicy("ProjectManager", policy => policy.RequireRole("ProjectManager", "Administrator"));
    options.AddPolicy("Administrator", policy => policy.RequireRole("Administrator"));
});

// Register application services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddSingleton<IDocumentStorageService, LocalDocumentStorageService>();

// Add HttpContextAccessor for accessing user claims
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated(); // For development - use migrations in production
        EnsureDocumentSchema(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the database.");
    }
}

static void EnsureDocumentSchema(ApplicationDbContext context)
{
    context.Database.ExecuteSqlRaw(@"
IF OBJECT_ID(N'[Documents]', N'U') IS NULL
BEGIN
    CREATE TABLE [Documents]
    (
        [DocumentId] int NOT NULL IDENTITY,
        [Title] nvarchar(255) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [Category] nvarchar(100) NOT NULL,
        [UploaderId] int NOT NULL,
        [ProjectId] int NULL,
        [StoredFileName] nvarchar(255) NOT NULL,
        [StoredFilePath] nvarchar(1024) NOT NULL,
        [FileSizeBytes] int NOT NULL,
        [ContentType] nvarchar(255) NOT NULL,
        [UploadedUtc] datetime2 NOT NULL,
        [IsDeleted] bit NOT NULL CONSTRAINT [DF_Documents_IsDeleted] DEFAULT 0,
        CONSTRAINT [PK_Documents] PRIMARY KEY ([DocumentId]),
        CONSTRAINT [FK_Documents_Users_UploaderId] FOREIGN KEY ([UploaderId]) REFERENCES [Users] ([UserId]),
        CONSTRAINT [FK_Documents_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([ProjectId])
    );

    CREATE INDEX [IX_Documents_UploaderId_IsDeleted] ON [Documents] ([UploaderId], [IsDeleted]);
    CREATE INDEX [IX_Documents_ProjectId] ON [Documents] ([ProjectId]);
END

IF OBJECT_ID(N'[DocumentShares]', N'U') IS NULL
BEGIN
    CREATE TABLE [DocumentShares]
    (
        [ShareId] int NOT NULL IDENTITY,
        [DocumentId] int NOT NULL,
        [SharedByUserId] int NOT NULL,
        [SharedWithUserId] int NOT NULL,
        [SharedUtc] datetime2 NOT NULL,
        [NotificationSent] bit NOT NULL CONSTRAINT [DF_DocumentShares_NotificationSent] DEFAULT 0,
        [IsActive] bit NOT NULL CONSTRAINT [DF_DocumentShares_IsActive] DEFAULT 1,
        CONSTRAINT [PK_DocumentShares] PRIMARY KEY ([ShareId]),
        CONSTRAINT [FK_DocumentShares_Documents_DocumentId] FOREIGN KEY ([DocumentId]) REFERENCES [Documents] ([DocumentId]) ON DELETE CASCADE,
        CONSTRAINT [FK_DocumentShares_Users_SharedByUserId] FOREIGN KEY ([SharedByUserId]) REFERENCES [Users] ([UserId]),
        CONSTRAINT [FK_DocumentShares_Users_SharedWithUserId] FOREIGN KEY ([SharedWithUserId]) REFERENCES [Users] ([UserId])
    );

    CREATE INDEX [IX_DocumentShares_DocumentId_SharedWithUserId_IsActive]
        ON [DocumentShares] ([DocumentId], [SharedWithUserId], [IsActive]);
END");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    // Use HSTS even in development for training purposes
    app.UseHsts();
}

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    
    // Content Security Policy for Blazor Server
    context.Response.Headers["Content-Security-Policy"] = 
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net; " +
        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
        "font-src 'self' https://cdn.jsdelivr.net; " +
        "img-src 'self' data: https:; " +
        "connect-src 'self' wss: ws:;";
    
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
