using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CLF.Common.Configuration
{
    public partial class AppConfig
    {
        public string SqlServerConnectionString { get; set; }

        //mysql
        public bool MySqlEnabled { get; set; }
        public string MySqlConnectionString { get; set; }

        //redis
        public string RedisConnectionString { get; set; }
        public int? RedisDatabaseId { get; set; }
        public bool RedisEnabled { get; set; }
        public bool UseRedisForCaching { get; set; }

        public int DefaultCookieExpiredHours { get; set; }

        //log
        public string LogFilePath { get; set; }
        public string LogEventLevel { get; set; }
        public string RollingInterval { get; set; }

        public bool JwtAuthenticationEnabled { get; set; }
        public bool OAuthAuthenticationEnabled { get; set; }

    }

    public partial class JwtConfig
    {
        //jwt
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string SecurityKey { get; set; }
        public string Issuer { get; set; }
        /// <summary>
        /// 过期时间分钟
        /// </summary>
        public int ExpiredMinutes =>120;
    }
}
