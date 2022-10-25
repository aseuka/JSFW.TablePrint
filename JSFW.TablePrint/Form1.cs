using JSFW.Utils.TablePrint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JSFW.TablePrint
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent(); 

            Table tb = new Table("T1"); 
            tb.AddRowData("A", "A", "A", "F", "H");
            tb.AddRowData("B", "B", "C", "F", "G");
            tb.AddRowData("D", "E", "C", "F", "G");
            tb.AddRowData("Z", "Y", "X", "W", "P"); 

            tb.HeaderCount = 1; 

            tb.AddRowData("데이타1", "데이타2", "데이타3", "데이타4", "데이타5");

            Table tb2 = new Table("T2"); 
            tb2.AddRowData("데이타(가)", "데이타(나)", "데이타(다)");
            tb2.HeaderCount = 2;

            tb2.AddRowData("데이타(가)", "2", "1");
            tb2.AddRowData("1", "2", "3");
            tb2.AddRowData("1", "2");
            tb2.AddRowData("1", "2");


            Table tb3 = new Table("T3"); 
            tb3.AddRowData("데이타a", "데이타b", "데이타c");
            tb3.HeaderCount = 1;
            tb3.AddRowData("1", "abcdef");
            tb3.AddRowData("2", "abcdef");
            tb3.AddRowData("3", "abcdef");
            tb3.AddRowData("4", "abcdef");
            tb3.AddRowData("5", "abcdef");

            tb.AddRowData(tb3, "2", $"{tb2:Text}");

            textBox1.Text = string.Format("{0:Text}", tb);


            textBox1.Text += string.Format("{0:Text}", tb2);
        } 
    }
}

namespace JSFW.Utils.TablePrint
{
    public class Table : IDisposable, IFormattable
    {
        public Table()
        {
            Rows = new List<Row>();
            Cols = new List<Col>();
        }

        public Table(string tableName) : this()
        {
            TableName = tableName;
        }

        public string TableName { get; set; }

        public List<Row> Rows { get; private set; }

        public List<Col> Cols { get; private set; }

        public int HeaderCount { get; internal set; }

        private void AddCol(string colName)
        {
            Col c = new Col() { Name = colName, Index = Cols.Count };
            Cols.Add(c);
        }

