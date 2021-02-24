using FluentMigrator;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Data.Mapping;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.UpgradeTo440
{
    [TvProgMigration("2020/03/08 11:26:08:9037680", "Specification attribute grouping")]
    [SkipMigrationOnInstall]
    public class SpecificationAttributeGroupingMigration : MigrationBase
    {
        #region Fields

        private readonly IMigrationManager _migrationManager;

        #endregion

        #region Ctor

        public SpecificationAttributeGroupingMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(SpecificationAttributeGroup))).Exists())
                _migrationManager.BuildTable<SpecificationAttributeGroup>(Create);

            if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(SpecificationAttribute))).Column(nameof(SpecificationAttribute.SpecificationAttributeGroupId)).Exists())
            {
                //add new column
                Alter.Table(NameCompatibilityManager.GetTableName(typeof(SpecificationAttribute)))
                    .AddColumn(nameof(SpecificationAttribute.SpecificationAttributeGroupId)).AsInt32().Nullable().ForeignKey<SpecificationAttributeGroup>();
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }

        #endregion
    }
}
