using BusinessLogic.Services.Tokens.Implementation;
using BusinessLogic.Services.Tokens.Interface;
using BusinessLogic.Services.Users.Implementation;
using BusinessLogic.Services.Users.Interface;
using BusinessLogic.Services.TwoFactorsAuth.Interface;
using BusinessLogic.Services.TwoFactorsAuth.Implementation;
using BusinessLogic.Services.Emails.Interface;
using BusinessLogic.Services.Emails.Implementation;
using BusinessLogic.Services.ApiKeys.Interface;
using BusinessLogic.Services.ApiKeys.Implemention;
using BusinessLogic.Services.Ips.Implementation;
using BusinessLogic.Services.OAuths.Interface;
using BusinessLogic.Services.OAuths.Implementation;
using BusinessLogic.Services.Ips.Interface;
using DAL;
using DAL.Repositories.Users.Interface;
using DAL.Repositories.Users.Implementation;
using DAL.Repositories.RefreshTokens.Interface;
using DAL.Repositories.RefreshTokens.Implementation;
using DAL.Repositories.ApiKeys.Interface;
using DAL.Repositories.ApiKeys.Implementation;
using DAL.Repositories.WhiteListIps.Implementation;
using DAL.Repositories.WhiteListIps.Interface;
using DAL.Repositories.StorageLinkPolicies.Interface;
using DAL.Repositories.Storages.Interface;
using DAL.Repositories.StoragePolicies.Interface;
using DAL.Repositories.UserRoles.Interface;
using DAL.Repositories.Roles.Interface;
using DAL.Repositories.RoleTypes.Interface;
using DAL.Repositories.Roles.Implementation;
using DAL.Repositories.UserRoles.Implementation;
using DAL.Repositories.RoleTypes.Implementation;
using DAL.Repositories.StoragePolicies.Implementation;
using DAL.Repositories.Storages.Implementation;
using DAL.Repositories.StorageLinkPolicies.Implementation;
using Encryptarium.Auth.Requirements;
using Encryptarium.Auth.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Model;
using Encryptarium.Auth.Middlewares;
using BusinessLogic.Services.RabbitMQ.Implementation;
using BusinessLogic.Services.RabbitMQ.Interface;
using MassTransit;
using Encryptarium.Storage.Middlewares;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri("rabbitmq://localhost"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});
builder.Services.AddControllers();
//Npgsql.EntityFrameworkCore.PostgreSQL
builder.Services.AddDbContextFactory<SecretContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<ITwoFactorAuthService, TwoFactorAuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
builder.Services.AddScoped<IIpService, IpService>();
builder.Services.AddHttpClient<IGitHubService, GitHubService>();
builder.Services.AddScoped<IGitHubService, GitHubService>();
builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleTypeRepository, RoleTypeRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IStoragePolicyRepository, StoragePolicyRepository>();
builder.Services.AddScoped<IStorageRepository, StorageRepository>();
builder.Services.AddScoped<IStorageLinkPolicyRepository, StorageLinkPolicyRepository>();
builder.Services.AddScoped<IWhiteListIpRepository, WhiteListIpRepository>();
builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Constants.TokenPolicy, policy =>
        policy.Requirements.Add(
            new TokenRequirement()
        ));
});

builder.Services.AddSingleton<IAuthorizationHandler, TokenHandler>();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var basePath = AppContext.BaseDirectory;

    var xmlPath = Path.Combine(basePath, "Encryptarium.Auth.xml");
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();
app.Use(next => context => {
    context.Request.EnableBuffering();
    return next(context);
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<MonitoringMiddleware>();
app.UseMiddleware<CorsMiddleware>();
app.UseHttpsRedirection();
app.UseCors(x => x.WithOrigins("https://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
app.UseAuthorization();

app.MapControllers();

app.Run();
