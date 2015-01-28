using System;
using System.Data;
using System.IO;
using PDNUtils.Runner.Attributes;

namespace PDNUtils.Help
{
    /// <summary>
    /// csv helper...
    /// </summary>
    [RunableClass]
    public class CSVHelper
    {

        /// <summary>
        /// Writes csv to <paramref name="stream"/>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="table"></param>
        /// <param name="separator"></param>
        /// <param name="header"></param>
        /// <param name="quoteall"></param>
        public static void WriteCSV(TextWriter stream, DataTable table, string separator, bool header, bool quoteall)
        {
            string sep = !string.IsNullOrEmpty(separator) ? separator : ",";
            if (header)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    WriteItem(stream, table.Columns[i].Caption, quoteall);
                    if (i < table.Columns.Count - 1)
                        stream.Write(sep);
                    else
                        stream.Write('\n');
                }
            }
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    WriteItem(stream, row[i], quoteall);
                    if (i < table.Columns.Count - 1)
                        stream.Write(sep);
                    else
                        stream.Write('\n');
                }
            }
        }

        private static void WriteItem(TextWriter stream, object item, bool quoteall)
        {
            if (item == null)
                return;
            string s = item.ToString();
            if (quoteall || s.IndexOfAny("\",\x0A\x0D".ToCharArray()) > -1)
                stream.Write("\"" + s.Replace("\"", "\"\"") + "\"");
            else
                stream.Write(s);
        }

        protected static DataTable GetDataTable()
        {
            DataTable dt = new DataTable();

            // Declare DataColumn and DataRow variables.
            DataColumn dc;
            DataRow dr;

            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            dc = new DataColumn();
            dc.DataType = System.Type.GetType("System.Int32");
            dc.ColumnName = "id";
            dc.Caption = "id";
            dt.Columns.Add(dc);

            // Create second column.
            dc = new DataColumn();
            dc.DataType = Type.GetType("System.String");
            dc.ColumnName = "item";
            dc.Caption = "item";
            dt.Columns.Add(dc);

            // Create new DataRow objects and add to DataTable.    
            for (int i = 0; i < 10; i++)
            {
                dr = dt.NewRow();
                dr["id"] = i;
                dr["item"] = "item " + i;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        [Run(0)]
        protected static void Test()
        {
            StringWriter sw = new StringWriter();
            WriteCSV(sw, GetDataTable(), ";", true, false);
            Console.WriteLine(sw.ToString());
        }
    }

}
