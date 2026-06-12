using LabelForge.Core.Models.DataSources;

namespace LabelForge.Core.Interfaces;

public interface IDataSourceService
{
    Task<DataSource> CreateAsync(DataSource dataSource);
    Task<DataSource?> GetByIdAsync(Guid id);
    Task<IEnumerable<DataSource>> GetAllAsync();
    Task<DataSource> UpdateAsync(DataSource dataSource);
    Task DeleteAsync(Guid id);
    Task<bool> TestConnectionAsync(Guid id);
    Task<IEnumerable<Dictionary<string, object?>>> PreviewAsync(Guid id, int limit = 10);
    Task<IEnumerable<Dictionary<string, object?>>> GetRecordsAsync(Guid id);
}