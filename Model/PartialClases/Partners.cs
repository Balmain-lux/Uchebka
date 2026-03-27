using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test942.Model
{
    public partial class Partners
    {
        public string SizeDiscount
        {
            get
            {
                decimal? sum = Request.Sum(x => x.TotalAmountReq);
                if(sum> 10000 && sum<50000)
                {
                    return "5%";
                }
                else
                if(sum >=50000 && sum<300000)
                {
                    return "10%";
                }
                else
                if (sum >= 30000)
                {
                    return "15%";
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
