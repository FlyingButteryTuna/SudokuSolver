using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime;
using System.Runtime.Remoting.Messaging;
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
    public int row { get; set; }

    public DancingNode(int row = -1)
    {
      left = right = top = bottom = null;
      this.row = row;
    }

  }

  internal class ColumnNode : DancingNode
  {
    public DancingNode node { get; set; }
    public int column { get; set; }

    public ColumnNode(int column = -1) :
      base()
    {
      this.column = column;
    }
  }

  class QuadLinkedList
  {
    List<ColumnNode> column_nodes = new List<ColumnNode>();
    private BinaryMatrix matrix;
    ColumnNode head = new ColumnNode();
    ColumnNode tail = new ColumnNode();


    public QuadLinkedList(ref BinaryMatrix matrix)
    {
      this.matrix = matrix;
      head.left = tail;
      tail.right = head;
      ColumnNode previous_column = null;
      for (int i = 0; i < matrix.width; i++)
      {
        ColumnNode new_column = new ColumnNode(i);
        if (i == 0)
        {
          head.right = new_column;
          new_column.left = head;
        }
        else if (i == matrix.width - 1)
        {
          tail.left = new_column;
          new_column.right = tail;
          new_column.left = previous_column;
          previous_column.right = new_column;
        }
        else
        {
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
        DancingNode new_node = new DancingNode(positions_in_column[0]);
        column.top = new_node;
        column.bottom = new_node;
        new_node.top = column;
        new_node.bottom = column;
        return;
      }
      DancingNode previous_node = null;
      for (int i = 0; i < list_size; i++)
      {
        DancingNode new_node = new DancingNode(positions_in_column[i]);
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
      } while (tmp.right != tail);
      Console.WriteLine(result);
    }
  }


}
