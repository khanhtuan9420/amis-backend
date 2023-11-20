using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.DL
{
    public interface IBaseDL<T>
    {
        IUnitOfWork Uow { get; }
        /// <summary>
        /// Filter Record
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        /// Xuân Đào (28/03/2023)
        public Task<IEnumerable<T>> FilterRecordAsync(string? keyWord, int? pageSize, int? pageNumber, IUnitOfWork? uow=null);

        /// <summary>
        /// Lấy nhiều bản ghi theo keyword
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        public Task<IEnumerable<T>> FilterRecordExpandAsync(string? keyWord, int? pageSize, int? pageNumber, IUnitOfWork? uow=null);

        /// <summary>
        /// Lấy các bản ghi con theo id của bản ghi cha và keyword
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        public Task<IEnumerable<T>> ExpandRowAsync(string? keyWord, Guid id, IUnitOfWork? uow=null);

        /// <summary>
        /// Tìm kiếm bản ghi theo id
        /// </summary>
        /// <param name="id">id bản ghi cần tìm</param>
        /// <returns>Generic</returns>
        /// Created by: dktuan (01/11/2023)
        public Task<T> GetRecordByIdAsync(Guid id, IUnitOfWork? uow=null);

        /// <summary>
        /// Lấy toàn bộ record
        /// </summary>
        /// <returns>IEnumerable</returns>
        /// Xuân Đào (28/03/2023)
        public Task<IEnumerable<dynamic>> GetRecordsAsync(IUnitOfWork? uow=null);

        /// <summary>
        /// Xóa một bản ghi theo id
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        /// Xuân Đào (28/03/2023)
        public Task<int> DeleteRecordAsync(Guid recordId, IUnitOfWork? uow=null);

        public Task<int> MultipleDeleteRecord(List<Guid> recordIds, IUnitOfWork? uow = null);

        /// <summary>
        /// Tạo mới bản ghi
        /// </summary>
        /// <param name="record">Thông tin bản ghi</param>
        /// <returns>Service result</returns>
        /// Created by: dktuan (01/11/2023)
        public Task<int> CreateRecordAsync(T record, IUnitOfWork? uow=null);

        /// <summary>
        /// Cập nhật thông tin bản ghi
        /// </summary>
        /// <param name="record">Thông tin bản ghi</param>
        /// <returns>Service result</returns>
        /// Created by: dktuan (07/11/2023)
        public Task<int> UpdateRecordAsync(Guid id, T record, IUnitOfWork? uow=null);


        public Task<dynamic> BulkCreate(IEnumerable<T> RecordList, IUnitOfWork? uow=null);

        public Task<dynamic> BulkUpdate(IEnumerable<T> recordList, IUnitOfWork? uow=null);

        public Task<int> GetTotalRecordsAsync(IUnitOfWork? uow=null);

        public Task<bool> CheckConflictCode(string code, IUnitOfWork? uow = null);

    }
}
