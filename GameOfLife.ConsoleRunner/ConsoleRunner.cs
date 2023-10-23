using GameOfLife.Core;

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
for (int i = 1; i < generations; i++) {	//skip 1st generation
	board[i] = new bool[board[0].Length][];
	for (int j = 0; j < board[i].Length; j++) {
		board[i][j] = Enumerable.Repeat(false, board[0][0].Length).ToArray();
	}
}
var game = new GameOfLife.Core.GameOfLife(board, null, generations, "boardHistory.txt", true, true, newMode: false);
await game.Start();

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
var game2 = new GameOfLife.Core.GameOfLife(null, board2, generations, "boardHistory.txt", true, true, newMode: true);
await game2.Start();
