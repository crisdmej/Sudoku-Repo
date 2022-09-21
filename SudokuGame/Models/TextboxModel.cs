using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SudokuGame.Models
{
    public class TextboxModel
    {
        public TextBox SudokuTextBox { get; set; }

        public TextBlock[] SudokuNotes { get; set; }

        public TextboxModel(TextBox textBox, TextBlock[] sudokuNotes)
        {
            SudokuTextBox = textBox;
            SudokuNotes = sudokuNotes;
        }
    }
}
