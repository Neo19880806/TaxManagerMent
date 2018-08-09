using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxManagerMent.Models
{
    //用于管理应用内数据
    class dataManager
    {
        private static dataManager mInstance;
        public static dataManager DefaultInstance { get { return getInstance(); } }
        public static dataManager getInstance()
        {
            if (null == mInstance)
            {
                mInstance = new dataManager();
            }
            return mInstance;
        }

        public RequestInfo RequestInfo = new RequestInfo();
        public CalculationInfo CalculationInfo = new CalculationInfo();
    }

    //用于处理查询信息请求
    class RequestInfo
    {
        public const int TYPE_NONE = 0;
        public const int TYPE_TYPE = 1;
        public const int TYPE_DATE = 2;
        public const int TYPE_BOTH = 3;
        public int Type { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    class CalculationInfo
    {
        public const int TYPE_SELLPRICE     = 0;//统计售价
        public const int TYPE_BOUGHTPRICE   = 1;//统计购买价
        public const int TYPE_NETPRICE      = 2;//统计净价
        public const int TYPE_COUNT         = 3;//统计数量

        public const int STATE_UNSENT       = (1<<0);//未发货状态
        public const int STATE_SENT         = (1<<1);//已发货状态
        public const int STATE_FINISHED     = (2<<1);//已完成状态
        public int type { get; set; }
        public int state { get; set; }

        public void AddState(int newState)
        {
            state |= newState;
        }

        public bool CheckState(int newState)
        {
            return ((state & newState)!=0);
        }

        public void Reset()
        {
            state   = 0;
            type    = 0;
        }
    }
}
