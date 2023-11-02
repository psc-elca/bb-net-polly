using Service.Clients;
using Service.Interfaces;
using Service.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Polly Policies
builder.Services.AddSingleton<Policies>();

// 3rd Party Service
builder.Services.AddHttpClient<IThirdPartyService, ThirdPartyService>
    (client => client.BaseAddress = new Uri("http://localhost:8123"));

// 3rd Party Service (With Token)
builder.Services.AddHttpClient<ITokenProvider, TokenProvider>
    (client => client.BaseAddress = new Uri("http://localhost:8123"));

builder.Services.AddTransient<TokenDelegatingHandler>();
builder.Services.AddHttpClient<IThirdPartyServiceWithToken, ThirdPartyServiceWithToken>
    (client => client.BaseAddress = new Uri("http://localhost:8123"))
    .AddPolicyHandler((provider, _) => Policies.GetTokenRefreshPolicy(provider))
    .AddHttpMessageHandler<TokenDelegatingHandler>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();