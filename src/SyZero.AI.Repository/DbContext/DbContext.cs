
using SqlSugar;
using System;
using SyZero.AI.Core.Chat;
using SyZero.SqlSugar.DbContext;

namespace SyZero.AI.Repository
{
    public class DbContext : SyZeroDbContext
    {
        public DbContext(ConnectionConfig config) : base(config)
        {
          
        }
    }
}