        public void AddRowData(params object[] datas)
        {
            if (datas == null) return;
            if (datas.Length <= 0) return;

            for (int c = 0; c < datas.Length; c++)
            {
                if (Cols.Count <= c)
                {
                    AddCol("col_" + Cols.Count);
                }
            }

            Row row = new Row();
            for (int c = 0; c < datas.Length && c < Cols.Count; c++)
            {
                row.AddCell(Cols[c], datas[c]);
            }
            Rows.Add(row);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면
       
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                    if (Rows != null)
                    {
                        for (int ri = Rows.Count - 1; ri >= 0; ri--)
                        {
                            using (Rows[ri]) { }
                        }
                        Rows.Clear();
                    }

                    if (Cols != null)
                    {
                        Cols.Clear();
                    }
                }
                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~Table() {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == "Text")
            {
                string result = "";

                for (int r = 0; r < Rows.Count; r++)
                {
                    Rows[r].InnerRowsCount = 0;
                    for (int c = 0; c < Cols.Count; c++)
                    {
                        if (c < Rows[r].Cells.Count) continue;
                        Rows[r].Cells.Add(new Cell(Rows[r], Cols[c]) { Data = "" });
                    }
                }
                for (int c = 0; c < Cols.Count; c++) { Cols[c].DataLength = 0; }

                // 계산 
                for (int r = 0; r < Rows.Count; r++)
                {
                    for (int c = 0; c < Cols.Count; c++)
                    {
                        string data = "" + Rows[r].Cells[c].Data;

                        if (Rows[r].Cells[c].Data is Table)
                        {
                            data = string.Format("{0:Text}", Rows[r].Cells[c].Data);
                        }

                        string[] split = data.Replace("\r", "").Split('\n');
                        List<string> lines = new List<string>(split);
                        lines.ForEach(txt =>
                        {
                            int datalength = Encoding.UTF8.GetByteCount(txt.Trim());
                            if (Cols[c].DataLength < datalength)
                            {
                                Cols[c].DataLength = datalength;
                            }
                        });

                        if (split != null)
                        {
                            Rows[r].Cells[c].DisplayData = split;
                            if (Rows[r].InnerRowsCount < split.Length)
                            {
                                Rows[r].InnerRowsCount = split.Length;
                            }
                        }
                        lines.Clear();
                        lines = null;
                    }
                }

                int width = Cols.Sum(c => c.DataLength) + Cols.Count + Cols.Count - 1;

                List<CellRange> Headers = Merge();

                if (0 < Headers.Count)
                {
                    // 헤더 그리기.
                    string[] headerText = new string[2 * HeaderCount - 1];
                    for (int loop = 0; loop < headerText.Length; loop++)
                    {
                        headerText[loop] += " ";
                    }

                    foreach (var h in Headers)
                    {
                        for (int c = h.c1; c <= h.c2; c++)
                        {
                            if (c == h.c1)
                            {    
                                //if (h.Value == typeof(Table).FullName)
                                //{
                                //    h.Value = string.Format("{0:Text}", Rows[ h.r1 ].Cells[ h.c1 ].Data );
                                //}
                                int len_ascii = Encoding.Default.GetByteCount(h.Value);
                                int len_utf = Encoding.UTF8.GetByteCount(h.Value);
                                int gap = len_ascii - len_utf;
                                headerText[h.r1 * 2] += " " + string.Format("{0, -"+ (Cols[c].DataLength + gap) + "}", h.Value);
                            }
                            else
                            {
                                headerText[h.r1 * 2] += "  " + new string(' ', Cols[c].DataLength ); //병합공백
                            }
                        }
                        headerText[h.r1 * 2] += string.Format("{0}", "|"); // 세로줄

                        for (int c = h.c1; c <= h.c2; c++)
                        {
                            for (int r = h.r1; r <= h.r2; r++)
                            {
                                if (r != h.r1)
                                { 
                                    headerText[(2 * r)] += " " + new string(' ', Cols[c].DataLength); //병합공백
                                    headerText[(2 * r)] += string.Format("{0}", "|"); // 세로줄
                                } 
                                if ((2 * r + 1) < headerText.Length)
                                {
                                    if (h.r1 == h.r2 || r == h.r2)
                                    {
                                        headerText[(2 * r + 1)] += "-" + new string('-', Cols[c].DataLength); // 밑줄
                                    }
                                    else
                                    {
                                        headerText[(2 * r + 1)] += " " + new string(' ', Cols[c].DataLength); //병합공백
                                    }

                                    if (h.c1 == h.c2 || c == h.c2)
                                    {
                                        headerText[(2 * r + 1)] += string.Format("{0}", "|"); // 세로줄
                                    }
                                    else
                                    {
                                        headerText[(2 * r + 1)] += string.Format("{0}", "-"); // 세로줄
                                    }
                                }
                            }
                        }
                    } 
                    for (int loop = 0; loop < headerText.Length; loop++)
                    {
                        headerText[loop] = headerText[loop].TrimEnd('|');
                    }
                    result += " " + new string('=', width) + Environment.NewLine;
                    result += string.Join(Environment.NewLine, headerText) + Environment.NewLine;
                    result += " " + new string('=', width) + Environment.NewLine;
                }

                List<string> innerDatas = new List<string>();
                // 그리기~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                for (int r = HeaderCount; r < Rows.Count; r++)
                { 
                    for (int ri = 0; ri < Rows[r].InnerRowsCount; ri++)
                    {
                        innerDatas.Clear();
                        for (int c = 0; c < Cols.Count; c++)
                        {
                            string data = "";
                            if (ri < Rows[r].Cells[c].DisplayData.Length)
                            {
                                data = Rows[r].Cells[c].DisplayData[ri];
                            }
                            else
                            {
                                data = new string(' ', Cols[c].DataLength);
                            }
                            int len_ascii = Encoding.Default.GetByteCount(data);
                            int len_utf = Encoding.UTF8.GetByteCount(data);
                            int gap = len_ascii - len_utf;
                            innerDatas.Add(string.Format(" {0,-" + (Cols[c].DataLength + gap) + "}", data.TrimEnd()));
                        }
                        result += " " + string.Join("|", innerDatas.ToArray()) + Environment.NewLine;
                    }
                    result += " " + new string('-', width)+ Environment.NewLine;
                }
                return result;
            }
            else
            {
                return TableName;
            }
        }
      
