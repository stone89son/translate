using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translate
{
    public class TranCellValue
    {
        public TranCellValue(int sheetIndex, int rowIndex, int colIndex, string text)
        {
            SheetIndex = sheetIndex;
            RowIndex = rowIndex;
            ColIndex = colIndex;
            Text = text;
        }

        public int SheetIndex { get; set; }
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public string Text { get; set; }
        public string TranslatedText { get; set; }
    }
}
