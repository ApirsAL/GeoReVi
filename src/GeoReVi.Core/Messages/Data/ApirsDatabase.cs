
using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Data.SQLite;
using System.Linq;

namespace GeoReVi
{
    public class ApirsDatabase : APIRSEntities
    {
        public ApirsDatabase()
    : base()
        {
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 5;
        }

        //private ApirsDatabase(string context)
        //: base(context)
        //{
        //    Database.SetInitializer(new CustomInitializer());
        //    Database.Initialize(true);
        //}

        //public static ApirsDatabase Create(string contextName)
        //{
        //    return new ApirsDatabase(contextName);
        //}
    }

    //class CustomInitializer : IDatabaseInitializer<ApirsDatabase>
    //{
    //    public void InitializeDatabase(ApirsDatabase context)
    //    {
    //        if (!context.Database.Exists() || !context.Database.CompatibleWithModel(false))
    //        {
    //            var configuration = new DbMigrationsConfiguration();
    //            var migrator = new DbMigrator(configuration);
    //            migrator.Configuration.TargetDatabase = new DbConnectionInfo(context.Database.Connection.ConnectionString, "System.Data.SqlClient");
    //            var migrations = migrator.GetPendingMigrations();
    //            if (migrations.Any())
    //            {
    //                var scriptor = new MigratorScriptingDecorator(migrator);
    //                string script = scriptor.ScriptUpdate(null, migrations.Last());
    //                if (!String.IsNullOrEmpty(script))
    //                {
    //                    context.Database.ExecuteSqlCommand(script);
    //                }
    //            }
    //        }
    //    }
    //}
}
