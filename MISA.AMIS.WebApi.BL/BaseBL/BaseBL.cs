using MISA.AMIS.WebApi.Common;
using MISA.AMIS.WebApi.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace MISA.AMIS.WebApi.BL
{
    public abstract class BaseBL<T> : IBaseBL<T> where T : IBaseEntity
    {
        protected readonly IBaseDL<T> _baseDL;

        public BaseBL(IBaseDL<T> baseDL)
        {
            _baseDL = baseDL;
        }

        protected async Task<TBusinessResult> ExecuteTransactionAsync<TBusinessResult>(
        Func<Task<TBusinessResult>> businessOperation)
        {
            try
            {
                // Bắt đầu giao dịch
                await _baseDL.Uow.BeginTransactionAsync();

                // Thực hiện hoạt động kinh doanh trong phạm vi giao dịch
                var result = await businessOperation();

                // Commit giao dịch nếu không có lỗi
                await _baseDL.Uow.CommitAsync();

                return result;
            }
            catch (Exception)
            {
                // Nếu có lỗi, hủy bỏ giao dịch
                await _baseDL.Uow.RollbackAsync();
                throw; // Chuyển tiếp lỗi để xử lý ở lớp gọi
            }
        }

        public Task<dynamic> BulkCreate(IEnumerable<T> records)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tạo 1 bản ghi mới
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        public async Task<int> CreateRecordAsync(T record)
        {
            if (record.GetId() == Guid.Empty)
            {
                record.SetId(Guid.NewGuid());
            }
            if (record is BaseEntity baseEntity)
            {
                baseEntity.CreatedBy ??= "Putin";
                baseEntity.CreatedDate ??= DateTime.Now;
                baseEntity.ModifiedBy ??= baseEntity.CreatedBy;
                baseEntity.ModifiedDate ??= DateTime.Now;
            }
            await ExcuteCreateBusiness(record);
            var result = await _baseDL.CreateRecordAsync(record, null);
            return result;
        }

        public async Task<int> DeleteRecordAsync(Guid id)
        {
            await ExecuteDeleteBusiness(id);
            var result = await _baseDL.DeleteRecordAsync(id);
            return result;
        }

        /// <summary>
        /// Lấy các bản ghi con theo id của bản ghi cha và keyword
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        public async Task<IEnumerable<T>> ExpandRowAsync(string? keyWord, Guid id)
        {
            var result = await _baseDL.ExpandRowAsync(keyWord, id);
            return result;
        }

        public async Task<object> FilterRecordAsync(string? keyWord, int? pageSize, int? pageNumber, Guid? id)
        {
            object selected = null;
            if (id != null && id != Guid.Empty)
            {
                selected = await _baseDL.GetRecordByIdAsync((Guid)id);
            }
            var result = await _baseDL.FilterRecordAsync(keyWord, pageSize, pageNumber);

            if (selected != null)
            {
                if (pageNumber == 1 && keyWord==null)
                {
                    result = result.Where(entity => entity.GetId() != id);
                    result = new List<T> { (T)selected }.Concat(result);
                }
                else if(pageNumber>1)
                {
                    result = result.Where(entity => entity.GetId() != id);
                }
            }

            var totalRecord = await _baseDL.GetTotalRecordsAsync();

            return new
            {
                Data = result,
                TotalRecord = totalRecord
            };
        }

        /// <summary>
        /// Lấy nhiều bản ghi theo keyword
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        public async Task<object> FilterRecordExpandAsync(string? keyWord, int? pageSize, int? pageNumber, Guid? id)
        {
            object selected = null;
            if (id != null && id != Guid.Empty)
            {
                selected = await _baseDL.GetRecordByIdAsync((Guid)id);
            }
            var result = await _baseDL.FilterRecordExpandAsync(keyWord, pageSize, pageNumber);
            if (selected != null)
            {
                if (pageNumber == 1)
                {
                    result = result.Where(entity => entity.GetId() != id);
                    result = new List<T> { (T)selected }.Concat(result);
                }
                else if (pageNumber > 1)
                {
                    result = result.Where(entity => entity.GetId() != id);
                }
            }
            var totalRecord = await _baseDL.GetTotalRecordsAsync();

            return new
            {
                Data = result,
                TotalRecord = totalRecord
            };
        }

        /// <summary>
        /// Lấy một bản ghi bằng id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        public async Task<T> GetRecordByIdAsync(Guid id)
        {
            var result = await _baseDL.GetRecordByIdAsync(id);
            return result;
        }

        public Task<IEnumerable<dynamic>> GetRecordsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> MultipleDeleteRecordAsync(string recordIds)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sửa thông tin một bản ghi
        /// </summary>
        /// <param name="id"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        /// Created by: dktuan (07/11/2023)
        public async Task<int> UpdateRecordAsync(Guid id, T record)
        {
            if (record is BaseEntity baseEntity)
            {
                baseEntity.ModifiedBy ??= "Putin";
                baseEntity.ModifiedDate ??= DateTime.Now;
            }
            await ExecuteUpdateBusiness(id, record);
            var result = await _baseDL.UpdateRecordAsync(id, record);
            return result;
        }


        /// <summary>
        /// Validate giá trị bản ghi trước khi tạo
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// Created by: dktuan(07/11/2023)
        public virtual async Task ExcuteCreateBusiness(T entity)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Validate giá trị bản ghi trước khi update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// 
        /// 
        /// 
        /// Created by: dktuan(07/11/2023)
        public virtual async Task ExecuteUpdateBusiness(Guid id, T entity)
        {
            await Task.CompletedTask;
        }

        public virtual async Task ExecuteDeleteBusiness(Guid id)
        {
            await Task.CompletedTask;
        }
    }
}
