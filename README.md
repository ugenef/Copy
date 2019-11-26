# Copy

Copy is an Npgsql extension library for fast and effective row import to PostgreSQL.
Copy works as an extension-method for `DbContext` class in Entity Framework Core.
Supports .NET Standart 2.1 
### How to use:
Call extension method on your DbContext and pass IEnumerable of
your model`s type inside.
```cs
using (var context = new BillingContext())
{
    await context.BulkCopyAsync(calls).ConfigureAwait(false);
}
```
The code above will insert rows into the table corresponding to your model.
CLR types of model's properties will map to corresponding PostgreSQL data types.
However, you can set up custom mapping manually:
```cs
public class BillingContext : DbContext
{
    public DbSet<Call> Calls { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql("your connection string")
            .UseSnakeCaseNamingConvention()
            .UseCopyTypeMapping<BillingContext>(x =>
                x.MapType<Call>(m => m
                    .MapProperty(nameof(Call.Duration), NpgsqlDbType.Integer)
                    .MapProperty(nameof(Call.StartTime), NpgsqlDbType.Timestamp)));
        }
 }   
```
### How it works?
Copy is a smart wrapper above `NpgsqlBinaryImporter` class from Npgsql Data Provider.
`NpgsqlBinaryImporter` uses PostgreSQL-specific mechanism for effective multiple inserts, called `COPY FROM`, and smart wrapper makes it use easier.
When you call the method `context.BulkCopyAsync()` for the first time, the library will collect metadata about your model (including table and columns names in database) from Entity Frameworks's `DbContext` and will generate `Dynamic Method`, written in IL, that uses `NpgsqlBinaryImporter` in specific for your model scenario. Then each next call of `context.BulkCopyAsync()` will use that method. 
For example, the library will insert model `Call` 
```cs
class Call
{
    [Key] public string CallId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string CallingNumber { get; set; }
    public string CalledNumber { get; set; }
    public int Duration { get; set; }
    public CallType CallType { get; set; }
}
```
this way:
```cs
public async Task InsertAsync(Call[] calls)
{
    using (var conn = new NpgsqlConnection("your connection string"))
    {
        await conn.OpenAsync().ConfigureAwait(false);
        using (var writer = conn.BeginBinaryImport(
                    "COPY calls " +
                    "(start_time,end_time,calling_number," +
                    "called_number,duration,call_type,call_id) " +
                    "FROM STDIN (FORMAT BINARY)"))
        {
           foreach (var call in calls)
            {
            //Generates dynamicly for each model
                writer.StartRow();
                writer.Write(call.StartTime, NpgsqlDbType.Timestamp);
                writer.Write(call.EndTime);
                writer.Write(call.CallingNumber);
                writer.Write(call.CalledNumber);
                writer.Write(call.Duration, NpgsqlDbType.Integer);
                writer.Write(call.CallType);
                writer.Write(call.CallId);
            }

            await writer.CompleteAsync().ConfigureAwait(false);
        }
    }
}
```
The library saves methods for different DbContexts separately. This means that you can have different DbContexts for where the same model targets different tables in the database, and the library will insert in two different ways.

### Links
- See [PostgreSQL Docs](https://www.postgresql.org/docs/current/sql-copy.html) to find out more about `COPY`
- Using of `NpgsqlBinaryImporter` in [Npgsql Docs](https://www.npgsql.org/doc/copy.html) 

### TODO
- Write more tests
- Add auto-batching for inserting bulk
