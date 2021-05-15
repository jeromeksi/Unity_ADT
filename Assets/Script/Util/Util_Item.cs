using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    [Serializable]
    public class ItemAmount
    {
        public ItemRef ItemRef;
        public int Amount;
        public ItemAmount(ItemRef _ItemRef, int _Amount = 0)
        {
            ItemRef = _ItemRef;
            Amount = _Amount;
        }
        public ItemAmount()
        {
            ItemRef = null;
            Amount = 0;
        }
    }
}
