using BusinessLogic.Services.Audits.Implementation;
using BusinessLogic.Services.Audits.Interface;
using BusinessLogic.Services.RabbitMQ.Implementation;
using BusinessLogic.Services.RabbitMQ.Interface;
using BusinessLogic.Services.Tokens.Implementation;
using BusinessLogic.Services.Tokens.Interface;
using BusinessLogic.Services.Users.Implementation;
using BusinessLogic.Services.Users.Interface;
using DAL;
using DAL.Repositories.Audits.Implementation;
using DAL.Repositories.Audits.Interface;
using DAL.Repositories.Roles.Implementation;
using DAL.Repositories.Roles.Interface;
using DAL.Repositories.RoleTypes.Implementation;
using DAL.Repositories.RoleTypes.Interface;
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
using Encryptarium.Audit.Handlers;
using Encryptarium.Audit.Middlewares;
using Encryptarium.Audit.RabbitService;
using Encryptarium.Audit.Requirements;
using Encryptarium.Storage.Middlewares;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AuditConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri("rabbitmq://localhost"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("ENCQueue", ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(r => r.Interval(2, 100));
            ep.ConfigureConsumer<AuditConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddDbContextFactory<SecretContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IRoleTypeRepository, RoleTypeRepository>();
builder.Services.AddScoped<IStoragePolicyRepository, StoragePolicyRepository>();
builder.Services.AddScoped<IStorageLinkPolicyRepository, StorageLinkPolicyRepository>();
builder.Services.AddScoped<IStorageRepository, StorageRepository>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
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

app.UseAuthorization();
app.UseCors(x => x.WithOrigins("https://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
app.MapControllers();

app.Run();
