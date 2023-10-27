using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace GameOfLife.Core;
public class Game {
	private const long MAX_GENERATIONS = 1_000_000;

	private bool[][][]? board;
	private bool[][][]? board2;		//only 2 generations, values copied from 0 to 1 and then 1 to 0 every iteration
	private bool saveHistory;
	private bool SaveHistory => saveHistory;
	private string[][]? history;
	public long generations;
	public long Generations => generations;
	public bool printOutput;
	public bool PrintOutput => printOutput;
	private bool optimizedMode;
	private long generation;
	private bool running;
	public bool Running => running;
	private bool halts;
	public bool Halts => halts;
	private bool loops;
	public bool Loops => loops;

	private readonly StringBuilder? sb;
	private string outputFolderPath = $"{Assembly.GetExecutingAssembly().Location[0..(Assembly.GetExecutingAssembly().Location.IndexOf("GameOfLife.ConsoleRunner") + "GameOfLife.ConsoleRunner".Length)]}\\output\\";
	public void SetOutputFolderName(string name) {
		outputFolderPath = name;
	}
	private string? outputFileName;
	private string? outputFilePath;
	public void SetOutputFileNameAndPath(string name) {
		outputFileName = name;
		outputFilePath = outputFolderPath + name;
	}

	public Game(int width, int height, int generations, string? fileName, bool printOutput, bool saveHistory, bool optimizedMode) {
		this.optimizedMode = optimizedMode;
		if (!optimizedMode) {
			this.board = CreateEmptyBoard(width, height, generations);
			this.board2 = null;
		}
		else {
			this.board2 = CreateEmptyBoard(width, height, generations);
			this.board = null;
		}
		Initialize(this.board, this.board2, generations, fileName, printOutput, saveHistory, optimizedMode, this.sb);
	}

