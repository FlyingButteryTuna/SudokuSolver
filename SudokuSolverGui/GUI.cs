using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SudokuSolverGui.ExactCover;

namespace SudokuSolverGui
{
  public partial class GUI : Form
  {
    private DataGridView sudoku_grid = new DataGridView();

    private Button solve_button = new Button();
    private Button input_from_box_button = new Button();

    private GroupBox input_mode = new GroupBox();
    private RadioButton direct_input_mode = new RadioButton();
    private RadioButton seed_input_mode = new RadioButton();

    private TextBox seed_input = new TextBox();

    public GUI()
    {
      this.Size = new Size(900, 620);
      loadSudokuGrid();
      loadSolveButton();
      loadSeedInputBox();
    }

    private void loadRadioButtons()
    {
      direct_input_mode.Text = "Direct input";
      seed_input_mode.Text = "Input via seed";
    }

    private void loadSolveButton()
    {
      this.Controls.Add(solve_button);
      solve_button.Text = "Sovle!";
      solve_button.Location = new Point(610, 529);
      solve_button.Size = new Size(250, 50);


      this.Controls.Add(input_from_box_button);
      input_from_box_button.Text = "Enter";
      input_from_box_button.Location = new Point(615, 160);
      input_from_box_button.Size = new Size(240, 25);
    }

    private void loadSudokuGrid()
    {
      this.Controls.Add(sudoku_grid);
      sudoku_grid.Size = new Size(579, 579);
      sudoku_grid.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
      sudoku_grid.ColumnHeadersVisible = false;
      sudoku_grid.RowHeadersVisible = false;
      sudoku_grid.ColumnCount = 9;
      sudoku_grid.RowCount = 10;
      sudoku_grid.AllowUserToDeleteRows = false;
      sudoku_grid.AllowUserToOrderColumns = false;
      sudoku_grid.AllowUserToResizeColumns = false;
      sudoku_grid.AllowUserToResizeRows = false;
      sudoku_grid.AllowUserToAddRows = false;

      foreach (DataGridViewColumn c in sudoku_grid.Columns)
      {
        c.Width = 64;
        c.DefaultCellStyle.Font = new Font("Arial", 30F, GraphicsUnit.Pixel);
        if ((c.Index == 2) || (c.Index == 5))
          c.DividerWidth = 4;
      }
      foreach (DataGridViewRow r in sudoku_grid.Rows)
      {
        r.Height = 64;
        if ((r.Index == 2) || (r.Index == 5))
          r.DividerHeight = 4;
      }
    }

    private void loadSeedInputBox()
    {
      this.Controls.Add(seed_input);
      seed_input.Location = new Point(615, 50);
      seed_input.Multiline = true;
      seed_input.ClientSize = new Size(235, 100);
      seed_input.MaxLength = 81;
      seed_input.Font = new Font("Arial", 16F, GraphicsUnit.Pixel);
    }

    protected override void OnLoad(EventArgs e)
    {
      sudoku_grid.CellValueChanged += new DataGridViewCellEventHandler(sudokuGrid_CellChanged);
      sudoku_grid.CellEndEdit += new DataGridViewCellEventHandler(sudokuGrid_CellEditEnd);
      solve_button.Click += new EventHandler(solveButton_SolveSudoku);
      input_from_box_button.Click += new EventHandler(inputFromBoxButton_UpdateSudokuField);
      
      base.OnLoad(e);
    }

    private void sudokuGrid_CellChanged(object sender, DataGridViewCellEventArgs e)
    {
      sudoku_grid.CellValueChanged -= sudokuGrid_CellChanged;
      int new_value;
      int.TryParse(sudoku_grid[e.ColumnIndex, e.RowIndex].Value.ToString(), out new_value);
      if (new_value < 1 || new_value > 9)
      {
        MessageBox.Show("You have to enter digits 1...9 only");
        sudoku_grid[e.ColumnIndex, e.RowIndex].Value = "";
      }
      sudoku_grid.CellValueChanged += sudokuGrid_CellChanged;
    }

    private void sudokuGrid_CellEditEnd(object sender, DataGridViewCellEventArgs e)
    {
      sudoku_grid.Rows[e.RowIndex].ErrorText = String.Empty;
    }

    private void solveButton_SolveSudoku(object sender, EventArgs e)
    {
      char[] data = getSudokuData();
      SudokuSolver sudoku_solve = new SudokuSolver(ref data);
      Console.WriteLine(sudoku_solve.converted_result.Count);
      fillSudokuGrid(sudoku_solve.converted_result[0]);
    }

    private void inputFromBoxButton_UpdateSudokuField(object sender, EventArgs e)
    {
      char[] new_sudoku = seed_input.Text.ToCharArray();
      if (new_sudoku.Length != 81)
      {
        MessageBox.Show("You have to enter 81 char long sudoku");
        seed_input.Text = "";
        return;
      }

      for (int i = 0; i < new_sudoku.Length; i++)
      {
        if (!Char.IsDigit(new_sudoku[i]) && new_sudoku[i] != '.')
        {
          MessageBox.Show("You have to enter only digits or dots in seed field");
          seed_input.Text = "";
          return;
        }
      }
      fillSudokuGrid(new_sudoku);
    }

    private void fillSudokuGrid(int[,] data)
    {
      for (int i = 0; i < 9; i++)
      {
        for (int j = 0; j < 9; j++)
        {
          sudoku_grid[i, j].Value = data[i, j].ToString();
        }
      }
    }

    private void fillSudokuGrid(char[] data)
    {
      for (int i = 0; i < 9; i++)
      {
        for (int j = 0; j < 9; j++)
        {
          if (data[i * 9 + j] != '.')
          {
            sudoku_grid[j, i].Value = data[i * 9 + j].ToString();
          }
        }
      }
    }

    private char[] getSudokuData()
    {
      char[] sudoku_data = new char[81];
      for (int i = 0; i < 9; i++)
      {
        for (int j = 0; j < 9; j++)
        {
          if (sudoku_grid[i, j].Value == null)
            sudoku_data[i * 9 + j] = '.';
          else
            sudoku_data[i * 9 + j] = Convert.ToChar(sudoku_grid[i, j].Value.ToString());
        }
      }
      return sudoku_data;
    }
  }
}
