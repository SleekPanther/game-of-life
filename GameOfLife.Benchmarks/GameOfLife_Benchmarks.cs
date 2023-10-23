using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;

namespace GameOfLife.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class GameOfLife_Benchmarks {
	//if io bound, consider using stringbuilder and only writing @ end
	//todo consider adding parallelism to look at parts of the board, what is shared resounce? hopefully nothing but file if divided correctly

	//[Benchmark]
	public void CheckCycle_Benchmarks() {
		var board = new bool[][][] {
			new bool[][] {
				new bool[] { false, true, false, },
				new bool[] { false, true, false, },
				new bool[] { true, true, false, },
			},
		};
		//var history = new string[][]{
		//	new []{"00000", "11111", "00000", },
		//	new []{"00000", "11111", "00000", },
		//	new []{"11111", "11111", "00000", },
		//};
		int generations = 5;
		//GameOfLife.Core.GameOfLife.CheckCycle(board, history, generations);
	}

	[Benchmark]
	public async Task<bool> Game1_Benchmarks() {
		long generations = 10;
		var board = GameOfLife.Core.GameOfLife.CreateEmptyBoard(5, 5, generations);
		board[0] = new bool[][] {
			new bool[] { true, true, false, false, false, },
			new bool[] { true, true, false, false, false, },
			new bool[] { false, true, false, false, false, },
			new bool[] { false, false, false, false, false, },
			new bool[] { false, false, false, false, false, },
		};

		//todo change
		for (int i = 1; i < generations; i++) { //skip 1st generation
			board[i] = new bool[board[0].Length][];
			for (int j = 0; j < board[i].Length; j++) {
				board[i][j] = Enumerable.Repeat(false, board[0][0].Length).ToArray();
			}
		}
		var game = new GameOfLife.Core.GameOfLife(board, null, generations, "boardHistory.txt", true, true, newMode: false);
		await game.Start();
		return true;
	}

	[Benchmark]
	public async Task<bool> Game2_Benchmarks() {
		var board2 = new bool[][][] {
			new bool[][] {
				new bool[] { true, true, false, false, false, },
				new bool[] { true, true, false, false, false, },
				new bool[] { false, true, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
			},
			new bool[][] {
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
			},
		};
		var game2 = new GameOfLife.Core.GameOfLife(null, board2, 10, "boardHistory.txt", true, true, newMode: true);
		await game2.Start();
		return true;
	}

	public static void Main(string[] args) {
		BenchmarkRunner.Run<GameOfLife_Benchmarks>();
	}
}
