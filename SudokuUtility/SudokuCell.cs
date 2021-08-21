
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SudokuUtility
{

	/// <summary>
	/// Representation of a cell in a Sudoku board
	/// </summary>
	public class SudokuCell
	{

		#region Static members...

		/// <summary>
		/// Character to be used when representing the content of this cell as a string.
		/// </summary>
		public static char UnknowCellValueChar = '?';

		#endregion

		#region Private fields...

		/// <summary>
		/// Row for this cell into a Sudoku board (zero-based).
		/// </summary>
		private int row;

		/// <summary>
		/// Column for this cell into a Sudoku board (zero-based).
		/// </summary>
		private int col;

		/// <summary>
		/// Value for this cell into a Sudoku board.
		/// </summary>
		private int value;

		/// <summary>
		/// Bit mask with the possible values for this cell.
		/// </summary>
		private BitArray possibilities;

		/// <summary>
		/// Number of possible values for this cell.
		/// </summary>
		private int numPossibilities;

		#endregion

		#region Public properties...

		/// <summary>
		/// Row for this cell into a Sudoku board (zero-based).
		/// </summary>
		public int Row
		{
			get
			{
				return this.row;
			}
		}

		/// <summary>
		/// Column for this cell into a Sudoku board (zero-based).
		/// </summary>
		public int Col
		{
			get
			{
				return this.col;
			}
		}

		/// <summary>
		/// Value for this cell into a Sudoku board (-1 if value is unknown)
		/// </summary>
		public int Value
		{
			get
			{
				if (this.IsValid)
				{
					return this.value;
				}
				return -1;
			}
		}

		/// <summary>
		/// Has this cell a valid value?
		/// </summary>
		public bool IsValid
		{
			get
			{
				return (this.value >= 1 && this.value <= 9);
			}
		}

		/// <summary>
		/// Number of possible values for this cell.
		/// </summary>
		public int NumPossibilities
		{
			get
			{
				if (this.IsValid)
				{
					return 0;
				}

				return this.numPossibilities;
			}
		}

		#endregion

		#region Constructors...

		/// <summary>
		/// Constructor for this class.
		/// </summary>
		/// <param name="row">
		/// Row for this cell into a Sudoku board (zero-based).
		/// </param>
		/// <param name="col">
		/// Column for this cell into a Sudoku board (zero-based).
		/// </param>
		internal SudokuCell(int row, int col)
		{
			this.row = row;
			this.col = col;
			this.value = 0;
			this.possibilities = new BitArray(new int[] { 0x01ff });
			this.numPossibilities = 9;
		}

		#endregion

		#region Methods...

		/// <summary>
		///  Return an Int32 array with the possible values for this cell.
		/// </summary>
		public int[] GetPossibilities()
		{
			if (this.IsValid)
			{
				return new int[] { };
			}

			List<int> possibilities = new List<int>();

			for (int i = 0; i < 9; i++)
			{
				if (this.possibilities.Get(i))
				{
					possibilities.Add(i + 1);
				}
			}

			return possibilities.ToArray();
		}

		/// <summary>
		/// Randomly picks a possible value for this cell.
		/// </summary>
		public int PickRandomPossibility()
		{
			int[] possibilities = this.GetPossibilities();

			if (possibilities.Length == 0)
			{
				return -1;
			}

			Random rnd = new Random(DateTime.Now.Millisecond);
			int index = rnd.Next(possibilities.Length);
			return possibilities[index];
		}

		/// <summary>
		/// Removes a value from the bitmask of possible values.
		/// </summary>
		/// <param name="value">Possible value to be removed.</param>
		internal void RemovePossibility(int value)
		{
			this.CheckValue(value);
			if (this.possibilities.Get(value - 1))
			{
				this.possibilities.Set(value - 1, false);
				this.numPossibilities--;
			}
		}

		/// <summary>
		/// Sets a value for this cell.
		/// </summary>
		/// <param name="value"></param>
		internal void SetValue(int value)
		{
			if (this.IsValid)
			{
				throw new ApplicationException(
					string.Format("Already has been set a value for cell at [{0}, {1}].",
					this.row + 1, this.col + 1));
			}

			this.CheckValue(value);

			int[] possibilities = this.GetPossibilities();

			if (Array.IndexOf<int>(possibilities, value) == -1)
			{
				throw new ApplicationException(
					string.Format("Invalid value {2} for cell at [{0}, {1}].",
					this.row + 1, this.col + 1, value));
			}

			this.value = value;
		}

		/// <summary>
		/// Checks if the specified value is valid for a cell.
		/// </summary>
		/// <param name="value"></param>
		private void CheckValue(int value)
		{
			if (value < 1 || value > 9)
			{
				throw new ApplicationException(
					string.Format("Invalid value {2} for cell at [{0}, {1}].",
					this.row + 1, this.col + 1, value));
			}
		}

		#endregion

		#region Overrides...

		public override string ToString()
		{
			if (this.IsValid)
			{
				return this.value.ToString();
			}

			return UnknowCellValueChar.ToString();
		}

		#endregion

	}

}
