using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Persistence.AzureTable.Common
{
    internal interface ITableMapper<TDomain, TEntity>
            where TEntity : class, ITableEntity, new()
    {
        TEntity ToEntity(TDomain domain);
        TDomain? ToDomain(TEntity entity);
    }
}
