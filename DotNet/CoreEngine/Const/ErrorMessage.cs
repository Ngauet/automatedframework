using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Driver
{
    public static class ErrorMessage
    {
        public const string NULL_VALUE = "ERROR 1: INPUT DATA CONTAIN NULL VALUE.";
        public const string NO_RECORD = "ERROR 2: NO VALID RECORD WAS READ.";
        public const string NO_MAPPING_KEYWORD = "ERROR 3: CANNOT INDENTIFY KEYWORD MAPPING.";
        public const string NOT_EXIST_VARIABLE = "ERROR 4: CANNOT INDENTIFY VARIABLE IN THIS CONTEXT.";
        public const string INVALID_METHOD_ARGUMENT = "ERROR 5: THE INPUT ARGUMENTS ISNOT VALID FOR THIS METHOD.";
        public const string CONVERSION_TYPE = "ERROR 6: CANNOT CONVERT VALUE'S TYPE.";
        public const string USERCODE = "ERROR";
    }
}
