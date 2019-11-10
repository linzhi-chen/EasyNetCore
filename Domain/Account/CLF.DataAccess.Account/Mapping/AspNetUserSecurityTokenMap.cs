using CLF.Domain.Core.Mapping;
using CLF.Model.Account;
using CLF.Model.Account.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLF.DataAccess.Account.Mapping
{
   public class AspNetUserSecurityTokenMap: EntityTypeConfiguration<AspNetUserSecurityToken>
    {
        public override void Configure(EntityTypeBuilder<AspNetUserSecurityToken> builder)
        {
            builder.ToTable(Tables.AspNetUserSecurityToken);
            builder.HasKey(o => o.Id);
            builder.Property(p => p.Id)
                .UseSqlServerIdentityColumn()
                .UseMySqlIdentityColumn();

            builder.Property(p => p.RefreshToken).IsRequired();
            builder.Property(p => p.UserName).HasMaxLength(256).IsRequired();
            builder.Property(p => p.ClientId).HasMaxLength(512);

            base.Configure(builder);
        }
    }
}
