using GameOfLife.Core;

long generations = 10;
var board = new bool[generations][][];
board[0] = new bool[][] {
	new bool[] { true, true, false, },
	new bool[] { true, true, false, },
	new bool[] { false, false, false, },
};
int height = board[0].Length;
int width = board[0][0].Length;


//todo change
for (int i = 1; i < generations; i++) {	//skip 1st generation
	board[i] = new bool[height][];
	for (int j = 0; j < board[i].Length; j++) {
		board[i][j] = Enumerable.Repeat(false, width).ToArray();
	}
}
var game = new GameOfLife.Core.GameOfLife(3 , 3, board, generations, true);
await game.Start();
