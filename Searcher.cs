using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using techsearch.Model;

namespace techsearch
{
	class Searcher
	{
		// The key to you cosmosdb
		const string azureSearchKey = "2D53E995FF88B3FB0FEA04704C44DE80";

		public Searcher(string searchName, string indexName)
		{
			SearchName = searchName ?? throw new System.ArgumentNullException(nameof(searchName));
			IndexName = indexName ?? throw new System.ArgumentNullException(nameof(indexName));
		}

		public string SearchName { get; }
		public string IndexName { get; }

		public Index CreateIndex(params Field[] fields)
		{
			// Create a service client connection
			var azureSearchService = new SearchServiceClient(this.SearchName, new SearchCredentials(azureSearchKey));

			// Get the Azure Search Index
			if (azureSearchService.Indexes.Exists(this.IndexName))
			{
				azureSearchService.Indexes.Delete(this.IndexName);
			}

			Index indexModel = new Index()
			{
				Name = this.IndexName,
				Fields = fields
			};

			return azureSearchService.Indexes.Create(indexModel);
		}

		public void AddDocument<TModel>(IEnumerable<TModel> models) where TModel : class
		{
			var azureSearchService = new SearchServiceClient(this.SearchName, new SearchCredentials(azureSearchKey));
			ISearchIndexClient indexClient = azureSearchService.Indexes.GetClient(this.IndexName);
			indexClient.Documents.Index(IndexBatch.MergeOrUpload<TModel>(models.ToList()));
		}

		public async Task<IEnumerable<TipModel>> SearchAsync(
			string searchText,
			string filter = null,
			List<string> order = null,
			List<string> facets = null)
		{
			var azureSearchService = new SearchServiceClient(this.SearchName, new SearchCredentials(azureSearchKey));
			// Execute search based on search text and optional filter
			var sp = new SearchParameters();

			// Add Filter
			if (!String.IsNullOrEmpty(filter))
			{
				sp.Filter = filter;
			}

			// Order
			if (order != null && order.Count > 0)
			{
				sp.OrderBy = order;
			}

			// facets
			if (facets != null && facets.Count > 0)
			{
				sp.Facets = facets;
			}

			// Search
			ISearchIndexClient indexClient = azureSearchService.Indexes.GetClient(this.IndexName);
			DocumentSearchResult<TipModel> response = await indexClient.Documents.SearchAsync<TipModel>(searchText, sp);
			return response.Results.Select(r => r.Document);
		}
	}
}