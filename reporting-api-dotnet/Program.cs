using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var connStr = builder.Configuration["DB_CONN"]
    ?? "Host=localhost;Port=5432;Database=retail;Username=postgres;Password=postgres";
builder.Services.AddSingleton(NpgsqlDataSource.Create(connStr));
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/reports/top-products", (NpgsqlDataSource ds) => Query(ds,
    @"SELECT product, SUM(quantity)::int AS units, SUM(quantity * unit_price) AS revenue
      FROM sales GROUP BY product ORDER BY revenue DESC LIMIT 10"));

app.MapGet("/api/reports/revenue-by-store", (NpgsqlDataSource ds) => Query(ds,
    @"SELECT store, SUM(quantity * unit_price) AS revenue
      FROM sales GROUP BY store ORDER BY revenue DESC"));

app.MapGet("/api/reports/recent", (NpgsqlDataSource ds) => Query(ds,
    @"SELECT id, store, product, quantity, unit_price, sold_at
      FROM sales ORDER BY id DESC LIMIT 15"));

app.MapGet("/api/health", () => new { status = "UP" });

app.Run();

static async Task<List<Dictionary<string, object?>>> Query(NpgsqlDataSource ds, string sql)
{
    await using var cmd = ds.CreateCommand(sql);
    await using var r = await cmd.ExecuteReaderAsync();
    var rows = new List<Dictionary<string, object?>>();
    while (await r.ReadAsync())
    {
        var row = new Dictionary<string, object?>();
        for (int i = 0; i < r.FieldCount; i++)
            row[r.GetName(i)] = r.IsDBNull(i) ? null : r.GetValue(i);
        rows.Add(row);
    }
    return rows;
}
