var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddPrettyConsole();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
