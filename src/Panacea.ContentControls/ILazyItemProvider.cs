using Panacea.Models;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Panacea.ContentControls
{
    public interface ILazyItemProvider
    {
        bool IsBusy { get; }

        string Search { get; set; }

        List<ServerItem> Items { get; }

        ServerGroupItem SelectedCategory { get; set; }

        List<ServerGroupItem> Categories { get; }

        Task Initialize();

        void Refresh();

        event EventHandler Refreshed;

        int CurrentPage { get; set; }

        int TotalPages { get; }
    }

    public abstract class BaseLazyItemProvider : PropertyChangedBase, ILazyItemProvider
    {
        private bool _isBusy;
        public virtual bool IsBusy
        {
            get => _isBusy;
            protected set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private string _search;
        public virtual string Search
        {
            get => _search;
            set
            {
                _search = value;
                OnPropertyChanged();
            }
        }


        private int _currentPage;
        public virtual int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        int _totalPages;
        public virtual int TotalPages
        {
            get => _totalPages;
            protected set
            {
                _totalPages = value;
                OnPropertyChanged();
            }
        }

        List<ServerItem> _items;
        public virtual List<ServerItem> Items
        {
            get => _items;
            protected set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        List<ServerGroupItem> _categories;
        public virtual List<ServerGroupItem> Categories
        {
            get => _categories;
            protected set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        ServerGroupItem _selectedCategory;
        public virtual ServerGroupItem SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler Refreshed;

        public virtual void OnRefreshed()
        {
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public abstract void Refresh();

        public abstract Task Initialize();
    }
}