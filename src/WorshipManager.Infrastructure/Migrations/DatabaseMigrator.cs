using System.Reflection;
using DbUp;
using Microsoft.Extensions.Logging;

namespace WorshipManager.Infrastructure.Migrations;

public static class DatabaseMigrator
{
    public static void Migrate(string connectionString, ILogger logger)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly(),
                s => s.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            .WithTransactionPerScript()
            .LogTo(new DbUpLogger(logger))
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            throw new Exception("Database migration failed", result.Error);
        }

        logger.LogInformation("Database migration completed successfully. {ScriptCount} script(s) executed.",
            result.Scripts.Count());
    }
}