	private void Initialize(bool[][][]? board, bool[][][]? board2, int generations, string? fileName, bool printOutput, bool saveHistory, bool optimizedMode, StringBuilder sb) {
		this.board = board;
		this.board2 = board2;
		this.optimizedMode = optimizedMode;
		this.saveHistory = saveHistory;
		if (saveHistory) {
			if (optimizedMode) {
				this.history = new string[board2![0].Length][];
				for (int i = 0; i < history.Length; i++) {
					history[i] = Enumerable.Repeat(string.Empty, board2![0].Length).ToArray();
				};
			}
			else {
				this.history = new string[board![0].Length][];
				for (int i = 0; i < history.Length; i++) {
					history[i] = Enumerable.Repeat(string.Empty, board[0].Length).ToArray();
				};
			}
		}
		if (generations > MAX_GENERATIONS) {
			throw new ArgumentException($"{generations} must be less than {MAX_GENERATIONS + 1}", nameof(generations));
		}
		if (generations < 0) {
			throw new ArgumentException($"{generations} must be positive", nameof(generations));
		}
		this.generations = generations;
		this.printOutput = printOutput;
		if (printOutput) {
			sb = new StringBuilder();
			try {
				outputFileName = fileName!;
				SetOutputFileNameAndPath(fileName!);
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

	public Game(bool[][][]? board, bool[][][]? board2, int generations, string? fileName, bool printOutput, bool saveHistory, bool optimizedMode) {
		Initialize(board, board2, generations, fileName, printOutput, saveHistory, optimizedMode, this.sb);
	}
	public static bool[][][] CreateEmptyBoard(int width, int height, int generations) {
		var board = new bool[generations][][];
		for (int i = 0; i < generations; i++) {
			board[i] = new bool[height][];
			for (int j = 0; j < board[i].Length; j++) {
				board[i][j] = Enumerable.Repeat(false, width).ToArray();
			}
		}
		return board;
	}

	public static void InitializeRestOfBoard(bool[][][] board, int generations) {
		for (int i = 1; i < generations; i++) {		//skip 1st generation
			board[i] = new bool[board[0].Length][];
			for (int j = 0; j < board[i].Length; j++) {
				board[i][j] = Enumerable.Repeat(false, board[0][0].Length).ToArray();
			}
		}
	}

	private static void ClearOutputFile(bool printOutput, string outputFilePath) {
		if (printOutput) {
			try {
				if (File.Exists(outputFilePath)) {
					File.WriteAllText(outputFilePath, string.Empty);    //clear any existing file
				}
			}
			catch (Exception ex) {
				Console.WriteLine($"Error clearing output file: {outputFilePath}{Environment.NewLine}{ex.Message}{Environment.NewLine}{Environment.StackTrace}");
				throw;
			}
		}
	}

	public async Task Start() {
		running = true;
		ClearOutputFile(PrintOutput, outputFilePath);
		if (!optimizedMode) {
			await PrintBoard(board![0], outputFilePath, PrintOutput, sb).ConfigureAwait(false);
			PrintNeighborCount(board[0], outputFilePath, PrintOutput, sb);
			for (generation = 1; generation < Generations; generation++) {      //class state variable for generation, not loop variable
				CopyBoard(board, generation);
				CheckNeighbors(board, history, generation, this.saveHistory);
				//todo only do this @ the end
				await PrintBoard(board[generation], outputFilePath, PrintOutput, sb).ConfigureAwait(false);
				PrintNeighborCount(board[generation], outputFilePath, PrintOutput, sb);

				if (AllDead(board)) {
					this.halts = true;
					return;
				}
				if (AllAlive(board)) {
					this.halts = true;
					return;
				}
			}
		}
		else {
			await PrintBoard(board2![0], outputFilePath, PrintOutput, sb).ConfigureAwait(false);
			PrintNeighborCount(board2[0], outputFilePath, PrintOutput, sb);
			for (generation = 1; generation < Generations; generation++) {      //class state variable for generation, not loop variable
				CopyBoard2(board2, generation);
				CheckNeighbors(board2, history, generation, this.saveHistory);
				//todo only do this @ the end
				long index = generation %2;
				await PrintBoard(board2[index], outputFilePath, PrintOutput, sb).ConfigureAwait(false);
				PrintNeighborCount(board2[index], outputFilePath, PrintOutput, sb);

				if (AllDead(board2)) {
					this.halts = true;
					return;
				}
				if (AllAlive(board2)) {
					this.halts = true;
					return ;
				}
			}
		}
		await PrintFinishedMessage().ConfigureAwait(false);
	}
	public async Task PrintFinishedMessage() {
		if (!printOutput) return;

		string message;
		if (Running) {
			message = $"Game is still running on generation {generation}";
			Console.WriteLine(message);
		}
		else {
			message = $"Game finished after {generation} generations which is {(generation < Generations ? "" : "not")} less than its max generations of {Generations}. {(Halts ? "Halts" : "")}{(Loops ? "Loops" : "")}";
			Console.WriteLine(message);
			if (Loops) {

			}
		}
		await File.WriteAllTextAsync(outputFilePath!, message).ConfigureAwait(false);
	}
	public static bool AllDead(bool[][][] board) {
		for (int i = 0; i < board.Length; i++) {
			for (int j = 0; j < board[i].Length; j++) {
				for (int k = 0; k < board[i][j].Length; k++) {
					if (board[i][j][k]) {
						return false;
					}
				}
			}
		}
		return true;
	}
	public static bool AllAlive(bool[][][] board) {
		for (int i = 0; i < board.Length; i++) {
			for (int j = 0; j < board[i].Length; j++) {
				for (int k = 0; k < board[i][j].Length; k++) {
					if (!board[i][j][k]) {
						return false;
					}
				}
			}
		}
		return true;
	}

	private static void CheckNeighbors(bool[][][] board, string[][]? history, long generation, bool saveHistory) {
		long index = generation % 2;
		for (int i = 0; i < board[index].Length; i++) {
			for (int j = 0; j < board[index][0].Length; j++) {
				short neighborCount = 0;
				foreach(var (x, y) in neighborDirections.Where(point => !(point.x == i && point.y == j)))   //skip current cell
					{
					if (InBounds(board[index], i + x, j + y)
						&& board[index][i + x][j + y]) {
						neighborCount++;
					}
				}
				if (neighborCount < 2) {        //dies
					board[index][i][j] = false;
				}
				else if (neighborCount == 2		//stays alive
					|| neighborCount == 3) {	//born
					board[index][i][j] = true;
				}
				//else leave alone

				if (saveHistory) {
					history![i][j] += board[index][i][j] ? 1 : 0;
				}
			}
		}
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
		return x >= 0 && x < board[0].Length
			&& y >= 0 && y < board[0].Length;
	}
	private static void CopyBoard(bool[][][] board, long generation) {
		for (int i = 0; i < board[0].Length; i++) {
			for (int j = 0; j < board[0][0].Length; j++) {
				board[generation][i][j] = board[generation -1][i][j];
			}
		}
	}
	private static void CopyBoard2(bool[][][] board, long generation) {
		long index = generation % 2;
		for (int i = 0; i < board[0].Length; i++) {
			for (int j = 0; j < board[0][0].Length; j++) {
				if (index == 0) {
					board[index][i][j] = board[index +1][i][j];
				}
				else {
					board[index][i][j] = board[index -1][i][j];
				}
			}
		}
	}

	private static async Task<bool> PrintBoard(bool[][][] board, string outputFilePath, bool printOutput, StringBuilder sb) {
		if (!printOutput) return false;

		sb.Clear();
		for(int i = 0; i < board.Length; i++) {
			for (int j = 0; j < board[i].Length; j++) {
				for (int k = 0; k < board[i][j].Length; k++) {
					sb.AppendLine(string.Join(" ", board[i][j].Select(c => c ? 1 : 0)));
				}
			}
		}
		
		try {
			await File.AppendAllTextAsync(outputFilePath, sb.ToString()).ConfigureAwait(false);
		}
		catch (Exception ex) {
			Console.WriteLine($"Error writing to output file to: {outputFilePath}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}");
			return false;
		}
		return true;
	}
	private async static Task<bool> PrintBoard(bool[][] board, string outputFilePath, bool printOutput, StringBuilder sb) {
		if (!printOutput) return false;

		sb.Clear();
		for(int  i = 0; i < board.Length; i++) {
			sb.AppendLine(string.Join(" ", board[i].Select(c => c ? 1 : 0)));
		}
		sb.AppendLine();
		try {
			await File.AppendAllTextAsync(outputFilePath, sb.ToString()).ConfigureAwait(false);
		}
		catch (Exception ex) {
			Console.WriteLine($"Error writing to output file: {outputFilePath}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}");
			return false;
		}
		return true;
	}
	private async static void PrintNeighborCount(bool[][] board, string outputFilePath, bool printOutput, StringBuilder sb) {
		if (!printOutput) return;

		sb.Clear();
		sb.AppendLine("Neighbor Count");
		for (int i = 0; i < board.Length; i++) {
			for(int j = 0; j < board[i].Length; j++) {
				short neighborCount = 0;
				foreach (var (x, y) in neighborDirections
					.Where(point => !(point.x == i && point.y == j)))   //skip current cell
					{
					if (InBounds(board, i + x, j + y)
						&& board[i + x][j + y]) {
						neighborCount++;
					}
				}
				sb.Append(" " + neighborCount);
			}
			sb.AppendLine();
		}
		sb.AppendLine();
		try {
			await File.AppendAllTextAsync(outputFilePath, sb.ToString()).ConfigureAwait(false);
			var res = string.Join(Environment.NewLine, sb);     //debugging only
		}
		catch (Exception ex) {
			Console.WriteLine($"Error writing to output file: {outputFilePath}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}");
		}
	}

}
