using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translate
{
    public class TranExcelFile
    {
        private ChatBot _chatBot;
        private List<string> charIgnores = new List<string>() { "（","）","(",")" };
        public TranExcelFile()
        {
            _chatBot = new ChatBot();
        }
        public async Task Translate(string pathFile, 
            List<string> areaTranslate,
            RichTextBox rtbResult
            )
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(pathFile))
            {
                for (int i = 0, len = package.Workbook.Worksheets.Count; i < len; i++)
                {
                    var worksheet = package.Workbook.Worksheets[i];
                    int startRow = 1;
                    int startCol = 1;
                    if (worksheet.Dimension != null)
                    {
                        int endRow = worksheet.Dimension.Rows;
                        int endCol = worksheet.Dimension.Columns;

                        if (areaTranslate.Count > i)
                        {
                            string areaTranslateText = areaTranslate[i];

                            if (!string.IsNullOrWhiteSpace(areaTranslateText))
                            {
                                (startRow, startCol, endRow, endCol) = ParseRange(areaTranslate[i]);
                            }
                        }

                        for (int row = startRow; row <= endRow; row++)
                        {
                            for (int col = startCol; col <= endCol; col++)
                            {
                                if (worksheet.Cells[row, col].Value is null)
                                {
                                    continue;
                                }
                                string cellValue = worksheet.Cells[row, col].Value.ToString();
                                rtbResult.AppendText($"[{row}:{col}] {cellValue} {Environment.NewLine}");
                                Application.DoEvents();
                                worksheet.Cells[row, col].Value = await SendTranslateText(cellValue);
                            }
                        }
                    }

                    var drawings = worksheet.Drawings;
                    foreach (var drawing in drawings)
                    {
                        if (drawing is ExcelShape shape)
                        {
                            if (shape.RichText.Count > 0)
                            {
                                rtbResult.AppendText($"[RichText] {shape.Text} {Environment.NewLine}");
                                Application.DoEvents();
                                shape.Text = await SendTranslateText(shape.Text);
                            }
                        }
                    }
                }

                //カラムの幅を自動調整
                //  sheet.Cells.AutoFitColumns(1);

                //作ったファイルを保存
                FileInfo fileInfo = new FileInfo(pathFile);
                string saveFile = $"{Path.GetFileNameWithoutExtension(pathFile)}_Tranlated{fileInfo.Extension}";

                package.SaveAs(Path.Combine(fileInfo.DirectoryName, saveFile));
            }
        }

        private async Task<string> SendTranslateText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            if (charIgnores.Contains(text)) {
                return text;
            }
            bool isNumber = double.TryParse(text, out double resultNumber);
            if (isNumber) {
                return text;
            }

            string translatedText = await _chatBot.SendMessageAsync($"Translate the following from Japanese to Vietnamese: {text.Trim()}");
            Thread.Sleep(4300);
            return translatedText;
        }

        private (int, int, int, int) ParseRange(string range)
        {
            string pattern = @"([A-Z]+)(\d+):([A-Z]+)(\d+)";
            Match match = Regex.Match(range, pattern);

            if (!match.Success)
                throw new ArgumentException("Định dạng không hợp lệ");

            string startColStr = match.Groups[1].Value;
            int startRow = int.Parse(match.Groups[2].Value);
            string endColStr = match.Groups[3].Value;
            int endRow = int.Parse(match.Groups[4].Value);

            int startCol = ConvertColumnToNumber(startColStr);
            int endCol = ConvertColumnToNumber(endColStr);

            return (startRow, startCol, endRow, endCol);
        }

        private int ConvertColumnToNumber(string column)
        {
            int result = 0;
            foreach (char c in column)
            {
                result = result * 26 + (c - 'A' + 1);
            }
            return result;
        }
    }
}
