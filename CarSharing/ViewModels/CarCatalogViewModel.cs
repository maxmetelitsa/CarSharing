using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CarSharing.CarsInfo;
using CarSharing.Data;
using CarSharing.Enums;
using CarSharing.Mvvm;

namespace CarSharing.ViewModels
{
    public enum PriceSort
    {
        None,
        CheapFirst,
        ExpensiveFirst
    }

    /// <summary>An item of a filter drop-down: the value and the text shown for it.</summary>
    public class FilterOption<T>
    {
        public FilterOption(T value, string title)
        {
            Value = value;
            Title = title;
        }

        public T Value { get; }

        public string Title { get; }

        public override string ToString() => Title;
    }

    /// <summary>Car catalog with filters by brand and class and sorting by price.</summary>
    public class CarCatalogViewModel : ViewModelBase
    {
        private const string Any = "Все";

        private readonly List<CarCardViewModel> allCars;
        private FilterOption<Brand> selectedBrand;
        private FilterOption<CarType> selectedClass;
        private FilterOption<PriceSort> selectedSort;
        private string searchText = string.Empty;

        public CarCatalogViewModel(ICarRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            allCars = repository.GetAll().Select(car => new CarCardViewModel(car)).ToList();
            Cars = new ObservableCollection<CarCardViewModel>();

            // Only brands and classes that are actually in the car park are offered as filters.
            Brands = new[] { new FilterOption<Brand>(Brand.None, Any) }
                .Concat(allCars.Select(c => c.Car.Brand).Distinct().OrderBy(b => b.ToString())
                    .Select(b => new FilterOption<Brand>(b, b.ToString())))
                .ToList();

            Classes = new[] { new FilterOption<CarType>(CarType.None, Any) }
                .Concat(allCars.Select(c => c.Car.CarType).Distinct().OrderBy(t => t)
                    .Select(t => new FilterOption<CarType>(t, t.ToString())))
                .ToList();

            SortOptions = new List<FilterOption<PriceSort>>
            {
                new FilterOption<PriceSort>(PriceSort.None, "Без сортировки"),
                new FilterOption<PriceSort>(PriceSort.CheapFirst, "Дешевые сначала"),
                new FilterOption<PriceSort>(PriceSort.ExpensiveFirst, "Дорогие сначала")
            };

            ClearFiltersCommand = new RelayCommand(ClearFilters);
            ClearFilters();
        }

        public ObservableCollection<CarCardViewModel> Cars { get; }

        public IReadOnlyList<FilterOption<Brand>> Brands { get; }

        public IReadOnlyList<FilterOption<CarType>> Classes { get; }

        public IReadOnlyList<FilterOption<PriceSort>> SortOptions { get; }

        public RelayCommand ClearFiltersCommand { get; }

        public FilterOption<Brand> SelectedBrand
        {
            get => selectedBrand;
            set
            {
                if (SetProperty(ref selectedBrand, value))
                {
                    ApplyFilters();
                }
            }
        }

        public FilterOption<CarType> SelectedClass
        {
            get => selectedClass;
            set
            {
                if (SetProperty(ref selectedClass, value))
                {
                    ApplyFilters();
                }
            }
        }

        public FilterOption<PriceSort> SelectedSort
        {
            get => selectedSort;
            set
            {
                if (SetProperty(ref selectedSort, value))
                {
                    ApplyFilters();
                }
            }
        }

        /// <summary>Search by model name or brand (car park window).</summary>
        public string SearchText
        {
            get => searchText;
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    ApplyFilters();
                }
            }
        }

        public void ClearFilters()
        {
            selectedBrand = Brands[0];
            selectedClass = Classes[0];
            selectedSort = SortOptions[0];
            OnPropertyChanged(nameof(SelectedBrand));
            OnPropertyChanged(nameof(SelectedClass));
            OnPropertyChanged(nameof(SelectedSort));
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<CarCardViewModel> cars = allCars;

            Brand brand = SelectedBrand?.Value ?? Brand.None;
            if (brand != Brand.None)
            {
                cars = cars.Where(c => c.Car.Brand == brand);
            }

            CarType carClass = SelectedClass?.Value ?? CarType.None;
            if (carClass != CarType.None)
            {
                cars = cars.Where(c => c.Car.CarType == carClass);
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string query = SearchText.Trim();
                cars = cars.Where(c => Contains(c.Name, query) || Contains(c.Brand, query));
            }

            switch (SelectedSort?.Value ?? PriceSort.None)
            {
                case PriceSort.CheapFirst:
                    cars = cars.OrderBy(c => c.DayPrice);
                    break;
                case PriceSort.ExpensiveFirst:
                    cars = cars.OrderByDescending(c => c.DayPrice);
                    break;
            }

            Cars.Clear();
            foreach (CarCardViewModel car in cars)
            {
                Cars.Add(car);
            }
        }

        private static bool Contains(string text, string query)
        {
            return text != null && text.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }
}
