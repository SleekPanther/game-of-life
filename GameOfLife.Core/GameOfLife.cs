using System.Reflection;

namespace GameOfLife.Core;
public class GameOfLife {
	private int height => board[0].Length;
	private int width => board[0][0].Length;
	private readonly bool[][][] board;
	private readonly bool[][][] board2;	//only 2 states? or 3?
	public bool[][][] Board => board;
	private readonly string[][] history;
	private readonly long generations;
	private readonly bool printOutput;
	public bool PrintOutput => printOutput;
	//private bool halts;
	//public bool Halts => halts;
	//private bool loops;
	//public bool Loops => loops;

	private const long MAX_GENERATIONS = 1_000_000;
	private static readonly string outputFilePath =
		$"{Assembly.GetExecutingAssembly().Location[0..(Assembly.GetExecutingAssembly().Location.IndexOf("GameOfLife.ConsoleRunner") + "GameOfLife.ConsoleRunner".Length)]}\\board.txt";

	public GameOfLife(int width, int height, long generations, bool printOutput) {
		//todo how to call other constructor and return something?
		this.board = new bool[generations][][];		//default board
		for (int i = 0; i < board.Length; i++) {
			board[i] = new bool[height][];
			for (int j = 0; j < board[i].Length; j++) {
				board[i][j] = Enumerable.Repeat(false, width).ToArray();
			}
		}
		//var x = new GameOfLife(width, height, board, generations);	//todo call other constructor, but :GameOfLife didn't worrlk
		this.history = new string[height][];
		for (int i = 0; i < history.Length; i++) {
			history[i] = Enumerable.Repeat(string.Empty, width).ToArray();
		};
		this.generations = generations;
		this.printOutput = printOutput;
		if (printOutput) {
			try {
				if (File.Exists(outputFilePath)) {
					File.WriteAllText(outputFilePath, string.Empty);
				}
			}
			catch (Exception ex) {
				Console.WriteLine($"Error creating output file: {outputFilePath}{Environment.NewLine}{ex.Message}{Environment.NewLine}{Environment.StackTrace}");
				throw;
			}
		}
	}
	public GameOfLife(int width, int height, bool[][][] board, long generations, bool printOutput) {
		this.board = board;
		this.history = new string[height][];
		for (int i = 0; i < history.Length; i++) {
			history[i] = Enumerable.Repeat(string.Empty, width).ToArray();
		};
		if(generations > MAX_GENERATIONS) {
			throw new ArgumentException($"{generations} must be less than {MAX_GENERATIONS + 1}", nameof(generations));
		}
		this.generations = generations;
		this.printOutput = printOutput;
		if (printOutput) {
			try {
				if (File.Exists(outputFilePath)) {
					File.WriteAllText(outputFilePath, string.Empty);
				}
			}
			catch (Exception ex) {
				Console.WriteLine($"Error creating output file: {outputFilePath}{Environment.NewLine}{ex.Message}{Environment.NewLine}{Environment.StackTrace}");
				throw;
			}
		}
	}

	public async Task<bool> Start() {
		if (PrintOutput) {
			await PrintBoard(board[0]);
		}

		for (long generation = 1; generation < generations; generation++) {
			CheckNeighbors(board, history, generation, generations);
			//todo only do this @ the end
			if (PrintOutput) {
				await PrintBoard(board[generation]);
			}

			//PrintNeighborCount(board
			//Thread.Sleep(10000);

			//if (CheckCycle(board, history, generation)) {
			//	//found cycle & alert with number of iterations
			//	loops = true;
			//	break;
			//}
			//else could be looping forever
		}

		if (PrintOutput) {
			await PrintBoard(board);
		}

		var input = new[] {
			1,
			2,
			3,
			1,
			2,
			3,
			1,
			2,
			3,
			4,
			1,
		};
		List<(int, int, int)> result = Group(input).ToList();

		return true;
	}

	//remove, unsolvable
	public static bool CheckCycle(bool[][][] board, string[][] history, long generation) {
		for (int i = 0; i < board.Length; i++) {
			for (int j = 0; j < board[i].Length; j++) {
				for(int k = 0; k < board[i][j].Length; k++) {
					if (board[i][j][k]) {	//xor==0? or 1
						return false;
					}
				}
			}
		}

		for (long i = 0; i< generation; i++) {
			//all possible subarrays?
			//size to to gen, compare neighbors & break 
			//for each cell, all must be a repeating cycle up to generations
		}
		return true;
	}

