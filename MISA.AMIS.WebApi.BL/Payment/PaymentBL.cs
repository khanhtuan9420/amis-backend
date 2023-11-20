using MISA.AMIS.WebApi.Common;
using MISA.AMIS.WebApi.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.POIFS.Properties;
using MISA.AMIS.WebApi.Common.Enum;

namespace MISA.AMIS.WebApi.BL
{
    public class PaymentBL : BaseBL<Payment>, IPaymentBL
    {
        private readonly IPaymentDL _paymentDL;
        private readonly IPaymentDetailDL _paymentDetailDL;
        private List<string> HeaderTexts { get; set; }
        private List<string> Headers { get; set; }
        private string SheetTitle { get; set; }
        private string TableTitle { get; set; }
        public PaymentBL(IPaymentDL paymentDL, IPaymentDetailDL paymentDetailDL) : base(paymentDL)
        {
            _paymentDL = paymentDL;
            _paymentDetailDL = paymentDetailDL;

            HeaderTexts = new List<string>
            {
                "Ngày hạch toán",
                "Ngày chứng từ",
                "Số chứng từ",
                "Diễn giải",
                "Số tiền",
                "Mã đối tượng",
                "Địa chỉ"
            };
            Headers = new List<string>
            {
                "PaymentDate",
                "RecordDate",
                "PaymentNo",
                "Explain",
                "Total",
                "SupplierCode",
                "Address"
            };
            SheetTitle = "Danh sách chi tiền";
            TableTitle = "Danh sách chi tiền";
        }

        public async Task<dynamic> GetPaymentsAsync(string? keyWord, int? pageSize, int? pageNumber)
        {
            var payments = await _paymentDL.FilterRecordAsync(keyWord, pageSize, pageNumber);
            var total = await _paymentDL.GetTotalAsync();
            var totalRecord = await _paymentDL.GetTotalRecordsAsync();
            var result = new
            {
                Data = payments,
                Total = total,
                TotalRecord = totalRecord,
            };
            return result;
        }

        public async Task<Payment> CreateRecordAsync(Payment payment, List<PaymentDetail> details)
        {
            if (payment.GetId() == Guid.Empty)
            {
                payment.SetId(Guid.NewGuid());
            }
            if (payment is BaseEntity baseEntity)
            {
                baseEntity.CreatedBy ??= "Putin";
                baseEntity.CreatedDate ??= DateTime.Now;
                baseEntity.ModifiedBy ??= baseEntity.CreatedBy;
                baseEntity.ModifiedDate ??= DateTime.Now;
            }
            foreach (var item in details)
            {
                if (item.GetId() == Guid.Empty)
                {
                    item.SetId(Guid.NewGuid());
                }
                item.PaymentId = payment.GetId();
                item.CreatedBy ??= "Putin";
                item.CreatedDate ??= DateTime.Now;
                item.ModifiedBy ??= item.CreatedBy;
                item.ModifiedDate ??= DateTime.Now;

            }
            var isValidCode = await _paymentDL.CheckConflictCode(payment.PaymentNo);
            if(!isValidCode)
            {
                var newCode = await _paymentDL.GetNewCode();
                throw new ConflictException
                {
                    Exception = new BaseException
                    {
                        ErrorCode = ErrorCode.CodeExisted,
                        UserMessage = "Số phiếu chi đã tồn tại",
                        OtherData = newCode,
                        Errors = new[]
                        {
                            new
                            {
                                Field = "PaymentNo",
                                Msg = "Số phiếu chi đã tồn tại"
                            }
                        }
                    }
                };
            }
            return await ExecuteTransactionAsync(async () =>
            {
                await _paymentDL.CreateRecordAsync(payment);
                await _paymentDetailDL.BulkCreate(details, _paymentDL.Uow);
                return payment;
            });
        }

        public async Task UpdateRecordAsync(Payment payment, IEnumerable<PaymentDetail> paymentDetailsUpdate, IEnumerable<PaymentDetail> paymentDetailsCreate, List<Guid> detailDeleteIds)
        {
            foreach (var item in paymentDetailsCreate)
            {
                if (item.GetId() == Guid.Empty) item.SetId(Guid.NewGuid());
                item.PaymentId = payment.GetId();
                item.CreatedBy ??= "Putin";
                item.CreatedDate ??= DateTime.Now;
                item.ModifiedBy ??= item.CreatedBy;
                item.ModifiedDate ??= DateTime.Now;
            }
            foreach (var item in paymentDetailsUpdate)
            {
                item.ModifiedBy ??= "Putin";
                item.ModifiedDate ??= DateTime.Now;
            }
            await ExecuteTransactionAsync(async () =>
            {
                await _paymentDL.UpdateRecordAsync(payment.GetId(), payment);
                await _paymentDetailDL.MultipleDeleteRecord(detailDeleteIds, _paymentDL.Uow);
                await _paymentDetailDL.BulkCreate(paymentDetailsCreate, _paymentDL.Uow);
                await _paymentDetailDL.BulkUpdate(paymentDetailsUpdate, _paymentDL.Uow);
                return payment;
            });
        }

        public async Task<string> GetNewCode()
        {
            var result = await _paymentDL.GetNewCode();
            return result;
        }

