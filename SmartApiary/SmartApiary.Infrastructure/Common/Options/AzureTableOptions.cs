using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Common.Options
{
    internal class AzureTableOptions
    {
        public string ConnectionString { get; init; } = string.Empty;
        public string ScaleMeasurementsTable { get; init; } = string.Empty;
    }
}
