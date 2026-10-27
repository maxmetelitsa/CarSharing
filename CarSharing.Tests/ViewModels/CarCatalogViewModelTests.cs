using System.Linq;
using CarSharing.Enums;
using CarSharing.Tests.Fakes;
using CarSharing.ViewModels;
using Xunit;

namespace CarSharing.Tests.ViewModels
{
    public class CarCatalogViewModelTests
    {
        private static CarCatalogViewModel CreateCatalog()
        {
            var repository = new FakeCarRepository(
                TestData.Car(1, "BMW X6", Brand.BMW, CarType.Business, 180),
                TestData.Car(2, "Kia Ceed", Brand.Kia, CarType.Economy, 100),
                TestData.Car(3, "BMW M8", Brand.BMW, CarType.Business, 200),
                TestData.Car(4, "Kia Cerato Coupe", Brand.Kia, CarType.Coupe, 100));

            return new CarCatalogViewModel(repository);
        }

        [Fact]
        public void Constructor_ShowsAllCars()
        {
            Assert.Equal(4, CreateCatalog().Cars.Count);
        }

        [Fact]
        public void Filters_OfferOnlyBrandsAndClassesFromTheCarPark()
        {
            CarCatalogViewModel catalog = CreateCatalog();

            Assert.Equal(new[] { "Все", "BMW", "Kia" }, catalog.Brands.Select(b => b.Title));
            Assert.Equal(new[] { "Все", "Economy", "Business", "Coupe" }, catalog.Classes.Select(c => c.Title));
        }

        [Fact]
        public void SelectingBrand_ShowsOnlyThatBrand()
        {
            CarCatalogViewModel catalog = CreateCatalog();

            catalog.SelectedBrand = catalog.Brands.Single(b => b.Value == Brand.Kia);

            Assert.Equal(new[] { 2, 4 }, catalog.Cars.Select(c => c.Car.ID));
        }

        [Fact]
        public void BrandAndClassFilters_Combine()
        {
            CarCatalogViewModel catalog = CreateCatalog();

            catalog.SelectedBrand = catalog.Brands.Single(b => b.Value == Brand.Kia);
            catalog.SelectedClass = catalog.Classes.Single(c => c.Value == CarType.Coupe);

            Assert.Equal(4, Assert.Single(catalog.Cars).Car.ID);
        }

        [Fact]
        public void SortingByPrice_OrdersCars()
        {
            CarCatalogViewModel catalog = CreateCatalog();

            catalog.SelectedSort = catalog.SortOptions.Single(s => s.Value == PriceSort.ExpensiveFirst);

            Assert.Equal(new double[] { 200, 180, 100, 100 }, catalog.Cars.Select(c => c.DayPrice));
        }

        [Fact]
        public void ClearFilters_ShowsAllCarsAgain()
        {
            CarCatalogViewModel catalog = CreateCatalog();
            catalog.SelectedBrand = catalog.Brands.Single(b => b.Value == Brand.BMW);

            catalog.ClearFiltersCommand.Execute(null);

            Assert.Equal(4, catalog.Cars.Count);
            Assert.Equal("Все", catalog.SelectedBrand.Title);
        }

        [Fact]
        public void SearchText_MatchesNameOrBrandIgnoringCase()
        {
            CarCatalogViewModel catalog = CreateCatalog();

            catalog.SearchText = "coupe";

            Assert.Equal(4, Assert.Single(catalog.Cars).Car.ID);
        }
    }
}
