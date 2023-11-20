using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.BL
{
    public interface IBaseBL<T>
    {
        /// <summary>
        /// Lọc bản ghi theo điều kiện
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        /// Xuân Đào (25/03/2023)
        public Task<object> FilterRecordAsync(string? keyWord, int? pageSize, int? pageNumber, Guid? id);

        /// <summary>
        /// Lấy nhiều bản ghi theo keyword
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        public Task<object> FilterRecordExpandAsync(string? keyWord, int? pageSize, int? pageNumber, Guid? id);

        /// <summary>
        /// Lấy các bản ghi con theo id của bản ghi cha và keyword
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        public Task<IEnumerable<T>> ExpandRowAsync(string? keyWord, Guid id);

        /// <summary>
        /// Tìm kiếm bản ghi theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        public Task<T> GetRecordByIdAsync(Guid id);

        /// <summary>
        /// Lấy toàn bộ bản ghi
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<dynamic>> GetRecordsAsync();

        /// <summary>
        /// Xóa 1 bản ghi theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<int> DeleteRecordAsync(Guid id);

        public Task<int> MultipleDeleteRecordAsync(string recordIds);

        /// <summary>
        /// Tạo 1 bản ghi mới
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        public Task<int> CreateRecordAsync(T record);

        /// <summary>
        /// Sửa thông tin một bản ghi
        /// </summary>
        /// <param name="id"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        /// Created by: dktuan (07/11/2023)
        public Task<int> UpdateRecordAsync(Guid id, T record);


        public Task<dynamic> BulkCreate(IEnumerable<T> records);
    }
}