        internal List<CellRange> Merge()
        {
            int rowCount = HeaderCount;
            int colCount = Cols.Count;

            // Col Max 초기값 셋팅!
            int[] rowColMax = new int[rowCount];
            for (int loop = 0; loop < rowCount; loop++)
            {
                rowColMax[loop] = colCount;
            }

            Dictionary<int, List<int>> Cells = new Dictionary<int, List<int>>();

            List<CellRange> range = new List<CellRange>();

            int stRow = 0;
            int stCol = 0;

            int cnt = 0;
            while (stRow < rowCount && stCol < colCount)
            {
                //todo : cell 값이 같은 목록을 구함 
                // 각 Row별 cell목록이 같은 목록을 구한다.  

                string cellValue = ""+Rows[stRow].Cells[ stCol ].Data;// GRID[stRow][stCol];
                for (int row = stRow; row < rowCount; row++)
                {
                    if ( Rows[row].Cells[stCol].Data == null || cellValue != ("" + Rows[row].Cells[stCol].Data))//GRID[row][stCol])
                    {
                        rowColMax[row] = 0;
                        continue;
                    }

                    cellValue = "" + Rows[row].Cells[stCol].Data;//GRID[row][stCol];
                    Cells.Add(row, new List<int>());

                    for (int col = stCol; col < rowColMax[row]; col++)
                    {
                        if (cellValue == "" + Rows[row].Cells[col].Data)//GRID[row][col])
                        {
                            Cells[row].Add(col);
                        }
                        else
                        {
                            if (col < colCount)
                            {
                                //각 row별 colmax 값 셋팅.
                                rowColMax[row] = col;
                            }
                            break;
                        }
                    }
                }

                // todo : Cells 에는 머지 가능한 Cell목록이 나오므로.
                var cellrange = new CellRange() { Value = cellValue, r1 = stRow, c1 = stCol, r2 = stRow, c2 = stCol };
                range.Add(cellrange);
                for (int row = 0; row < Cells.Count; row++)
                {
                    var RowCells = Cells.ElementAt(row);
                    int _colCnt = RowCells.Value.Count;
                    int _colcnt_NextRow = _colCnt;
                    if (row + 1 < Cells.Count)
                    {
                        _colcnt_NextRow = Cells.ElementAt(row + 1).Value.Count;
                    }

                    if (_colCnt == 0) continue;

                    if (_colCnt == _colcnt_NextRow)
                    {
                        cellrange.r2 = RowCells.Key;
                        cellrange.c2 = RowCells.Value[_colCnt - 1];
                    }
                    else
                    {
                        if (RowCells.Key == 0)
                        {
                            cellrange.r2 = RowCells.Key;
                            cellrange.c2 = RowCells.Value[_colCnt - 1];
                        }
                        break;
                    }
                }

                Cells.Clear();

                stRow = cellrange.r1;
                stCol = cellrange.c1;

                int idx = cellrange.r2 * colCount + cellrange.c2;
                if (idx >= (colCount * rowCount - 1)) break;

                if (cellrange.r2 + 1 < rowCount)
                {
                    if (rowColMax[cellrange.r2] > rowColMax[cellrange.r2 + 1])
                    {
                        for (int loop = cellrange.r2; loop < rowCount; loop++) rowColMax[loop] = rowColMax[cellrange.r2];
                        stRow = cellrange.r2 + 1;
                    }
                    else
                    {
                        if (stRow == 0)
                        {
                            stCol = cellrange.c2 + 1;
                            for (int loop = stRow; loop < rowCount; loop++) rowColMax[loop] = colCount;
                        }
                        else
                        {
                            stRow--;
                            stCol = cellrange.c2 + 1;
                            for (int loop = stRow; loop < rowCount; loop++) rowColMax[loop] = colCount;
                        }
                    }
                }
                else
                {
                    stRow = cellrange.r2;
                    if (stRow == 0)
                    {
                        rowColMax[stRow] = colCount;
                    }
                    else
                    {
                        do
                        {
                            if (rowColMax[stRow - 1] > rowColMax[stRow])
                            {
                                for (int loop = stRow; loop < rowCount; loop++) rowColMax[loop] = rowColMax[stRow - 1];
                                break;
                            }
                            stRow--;

                            if (stRow == 0)
                            {
                                for (int loop = stRow; loop < rowCount; loop++) rowColMax[loop] = colCount;
                                break;
                            }
                        } while (true);
                    }

                    stCol = cellrange.c2 + 1;

                    // row증가를 시킬수 없을때 col Index 증가 가능 여부를 체크
                    if (stCol >= rowColMax[stRow])
                    {
                        // col 값이 꽉 찼을때 row -- 를 시켜 다음 진행 순서를 찾음.  
                        for (int loop = stRow - 1; loop >= 0; loop--)
                        {
                            if (stCol <= rowColMax[loop])
                            {
                                stRow = loop;
                                break;
                            }
                        }

                        if (stRow == 0)
                        {
                            for (int loop = stRow; loop < rowCount; loop++) rowColMax[loop] = colCount;
                        }
                        else
                        {
                            // row를 찾고
                            if (rowColMax[stRow - 1] > rowColMax[stRow])
                            {
                                for (int loop = stRow; loop < rowCount; loop++) rowColMax[loop] = rowColMax[stRow - 1];
                            }
                        }
                    }
                }
                cnt++;
            }
            // cellranges 목록 병합 소스 ! 
            return range;
        }

