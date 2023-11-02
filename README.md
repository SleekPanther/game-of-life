# John Conway's Game of Life Simulation
A C# visualization of John Conway's Game of Life

## Rules
At each generation
- If cell has **< 2 neighbors**, it dies
- If it has **exactly 2 neighbors** it stays alive
- If it has **exactly 3 neighbors**, it will be born

Glider = same pattern, shifted/translated across the board

Enhancements
- auto-grow board until a user-defined limit (with an internal max)
- parallelization?

## Projects
- GameOfLife.Core  
Contains the main game logic for the simulation
- GameOfLife.ConsoleRunner  
Console App to test game functionality and write output to a text file
- GameOfLife.UI  
A WPF Application for visualization and animation
- GameOfLife.Benchmarks  
Benchmark tests for performance analysis
- GameOfLife.Tests
An xUnit test project for game logic correctness

## References
- [Modified Game of Life simulator](https://playgameoflife.com/)

## TODO
- remove history option (not needed for visualizer)
- Investigate Span
- [detect cycles with hash](https://stackoverflow.com/a/45175648/8050097) & optimize
- More benchmarks
- Fix filename clash better than Thread.Sleep(1500). (Tried using FileStream & StreamWriter already and I didn't fix it)