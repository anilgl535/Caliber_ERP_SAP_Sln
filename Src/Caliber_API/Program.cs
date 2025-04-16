using Caliber_API;
using Caliber_Components.Authentication.APIKey;
using Caliber_Components.Authentication.Token;
using Caliber_Components.Autherization;
using Caliber_Components.DBComponents;
using Caliber_Components.Token;

var builder = WebApplication.CreateBuilder(args);

var tokenSettings = builder.Configuration.GetSection("TokenSettings").Get<TokenSettings>();
if (tokenSettings != null)
{
    Tokens.SetTokenSettings(tokenSettings);
}
var apiKeys = builder.Configuration.GetSection("ApiKey").Get<ApiKeyModel>();
if (apiKeys != null)
{
    ApiKey.SetApiKeys(apiKeys);
}

Tokens.AuthenticationType = builder.Configuration.GetSection("AuthType").Get<int?>() ?? default(int);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDBAccess>(provider => new DBAccessCls(builder.Configuration.GetConnectionString("ERPDb") ?? string.Empty))
                .AddScoped<AuthFilter>()
                .AddScoped<PreInsert>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//else
//{
app.UseSwagger();

app.UseSwaggerUI();

//}
app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();
