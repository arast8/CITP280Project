# Andrew Rast's CITP 280 project

## Description:
This is a game in which the player moves a character across a flat, two-dimensional world.
I plan for it to be a farming game where you search for different plants to grow, but the only thing you can do so far is move around.
I haven't come up with a good name for my project yet, so I just called it CITP 280 Project.

## Changelog:
### Part 1 Implementation Submission:
The game window is a Windows Form. It has a timer calling a method every few milliseconds.
That method tells the character to move and draws an updated image of the game world to the window.
The inheritance hierarchy requirement for part 1 is met by the Layer inheritance hierarchy.
Layer is the base class, and BackgroundLayer, ForegroundLayer, and UILayer inherit from Layer.
The polymorphism requirment is implemented in the Draw() method of the Game class.
Game.Draw() iterates over an array of Layers and calls Draw() on each of them, which causes it to update and return an image of itself.
BackgroundLayer.Draw() draws a grid of Tiles, which is the ground of the game world, ForegroundLayer.Draw() draws the character, and UILayer.Draw() draws a hunger bar.

### Part 2 UML Diagram Submission:
* Rename Game to GameWindow.
* Rename Character to Player.
* Rename Player's Move event to Moved and the KeyboardMove method to Move.
* Rename Chunk to Zone.
* Replace WorldMap with World. World now manages the Dictionary of Zones and finding which material is at a location, as well as a other things like creating and moving the Player.
* Create WorldView, which takes some functionality from GameWindow. Specifically, it contains the list of Layers and iterates over each one's Draw() method to draw each frame. GameWindow now calls WorldView.GetImage() to trigger drawing a frame and still draws the resulting Bitmap to itself.
* Create the Images static class, which stores image resources for the game in static properties. It still gets them from the project's resource file but will load them from disk in the next version.
* Replace Tile with Material. Material has no Location property and stores instances of itself in static properties. Zones contain many references to the same Material objects.
* GameWindow's constructor calls Images.Initialize() and Material.Initialize() to initialize the game's images and Material objects.

### Part 2 Implementation Submission:
* Read image files in Images.Initialize() instead of using project's resource file.
	* The file reading is actually implemented in Images.TryGetBitmap(), which Initialize() calls.
	* Remove images from the project's resource file.
* Add a menu Panel to the GameWindow with a label that shows how many images succeeded or failed to load a start Button. The start Button causes the game world to be shown;
* Layers no longer draw on their own Bitmap image, and instead draw directly on the WorldView's image. This was meant to increase performance by eliminating a step to drawing a frame.
	* Layers no longer implement IDrawable.
	* Change Layer and subclass constructors to take a WorldView instead of multiple fields/properties of the WorldView, and reference those properties in the Draw() method.
	* Remove the Resize method from Layer and subclasses.
	* Move the Player_Moved event handler method from BackgroundLayer to WorldView.
	* Move BackgroundLayer.visibleArea to WorldView.visibleArea.
	* Replace BackgroundLayer.tileSize and ForegroundLayer.playerSize with WorldView.TileSize.
* Add another array of Materials to Zone called PlayerLevel. These are drawn by ForegroundLayer and appear above the ground level.
	* In ForegroundLayer, draw the Player halfway through drawing PlayerLevel Materials. Drawing some behind and some in front of the Player gives the illusion of depth.
	* Rename World.GetMaterial() to GetGroundMaterial() and add a GetPlayerLevelMaterial() method with similar function.
* In WorldView.GetImage(), reset the image to black before starting drawing. For now, this results in black lines showing on the ground, because all images are being blurred when they are drawn.
* Double Player move speed.
* Give Player a random initial position close to the origin.
* Create the RandomExtensions static class with an extension method for System.Random that returns a random double within a specified range.
* In WorldView add the ToWorldLocation() method, which converts a point on the window to a point in the World coordinate system.
	* Use this method in GameWindow to add a line to the debug text that shows the cursor location.
* Add Grass, Wheat, Stone, and StoneFlower Materials and images for them.
* Add Biome enumeration and add a Biome property to Zone
* In the Zone constructor, choose a random Biome and call the relevant method for setting it up. The Grass Biome has Grass as the ground with a chance of Wheat in PlayerLevel, and the Stone Biome has Stone as the ground with a chance of StoneFlower in PlayerLevel.
* Prevent crashing when the GameWindow is resized and the client area height is 0.