        public void SetCellValue(XSSFWorkbook workbook, ICell cell, Type type, object value, string dateFormat)
        {
            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Left;
            cell.CellStyle = cellStyle;
            if (type.Equals(typeof(int)))
            {
                cell.SetCellValue((int)value);
            }
            else if (type.Equals(typeof(string)))
            {
                if (long.TryParse((string)value, out long numValue))
                {
                    cell.SetCellValue(numValue);
                }
                else
                {
                    cell.SetCellValue((string)value);
                }
            }
            else if (value is DateTime date)
            {
                ICellStyle dateCellStyle = workbook.CreateCellStyle();
                dateCellStyle.Alignment = HorizontalAlignment.Center;
                // Tạo một đối tượng DataFormat để định dạng ngày tháng
                IDataFormat dataFormat = workbook.CreateDataFormat();
                dateCellStyle.DataFormat = dataFormat.GetFormat(dateFormat);
                cell.CellStyle = dateCellStyle;
                cell.SetCellValue(date);
            }
        }

        /// <summary>
        /// Định dạng cho ô tiêu đề
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="cell">Ô tiêu đề</param>
        /// Created by: dktuan (29/09/2023)
        public void StyleSheetTitle(XSSFWorkbook workbook, ICell cell)
        {
            ICellStyle sheetTitleStyle = workbook.CreateCellStyle();
            IFont sheetTitleFont = workbook.CreateFont();
            sheetTitleFont.FontHeightInPoints = 18;
            sheetTitleFont.IsBold = true;
            sheetTitleStyle.Alignment = HorizontalAlignment.Center;
            sheetTitleStyle.SetFont(sheetTitleFont);
            cell.SetCellValue(TableTitle);
            cell.CellStyle = sheetTitleStyle;

        }

        /// <summary>
        /// Định dạng cho các ô tiêu đề của cột
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="cell">Ô tiêu đề của cột</param>
        /// Created by: dktuan (29/09/2023)
        public void StyleHeaderCell(XSSFWorkbook workbook, ICell cell)
        {
            IFont headerFont = workbook.CreateFont();
            headerFont.IsBold = true;
            // Tạo một đối tượng CellStyle
            ICellStyle headerStyle = workbook.CreateCellStyle();

            // Đặt màu nền xám cho CellStyle
            headerStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            headerStyle.FillPattern = FillPattern.SolidForeground;
            headerStyle.SetFont(headerFont);
            cell.CellStyle = headerStyle;
        }

        /// <summary>
        /// ExportAsync file excel
        /// </summary>
        /// <param name="filters">Giá trị lọc để giới hạn các bản ghi</param>
        /// <returns></returns>
        public async Task<MemoryStream> ExportAsync(string filters)
        {
            var jsonData = new List<Payment>();
            jsonData = (await _paymentDL.FilterRecordAsync(filters, null, null)).ToList();

            var workbook = new XSSFWorkbook();

            var sheet = workbook.CreateSheet(SheetTitle);

            var sheetTitleRow = sheet.CreateRow(0);

            var headerRow = sheet.CreateRow(1);

            ICellStyle alignLeft = workbook.CreateCellStyle();
            alignLeft.Alignment = HorizontalAlignment.Left;
            headerRow.CreateCell(0).SetCellValue("STT");
            StyleHeaderCell(workbook, headerRow.GetCell(0));
            sheetTitleRow.CreateCell(0);
            for (int i = 1; i <= HeaderTexts.Count; i++)
            {
                ICell cell = sheetTitleRow.CreateCell(i);
                headerRow.CreateCell(i).SetCellValue(HeaderTexts[i - 1]);
                StyleHeaderCell(workbook, headerRow.GetCell(i));
            }
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, HeaderTexts.Count));

            StyleSheetTitle(workbook, sheetTitleRow.GetCell(0));

            int rowNumber = 2;
            int count = 0;
            foreach (var item in jsonData)
            {
                count++;
                var dataRow = sheet.CreateRow(rowNumber);
                dataRow.CreateCell(0).SetCellValue(count);
                dataRow.GetCell(0).CellStyle = alignLeft;
                for (int i = 1; i <= Headers.Count; i++)
                {
                    var type = item.GetType().GetProperty(Headers[i - 1]).PropertyType;
                    var valueCell = item.GetType().GetProperty(Headers[i - 1]).GetValue(item, null);
                    var cell = dataRow.CreateCell(i);
                    SetCellValue(workbook, cell, type, valueCell, "dd/mm/yyyy");
                }
                rowNumber++;
            }

            for (int i = 0; i <= Headers.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            // Tạo MemoryStream, ghi dữ liệu vào nó
            var memoryStream = new MemoryStream();
            workbook.Write(memoryStream);

            // Tạo một bản sao của MemoryStream
            var memoryStreamCopy = new MemoryStream(memoryStream.ToArray());
            return memoryStreamCopy;
        }

        public async Task<int> DeleteRecordsAsync(List<Guid> ids)
        {
            return await ExecuteTransactionAsync(async () =>
            {
                var result = 0;
                result += await _paymentDetailDL.DeleteRecordsByMasterIds(ids, _paymentDetailDL.Uow);
                result += await _paymentDL.MultipleDeleteRecord(ids);
                return result;
            });
        }
    }
}
