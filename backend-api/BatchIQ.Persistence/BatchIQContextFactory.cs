using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BatchIQ.Persistence;

public class BatchIQContextFactory :IDesignTimeDbContextFactory<BatchIQDbContext>
{
    public BatchIQDbContext CreateDbContext(string[] args = null)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BatchIQDbContext>();
        optionsBuilder.UseSqlite("Data Source=batchiq.db");

        return new BatchIQDbContext(optionsBuilder.Options);
    }
}