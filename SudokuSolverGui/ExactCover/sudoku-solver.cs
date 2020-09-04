using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolverGui.ExactCover
{
  public partial class SudokuSolver
  {
    private List<ColumnNode> column_nodes = new List<ColumnNode>();
    private List<List<DancingNode>> solutions = new List<List<DancingNode>>();
    private BinaryMatrix matrix;
    private ColumnNode head = new ColumnNode(-4);
    public List<int[,]> converted_result { get; } = new List<int[,]>();

    public SudokuSolver(ref char[] sudoku)
    {
      matrix = new BinaryMatrix(ref sudoku);
      ColumnNode previous_column = null;
      for (int i = 0; i < matrix.width; i++)
      {
        ColumnNode new_column = new ColumnNode(i);
        if (i == 0)
        {
          head.right = new_column;
          new_column.left = head;
        }
        else
        {
          if (i == matrix.width - 1)
          {
            new_column.right = head;
            head.left = new_column;
          }
          new_column.left = previous_column;
          previous_column.right = new_column;
        }
        connectNodesInColumn(ref new_column);
        column_nodes.Add(new_column);
        previous_column = new_column;

      }
      DancingNode current_column = head;
      for (int i = 0; i < matrix.sudoku_size; i++)
      {
        current_column = current_column.right;
        connectNodesInRow(ref current_column);
      }
      List<DancingNode> solution_tmp = new List<DancingNode>();
      algorithmX(ref solution_tmp);
      convertResultToGrid();
    }

    private void connectNodesInColumn(ref ColumnNode column)
    {
      List<int> positions_in_column = matrix.getTrueValuesPositionsInColumn(column.column);
      int list_size = positions_in_column.Count;
      if (list_size == 1)
      {
        DancingNode new_node = new DancingNode(ref column, positions_in_column[0]);
        column.top = new_node;
        column.bottom = new_node;
        new_node.top = column;
        new_node.bottom = column;
        return;
      }
      DancingNode previous_node = null;
      for (int i = 0; i < list_size; i++)
      {
        DancingNode new_node = new DancingNode(ref column, positions_in_column[i]);
        if (i == 0)
        {
          new_node.top = column;
          column.bottom = new_node;
        }
        else if (i == list_size - 1)
        {
          column.top = new_node;
          previous_node.bottom = new_node;
          new_node.bottom = column;
          new_node.top = previous_node;
        }
        else
        {
          new_node.top = previous_node;
          previous_node.bottom = new_node;
        }
        previous_node = new_node;
        column.size++;
      }
    }

    private void connectNodesInRow(ref DancingNode column)
    {
      DancingNode tmp = column;
      do
      {
        tmp = tmp.bottom;
        List<int> positions_in_row = matrix.getTrueValuesPositionsInRow(tmp.row);
        int list_size = positions_in_row.Count;
        for (int i = 0; i < list_size - 1; i++)
        {
          DancingNode connectNode1 = column_nodes[positions_in_row[i]].bottom;
          DancingNode connectNode2 = column_nodes[positions_in_row[i + 1]].bottom;
          while (connectNode1.row != tmp.row)
          {
            connectNode1 = connectNode1.bottom;
          }
          while (connectNode2.row != tmp.row)
          {
            connectNode2 = connectNode2.bottom;
          }
          if (i == list_size - 2)
          {
            tmp.left = connectNode2;
            connectNode2.right = tmp;
          }
          connectNode1.right = connectNode2;
          connectNode2.left = connectNode1;
        }
      } while (tmp.bottom.row != -1);
    }

    private void algorithmX(ref List<DancingNode> solution)
    {
      if (head.right == head)
      {
        solutions.Add(new List<DancingNode>(solution));
        return;
      }
      else
      {
        ColumnNode column = getLowestSizeColumn().column_node;
        column.cover();

        DancingNode row;
        for (row = column.bottom; row != column; row = row.bottom)
        {
          solution.Add(row);

          for (DancingNode node = row.right; node != row; node = node.right)
          {
            node.column_node.cover();
          }

          algorithmX(ref solution) ;
          solution.Remove(row);
          column = row.column_node;
          for (DancingNode node = row.left; node != row; node = node.left)
          {
            node.column_node.uncover();
          }
        }
        column.uncover();
      }
    }

    private DancingNode getLowestSizeColumn()
    {
      DancingNode tmp = head;
      DancingNode result = head.right;
      do
      {
        tmp = tmp.right;
        if (result.column_node.size >= tmp.column_node.size)
        {
          result = tmp;
        }
      } while (tmp.right != head);
      return result;
    }

    private void convertResultToGrid()
    {
      foreach (List<DancingNode> solution in solutions)
      {
        int[,] result = new int[9, 9];
        foreach (DancingNode node in solution)
        {
          DancingNode first_node_in_row = node;
          int column = node.column_node.column;

          for (DancingNode i = node.right; i != node; i = i.right)
          {
            if (i.column_node.column < column)
            {
              first_node_in_row = i;
              column = i.column_node.column;
            }
          }
          int r = first_node_in_row.column_node.column / 9;
          int c = first_node_in_row.column_node.column % 9;
          int ans1 = first_node_in_row.right.column_node.column;
          int value = ((ans1 - 81) / 9) + 1;
          result[r, c] = value;
        }
        converted_result.Add(result);
      }
    }

    private void printResult(ref int[,] result)
    {
      for (int i = 0; i < 9; i++)
      {
        for (int j = 0; j < 9; j++)
        {
          if (j == 3 || j == 6)
            Console.Write(" ");
          Console.Write(result[i, j]);
        }
        Console.Write("\n");
        if (i == 2 || i == 5)
          Console.Write("\n");

      }
    }
  }
}
