using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using GameOfLife.Core;

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
		//Game.CheckCycle(board, history, generations);
	}

	[Benchmark]
	public async Task<bool> GameNonOptimized_Benchmarks() {
		int generations = 300;
		var board = Game.CreateEmptyBoard(5, 5, generations);
		board[0] = new bool[][] {
			new bool[] { true, true, false, false, false, },
			new bool[] { true, true, false, false, false, },
			new bool[] { true, true, false, false, false, },
			new bool[] { false, false, false, false, false, },
			new bool[] { false, false, false, false, false, },
			new bool[] { false, false, false, false, false, },
			new bool[] { false, false, false, false, false, },
			new bool[] { false, false, false, false, false, },
			new bool[] { false, false, false, false, false, },
			new bool[] { false, false, false, false, false, },
		};

		Game.InitializeRestOfBoard(board, generations);
		var game = new Game(board, null, generations, "boardHistory.txt", printOutput: false, saveHistory: true, optimizedMode: false);
		await game.Start().ConfigureAwait(false);
		return true;
	}

	[Benchmark]
	public async Task<bool> GameOptimized_Benchmarks() {
		var board2 = new bool[][][] {
			new bool[][] {
				new bool[] { true, true, false, false, false, },
				new bool[] { true, true, false, false, false, },
				new bool[] { true, true, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
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
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
				new bool[] { false, false, false, false, false, },
			},
		};
		var generations = 300;
		var game2 = new Game(null, board2, generations, "boardHistory.txt", printOutput: false, saveHistory: true, optimizedMode: true);
		await game2.Start().ConfigureAwait(false);
		return true;
	}

	public static void Main(string[] args) {
		BenchmarkRunner.Run<GameOfLife_Benchmarks>();
	}
}
