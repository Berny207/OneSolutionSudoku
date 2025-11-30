using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace OneSolutionSudoku
{
    class SudokuSavingHandler
    {
        public static string defaultSaveLocation = AppContext.BaseDirectory + "SavedSudokus";
        private static string _saveLocation;
		public static string saveLocation
        {
            get { return _saveLocation; }
            set
            {
                if (value != null)
                {
                    _saveLocation = value;
                }
                else
                {
                    _saveLocation = defaultSaveLocation;
                }
            }
		}
		public static void SaveSudoku(Sudoku sudoku, string fileName)
        {
			FileStream sudokuFile = File.Create(fileName);
            StreamWriter writer = new StreamWriter(sudokuFile);
            writer.WriteLine(sudoku.ToString());
            writer.Close();
            sudokuFile.Close();
		}
        public static Sudoku LoadSudoku(string fileName)
        {
			string fullPath = Path.Combine(saveLocation, fileName);
            StreamReader reader = new StreamReader(fullPath);
			string SudokuString = reader.ReadToEnd();
			reader.Close();
            return StringToSudoku(SudokuString);
		}

        public static Sudoku StringToSudoku(string stringDoku)
        {
            Sudoku sudoku = new Sudoku();
            stringDoku.Split(' ');
            int i = 0;
            foreach (string part in stringDoku.Split(' '))
            {
                if(part == "\r\n\r\n")
                {
                    // Skip part
                }
                else
                {
                    Coordinates coordinates = new Coordinates(i/9, i%9);
                    sudoku.SetCell(coordinates, int.Parse(part));
                    i++;
                }
            }
            return sudoku;
        }
	}
}
