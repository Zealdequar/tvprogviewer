using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TVProgViewer.DataAccess.Models
{
    public partial class TVProgBaseProdContext : DbContext
    {
        public TVProgBaseProdContext()
        {
        }

        public TVProgBaseProdContext(DbContextOptions<TVProgBaseProdContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Channels> Channels { get; set; }
        public virtual DbSet<ExtUserSettings> ExtUserSettings { get; set; }
        public virtual DbSet<GenreClassificator> GenreClassificator { get; set; }
        public virtual DbSet<Genres> Genres { get; set; }
        public virtual DbSet<LogActions> LogActions { get; set; }
        public virtual DbSet<MediaPic> MediaPic { get; set; }
        public virtual DbSet<Programmes> Programmes { get; set; }
        public virtual DbSet<RatingClassificator> RatingClassificator { get; set; }
        public virtual DbSet<Ratings> Ratings { get; set; }
        public virtual DbSet<SearchSettings> SearchSettings { get; set; }
        public virtual DbSet<SystemUsers> SystemUsers { get; set; }
        public virtual DbSet<TvprogProviders> TvprogProviders { get; set; }
        public virtual DbSet<TypeProg> TypeProg { get; set; }
        public virtual DbSet<UpdateProgLog> UpdateProgLog { get; set; }
        public virtual DbSet<UserChannels> UserChannels { get; set; }
        public virtual DbSet<UsersPrograms> UsersPrograms { get; set; }
        public virtual DbSet<WebResources> WebResources { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=TVProgBaseProd;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Channels>(entity =>
            {
                entity.HasKey(e => e.ChannelId);

                entity.HasIndex(e => e.Deleted)
                    .HasName("IX_Deleted");

                entity.HasIndex(e => e.IconId)
                    .HasName("IX_IconID");

                entity.HasIndex(e => e.InternalId)
                    .HasName("IX_InternalID");

                entity.HasIndex(e => e.TvprogProviderId)
                    .HasName("IX_TVProgProviderID");

                entity.HasIndex(e => new { e.ChannelId, e.TvprogProviderId, e.InternalId, e.IconId, e.CreateDate, e.TitleChannel, e.IconWebSrc, e.Deleted })
                    .HasName("_dta_index_Channels_5_1093578934__col__");

                entity.Property(e => e.ChannelId).HasColumnName("ChannelID");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(sysdatetimeoffset())");

                entity.Property(e => e.Deleted).HasColumnType("datetime");

                entity.Property(e => e.IconId).HasColumnName("IconID");

                entity.Property(e => e.IconWebSrc).HasMaxLength(550);

                entity.Property(e => e.InternalId).HasColumnName("InternalID");

                entity.Property(e => e.TitleChannel)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.TvprogProviderId).HasColumnName("TVProgProviderID");

                entity.HasOne(d => d.Icon)
                    .WithMany(p => p.Channels)
                    .HasForeignKey(d => d.IconId)
                    .HasConstraintName("FK__Channels__IconID__440B1D61");

                entity.HasOne(d => d.TvprogProvider)
                    .WithMany(p => p.Channels)
                    .HasForeignKey(d => d.TvprogProviderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Channels__TVProg__4316F928");
            });

            modelBuilder.Entity<ExtUserSettings>(entity =>
            {
                entity.Property(e => e.ExtUserSettingsId).HasColumnName("ExtUserSettingsID");

                entity.Property(e => e.TvprogProviderId).HasColumnName("TVProgProviderID");

                entity.Property(e => e.Uid).HasColumnName("UID");

                entity.HasOne(d => d.TvprogProvider)
                    .WithMany(p => p.ExtUserSettings)
                    .HasForeignKey(d => d.TvprogProviderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ExtUserSe__TVPro__787EE5A0");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.ExtUserSettings)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ExtUserSett__UID__778AC167");
            });

            modelBuilder.Entity<GenreClassificator>(entity =>
            {
                entity.HasIndex(e => e.Gid)
                    .HasName("IX_GID");

                entity.HasIndex(e => e.Uid)
                    .HasName("IX_UID");

                entity.Property(e => e.GenreClassificatorId).HasColumnName("GenreClassificatorID");

                entity.Property(e => e.ContainPhrases).HasMaxLength(350);

                entity.Property(e => e.DeleteAfterDate).HasColumnType("datetime");

                entity.Property(e => e.Gid).HasColumnName("GID");

                entity.Property(e => e.NonContainPhrases).HasMaxLength(350);

                entity.Property(e => e.Uid).HasColumnName("UID");

                entity.HasOne(d => d.G)
                    .WithMany(p => p.GenreClassificator)
                    .HasForeignKey(d => d.Gid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__GenreClassi__GID__46B27FE2");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.GenreClassificator)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__GenreClassi__UID__47A6A41B");
            });

            modelBuilder.Entity<Genres>(entity =>
            {
                entity.HasKey(e => e.GenreId);

                entity.HasIndex(e => e.IconId)
                    .HasName("IX_IconID");

                entity.HasIndex(e => e.Uid)
                    .HasName("IX_UID");

                entity.Property(e => e.GenreId).HasColumnName("GenreID");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(sysdatetimeoffset())");

                entity.Property(e => e.GenreName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.IconId).HasColumnName("IconID");

                entity.Property(e => e.Uid).HasColumnName("UID");

                entity.HasOne(d => d.Icon)
                    .WithMany(p => p.Genres)
                    .HasForeignKey(d => d.IconId)
                    .HasConstraintName("FK__Genres__IconID__42E1EEFE");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Genres)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__Genres__UID__41EDCAC5");
            });

            modelBuilder.Entity<LogActions>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.Property(e => e.LogId).HasColumnName("LogID");

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50);

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(70);

                entity.Property(e => e.MessageAction)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.TsAction).HasDefaultValueSql("(sysdatetimeoffset())");

                entity.Property(e => e.UserAgent).HasMaxLength(500);
            });

            modelBuilder.Entity<MediaPic>(entity =>
            {
                entity.HasKey(e => e.IconId);

                entity.HasIndex(e => e.FileName)
                    .HasName("Unique_FileName")
                    .IsUnique();

                entity.HasIndex(e => new { e.Path25, e.FileName, e.IconId })
                    .HasName("_dta_index_MediaPic_5_1045578763__K2_K1_9");

                entity.HasIndex(e => new { e.IconId, e.FileName, e.ContentType, e.ContentCoding, e.Length, e.Length25, e.IsSystem, e.PathOrig, e.Path25 })
                    .HasName("_dta_index_MediaPic_5_1045578763__col__");

                entity.Property(e => e.IconId).HasColumnName("IconID");

                entity.Property(e => e.ContentCoding)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.ContentType)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Path25).HasMaxLength(256);

                entity.Property(e => e.PathOrig).HasMaxLength(256);
            });

            modelBuilder.Entity<Programmes>(entity =>
            {
                entity.HasIndex(e => e.Category)
                    .HasName("IX_Category");

                entity.HasIndex(e => e.Cid)
                    .HasName("IX_CID");

                entity.HasIndex(e => e.InternalChanId)
                    .HasName("IX_InternalChanID");

                entity.HasIndex(e => e.Tid)
                    .HasName("IX_TID");

                entity.HasIndex(e => e.Title)
                    .HasName("IX_Title");

                entity.HasIndex(e => e.TsStartMo)
                    .HasName("IX_TsStartMO");

                entity.HasIndex(e => e.TsStopMo)
                    .HasName("IX_TsStopMO");

                entity.HasIndex(e => new { e.ProgrammesId, e.Tid, e.Cid, e.InternalChanId, e.TsStart, e.TsStop, e.TsStartMo, e.TsStopMo, e.Title, e.Descr, e.Category })
                    .HasName("_dta_index_Programmes_5_1701581100__col__");

                entity.Property(e => e.ProgrammesId).HasColumnName("ProgrammesID");

                entity.Property(e => e.Category).HasMaxLength(500);

                entity.Property(e => e.Cid).HasColumnName("CID");

                entity.Property(e => e.Descr).HasMaxLength(1000);

                entity.Property(e => e.InternalChanId).HasColumnName("InternalChanID");

                entity.Property(e => e.Tid).HasColumnName("TID");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.TsStartMo)
                    .HasColumnName("TsStartMO")
                    .HasColumnType("datetime");

                entity.Property(e => e.TsStopMo)
                    .HasColumnName("TsStopMO")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.C)
                    .WithMany(p => p.Programmes)
                    .HasForeignKey(d => d.Cid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Programmes__CID__68487DD7");

                entity.HasOne(d => d.T)
                    .WithMany(p => p.Programmes)
                    .HasForeignKey(d => d.Tid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Programmes__TID__6754599E");
            });

            modelBuilder.Entity<RatingClassificator>(entity =>
            {
                entity.HasIndex(e => e.Rid)
                    .HasName("IX_RID");

                entity.HasIndex(e => e.Uid)
                    .HasName("IX_UID");

                entity.Property(e => e.RatingClassificatorId).HasColumnName("RatingClassificatorID");

                entity.Property(e => e.ContainPhrases)
                    .IsRequired()
                    .HasMaxLength(350);

                entity.Property(e => e.DeleteAfterDate).HasColumnType("datetime");

                entity.Property(e => e.NonContainPhrases).HasMaxLength(350);

                entity.Property(e => e.Rid).HasColumnName("RID");

                entity.Property(e => e.Uid).HasColumnName("UID");

                entity.HasOne(d => d.R)
                    .WithMany(p => p.RatingClassificator)
                    .HasForeignKey(d => d.Rid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RatingClass__RID__4F47C5E3");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.RatingClassificator)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__RatingClass__UID__503BEA1C");
            });

            modelBuilder.Entity<Ratings>(entity =>
            {
                entity.HasKey(e => e.RatingId);

                entity.HasIndex(e => e.IconId)
                    .HasName("IX_IconID");

                entity.HasIndex(e => e.Uid)
                    .HasName("IX_UID");

                entity.Property(e => e.RatingId).HasColumnName("RatingID");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(sysdatetimeoffset())");

                entity.Property(e => e.IconId).HasColumnName("IconID");

                entity.Property(e => e.RatingName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Uid).HasColumnName("UID");

                entity.HasOne(d => d.Icon)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.IconId)
                    .HasConstraintName("FK__Ratings__IconID__4B7734FF");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.Uid)
                    .HasConstraintName("FK__Ratings__UID__4A8310C6");
            });

            modelBuilder.Entity<SearchSettings>(entity =>
            {
                entity.Property(e => e.SearchSettingsId).HasColumnName("SearchSettingsID");

                entity.Property(e => e.Match).HasMaxLength(1000);

                entity.Property(e => e.NotMatch).HasMaxLength(1000);

                entity.Property(e => e.TsFinalFrom).HasColumnType("datetime");

                entity.Property(e => e.TsFinalTo).HasColumnType("datetime");

                entity.Property(e => e.Uid).HasColumnName("UID");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.SearchSettings)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SearchSetti__UID__74AE54BC");
            });

            modelBuilder.Entity<SystemUsers>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.HasIndex(e => e.Email)
                    .HasName("UQ__SystemUs__A9D1053497681097")
                    .IsUnique();

                entity.HasIndex(e => e.UserName)
                    .HasName("UQ__SystemUs__C9F2845679D4A296")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Address).HasMaxLength(1000);

                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.Catalog)
                    .IsRequired()
                    .HasMaxLength(36);

                entity.Property(e => e.Country).HasMaxLength(40);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(sysdatetimeoffset())");

                entity.Property(e => e.DateBegin)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('1800-01-01')");

                entity.Property(e => e.DateEnd)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('9999-31-12')");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.GmtZone).HasMaxLength(10);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.MiddleName).HasMaxLength(150);

                entity.Property(e => e.MobPhoneNumber).HasMaxLength(25);

                entity.Property(e => e.OtherPhoneNumber1).HasMaxLength(25);

                entity.Property(e => e.OtherPhoneNumber2).HasMaxLength(25);

                entity.Property(e => e.PassExtend)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PassHash)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(70);
            });

            modelBuilder.Entity<TvprogProviders>(entity =>
            {
                entity.HasKey(e => e.TvprogProviderId);

                entity.ToTable("TVProgProviders");

                entity.Property(e => e.TvprogProviderId).HasColumnName("TVProgProviderID");

                entity.Property(e => e.ContactEmail).HasMaxLength(100);

                entity.Property(e => e.ContactName).HasMaxLength(250);

                entity.Property(e => e.ProviderName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.ProviderWebSite)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Rss).HasMaxLength(200);
            });

            modelBuilder.Entity<TypeProg>(entity =>
            {
                entity.Property(e => e.TypeProgId).HasColumnName("TypeProgID");

                entity.Property(e => e.FileFormat).HasMaxLength(5);

                entity.Property(e => e.TvprogProviderId).HasColumnName("TVProgProviderID");

                entity.Property(e => e.TypeName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(d => d.TvprogProvider)
                    .WithMany(p => p.TypeProg)
                    .HasForeignKey(d => d.TvprogProviderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TypeProg__TVProg__5DCAEF64");
            });

            modelBuilder.Entity<UpdateProgLog>(entity =>
            {
                entity.Property(e => e.UpdateProgLogId).HasColumnName("UpdateProgLogID");

                entity.Property(e => e.ErrorMessage).HasMaxLength(1000);

                entity.Property(e => e.TsUpdateEnd).HasDefaultValueSql("(sysdatetimeoffset())");

                entity.Property(e => e.Wrid).HasColumnName("WRID");

                entity.HasOne(d => d.Wr)
                    .WithMany(p => p.UpdateProgLog)
                    .HasForeignKey(d => d.Wrid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UpdateProg__WRID__6383C8BA");
            });

            modelBuilder.Entity<UserChannels>(entity =>
            {
                entity.HasKey(e => e.UserChannelId);

                entity.Property(e => e.UserChannelId).HasColumnName("UserChannelID");

                entity.Property(e => e.Cid).HasColumnName("CID");

                entity.Property(e => e.DisplayName).HasMaxLength(300);

                entity.Property(e => e.IconId).HasColumnName("IconID");

                entity.Property(e => e.Uid).HasColumnName("UID");

                entity.HasOne(d => d.C)
                    .WithMany(p => p.UserChannels)
                    .HasForeignKey(d => d.Cid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserChannel__CID__59FA5E80");

                entity.HasOne(d => d.Icon)
                    .WithMany(p => p.UserChannels)
                    .HasForeignKey(d => d.IconId)
                    .HasConstraintName("FK__UserChann__IconI__5AEE82B9");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.UserChannels)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserChannel__UID__59063A47");
            });

            modelBuilder.Entity<UsersPrograms>(entity =>
            {
                entity.HasKey(e => e.UserProgramsId);

                entity.HasIndex(e => e.Gid)
                    .HasName("IX_GID");

                entity.HasIndex(e => e.Pid)
                    .HasName("IX_PID");

                entity.HasIndex(e => e.Rid)
                    .HasName("IX_RID");

                entity.HasIndex(e => e.Ucid)
                    .HasName("IX_UCID");

                entity.HasIndex(e => e.Uid)
                    .HasName("IX_UID");

                entity.Property(e => e.UserProgramsId).HasColumnName("UserProgramsID");

                entity.Property(e => e.Anons).HasMaxLength(1000);

                entity.Property(e => e.Gid).HasColumnName("GID");

                entity.Property(e => e.Pid).HasColumnName("PID");

                entity.Property(e => e.Rid).HasColumnName("RID");

                entity.Property(e => e.Ucid).HasColumnName("UCID");

                entity.Property(e => e.Uid).HasColumnName("UID");

                entity.HasOne(d => d.G)
                    .WithMany(p => p.UsersPrograms)
                    .HasForeignKey(d => d.Gid)
                    .HasConstraintName("FK__UsersProgra__GID__55F4C372");

                entity.HasOne(d => d.P)
                    .WithMany(p => p.UsersPrograms)
                    .HasForeignKey(d => d.Pid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UsersProgra__PID__55009F39");

                entity.HasOne(d => d.R)
                    .WithMany(p => p.UsersPrograms)
                    .HasForeignKey(d => d.Rid)
                    .HasConstraintName("FK__UsersProgra__RID__56E8E7AB");

                entity.HasOne(d => d.Uc)
                    .WithMany(p => p.UsersPrograms)
                    .HasForeignKey(d => d.Ucid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UsersProgr__UCID__540C7B00");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.UsersPrograms)
                    .HasForeignKey(d => d.Uid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UsersProgra__UID__531856C7");
            });

            modelBuilder.Entity<WebResources>(entity =>
            {
                entity.HasKey(e => e.WebResourceId);

                entity.Property(e => e.WebResourceId).HasColumnName("WebResourceID");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.ResourceName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.ResourceUrl)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Tpid).HasColumnName("TPID");

                entity.HasOne(d => d.Tp)
                    .WithMany(p => p.WebResources)
                    .HasForeignKey(d => d.Tpid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__WebResourc__TPID__60A75C0F");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
