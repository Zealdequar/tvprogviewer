using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVProgViewer.Common
{
    public static class ControllerExtensions
    {
        public static object GetJsonPagingInfo<T>(int page, int rows, KeyValuePair<int, T> result)
        {
            int pageSize = rows;
            int totalRecords = result.Key;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = result.Value
            };
            return jsonData;
        }
    }
}
