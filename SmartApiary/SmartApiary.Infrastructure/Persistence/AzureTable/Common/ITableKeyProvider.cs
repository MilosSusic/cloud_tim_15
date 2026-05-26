using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Persistence.AzureTable.Common
{
    internal interface ITableKeyProvider<T>
    {
        string GetPartitionKey(T model);
        string GetRowKey(T model);
    }
}
