using GameOfLife.Core;
using Xunit;

namespace GameOfLife.Tests;
public class GameOfLifeTests {
	//Don't run printOutput true in these tests since it can lock the file. Or consider generating unique names
	[Theory]
	[MemberData(nameof(EmptyBoard_Data))]
	public void EmptyBoard_Tests(int width, int height, int generations, bool[][][] expected) {
		var board = Game.CreateEmptyBoard(width, height, generations);
		AssertBoardsEqual(expected, board);
	}
	public static object[][] EmptyBoard_Data = new object[][] {
		new object[] {
			5, 4, 3,
			new bool[][][] {
				new bool[][] {
					new bool[] { false, false, false, false, false, },
					new bool[] { false, false, false, false, false, },
					new bool[] { false, false, false, false, false, },
					new bool[] { false, false, false, false, false, },
				},
				new bool[][] {
					new bool[] { false, false, false, false, false, },
					new bool[] { false, false, false, false, false, },
					new bool[] { false, false, false, false, false, },
					new bool[] { false, false, false, false, false, },
				},
				new bool[][] {
					new bool[] { false, false, false, false, false, },
					new bool[] { false, false, false, false, false, },
					new bool[] { false, false, false, false, false, },
					new bool[] { false, false, false, false, false, },
				},
			}
		},
	};
	private static void AssertBoardsEqual(bool[][][] board1, bool[][][] board2) {
		Assert.Equal(board1.Length, board2.Length);
		for (int i = 0; i < board1.Length; i++) {
			Assert.Equal(board1[i].Length, board2[i].Length);
		}
		for (int i = 0; i < board2.Length; i++) {	//check both in case 1 is larger. The first loop could stop early
			Assert.Equal(board1[i].Length, board2[i].Length);
		}
		for (int i=0; i<board1.Length; i++) {
			for(int j=0; j<board2.Length; j++) {
				Assert.Equal(board1[i][j], board2[i][j]);
			}
		}
	}

	[Theory]
	[MemberData(nameof(ValidateInitialBoards_Data))]
	public void ValidateInitialBoards_Tests(bool[][][] board, bool expected) {
		Assert.Equal(Game.ValidateInitialBoardDimensions(board), expected);
	}
	public static object[][] ValidateInitialBoards_Data = new object[][] {
		new object[] {
			new bool[][][] {
				new bool[][] {
					new bool[0],
				},
				new bool[][] {
					new bool[0],
				},
			},
			true,
		},
		new object[] {
			new bool[][][] {
				new bool[][] {
					new bool[] { false, false, false, },
					new bool[] { false, false, false, },
					new bool[] { false, false, false, },
					new bool[] { false, false, false, },
				},
				new bool[][] {
					new bool[] { false, false, false, },
					new bool[] { false, false, false, },
					new bool[] { false, false, false, },
					new bool[] { false, false, false, },
				},
			},
			true,
		},
		new object[] {
			new bool[][][] {
				new bool[][] {
					new bool[] { false, false, },
					new bool[] { false, false, },
					new bool[] { false, false, },
				},
				new bool[][] {
					new bool[] { false, false, },
					new bool[] { false, false, },
					new bool[] { false, false, },
				},
				new bool[][] {
					new bool[] { false, },
					new bool[] { false, false, },
					new bool[] { false, },
				},
			},
			false,
		},
		new object[] {
			new bool[][][] {
				new bool[][] {
					new bool[] { false, false, },
					new bool[] { false, false, },
					new bool[] { false, false, },
				},
			},
			false,
		},
		new object[] {
			new bool[][][] {
				new bool[][] {
					new bool[] { false, false, },
					new bool[] { false, false, },
					new bool[] { false, false, },
				},
				new bool[][] {
					new bool[] { false, false, },
					new bool[] { false, false, },
					new bool[] { false, false, },
					new bool[] { false, false, },
				},
			},
			false,
		},
		new object[] {
			new bool[][][] {
				new bool[][] {
					new bool[] { false, false, false, false, false, },
					new bool[] { false, false, false, false, false, },
					new bool[] { false, false, false, false, false, },
					new bool[] { false, false, false, false, false, },
				},
				new bool[][] {
					new bool[] { false, false, false, },
					new bool[] { false, false, false, false , },
					new bool[] { false, false, false, false, false, },
					new bool[] { false, false, false, false, false, },
				},
			},
			false,
		},
	};

