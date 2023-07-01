using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_WebDesignCore.Enums
{
    public enum AuthStatusEnum
    {
        FAILED = 1,
        SUCCESS = 2,
        UNACTIVATE = 3,
        INVALID_EMAIL = 4,
        INVALID_NAME = 5,
        EMAIL_EXISTED = 6,
        INVALID_TOKEN = 7,
        TOKEN_EXPIRED = 8,
        TOKEN_NOT_EXPIRED = 9,
        TOKEN_REVOKED = 10,
        REFRESH_TOKEN_EXPIRED = 11
    }
}
