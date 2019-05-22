using Panacea.Core;
using Panacea.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Web;
using Panacea.Multilinguality;

namespace Panacea.ContentControls
{
    public abstract class HospitalServerLazyItemProvider<T> : BaseLazyItemProvider where T : ServerItem
    {
        private readonly PanaceaServices _core;
        protected string categoriesUrl, itemsUrl, searchUrl;

        private readonly int _size = 10;

        public HospitalServerLazyItemProvider(PanaceaServices core, string categoriesUrl, string itemsUrl, string searchUrl, int pageSize)
        {
            _size = pageSize;
            this._core = core;
            this.categoriesUrl = categoriesUrl;
            this.itemsUrl = itemsUrl;
            this.searchUrl = searchUrl;
        }

        public override ServerGroupItem SelectedCategory
        {
            get => base.SelectedCategory;
            set
            {
                base.SelectedCategory = value;
                if (value != null)
                {
                    Search = null;
                    TotalPages = 1;
                    CurrentPage = 1;
                }
            }
        }
        public override int CurrentPage
        {
            get => base.CurrentPage;
            set
            {
                base.CurrentPage = value;
                if (!string.IsNullOrEmpty(Search))
                {
                    SearchNextAsync(Search);//TODO: SHOULD I AWAIT?
                }
                else
                {
                    GetItems();
                }
            }
        }

        public override string Search
        {
            get => base.Search;
            set
            {
                base.Search = value;
                if (!string.IsNullOrEmpty(Search))
                {
                    SelectedCategory = null;
                    TotalPages = 1;
                    CurrentPage = 1;
                }
            }
        }

        public virtual async Task<List<ServerGroupItem>> GetCategoriesAsync()
        {
            IsBusy = true;
            try
            {
                var res = await _core.HttpClient.GetObjectAsync<List<ServerGroupItem>>(categoriesUrl);
                return res.Result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        CancellationTokenSource _itemsCancellation;
        protected async Task GetItems()
        {
            if (_itemsCancellation != null)
            {
                _itemsCancellation.Cancel();
            }
            _itemsCancellation = new CancellationTokenSource();
            var source = _itemsCancellation;
            if (SelectedCategory == null) return;
            IsBusy = true;
            try
            {
                Items = null;
                var res = await _core.HttpClient.GetObjectAsync<ContentResponse<T>>(string.Format(itemsUrl, SelectedCategory.Id, (CurrentPage - 1) * _size, _size));
                TotalPages = (int)Math.Ceiling(res.Result.Total / (double)_size);
                if (source.IsCancellationRequested) return;
                Items = res.Result.Items.Cast<ServerItem>().ToList();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task SearchNextAsync(string wildcard)
        {
            IsBusy = true;
            try
            {
                Items = null;
                var res = await _core.HttpClient.GetObjectAsync<ContentResponse<T>>(string.Format(searchUrl, HttpUtility.UrlEncode(wildcard), (CurrentPage - 1) * _size, LanguageContext.Instance.Culture.Name, _size));
                TotalPages = (int)Math.Ceiling(res.Result.Total / (double)_size);
                Items = res.Result.Items.Cast<ServerItem>().ToList();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override async Task Initialize()
        {
            Categories = await GetCategoriesAsync();
            if (Categories.Count > 0)
            {
                SelectedCategory = Categories[0];
            }
        }

        public override void Refresh()
        {

        }

    }

    [DataContract]
    public class ContentResponse<T> where T : ServerItem
    {
        [DataMember(Name = "total")]
        public int Total { get; set; }

        [DataMember(Name = "items")]
        public List<T> Items { get; set; }
    }
}
