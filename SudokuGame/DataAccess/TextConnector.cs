using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace SudokuGame.DataAccess
{
    static internal class TextConnector
    {
        public static string FullFilePath(string difficulty, bool answer)
        {
            if (answer)
            {
                return $"{ConfigurationManager.AppSettings["filePath"]}\\{difficulty}\\{difficulty}Sudoku1Answer.csv";
            }
            else
            {
                return $"{ConfigurationManager.AppSettings["filePath"]}\\{difficulty}\\{difficulty}Sudoku1.csv";
            }
        }

        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        public static void ConvertListToSudokuBoardArray(this List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                string[] cols = lines[i].Split(',');

                for (int j = 0; j < cols.Length; j++)
                {
                    if (cols[j] != "0")
                    {
                        SudokuBoard.sudokuGame.SudokuBoard[i * 9 + j].SudokuTextBox.Text = cols[j];
                        SudokuBoard.sudokuGame.SudokuBoard[i * 9 + j].SudokuTextBox.Foreground = Brushes.Black;
                        SudokuBoard.sudokuGame.SudokuBoard[i * 9 + j].SudokuTextBox.IsReadOnly = true;

                    }
                }
            }
        }

        public static void ConvertListToSudokuBoardAnswerArray(this List<string> lines)
        {

            for (int i = 0; i < lines.Count; i++)
            {
                string[] cols = lines[i].Split(',');

                for (int j = 0; j < cols.Length; j++)
                {
                    if (cols[j] != "0")
                    {
                        TextBox textBox = new TextBox();
                        textBox.Text = cols[j];
                        SudokuBoard.sudokuGame.SudokuBoardAnswer[i * 9 + j] = textBox;
                    }
                }
            }
        }
    }
}
