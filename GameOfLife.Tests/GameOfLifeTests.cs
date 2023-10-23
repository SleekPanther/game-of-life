namespace GameOfLife.Tests;
public class GameOfLifeTests {
	[Theory]
	[MemberData(nameof(CheckNeighbors_Data))]
	public async void CheckNeighbors_Tests() {
		//var game = new GameOfLife.Core.GameOfLife(10, 10, 100, true, true);
		//await game.Start();
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
		//var game = new GameOfLife.Core.GameOfLife(10, 10, 100, true, true);
		//await game.Start();
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
