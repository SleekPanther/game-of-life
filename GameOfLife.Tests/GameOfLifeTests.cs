namespace GameOfLife.Tests;
public class GameOfLifeTests {
	//add some default boards to share between tests and properties

	[Theory]
	[MemberData(nameof(CheckNeighbors_Data))]
	public async void CheckNeighbors_Tests() {
		var game = new GameOfLife.Core.GameOfLife(10, 10, 100, true);
		await game.Start();
		var expectedBoard = new bool[][][] {
			new bool[][]{
				new bool[] { true },
			},
		};
		Assert.Equal(expectedBoard, game.Board);
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
		var game = new GameOfLife.Core.GameOfLife(10, 10, 100, true);
		await game.Start();
		var expectedBoard = new bool[][][] {
			new bool[][]{
				new bool[] { true },
			},
		};
		Assert.Equal(expectedBoard, game.Board);
		//Assert.False(game.Halts);
		//Assert.True(game.Loops);
	}
	public static object[][] PrintBoardTrue_Data = new object[][] {
		new object[] {

		},
	};

	[Fact]
	public async void PrintBoardFalse_Tests() {
		//Assert file doesn't exist
	}

}
