Andrew Rast's CITP 280 project

# Description:
This is a game in which the player moves a character across a flat, two-dimensional world.
I plan for it to be a farming game where you search for different plants to grow, but the only thing you can do so far is move around.
I haven't come up with a good name for my project yet, so I just called it CITP 280 Project.

# Part 1:
The game window is a Windows Form. It has a timer calling a method every few milliseconds.
That method tells the character to move and draws an updated image of the game world to the window.
The inheritance hierarchy requirement for part 1 is met by the Layer inheritance hierarchy.
Layer is the base class, and BackgroundLayer, ForegroundLayer, and UILayer inherit from Layer.
The polymorphism requirment is implemented in the Draw() method of the Game class.
Game.Draw() iterates over an array of Layers and calls Draw() on each of them, which causes it to update and return an image of itself.
BackgroundLayer.Draw() draws a grid of Tiles, which is the ground of the game world, ForegroundLayer.Draw() draws the character, and UILayer.Draw() draws a hunger bar.
