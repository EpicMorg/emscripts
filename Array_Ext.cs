using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vk_parser
{
    public static class Array_Ext
    {
        public static int BinarySearch<T>(T[] array, T value, Comparison<T> comparison)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (array.Rank > 1)
                throw new RankException("Only single dimension arrays are supported.");
            if (array.Length == 0)
                return -1;
            if (comparison == null)
                throw new ArgumentNullException("comparison");
            if ((value != null) && !(value is IComparable))
                throw new ArgumentException("comparer is null and value does not support IComparable.");
            return DoBinarySearch<T>(array, array.GetLowerBound(0), array.GetLength(0), value, comparison);
        }
        static int DoBinarySearch<T>(T[] array, int index, int length, T value, Comparison<T> comparison)
        {
            int iMin = index,
                iMax = index + length - 1,
                iCmp = 0,
                iMid;
            try
            {
                if (comparison(array[iMax], value) < 0)
                    return ~(index + length);
                if (comparison(array[iMin], value) > 0)
                    return ~(index);
                while (iMin <= iMax)
                {
                    iMid = (iMin + iMax) >> 1;
                    if ((iCmp = comparison(array[iMid], value)) == 0) return iMid;
                    if (iCmp > 0) iMax = iMid - 1;
                    else iMin = iMid + 1;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Comparer threw an exception.", e);
            }
            return ~iMin;
        }
    }
    
}
