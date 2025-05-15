using AnnouncementBoard.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AnnouncementBoard.Data
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly AnnouncementsDbContext _context;

        public AnnouncementRepository(AnnouncementsDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Announcement>> GetAllAsync()
        {
            return await _context.Announcements
                .FromSqlRaw("EXEC sp_GetAllAnnouncements")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Announcement?> GetByIdAsync(int id)
        {
            var list = await _context.Announcements
                .FromSqlRaw("EXEC sp_GetAnnouncementById @Id = {0}", id)
                .AsNoTracking()
                .ToListAsync();
            return list.FirstOrDefault();
        }

        public async Task<int> CreateAsync(Announcement a)
        {
            var conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "sp_CreateAnnouncement";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@Title", a.Title));
                cmd.Parameters.Add(new SqlParameter("@Description", a.Description));
                cmd.Parameters.Add(new SqlParameter("@Category", a.Category));
                cmd.Parameters.Add(new SqlParameter("@SubCategory", a.SubCategory));

                var raw = await cmd.ExecuteScalarAsync();

                return Convert.ToInt32(raw);
            }
            finally
            {
                await conn.CloseAsync();
            }
        }


        public async Task UpdateAsync(Announcement a)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_UpdateAnnouncement @Id = {0}, @Title = {1}, @Description = {2}, @Status = {3}, @Category = {4}, @SubCategory = {5}", a.Id, a.Title, a.Description, a.Status, a.Category, a.SubCategory);
        }

        public async Task DeleteAsync(int id)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteAnnouncement @Id = {0}", id);
        }

        public async Task<IEnumerable<AnnouncementCategory>> GetCategoriesAsync()
        {
            return await _context.AnnouncementCategories
                .FromSqlRaw("EXEC sp_GetAllCategories")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<AnnouncementSubCategory>> GetSubCategoriesAsync()
        {
            return await _context.AnnouncementSubCategories
                .FromSqlRaw("EXEC sp_GetAllSubCategories")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<AnnouncementSubCategory>> GetSubCategoriesByCategoryAsync(string category)
        {
            return await _context.AnnouncementSubCategories
                .FromSqlRaw("EXEC sp_GetSubCategoriesByCategory @Category = {0}", category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Announcement>> FilterAsync(IEnumerable<string>? categories, IEnumerable<string>? subcategories)
        {
            var categoryList = categories?.Any() == true ? string.Join(",", categories) : null;
            var subcategoryList = subcategories?.Any() == true ? string.Join(",", subcategories) : null;

            return await _context.Announcements
                .FromSqlRaw(
                    "EXEC sp_FilterAnnouncements @Categories = {0}, @SubCategories = {1}",
                    categoryList == null ? DBNull.Value : categoryList,
                    subcategoryList == null ? DBNull.Value : subcategoryList
                )
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
