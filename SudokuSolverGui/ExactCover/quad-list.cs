using System;
using System.Collections.Generic;

namespace SudokuSolverGui.ExactCover
{
  public partial class SudokuSolver
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
  }
}
