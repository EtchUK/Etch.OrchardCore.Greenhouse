using Etch.OrchardCore.Greenhouse.Indexes;
using OrchardCore.Data.Migration;
using OrchardCore.Recipes.Services;
using System;
using System.Threading.Tasks;
using YesSql.Sql;

namespace Etch.OrchardCore.Greenhouse
{
    public class Migrations : DataMigration
    {
        private readonly IRecipeMigrator _recipeMigrator;

        public Migrations(IRecipeMigrator recipeMigrator)
        {
            _recipeMigrator = recipeMigrator;
        }

        public async Task<int> CreateAsync()
        {
            await _recipeMigrator.ExecuteAsync("create.recipe.json", this);

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.CreateMapIndexTable<GreenhousePostingPartIndex>(table => table
                .Column<long>("GreenhouseId")
            );

            SchemaBuilder.AlterTable(nameof(GreenhousePostingPartIndex), table => table
                .CreateIndex("IDX_GreenhousePostingPartIndex_GreenhouseId", "GreenhouseId")
            );

            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(nameof(GreenhousePostingPartIndex), table => table
                .AddColumn<DateTime>("UpdatedAt")
            );

            return 3;
        }

        public async Task<int> UpdateFrom3Async()
        {
            await _recipeMigrator.ExecuteAsync("update3.recipe.json", this);

            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(nameof(GreenhousePostingPartIndex), table => table
                .AddColumn<long>("JobId")
            );

            return 5;
        }
    }
}
