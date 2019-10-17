﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * A Class to handle the Global Constants.
 */
namespace translation_validation_framework.util
{
    public class MainConsts
    {
        // 66 = all books
        public const int MAX_BOOK_NUM = 1;

        public const int MAX_CHECK_THREADS = 4;
        public static readonly string[] BOOK_NAMES = {
            "GEN","EXO","LEV","NUM","DEU","JOS","JDG","RUT",
            "1SA","2SA","1KI","2KI","1CH","2CH","EZR","NEH",
            "EST","JOB","PSA","PRO","ECC","SNG","ISA","JER",
            "LAM","EZE","DAN","HOS","JOL","AMO","OBA","JON",
            "MIC","NAM","HAB","ZEP","HAG","ZEC","MAL","MAT",
            "MRK","LUK","JHN","ACT","ROM","1CO","2CO","GAL",
            "EPH","PHP","COL","1TH","2TH","1TI","2TI","TIT",
            "PHM","HEB","JAS","1PE","2PE","1JN","2JN","3JN",
            "JUD","REV"
        };
    }
}