	private static void CheckNeighbors(bool[][][] board, string[][] history, long generation, long generations) {
		CopyBoard(board, generation);

		for (int i = 0; i < board[generation].Length; i++) {
			for (int j = 0; j < board[i].Length; j++) {
				int neighborCount = 0;
				foreach(var (x, y) in neighborDirections
					.Where(point => !(point.x == i && point.y == j)))   //skip current cell
					{
					//if (InBounds(board[generation - 1], i + x, j + y) && board[generation - 1][i + x][j + y]) {
					//	string pos = $"[{i + x}][{j + y}] {(board[generation - 1][i + x][j + y] ? 1 : 0)}";
					//	bool t = true;
					//}
					if (InBounds(board[generation -1], i + x, j + y)
						&& board[generation -1][i + x][j + y]) {
						neighborCount++;
					}
				}
				if (neighborCount < 2) {        //dies
					board[generation][i][j] = false;
				}
				else if (neighborCount == 2		//stays alive
					|| neighborCount == 3) {	//born
					board[generation][i][j] = true;
				}
				//else leave alone

				history[i][j] += board[generation][i][j] ? 1: 0;
			}
		}

		//Todo check if entire board is alive or dead and exit early
	}
	private static readonly (int x, int y)[] neighborDirections = new(int x, int y)[]{
		(-1, -1),
		(-1, 0),
		(-1, 1),
		(0, -1),
		(0, 1),
		(1, -1),
		(1, 0),
		(1, 1),
	};
	public static bool InBounds(bool[][] board, int x, int y) {
		return x >=0 && x < board.Length
			&& y >=0 && y < board.Length;
	}
	private static void CopyBoard(bool[][][] board, long generation) {
		for (int j = 0; j < board[generation -1].Length; j++) {
			for (int k = 0; k < board[generation -1][j].Length; k++) {
				board[generation][j][k] = board[generation - 1][j][k];
			}
		}
	}

	private async Task<bool> PrintBoard(bool[][][] board) {
		var result = new string[board[0].Length];
		for(int i = 0; i < board.Length; i++) {
			for (int j = 0; j < board[i].Length; j++) {
				for (int k = 0; j < board[i][j].Length; k++) {
					result[j] = string.Join(" ", board[i][j].Select(c => c ? 1 : 0));
				}
			}
		}
		
		try {
			await File.AppendAllLinesAsync(outputFilePath, result);
			var res = string.Join(Environment.NewLine, result);     //debugging only
		}
		catch (Exception ex) {
			Console.WriteLine($"Error writing to output file to: {outputFilePath}{Environment.NewLine}{ex.Message}");
			return false;
		}
		return true;
	}
	private async static Task<bool> PrintBoard(bool[][] board) {
		var result = new string[board[0].Length + 1];
		for(int  i = 0; i < board[0].Length; i++) {
			result[i] = string.Join(" ", board[i].Select(c => c ? 1 : 0));
		}
		try {
			await File.AppendAllLinesAsync(outputFilePath, result);
			var res = string.Join(Environment.NewLine, result);		//debugging only
		}
		catch (Exception ex) {
			Console.WriteLine($"Error writing to output file to: {outputFilePath}{Environment.NewLine}{ex.Message}");
			return false;
		}
		return true;
	}
	private static void PrintNeighborCount(bool[][] board, int width, int height) {
		var result = new string[height];
		for (int i = 0; i < height; i++) {
			result[i] = string.Join(" ", board[i].Select(c => c ? 1 : 0));
		}
		File.WriteAllLines(outputFilePath, result);
	}

	//todo remove
	public static IEnumerable<(T, int, int)> Group<T>(IEnumerable<T> source) where T : IEquatable<T> {
		if (!source.Any()) { // if input is empty, return empty sequence
			yield break;
		}
		// element of current sub array
		T currentElement = source.First();
		int startIndex = 0; // ...of current sub array
		int currentIndex = 0; // ...of the input sequence
		int count = 0; // ...of current sub array
		foreach (T elem in source) {
			if (elem.Equals(currentElement)) {
				count++;
			}
			else {
				// when we find a different element, return everything we know about the current sub array
				yield return (currentElement, startIndex, count);
				// reset info about the current sub array
				currentElement = elem;
				startIndex = currentIndex;
				count = 1;
			}
			currentIndex++;
		}
		// after we iterated the whole input sequence, we return another sub array
		yield return (currentElement, startIndex, count);
	}
}