	[Theory]
	[MemberData(nameof(InBounds_Data))]
	public void InBounds_Tests(bool[][] board, (int x, int y)[] coords, bool[] expected) {
		for(int i = 0; i < coords.Length; i++) {
			Assert.Equal(Game.InBounds(board, coords[i].y, coords[i].x), expected[i]);
		}
	}
	public static object[][] InBounds_Data = new object[][] {
		new object[] {
			new bool[][] {
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
			},
			new []{ (0, 0), (1, 1), (0, 5), (2, 6), (2, 4), (0, 5), (-1, 1), (-1, 5), (1, -3), },
			new [] { true, true, false, false, true, false, false, false, false, },
		},
	};

	[Theory]
	[MemberData(nameof(CheckNeighbors_Data))]
	public async void CheckNeighbors_Tests() {
		//var game = new GameOfLife(10, 10, 100, true, true);
		//await game.Start().ConfigureAwait(false);
		//var expectedBoard = new bool[][][] {
		//	new bool[][]{
		//		new bool[] { true },
		//	},
		//};
		//Assert.Equal(expectedBoard, game.Board);
		//Assert.False(game.Halts);
		//Assert.True(game.Loops);
	}
	public static object[][] CheckNeighbors_Data = new object[][] {
		new object[] {

		},
	};

	[Theory]
	[MemberData(nameof(PrintBoardTrue_Data))]
	public async void PrintBoardTrue_Tests() {
		//var game = new GameOfLife(10, 10, 100, true, true);
		//await game.Start().ConfigureAwait(false);
		//var expectedBoard = new bool[][][] {
		//	new bool[][]{
		//		new bool[] { true },
		//	},
		//};
		//Assert.Equal(expectedBoard, game.Board);
		//Assert.False(game.Halts);
		//Assert.True(game.Loops);
	}
	public static object[][] PrintBoardTrue_Data = new object[][] {
		new object[] {

		},
	};

	[Fact]
	public void PrintBoardFalse_Tests() {
		//Assert file doesn't exist
	}

	[Theory]
	[MemberData(nameof(AllDead_Data))]
	public void AllDead_Tests(bool[][][] board, bool expected) {

	}
	public static object[][] AllDead_Data = new object[][] {
		new object[] {
			new bool[][][] {
				new bool[][] {
					new bool[]{ false, false, false, },
					new bool[]{ false, false, false, },
					new bool[]{ false, false, false, },
				},
			},
			true,
		},
		new object[] {
			new bool[][][] {
				new bool[][] {
					new bool[]{ true, false, false, },
					new bool[]{ false, false, false, },
					new bool[]{ false, false, false, },
				},
			},
			false,
		},
		new object[] {
			new bool[][][] {
				new bool[][] {
					new bool[]{ true, false, false, },
					new bool[]{ false, true, false, },
					new bool[]{ false, false, true, },
				},
			},
			false,
		},
	};

	[Theory]
	[MemberData(nameof(AllTrue_Data))]
	public void AllTrue_Tests(bool[][][] board, bool expected) {

	}
	public static object[][] AllTrue_Data = new object[][] {
		new object[] {
			new bool[][][] {
				new bool[][] {
					new bool[]{ true, true, true, },
					new bool[]{ true, true, true, },
					new bool[]{ true, true, true, },
				},
			},
			true,
		},
		new object[] {
			new bool[][][] {
				new bool[][] {
					new bool[]{ false, true, true, },
					new bool[]{ true, true, true, },
					new bool[]{ true, true, true, },
				},
			},
			false,
		},
		new object[] {
			new bool[][][] {
				new bool[][] {
					new bool[]{ false, true, true, },
					new bool[]{ true, false, true, },
					new bool[]{ true, true, true, },
				},
			},
			false,
		},
	};

}
