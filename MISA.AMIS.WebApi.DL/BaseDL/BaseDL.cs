using Dapper;
using MISA.AMIS.WebApi.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.DL
{
    public abstract class BaseDL<T> : IBaseDL<T> where T : BaseEntity, IBaseEntity
    {
        protected IUnitOfWork Uow;
        protected virtual List<string> ListPropsExcluded { get; set; } = null;

        public BaseDL(IUnitOfWork uow)
        {
            Uow = uow;
        }

        public virtual string TableName { get; set; } = typeof(T).Name;
        public virtual string TableNameLower { get; set; } = typeof(T).Name.ToLower();

        IUnitOfWork IBaseDL<T>.Uow => Uow;

        public async Task<dynamic> BulkCreate(IEnumerable<T> recordList, IUnitOfWork? uow = null)
        {
            if (uow != null) Uow = uow;
            if (recordList.Count() < 1) return 0;
            var properties = typeof(T).GetProperties()
        .Where(p => ListPropsExcluded == null || !ListPropsExcluded.Contains(p.Name));

            var columns = string.Join(", ", properties.Select(p => $"\"{p.Name}\""));
            var valuesList = new List<string>();
            var parameters = new DynamicParameters();

            int parameterIndex = 0;
            foreach (var record in recordList)
            {
                var values = string.Join(", ", properties.Select(p =>
                {
                    var paramName = $"param{parameterIndex++}";
                    parameters.Add(paramName, p.GetValue(record));
                    return $"@{paramName}";
                }));
                valuesList.Add($"({values})");
            }

            var valuesBatch = string.Join(", ", valuesList);
            var insertQuery = $"INSERT INTO \"{TableName}\" ({columns}) VALUES {valuesBatch} RETURNING *";

            var result = await Uow.Connection.QueryAsync<T>(insertQuery, parameters);
            return result.Count();
        }

        /// <summary>
        /// Tạo mới bản ghi
        /// </summary>
        /// <param name="record">Thông tin bản ghi</param>
        /// <returns>Service result</returns>
        /// Created by: dktuan (01/11/2023)
        public async Task<int> CreateRecordAsync(T record, IUnitOfWork? uow = null)
        {
            if (uow != null) Uow = uow;
            var connection = await Uow.OpenConnectionAsync();
            var properties = record.GetType().GetProperties();
            var parameters = new DynamicParameters();
            foreach (var property in properties)
            {
                if (!ListPropsExcluded.Contains(property.Name))
                {
                    parameters.Add(property.Name.ToLower(), property.GetValue(record));
                }
            }
            var result = await connection.ExecuteAsync($"proc_{TableNameLower}_create", parameters, commandType: System.Data.CommandType.StoredProcedure);
            return result;
        }

        public async Task<int> DeleteRecordAsync(Guid recordId, IUnitOfWork? uow = null)
        {
            if (uow != null) Uow = uow;
            var sql = $"delete from \"{TableName}\" where \"{TableName}Id\" = @id";
            var param = new DynamicParameters();
            param.Add("id", recordId);
            var result = await Uow.Connection.ExecuteAsync(sql, param);
            return result;
        }

        /// <summary>
        /// Lấy các bản ghi con theo id của bản ghi cha và keyword
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        public async Task<IEnumerable<T>> ExpandRowAsync(string? keyWord, Guid id, IUnitOfWork? uow = null)
        {
            if (uow != null) Uow = uow;
            if (string.IsNullOrEmpty(keyWord)) keyWord = "";
            var connection = await Uow.OpenConnectionAsync();
            var sql = $"select * from func_{TableNameLower}_get_children(@keyword, @id)";
            var param = new DynamicParameters();
            param.Add("keyword", keyWord);
            param.Add("id", id);
            var result = await connection.QueryAsync<T>(sql, param);
            return result;
        }

        public async Task<IEnumerable<T>> FilterRecordAsync(string? keyWord, int? pageSize, int? pageNumber, IUnitOfWork? uow = null)
        {
            if (uow != null) Uow = uow;
            if (string.IsNullOrEmpty(keyWord)) keyWord = "";
            int? startRow = (pageNumber - 1) * pageSize;
            var connection = await Uow.OpenConnectionAsync();
            var sql = $"select * from func_{TableNameLower}_filter(@keyword, @pageSize, @startRow)";
            var param = new DynamicParameters();
            param.Add("keyword", keyWord);
            param.Add("pageSize", pageSize);
            param.Add("startRow", startRow);
            var result = await connection.QueryAsync<T>(sql, param);
            return result;
        }

        /// <summary>
        /// Lấy nhiều bản ghi theo keyword
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        public async Task<IEnumerable<T>> FilterRecordExpandAsync(string? keyWord, int? pageSize, int? pageNumber, IUnitOfWork? uow = null)
        {
            if (uow != null) Uow = uow;
            if (string.IsNullOrEmpty(keyWord)) keyWord = "";
            int? startRow = (pageNumber - 1) * pageSize;
            var connection = await Uow.OpenConnectionAsync();
            var sql = $"select * from func_{TableNameLower}_filter_expand(@keyword, @pageSize, @startRow)";
            var param = new DynamicParameters();
            param.Add("keyword", keyWord);
            param.Add("pageSize", pageSize);
            param.Add("startRow", startRow);
            var result = await connection.QueryAsync<T>(sql, param);
            return result;
        }

        /// <summary>
        /// Lấy một bản ghi bằng id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        public virtual async Task<T> GetRecordByIdAsync(Guid id, IUnitOfWork? uow = null)
        {
            if (uow != null) Uow = uow;
            var connection = await Uow.OpenConnectionAsync();
            var sql = $"select * from \"{TableName}\" WHERE \"{TableName}Id\" = @id";
            var param = new DynamicParameters();
            param.Add("id", id);
            var result = await connection.QuerySingleOrDefaultAsync<T>(sql, param);
            return result;
        }

        public Task<IEnumerable<dynamic>> GetRecordsAsync(IUnitOfWork? uow = null)
        {
            if (uow != null) Uow = uow;
            throw new NotImplementedException();
        }

        public async Task<int> MultipleDeleteRecord(List<Guid> recordIds, IUnitOfWork? uow = null)
        {
            if (uow != null) Uow = uow;
            if (recordIds.Count < 1) return 0;
            var param = new DynamicParameters();
            param.Add("idList", recordIds);
            string sql = $"delete from \"{TableName}\" where \"{TableName}\".\"{TableName}Id\" = ANY(@idList)";
            var result = await Uow.Connection.ExecuteAsync(sql, param, transaction: Uow.Transaction);
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin bản ghi
        /// </summary>
        /// <param name="record">Thông tin bản ghi</param>
        /// <returns>Service result</returns>
        /// Created by: dktuan (07/11/2023)
        public async Task<int> UpdateRecordAsync(Guid id, T record, IUnitOfWork? uow = null)
        {
            if (uow != null) Uow = uow;
            var connection = await Uow.OpenConnectionAsync();
            var properties = record.GetType().GetProperties().Where(p => ListPropsExcluded == null || !ListPropsExcluded.Contains(p.Name)); ;
            var parameters = new DynamicParameters();
            foreach (var property in properties)
            {
                if (property.Name != "CreatedBy" && property.Name != "CreatedDate")
                    parameters.Add(property.Name.ToLower(), property.GetValue(record));
            }
            var result = await connection.ExecuteAsync($"proc_{TableNameLower}_update", parameters, commandType: System.Data.CommandType.StoredProcedure);
            return result;
        }

        public async Task<dynamic> BulkUpdate(IEnumerable<T> recordList, IUnitOfWork? uow = null)
        {
            if (uow != null) Uow = uow;

            var properties = typeof(T).GetProperties()
                .Where(p => ListPropsExcluded == null || !ListPropsExcluded.Contains(p.Name));

            var updateStatements = new List<string>();
            var parameters = new DynamicParameters();

            int parameterIndex = 0;

            foreach (var record in recordList)
            {
                var setStatements = properties.Select(p =>
                {
                    var paramName = $"param{parameterIndex++}";
                    parameters.Add(paramName, p.GetValue(record));
                    return $"\"{p.Name}\" = @{paramName}";
                });

                var setClause = string.Join(", ", setStatements);
                var whereClause = $"\"{TableName}Id\" = '{record.GetId()}'"; 

                var updateStatement = $"UPDATE \"{TableName}\" SET {setClause} WHERE {whereClause}";
                updateStatements.Add(updateStatement);
            }

            var updateQuery = string.Join("; ", updateStatements);

            var result = await Uow.Connection.ExecuteAsync(updateQuery, parameters);
            return result;
        }

        public async Task<int> GetTotalRecordsAsync(IUnitOfWork? uow = null)
        {
            if (uow != null) Uow = uow;
            var sql = $"select count(*) from \"{TableName}\"";
            var result = await Uow.Connection.QueryFirstOrDefaultAsync<int>(sql);
            return result;
        }

        public virtual Task<bool> CheckConflictCode(string code, IUnitOfWork? uow = null)
        {
            throw new NotImplementedException();
        }
    }
}
