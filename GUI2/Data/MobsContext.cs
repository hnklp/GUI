using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EFCore.Data
{
    public class MobsContext : DbContext
    {
        public const string FileName = "data.db";

        public MobsContext()
            : this(new DbContextOptionsBuilder<MobsContext>().UseSqlite($"Data Source={FileName}").Options)
        { }

        public MobsContext(DbContextOptions options)
        : base(options)
        { }

        public DbSet<Mob> Mobs => Set<Mob>();

        public DbSet<Species> Species => Set<Species>();
    }
}
