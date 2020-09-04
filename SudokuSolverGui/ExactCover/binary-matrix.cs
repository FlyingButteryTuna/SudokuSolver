using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SudokuSolverGui.ExactCover
{
  public partial class SudokuSolver
  {
    internal class BinaryMatrix
    {
      public int sudoku_size { get; } = 81;
      public int line_size { get; } = 9;
      public int width { get; } = 324;
      public int height { get; }
      private BitArray matrix;

      public BinaryMatrix(ref char[] sudoku)
      {
        int used_cells = 0;
        foreach (char cell in sudoku)
        {
          if (cell != '.')
            used_cells++;
        }
        height = 729 - 8 * used_cells;
        matrix = new BitArray(height * width);
        matrix.SetAll(false);

        int current_height = 0;
        for (int i = 0; i < sudoku_size; i++)
        {
          int current_line = i / line_size;
          int current_column = i % line_size;
          int current_box = (current_line / 3) * 3 + current_column / 3;

          if (sudoku[i] == '.')
          {
            for (int j = 0; j < line_size; j++)
            {
              fillRow(current_height, current_line, current_column, current_box, i, j);
              current_height++;
            }
          }
          else if (char.IsDigit(sudoku[i]))
          {
            fillRow(current_height, current_line, current_column, current_box, i, sudoku[i] - 49);
            current_height++;
          }

        }
      }

      private void fillRow(int height, int line, int column, int box, int pos, int value)
      {
        matrix[height * width + pos] = true;
        int column_to_place_in = sudoku_size + line_size * value + line;
        matrix[height * width + column_to_place_in] = true;
        column_to_place_in = sudoku_size * 2 + line_size * value + column;
        matrix[height * width + column_to_place_in] = true;
        column_to_place_in = sudoku_size * 3 + 9 * value + box;
        matrix[height * width + column_to_place_in] = true;
      }

      public void printMatrix()
      {
        for (int i = 0; i < height; i++)
        {
          for (int j = 0; j < width; j++)
          {
            if (matrix[i * width + j])
              Console.Write(1);
            else
            {
              Console.Write(0);
            }
          }
          Console.Write("\nline ");

        }
      }

      public List<int> getTrueValuesPositionsInColumn(int column)
      {
        List<int> result = new List<int>();
        for (int i = 0; i < height; i++)
        {
          if (matrix[i * width + column])
          {
            result.Add(i);
          }
        }
        return result;
      }

      public List<int> getTrueValuesPositionsInRow(int row)
      {
        if (row >= height)
        {
          Console.ReadLine();
        }

        List<int> result = new List<int>();
        for (int i = 0; i < width; i++)
        {
          if (matrix[row * width + i])
          {
            result.Add(i);
          }
        }
        return result;
      }

      public bool this[int width, int height]
      {
        get => matrix[height * this.width + width];
      }
    }
  }
}
