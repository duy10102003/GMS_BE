using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWP.Core.GMSAttribute
{
    /// <summary>
    /// Setup mapping tên bảng trong database
    /// Created by: DuyLC(01/12/2025)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class GMSTableNameAttribute : Attribute
    {
        public string TableName { get; set; }
        public GMSTableNameAttribute(string tableName)
        {
               TableName = tableName;  
        }
    }
}
