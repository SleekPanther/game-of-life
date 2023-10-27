using GameOfLife.Core;


int generations = 20;
var board = Game.CreateEmptyBoard(5, 5, generations);
board[0] = new bool[][] {		//initialize with some live cells
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

var game = new Game(board, null, generations, "boardHistory.txt", printOutput: true, saveHistory: true, optimizedMode: false);
await game.Start().ConfigureAwait(false);

Thread.Sleep(15000);		//hack for shared file but doesn't work sometimes

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
var game2 = new Game(null, board2, generations, "boardHistory.txt", printOutput: true, saveHistory: true, optimizedMode: true);
await game2.Start().ConfigureAwait(false);
