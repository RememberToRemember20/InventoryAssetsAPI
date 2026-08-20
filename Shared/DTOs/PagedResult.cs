using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public MetaData MetaData { get; set; } = new MetaData();
    }
}
