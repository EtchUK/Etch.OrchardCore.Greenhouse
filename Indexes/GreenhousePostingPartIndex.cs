using Etch.OrchardCore.Greenhouse.Models;
using OrchardCore.ContentManagement;
using System;
using YesSql.Indexes;

namespace Etch.OrchardCore.Greenhouse.Indexes
{
    public class GreenhousePostingPartIndex : MapIndex
    {
        public long GreenhouseId { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class GreenhousePostingPartIndexProvider : IndexProvider<ContentItem>
    {
        public override void Describe(DescribeContext<ContentItem> context)
        {
            context.For<GreenhousePostingPartIndex>()
                .Map(contentItem =>
                {
                    var part = contentItem.As<GreenhousePostingPart>();

                    if (part == null)
                    {
                        return null;
                    }

                    return new GreenhousePostingPartIndex
                    {
                        GreenhouseId = part.GreenhouseId,
                        UpdatedAt = part.UpdatedAt
                    };
                });
        }
    }
}
