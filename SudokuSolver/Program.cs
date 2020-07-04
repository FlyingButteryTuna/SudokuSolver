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
      string sudoku = "..65....2847.......9..4....5.2.....3..8..............5.5...83....9.748..6....97..";
      char[] char_sudoku = sudoku.ToCharArray();
      Stopwatch stopwatch = new Stopwatch();

      stopwatch.Start();

      BinaryMatrix test = new BinaryMatrix(ref char_sudoku);
      //test.printMatrix();

      QuadLinkedList yo = new QuadLinkedList(ref test);

      //yo.printList();
      yo.algorithmX(0);

      yo.convertResultToGrid();

      stopwatch.Stop();
      Console.WriteLine("solved in {0} milliseconds", stopwatch.ElapsedMilliseconds);

      Console.ReadLine();



    }
  }
}
