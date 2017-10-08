﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.HabboHotel.Catalog.Vouchers
{
    public static class VoucherUtility
    {
        public static VoucherType GetType(string Type)
        {
            switch (Type)
            {
                default:
                case "credit":
                    return VoucherType.CREDIT;
                case "ducket":
                    return VoucherType.DUCKET;
            }
        }

        public static string FromType(VoucherType Type)
        {
            switch (Type)
            {
                default:
                case VoucherType.CREDIT:
                    return "credit";
                case VoucherType.DUCKET:
                    return "ducket";
            }
        }
    }
}
