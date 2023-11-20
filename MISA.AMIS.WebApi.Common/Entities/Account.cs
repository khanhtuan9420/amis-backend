using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.Common
{
    public class Account : BaseEntity, IBaseEntity
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public string? AccountNumber { get; set; }

        [Required]
        public string? AccountName { get; set; }

        public string? AccountEnglishName { get; set; }
        public Guid? GeneralAccountId { get; set; }
        public Nature? Nature { get; set; }
        public string? Explain { get; set; }
        public bool? ForeignCurrency { get; set; }
        public ObjectKind? FollowObject { get; set; }
        public bool? FollowBankAccount { get; set; }
        public Follow? FollowCollectionCosts { get; set; }
        public Follow? FollowConstruction { get; set; }
        public Follow? FollowOrder { get; set; }
        public Follow? FollowContract { get; set; }
        public Follow? FollowSellContract { get; set; }

        public Follow? FollowCostItem { get; set; }
        public Follow? FollowUnit { get; set; }
        public Follow? FollowStatisticalCode { get; set; }
        public bool? IsParent { get; set; }
        public int? Grade { get; set; }
        public bool? Active { get; set; }

        public Guid GetId()
        {
            return AccountId;
        }

        public void SetId(Guid id)
        {
            AccountId = id;
        }
    }
}
