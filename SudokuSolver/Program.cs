using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;


namespace SudokuSolver
{
  class Program
  {
    static void Main(string[] args)
    {
      string sudoku = ".....51..7..1..4.8..1...652.4..6.8..6.7.5...9.2.9.7..5.....49....9.....6..2......";
      char[] char_sudoku = sudoku.ToCharArray();
      Stopwatch stopwatch = new Stopwatch();

      stopwatch.Start();
      BinaryMatrix test = new BinaryMatrix(ref char_sudoku);
      //test.printMatrix();
      QuadLinkedList yo = new QuadLinkedList(ref test);
      stopwatch.Stop();
      Console.WriteLine(stopwatch.ElapsedMilliseconds);
      //yo.printList();
      Console.ReadLine();


    }
  }
}
