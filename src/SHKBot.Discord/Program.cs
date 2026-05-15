using Discord;
using Discord.WebSocket;
using MediatR;


var builder = WebApplication.CreateBuilder(args);

// Discord
builder.Services.AddSingleton<DiscordSocketClient>(sp =>
{
    var config = new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
    };
    return new DiscordSocketClient(config);
});

// MediatR — scans Application layer for handlers
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(SHKBot.Application.AssemblyReference).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(SHKBot.Application.AssemblyReference).Assembly);

// Infrastructure
builder.Services.AddSingleton<ILlmService, OllamaService>();
builder.Services.AddSingleton<IConversationRepository, InMemoryConversationRepository>();

// Discord event listener
builder.Services.AddSingleton<DiscordMessageListener>();

// Ollama HttpClient
builder.Services.AddHttpClient();

var app = builder.Build();

// Start Discord connection
var client = app.Services.GetRequiredService<DiscordSocketClient>();
var listener = app.Services.GetRequiredService<DiscordMessageListener>();

await listener.InitializeAsync();
await client.LoginAsync(TokenType.Bot, builder.Configuration["Discord:Token"]);
await client.StartAsync();

app.Run();