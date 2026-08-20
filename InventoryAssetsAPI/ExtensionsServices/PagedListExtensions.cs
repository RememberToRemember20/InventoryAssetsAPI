using Microsoft.EntityFrameworkCore;
using Shared.DTOs;

namespace InventoryAssetsAPI.ExtensionsServices
{
    public static class PagedListExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            // 1. حساب إجمالي العناصر
            var count = await source.CountAsync();

            // 2. جلب عناصر الصفحة الحالية فقط
            var items = await source.Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            // 3. تغليف النتيجة وإرجاعها
            return new PagedResult<T>
            {
                Items = items,
                MetaData = new MetaData
                {
                    TotalCount = count,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling(count / (double)pageSize)
                }
            };
        }
    }
}
