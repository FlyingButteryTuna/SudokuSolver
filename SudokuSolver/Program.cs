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
      string sudoku = "61...7.....2....8.....964..5....9....68......92........4.7....9.8.2..6..1...4..28";
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
