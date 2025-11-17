using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
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
		public static void SaveSudoku(string sudokuString, string fileName)
        {
            string fullPath = Path.Combine(saveLocation, fileName);
			FileStream sudokuFile = File.Create(fullPath);
            StreamWriter writer = new StreamWriter(sudokuFile);
            writer.WriteLine(sudokuString);
            writer.Close();
            sudokuFile.Close();
		}
	}
}
