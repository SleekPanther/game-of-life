using GameOfLife.Core;

int generations = 200;
var board = Game.CreateEmptyBoard(5, 5, generations);
board[0] = new bool[][] {		//initialize with some live cells
	new bool[] { false, false, false, false, false, },
	new bool[] { false, false, true, false, false, },
	new bool[] { false, false, true, false, false, },
	new bool[5],
	new bool[5],
	new bool[5],
	new bool[5],
	new bool[5],
	new bool[5],
	new bool[5],
};
Game.InitializeRestOfBoard(board, generations);

var game = new Game(board, null, generations, "boardHistory.txt", printOutput: true, saveHistory: true, optimizedMode: false);
//await game.Start().ConfigureAwait(false);

//Thread.Sleep(15000);		//hack for shared file but doesn't work sometimes

var board2 = new bool[][][] {
	new bool[][] {
		new bool[] { false, false, false, false, false, },
		new bool[] { false, false, true, false, false, },
		new bool[] { false, false, true, false, false, },
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

var game2 = new Game(null, board2, generations, "boardHistory.txt", printOutput: true, saveHistory: true, optimizedMode: true);
await game2.Start().ConfigureAwait(false);
