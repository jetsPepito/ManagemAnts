using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsClient.Models
{
    public class Login
    {
        public enum statusEnum
        {
            NOSTATUS = 0,
            REGISTERED,
            PSEUDO,
            PASSWORD,
            PSEUDO_PASSWORD,
            SOMETHING_WENT_WRONG,
        }

        public statusEnum status;
    }
}
