# John Conway's Game of Life Simulation
A C# visualization of John Conway's Game of Life

- start with logic, then potentially add visualization
- generation count (run until it halts or not, np hard)
- classes
    - Cell (alive or dead)
    - Board
    - Rules enum?
- Implementation
    - 2d bit array
    - check neighbors

## Rules
At each generation
- If cell has **< 2 neighbors**, it dies
- If it has **exactly 2 neighbors** it stays alive
- If it has **exactly 3 neighbors**, it will be born

Glider = same pattern, shifted

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

## TODO
- remove history option (not needed for visualizer)
