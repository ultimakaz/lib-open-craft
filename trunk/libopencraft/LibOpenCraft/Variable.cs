using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft
{
    public struct Variable
    {
        public vtype variable_type;
        public object _data;
    }
    public enum vtype
    {
        Byte,
        Short,
        Int,
        Long,
        Float,
        Double,
        String,
        Bool,
        Metadata,
    }
}
