using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using GameOfLife.Core;
using Microsoft.Diagnostics.Tracing.Analysis.GC;

namespace GameOfLife.Benchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
//[MaxIterationCount(50)]
public class GameOfLife_Benchmarks {
	//todo consider adding parallelism to look at parts of the board, what is shared resounce? hopefully nothing but file if divided correctly
	//commit message: Fix x/y error fr calculating neighbors, move printing to the end to avoid file busy errors, validate starting board dimensions are same in all generations, & benchmarks

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

	[ParamsSource(nameof(ValuesForGenerations))]
	public int Generations;
	//public int[] ValuesForGenerations => new[] { 100, 1_000, 10_000, };
	public int[] ValuesForGenerations => new[] { 20_000, 40_000, };
	//public int[] ValuesForGenerations => new[] { 50_000, 60_000, 70_000, };

	[Benchmark]
	public async Task<bool> GameOptimized_Benchmarks() {
		var board2 = new bool[][][] {
			new bool[][] {
				new bool[] { true, true, false, false, false, },
				new bool[] { true, true, false, false, false, },
				new bool[] { true, true, false, false, false, },
				new bool[5],
				new bool[5],
				new bool[5],
				new bool[5],
				new bool[5],
				new bool[5],
				new bool[5],
			},
			new bool[][] {
				new bool[5],
				new bool[5],
				new bool[5],
				new bool[5],
				new bool[5],
				new bool[5],
				new bool[5],
				new bool[5],
				new bool[5],
				new bool[5],
			},
		};
		var game2 = new Game(null, board2, Generations, "boardHistory.txt", printOutput: false, saveHistory: true, optimizedMode: true);
		await game2.Start().ConfigureAwait(false);
		return true;
	}

	[Benchmark]
	public async Task<bool> GameNonOptimized_Benchmarks() {
		var board = Game.CreateEmptyBoard(5, 5, Generations);
		board[0] = new bool[][] {
			new bool[] { true, true, false, false, false, },
			new bool[] { true, true, false, false, false, },
			new bool[] { true, true, false, false, false, },
			new bool[5],
			new bool[5],
			new bool[5],
			new bool[5],
			new bool[5],
			new bool[5],
			new bool[5],
		};

		Game.InitializeRestOfBoard(board, Generations);
		var game = new Game(board, null, Generations, "boardHistory.txt", printOutput: false, saveHistory: true, optimizedMode: false);
		await game.Start().ConfigureAwait(false);
		return true;
	}

	public static void Main(string[] args) {
		BenchmarkRunner.Run<GameOfLife_Benchmarks>();
	}
}
