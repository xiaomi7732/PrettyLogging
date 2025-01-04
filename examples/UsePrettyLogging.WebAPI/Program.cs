var builder = WebApplication.CreateBuilder(args);

// Append this line
builder.Logging.PrettyIt();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
