using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;

namespace GameOfLife.Benchmarks;
//todo get results as html file (nick chapsas vid)

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class GameOfLife_Benchmarks {
	//compare board vs board2 (probaly less memory, but does copying every time and opening the file multiple times slow it down
	//if io bound, consider using stringbuilder and only writing @ end
	//todo consider adding parallelism to look at parts of the board, what is shared resounce? hopefully nothing but file if divided correctly

	//xor
	[Benchmark]
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
		int generation = 5;
		//GameOfLife.Core.GameOfLife.CheckCycle(board, history, generation);
	}

	//equal comparison
	[Benchmark]
	public void CheckCycle_Benchmarks2() {
	}

	public static void Main(string[] args) {
		BenchmarkRunner.Run<GameOfLife_Benchmarks>();
	}
}
