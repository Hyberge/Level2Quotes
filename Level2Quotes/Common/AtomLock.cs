using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace Level2Quotes
{
    public class AtomLock
    {
        int mLockCount = 0;
        public bool Lock()
        {
            while (Interlocked.CompareExchange(ref mLockCount, 1, 0) == mLockCount)
            {
                Thread.Sleep(0);
            }

            return true;
        }

        public void Unlock()
        {
            while (Interlocked.CompareExchange(ref mLockCount, 0, 1) == mLockCount)
            {
                Thread.Sleep(0);
            }
        }
    }
}
