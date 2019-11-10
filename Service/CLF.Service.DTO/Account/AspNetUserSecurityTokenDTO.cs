using CLF.Service.DTO.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLF.Service.DTO.Account
{
  public  class AspNetUserSecurityTokenDTO: EntityDTO
    {
        public long Id { get; set; }
        public string ClientId { get; set; }
        public string UserName { get; set; }
        public string RefreshToken { get; set; }
        /// <summary>
        /// RefreshToken是否设置无效
        /// </summary>
        public bool IsRevoked { get; set; }
    }
}
