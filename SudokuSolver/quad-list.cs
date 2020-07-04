using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
  internal class DancingNode
  {
    public DancingNode left { get; set; }
    public DancingNode right { get; set; }
    public DancingNode top { get; set; }
    public DancingNode bottom { get; set; }
    public ColumnNode column_node { get; set; }
    public int row { get; set; }

    public DancingNode(int row = -1)
    {
      left = right = top = bottom = null;
      this.row = row;
    }

    public DancingNode(ref ColumnNode column, int row = -1) :
      this(row)
    {
      column_node = column;
    }

    public void removeLR()
    {
      left.right = right;
      right.left = left;
    }

    public void insertLR()
    {
      left.right = this;
      right.left = this;
    }

    public void removeTB()
    {
      top.bottom = bottom;
      bottom.top = top;
    }

    public void insertTB()
    {
      top.bottom = this;
      bottom.top = this;
    }
  }

  internal class ColumnNode : DancingNode
  {
    public DancingNode node { get; set; }
    public int column { get; set; }
    public int size { get; set; }

    public ColumnNode(int column = -1) :
      base()
    {
      this.column = column;
      column_node = this;
      size = 0;
    }

    public void cover()
    {
      removeLR();
      for (DancingNode i = bottom; i != this; i = i.bottom)
      {
        for (DancingNode j = i.right; j != i; j = j.right)
        {
          j.column_node.size--;
          j.removeTB();
        }
      }
    }

    public void uncover()
    {
      if (top == null)
      {
        insertLR();
        return;
      }
      for (DancingNode i = top; i != this; i = i.top)
      {
        for (DancingNode j = i.left; j != i; j = j.left)
        {
          j.column_node.size++;
          j.insertTB();
        }
      }
      insertLR();
    }
  }

  class QuadLinkedList
  {
    List<ColumnNode> column_nodes = new List<ColumnNode>();
    public List<DancingNode> solution { get; set; } = new List<DancingNode>();
    public List<DancingNode> solution1 { get; set; } = new List<DancingNode>();
    private BinaryMatrix matrix;
    ColumnNode head = new ColumnNode(-4);

    public QuadLinkedList(ref BinaryMatrix matrix)
    {
      this.matrix = matrix;
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

    public void printList()
    {
      DancingNode tmp = head;
      int result = 0;
      do
      {
        tmp = tmp.right;
        DancingNode tmp1 = tmp.bottom;
        do
        {
          result++;
          tmp1 = tmp1.bottom;
        } while (tmp1 != tmp);
      } while (tmp.right != head);
      Console.WriteLine(result);
    }

    public void algorithmX(int k)
    {
      if (head.right == head)
      {
        solution1 = new List<DancingNode>(solution);
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

          algorithmX(k + 1);
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

    public DancingNode getLowestSizeColumn()
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

    public void convertResultToGrid()
    {
      int[,] result = new int[9,9];

      foreach (DancingNode node in solution1)
      {
        //Console.WriteLine("row {0}, column {1}", node.row, node.column_node.column);
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
      printResult(ref result);
    }

    private void printResult(ref int[,] result)
    {
      for (int i = 0; i < 9; i++)
      {
        for (int j = 0; j < 9; j++)
        {
          if (j == 3 || j == 6 )
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
