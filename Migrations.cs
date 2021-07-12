using Etch.OrchardCore.Greenhouse.Indexes;
using OrchardCore.Data.Migration;
using OrchardCore.Recipes.Services;
using System;
using System.Threading.Tasks;

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
            SchemaBuilder.CreateMapIndexTable(nameof(GreenhousePostingPartIndex), table => table
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
    }
}