        internal class CellRange
        {
            public string Value { get; set; }
            public int r1;
            public int r2;
            public int c1;
            public int c2;

            public override string ToString()
            {
                return string.Format("Value={0}, R1=[{1}:{2}], R2=[{3}:{4}]", Value, r1, c1, r2, c2);
            }
        }
    }

     public class Row : IDisposable
    {
        public int Index { get; set; }

        internal int InnerRowsCount { get; set; }

        internal List<Cell> Cells { get; set; }

        internal void AddCell(Col col, object data)
        {
            Cell cell = new Cell(this, col); 
            cell.Data = data; 
            Cells.Add(cell);
        }

        public Row()
        {
            Cells = new List<Cell>();
        }

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                    for (int loop = 0; Cells != null &&  loop < Cells.Count; loop++)
                    {
                        using (Cells[loop]) { }
                    }
                    if (Cells != null)
                    {
                        Cells.Clear();
                    }
                } 
                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다. 
                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~Row() {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
      
    public class Col
    {
        public int Index { get; set; }

        public string Name { get; set; }

        internal int DataLength { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Cell : IDisposable
    {
        internal Col _Col { get; set; }
        internal Row _Row { get; set; }

        public object Data { get; internal set; }
        public string[] DisplayData { get; internal set; }

        public Cell(Row row, Col col)
        {
            _Col = col;
            _Row = row;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                    _Col = null;
                    _Row = null;
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~Cell() {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion 

    }
}

