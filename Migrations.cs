using Etch.OrchardCore.Greenhouse.Indexes;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using OrchardCore.Flows.Models;
using OrchardCore.Recipes.Services;
using System;
using System.Threading.Tasks;
using YesSql.Sql;

namespace Etch.OrchardCore.Greenhouse
{
    public class Migrations : DataMigration
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IRecipeMigrator _recipeMigrator;

        public Migrations(IContentDefinitionManager contentDefinitionManager, IRecipeMigrator recipeMigrator)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _recipeMigrator = recipeMigrator;
        }

        public async Task<int> CreateAsync()
        {
            await _recipeMigrator.ExecuteAsync("create.recipe.json", this);

            return 1;
        }

        public async Task<int> UpdateFrom1Async()
        {
            await SchemaBuilder.CreateMapIndexTableAsync<GreenhousePostingPartIndex>(table => table
                .Column<long>("GreenhouseId")
            );

            await SchemaBuilder.AlterTableAsync(nameof(GreenhousePostingPartIndex), table => table
                .CreateIndex("IDX_GreenhousePostingPartIndex_GreenhouseId", "GreenhouseId")
            );

            return 2;
        }

        public async Task<int> UpdateFrom2Async()
        {
            await SchemaBuilder.AlterTableAsync(nameof(GreenhousePostingPartIndex), table => table
                .AddColumn<DateTime>("UpdatedAt")
            );

            return 3;
        }

        public async Task<int> UpdateFrom3Async()
        {
            await _recipeMigrator.ExecuteAsync("update3.recipe.json", this);

            return 4;
        }

        public async Task<int> UpdateFrom4Async()
        {
            await SchemaBuilder.AlterTableAsync(nameof(GreenhousePostingPartIndex), table => table
                .AddColumn<long>("JobId")
            );

            return 5;
        }

        public async Task<int> UpdateFrom5()
        {
            await _recipeMigrator.ExecuteAsync("update5.recipe.json", this);

            return 6;
        }

        public async Task<int> UpdateFrom6()
        {
            await _recipeMigrator.ExecuteAsync("update6.recipe.json", this);

            return 7;
        }

        public async Task<int> UpdateFrom7Async()
        {
            await _contentDefinitionManager.AlterTypeDefinitionAsync("GreenhousePostings", builder => builder
                .WithPart("EmptyContent", nameof(FlowPart), part => part
                    .WithDisplayName("Empty Content")
                    .WithDescription("Content displayed when there are no postings.")
                    .WithPosition("5")
                )
            );

            return 8;
        }
    }
}
