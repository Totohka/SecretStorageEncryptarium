using BusinessLogic.Services.Policies.Implementation;
using BusinessLogic.Services.Policies.Interface;
using BusinessLogic.Services.RabbitMQ.Implementation;
using BusinessLogic.Services.RabbitMQ.Interface;
using BusinessLogic.Services.Roles.Implementation;
using BusinessLogic.Services.Roles.Interface;
using BusinessLogic.Services.Tokens.Implementation;
using BusinessLogic.Services.Tokens.Interface;
using BusinessLogic.Services.Users.Implementation;
using BusinessLogic.Services.Users.Interface;
using DAL;
using DAL.Repositories.Roles.Implementation;
using DAL.Repositories.Roles.Interface;
using DAL.Repositories.RoleTypes.Implementation;
using DAL.Repositories.RoleTypes.Interface;
using DAL.Repositories.SecretLinkPolicies.Implementation;
using DAL.Repositories.SecretLinkPolicies.Interface;
using DAL.Repositories.SecretPolicies.Implementation;
using DAL.Repositories.SecretPolicies.Interface;
using DAL.Repositories.Secrets.Implementation;
using DAL.Repositories.Secrets.Interface;
using DAL.Repositories.StorageLinkPolicies.Implementation;
using DAL.Repositories.StorageLinkPolicies.Interface;
using DAL.Repositories.StoragePolicies.Implementation;
using DAL.Repositories.StoragePolicies.Interface;
using DAL.Repositories.Storages.Implementation;
using DAL.Repositories.Storages.Interface;
using DAL.Repositories.UserRoles.Implementation;
using DAL.Repositories.UserRoles.Interface;
using DAL.Repositories.Users.Implementation;
using DAL.Repositories.Users.Interface;
using Encryptarium.Access.Handlers;
using Encryptarium.Access.Middlewares;
using Encryptarium.Access.Requirements;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Model;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
JsonSerializerOptions options = new() { IncludeFields = true };
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
builder.Services.AddDbContextFactory<SecretContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();
builder.Services.AddScoped<IRoleTypeService, RoleTypeService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IStoragePolicyService, StoragePolicyService>();
builder.Services.AddScoped<ISecretPolicyService, SecretPolicyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IRoleTypeRepository, RoleTypeRepository>();
builder.Services.AddScoped<IStoragePolicyRepository, StoragePolicyRepository>();
builder.Services.AddScoped<ISecretPolicyRepository, SecretPolicyRepository>();
builder.Services.AddScoped<IStorageLinkPolicyRepository, StorageLinkPolicyRepository>();
builder.Services.AddScoped<ISecretLinkPolicyRepository, SecretLinkPolicyRepository>();
builder.Services.AddScoped<ISecretRepository, SecretRepository>();
builder.Services.AddScoped<IStorageRepository, StorageRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Constants.TokenPolicy, policy => policy.Requirements.Add(new TokenRequirement()))
    .AddPolicy(Constants.StoragePolicy, policy => policy.Requirements.Add(new StorageRequirement()))
    .AddPolicy(Constants.SecretPolicy, policy => policy.Requirements.Add(new SecretRequirement()));

builder.Services.AddSingleton<IAuthorizationHandler, TokenHandler>();
builder.Services.AddHttpContextAccessor();
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
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:3000").AllowCredentials());
app.UseAuthorization();
app.MapControllers();

app.Run();
