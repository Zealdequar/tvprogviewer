using FluentMigrator;
using System;
using System.Linq;
using TvProgViewer.Core.Domain.TvProgMain;
using TvProgViewer.Core.Domain.Users;

namespace TvProgViewer.Data.Migrations.UpgradeTo489
{
    [TvProgMigration("2026-01-16 21:11:29", "4.89.0 Schema new objects", MigrationProcessType.Update)]
    public class DataMigration: Migration
    {
        private readonly ITvProgDataProvider _dataProvider;
        public DataMigration(ITvProgDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Собрать выражения для наката
        /// </summary>
        public override void Up()
        {
            // Добавить категории:
            if (!_dataProvider.GetTable<TvProgCategory>().Any(tpc => string.Compare(tpc.Ident, TvProgUserDefaults.GetAllTvProgCategories, true) == 0))
            {
                var allCategories = _dataProvider.InsertEntity(
                    new TvProgCategory()
                    {
                        Name = "Все категории",
                        Ident = TvProgUserDefaults.GetAllTvProgCategories,
                        Active = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Order = 0
                    });
            }
            if (!_dataProvider.GetTable<TvProgCategory>().Any(tpc => string.Compare(tpc.Ident, TvProgUserDefaults.GetChildTvProgCategory, true) == 0))
            {
                var childCategory = _dataProvider.InsertEntity(
                    new TvProgCategory()
                    {
                        Name = "Детям",
                        Ident = TvProgUserDefaults.GetChildTvProgCategory,
                        Active = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Order = 1
                    });
            }
            if (!_dataProvider.GetTable<TvProgCategory>().Any(tpc => string.Compare(tpc.Ident, TvProgUserDefaults.GetFeatureFilmCategory, true) == 0))
            {
                var featureFilmCategory = _dataProvider.InsertEntity(
                    new TvProgCategory()
                    {
                       Name = "Художественный фильм",
                       Ident = TvProgUserDefaults.GetFeatureFilmCategory,
                       Active = true,
                       CreatedOnUtc = DateTime.UtcNow,
                       Order = 2
                    });
            }
            if (!_dataProvider.GetTable<TvProgCategory>().Any(tpc => string.Compare(tpc.Ident, TvProgUserDefaults.GetSerialCategory, true) == 0))
            {
                var serialCategory = _dataProvider.InsertEntity(
                    new TvProgCategory()
                    {
                        Name = "Сериал",
                        Ident = TvProgUserDefaults.GetSerialCategory,
                        Active = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Order = 3
                    });
            }
            if (!_dataProvider.GetTable<TvProgCategory>().Any(tpc => string.Compare(tpc.Ident, TvProgUserDefaults.GetSportCategory, true) == 0))
            {
                var sportCategory = _dataProvider.InsertEntity(
                    new TvProgCategory()
                    {
                        Name = "Спорт",
                        Ident = TvProgUserDefaults.GetSportCategory,
                        Active = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Order = 4
                    });
            }
            if (!_dataProvider.GetTable<TvProgCategory>().Any(tpc => string.Compare(tpc.Ident, TvProgUserDefaults.GetAdultCategory, true) == 0))
            {
                var adultCategory = _dataProvider.InsertEntity(
                    new TvProgCategory()
                    {
                        Name = "Для взрослых",
                        Ident = TvProgUserDefaults.GetAdultCategory,
                        Active = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Order = 5
                    });
            }
            if (!_dataProvider.GetTable<TvProgCategory>().Any(tpc => string.Compare(tpc.Ident, TvProgUserDefaults.GetNonCategory, true) == 0))
            {
                var nonCatgegory = _dataProvider.InsertEntity(
                    new TvProgCategory()
                    {
                        Name = "Без категории",
                        Ident = TvProgUserDefaults.GetNonCategory,
                        Active = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Order = 1000000
                    });
            }
        }

        /// <summary>
        /// Собрать выражения для отката
        /// </summary>
        public override void Down()
        {
            // Добавить, если необходимо
        }

    }
}
