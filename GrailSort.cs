﻿namespace AlgoUWP
{
    /*
 * 
The MIT License (MIT)
Copyright (c) 2013 Andrey Astrelin
Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *
 */

    /********* Grail sorting *********************************/
    /*                                                       */
    /* (c) 2013 by Andrey Astrelin                           */
    /* Refactored by MusicTheorist                           */
    /*                                                       */
    /* Stable sorting that works in O(N*log(N)) worst time   */
    /* and uses O(1) extra memory                            */
    /*                                                       */
    /* Define int / SortComparator                           */
    /* and then call GrailSort() function                    */
    /*                                                       */
    /* For sorting w/ fixed external buffer (512 items)      */
    /* use GrailSortWithBuffer()                             */
    /*                                                       */
    /* For sorting w/ dynamic external buffer (sqrt(length)) */
    /* use GrailSortWithDynBuffer()                          */
    /*                                                       */
    /*********************************************************/

    public class GrailPair
    {
        public int leftOverLen;
        public int leftOverFrag;

        public GrailPair(int len, int frag)
        {
            leftOverLen = len;
            leftOverFrag = frag;
        }

        public int GetLeftOverLen()
        {
            return leftOverLen;
        }

        public int GetLeftOverFrag()
        {
            return leftOverFrag;
        }
    }
    internal class GrailSort
    {
    }
}
