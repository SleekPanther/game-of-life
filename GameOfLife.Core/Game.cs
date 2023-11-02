using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace GameOfLife.Core;
public class Game {
	private const int MAX_GENERATIONS = 1_000_000;

	private bool[][][]? board;
	private bool[][][]? board2;		//only 2 generations, values copied from 0 to 1 on odd generations and then 1 to 0 on even generations
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

	private StringBuilder? sb;
	private string outputFolderPath = $"{Assembly.GetExecutingAssembly().Location[0..(Assembly.GetExecutingAssembly().Location.IndexOf("GameOfLife.ConsoleRunner") + "GameOfLife.ConsoleRunner".Length)]}\\output\\";
	public void SetOutputFolderName(string name) {
		outputFolderPath = name;
	}
	private string? outputFileName;
	private string? outputFilePath;
	public void SetOutputFileNameAndPath(string name) {
		int extensionIndex = name.IndexOf(".txt");
		name = name.Substring(0, extensionIndex) + "-" + DateTime.Now.Ticks + ".txt";
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
		Initialize(this.board, this.board2, generations, fileName, printOutput, saveHistory, optimizedMode);
	}

	private void Initialize(bool[][][]? board, bool[][][]? board2, int generations, string? fileName, bool printOutput, bool saveHistory, bool optimizedMode) {
		this.board = board;
		this.board2 = board2;
		this.optimizedMode = optimizedMode;
		this.saveHistory = saveHistory;
		if (saveHistory) {
			if (optimizedMode) {
				if (!ValidateInitialBoardDimensions(board2!)) throw new InvalidDataException($"{nameof(board2)} is invalid because it doesn't have the same dimensions in all generations.");
				this.history = new string[board2![0].Length][];
				for (int i = 0; i < history.Length; i++) {
					history[i] = Enumerable.Repeat(string.Empty, board2![0].Length).ToArray();
				};
			}
			else {
				if (!ValidateInitialBoardDimensions(board!)) throw new InvalidDataException($"{nameof(board)} is invalid because it doesn't have the same dimensions in all generations.");
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
			this.sb = new StringBuilder();
				SetOutputFileNameAndPath(fileName!);
		}
	}
	public static bool ValidateInitialBoardDimensions(bool[][][] board) {
		if(board == null) throw new ArgumentNullException(nameof(board));

		if (board.Length < 2) return false;

		for (int i = 0; i < board.Length - 1; i++) {
			for (int j = 0; j < board[0].Length; j++) {
				if (board[0].Length != board[i + 1].Length)		//check row count
					return false;
				for (int k = 0; k < board[0][0].Length - 1; k++) {	//now check all columns
					if (board[0][0].Length != board[i +1][k].Length)
						return false;
				}
			}
		}
		return true;
	}

	public Game(bool[][][]? board, bool[][][]? board2, int generations, string? fileName, bool printOutput, bool saveHistory, bool optimizedMode) {
		Initialize(board, board2, generations, fileName, printOutput, saveHistory, optimizedMode);
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

	public async Task Start() {
		running = true;
		if (!optimizedMode) {
			PrintBoard(board![0], generation, outputFilePath, PrintOutput, sb);
			PrintNeighborCount(board[0], outputFilePath, PrintOutput, sb);
			for (generation = 1; generation < Generations; generation++) {      //class state variable for generation, not loop variable
				CheckNeighbors(board, history, generation, this.saveHistory, optimizedMode: false);
				PrintBoard(board[generation], generation, outputFilePath, PrintOutput, sb);
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
			PrintBoard(board2![0], generation, outputFilePath, PrintOutput, sb);
			PrintNeighborCount(board2[0], outputFilePath, PrintOutput, sb);
			for (generation = 1; generation < Generations; generation++) {      //class state variable for generation, not loop variable
				long previousGeneration = Math.Abs(generation % 2 - 1);      //alternates between 0 and 1
				CopyBoard2(board2, previousGeneration);
				CheckNeighbors(board2, history, generation, this.saveHistory, this.optimizedMode);
				PrintBoard(board2[(previousGeneration +1) %2], generation, outputFilePath, PrintOutput, sb);
				PrintNeighborCount(board2[(previousGeneration + 1) % 2], outputFilePath, PrintOutput, sb);
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
		await PrintFinishedMessage();
	}

	public async Task PrintFinishedMessage() {
		if (!printOutput) return;

		try {
			await File.AppendAllTextAsync(outputFilePath!, sb.ToString());
		}
		catch (Exception ex) {
			Console.WriteLine($"Error writing to output file: {outputFilePath}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}");
			Thread.Sleep(5000);    //wait before writing next in case file is bust
		}

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
		await File.AppendAllTextAsync(outputFilePath!, message);
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
	private static void CheckNeighbors(bool[][][] board, string[][]? history, long generation, bool saveHistory, bool optimizedMode) {
		long previousGeneration = generation -1;
		if (optimizedMode) {
			previousGeneration = Math.Abs(generation % 2 - 1);
			generation = generation % 2;
		}
		for (int i = 0; i < board[generation].Length; i++) {
			for (int j = 0; j < board[generation][0].Length; j++) {
				short neighborCount = 0;
				foreach (var (y, x) in neighborDirections) {
					if (InBounds(board[previousGeneration], i + y, j + x)	//look @ previous generation
						&& board[previousGeneration][i + y][j + x]) {
						neighborCount++;
					}
				}
				if (neighborCount < 2) {        //dies
					board[generation][i][j] = false;
				} else if (neighborCount == 2		//born
					|| neighborCount == 3) {	//stays alive
					board[generation][i][j] = true;
				} else {    //dies of overpopulation
					board[generation][i][j] = false;
				}

				if (saveHistory) {
					history![i][j] += board[generation][i][j] ? 1 : 0;
				}
			}
		}
	}
	//Note y comes first since we access arrays row-wise (not like a mathematical point (x, y))
	private static readonly (int y, int x)[] neighborDirections = new(int y, int x)[]{
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

	private static void CopyBoard2(bool[][][] board, long previousGeneration) {
		for (int i = 0; i < board[0].Length; i++) {
			for (int j = 0; j < board[0][0].Length; j++) {
				if (previousGeneration == 0) {
					board[previousGeneration +1][i][j] = board[previousGeneration][i][j];	//copy board[0] to board[1]
				}
				else {
					board[previousGeneration -1][i][j] = board[previousGeneration][i][j]; //copy board[1] to board[0]
				}
			}
		}
	}
	private static void PrintBoard(bool[][][] board, string outputFilePath, bool printOutput, StringBuilder sb) {
		if (!printOutput) return;

		for (int i = 0; i < board.Length; i++) {
			for (int j = 0; j < board[i].Length; j++) {
				for (int k = 0; k < board[i][j].Length; k++) {
					sb.AppendLine(string.Join(" ", board[i][j].Select(c => c ? 1 : 0)));
				}
			}
		}
	}
	private static void PrintBoard(bool[][] board, long generation, string outputFilePath, bool printOutput, StringBuilder sb) {
		if (!printOutput) return;

		sb.AppendLine($"Generation {generation}");
		for (int  i = 0; i < board.Length; i++) {
			sb.AppendLine(string.Join(" ", board[i].Select(c => c ? 1 : 0)));
		}
		sb.AppendLine();
	}
	private static void PrintNeighborCount(bool[][] board, string outputFilePath, bool printOutput, StringBuilder sb) {
		if (!printOutput) return;

		sb.AppendLine($"Neighbor Count");
		for (int i = 0; i < board.Length; i++) {
			for(int j = 0; j < board[i].Length; j++) {
				short neighborCount = 0;
				foreach (var (y, x) in neighborDirections) {
					//if (InBounds(board, i + x, j + y)
					//	&& board[i + x][j + y]) {
					if (InBounds(board, i + y, j + x)) {
						if (board[i + y][j + x]) {
							neighborCount++;
						}
					}
				}
				sb.Append(" " + neighborCount);
			}
			sb.AppendLine();
		}
		sb.AppendLine();
	}

}
