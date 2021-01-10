using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Data.Mapping.TvProgMain
{
    /// <summary>
    /// Конфигурирование связей телепрограммы с пользователями
    /// </summary>
    public partial class UserProgramsMap: TvProgEntityTypeConfiguration<UsersPrograms>
    {
        #region Методы

        public override void Configure(EntityMappingBuilder<UsersPrograms> builder)
        {
            builder.HasTableName(nameof(UsersPrograms));

            builder.Property(usersProgram => usersProgram.UserId).IsNullable(false);
            builder.Property(usersProgram => usersProgram.UserChannelId).IsNullable(false);
            builder.Property(usersProgram => usersProgram.ProgrammesId).IsNullable(false);
            builder.Property(usersProgram => usersProgram.GenreId);
            builder.Property(usersProgram => usersProgram.RatingId);
            builder.Property(usersProgram => usersProgram.Anons).HasLength(1000);
            builder.Property(usersProgram => usersProgram.Remind).IsNullable(false);
        }

        #endregion
    }
}
