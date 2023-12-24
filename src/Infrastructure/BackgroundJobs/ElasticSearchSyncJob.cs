using Application.Common.Data;
using Domain.ModelsSnapshots;
using Microsoft.EntityFrameworkCore;
using Nest;
using Quartz;

namespace Infrastructure.BackgroundJobs;

public sealed class ElasticSearchSyncJob(IApplicationDbContext context, IElasticClient elasticClient) : IJob
{
    private readonly IApplicationDbContext _context = context;
    private readonly IElasticClient _elasticClient = elasticClient;

    public async Task Execute(IJobExecutionContext jobContext)
    {
        var dbProductsDict = await _context
                .Products
                .Include(p => p.Categories)
                .Select(p => ProductSnapshot.Snapshot(p))
                .ToDictionaryAsync(p => p.Id, p => p);


        var scrollResponse = _elasticClient.Search<ProductSnapshot>(s => s
            .Scroll("5m") // Set the scroll timeout
            .Size(500)   // Set the batch size
        );

        var elasticProducts = new List<ProductSnapshot>();
        while (scrollResponse.IsValid && scrollResponse.Documents.Count != 0)
        {
            // Process each batch of documents
            elasticProducts.AddRange(scrollResponse.Documents);

            // Fetch the next batch
            scrollResponse = _elasticClient.Scroll<ProductSnapshot>("5m", scrollResponse.ScrollId);
        }

        var elasticProductsDict = elasticProducts.ToDictionary(p => p.Id, p => p);

        List<ProductSnapshot> toUpdate = [];
        List<ProductSnapshot> toDelete = [];
        List<ProductSnapshot> toAdd = [];
        foreach (var item in dbProductsDict.Values)
        {
            if (elasticProductsDict.TryGetValue(item.Id, out var elasticProduct))
            {
                if (!ProductSnapshot.Equals(item, elasticProduct))
                {
                    toUpdate.Add(item);
                }
            }
            else
            {
                toAdd.Add(item);
            }
        }

        toDelete.
            AddRange(elasticProductsDict
                .Values
                .Where(elasticProduct => !dbProductsDict.ContainsKey(elasticProduct.Id)));

        foreach (var item in toUpdate)
        {
            await _elasticClient.UpdateAsync<ProductSnapshot>(item.Id, u => u
                    .Doc(item)
                    .Refresh(Elasticsearch.Net.Refresh.True));
        }

        foreach (var item in toDelete)
        {
            await _elasticClient.DeleteAsync<ProductSnapshot>(item.Id);
        }

        foreach (var item in toAdd)
        {
            await _elasticClient.IndexDocumentAsync(item);
        }
    }
}
