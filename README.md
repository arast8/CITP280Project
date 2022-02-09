# Andrew Rast's CITP 280 project

## Description
This is a game in which the player moves a character across a flat, two-dimensional world.
I planned for it to be a farming game where you search for different plants to grow.
However, it is unfinished.
I created this app for Advanced C#.NET Programming class (CITP 280) at Lansing Community College.
This project was completed in five parts and had to use certain C# and .NET programming features, which are listed in the [Requirements](#requirements) section.
In addition, I was required to keep a [changelog](#changelog) and create UML class diagrams. See the "UML Class Diagram" folder for the diagrams.

## Instructions
After starting the program, click the start button to load the world named "test world" and the player named "test player".
Use the W, S, A, and D keys to move the player.
Hold left CTRL to move faster.
Hold left shift to move slower.
Scroll the mouse wheel to zoom in and out.
Left click to dig a material and put it in the inventory.
Rickt click to place a material from the inventory.
Click the hotbar to change the selected material.
Exit by closing the window.

## Requirements
### Part 1
 * Inheritance: BackgroundLayer, ForegroundLayer, and UILayer inherit from Layer.
 * Polymorphism: WorldView.Draw() calls .Draw() on each Layer in an array of Layers, and each Layer subclass implements the method differently.
### Part 2
 * Files: Images.TryGetBitmap() reads from image files by passing a filename to the Bitmap constructor.
 * Exception Handling: Images.TryGetBitmap() handles ArgumentExceptions and IOExceptions with a try-catch block.
 * Interface: IDrawable is an interface and is implemented by Player and Material.
### Part 3
 * Database: GameDatabase includes many methods for interacting with a Sqlite database. SQL statements are stored in string constants of the GameDatabase class.
 * LINQ: World.UnloadFarZones() uses LINQ query syntax to query the World.zones dictionary and loops over the results.
 * Extension Methods: RandomExtensions.NextDouble() is an extension method for System.Random. PointExtensions.DistanceTo() is an extension method for Point\<T\> (PointExtensions is in Point.cs).
### Part 4
 * Generics
	* Generic Collection: World.zones and World.zonesToLoad are both generic collections. zones is a ConcurrentDictionary\<Point\<int\>, Zone\>, and zonesToLoad is a ConcurrentQueue\<Point\<int\>\>.
	* Generic class or method: Point\<T\> is a generic struct.
 * Threading: World.StartLoadingZonesOnDBThread() and World.StartSavingOnDBThread() both create and start a new thread.
### Part 5
* Delegate: MovedEventHandler is a type of delegate. Player.movedDelegate is an instance of this delegate type and is used as a backend for the Player.Moved event.
* Event: Player.Moved is an event, and WorldView subscribes to it with WorldView.Player_Moved().
* Constructors: There are many constructors. One good example is the Player constructors, where the overloaded ones call the parameterless one using ": this()". Another example is the BackgroundLayer, ForegroundLayer, and UILayer constructors, which call the Layer constructor using ": base()".

## Changelog
### 1.0 (Part 1 Implementation Submission)
The game window is a Windows Form. It has a timer calling a method every few milliseconds.
That method tells the character to move and draws an updated image of the game world to the window.
The inheritance hierarchy requirement for part 1 is met by the Layer inheritance hierarchy.
Layer is the base class, and BackgroundLayer, ForegroundLayer, and UILayer inherit from Layer.
The polymorphism requirment is implemented in the Draw() method of the Game class.
Game.Draw() iterates over an array of Layers and calls Draw() on each of them, which causes it to update and return an image of itself.
BackgroundLayer.Draw() draws a grid of Tiles, which is the ground of the game world, ForegroundLayer.Draw() draws the character, and UILayer.Draw() draws a hunger bar.

### 2.0
* Rename Game to GameWindow.
* Rename Character to Player.
* Rename Player's Move event to Moved and the KeyboardMove method to Move.
* Rename Chunk to Zone.
* Replace WorldMap with World. World now manages the Dictionary of Zones and finding which material is at a location, as well as a other things like creating and moving the Player.
* Create WorldView, which takes some functionality from GameWindow. Specifically, it contains the list of Layers and iterates over each one's Draw() method to draw each frame. GameWindow now calls WorldView.GetImage() to trigger drawing a frame and still draws the resulting Bitmap to itself.
* Create the Images static class, which stores image resources for the game in static properties. It still gets them from the project's resource file but will load them from disk in the next version.
* Replace Tile with Material. Material has no Location property and stores instances of itself in static properties. Zones contain many references to the same Material objects.
* GameWindow's constructor calls Images.Initialize() and Material.Initialize() to initialize the game's images and Material objects.

### 2.1 (Part 2 Implementation Submission)
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
	* For this feature, create an extension method in RandomExtensions.cs for System.Random objects that returns a random double within a specified range.
* Create the RandomExtensions static class with an extension method for System.Random that returns a random double within a specified range.
* In WorldView add the ToWorldLocation() method, which converts a point on the window to a point in the World coordinate system.
	* Use this method in GameWindow to add a line to the debug text that shows the cursor location.
* Add Grass, Wheat, Stone, and StoneFlower Materials and images for them.
* Add Biome enumeration and add a Biome property to Zone
* In the Zone constructor, choose a random Biome and call the relevant method for setting it up. The Grass Biome has Grass as the ground with a chance of Wheat in PlayerLevel, and the Stone Biome has Stone as the ground with a chance of StoneFlower in PlayerLevel.
* Prevent crashing when the GameWindow is resized and the client area height is 0.

### 3.0
* Save and load Worlds to and from an SQLite database.
	* Uses the Microsoft.Data.Sqlite NuGet package.
	* World.OpenDB() opens/creates the SQLite database. It saves the connection in a private property until the World is disposed.
	* Add many methods starting with "DB" to interact with the database.
	* Put SQL commands in string constants in the World class, and use placeholders for parameters.
	* Add World.Save(), which saves the world's state to the database.
	* Make World implement IDisposable. Dispose() tries to call Save() and always closes the database connection.
	* Add a Name property to World, and require it be passed to the constructor. It is used in the database file name.
	* Handle database errors in the Main method of Program.
* Add a FormClosing event handler to GameWindow, which tries to call World.Dispose();
* Throughout the program, use Point to refer to System.Drawing.Point and PointD to refer to System.Windows.Point. The distinction is important, because System.Drawing.Point uses int for its coordinates and System.Windows.Point uses double for its coordinates.
* Add World.UnloadFarZones(), which uses LINQ to remove Zones from World.zones that are far away from the player.
	* Add MathHelper.Distance(), which calculates the distance between two points and accepts any combination of Point and PointD.
* Add a timer to World (saveTimer) that calls Save() and UnloadFarZones() every ten seconds.
* Add Unknown to the Biome enumeration, which is used in World.DBTryGetZone() if a Biome name saved to the database doesn't match any other Biome.
* Add Material.GetMaterial(), which returns the instance of Material with the given name.
* Rename Player.CurrentHunger to Player.Hunger.
* Add a default Player constructor and a Player constructor that takes name, location, and hunger parameters. The latter is used when reading a Player from the database.
* Add a 2D array of booleans to Zone (Zone.Changed), which will keep track of which locations have been changed since they were last saved to the database.
* Delay filling Zones with random Materials from the Zone constructor to a call to Zone.Init(). This allows the functionality to be skipped when loading from the database.
* Make WorldView.ToWorldLocation() return PointD instead of PointF. Also round the coordinates in GameWindow.timerTick_Tick() when writing them to lblDebug.

### 3.1 (Part 3 Implementation Submission)
* Set InterpolationMode to NearestNeighbor and PixelOffsetMode to Half on the Graphics object in WorldView.Resize(). This gets rid of the blurriness and the black grid lines.
* Draw to a BufferedGraphics object before drawing to the GameWindow (in GameWindow.DrawFrame()). This gets rid of screen-tearing and makes the animation look a lot smoother.
* Calculate FPS (Frames Per Second) in GameWindow.CalculateFPS() and show the result in the debug Label.

### 4.0
* Create the Point\<T\> struct as a generic struct that can hold X and Y coordinates of any numeric type. This replaces using System.Drawing.Point and System.Windows.Point for the most part, and should make it easy to tell what type the coordinates are.
	* Create the PointExtensions class and its overloaded DistanceTo() extension method in the same file as Point\<T\> (Point.cs). DistanceTo() was easier to implement as extension methods than a normal method, because it depends on Point's type parameter being numeric when it can be any struct.
	* Use Point\<T\> instead of System.Drawing.Point and System.Windows.Point for Player.Location, Zone.Location, Zone.Center, and World.zones.
* Remove unnecessary Math.Abs() call in RandomExtensions.NextDouble().

### 4.1 (Part 4 Implementation Submission)
* Create the GameDatabase class and move SQL strings and methods that access the database from World to GameDatabase.
* Move some database access to another thread stored in World.dbThread. Two operations take turns executing on it: loading/creating Zones, and saving.
	* World.StartLoadingZonesOnDBThread() creates and starts World.dbThread executing LoadZones(), which tries to read Zones from the database at the locations in the zonesToLoad queue, creating the Zones they do not exist.
	* World.StartSavingOnDBThread() creates and starts World.dbThread executing Save(), which saves the World's state to the database.
	* Change World.zones from a Dictionary to a ConcurrentDictionary to avoid issues with two threads accessing it at the same time.
* Add a null check in BackgroundLayer.Draw() when it gets a Material, so that it can handle them not being loaded yet. It won't draw anything, and that portion of the screen will be black.
* Load Zones before they are needed. This helps prevent seeing black areas where Zones are not loaded yet. World.QueueZonesNearPlayer() Adds Zone locations that might be needed to the zonesToLoad queue. The next execution of World.LoadZones(), which runs on another thread, will then either read them from the database or create them.
* Have WorldView and Layers draw directly to the Graphics object from GameWindow's BufferedGraphics. This eliminates another step in the rendering process and increases performance.
	* WorldView no longer implements IDrawable and does not have a CurrentImage Bitmap.
	* In GameWindow, pass the BufferedGraphics's Graphics object to WorldView when creating it and when resizing the window. In WorldView, pass the Graphics object to Layers as a parameter to Layer.Draw().
* When moving the Player diagonally, move the same distance as when moving horizontally or vertically, instead of moving a greater distance.
* Store the direction the Player is moving in Player.Heading, and use it in the get accessor for CurrentImage. It is not saved to the database yet. 
* Use the lock statement in Player.Move() and GameDatabase.UpdatePlayer() to ensure that they can't modify a Player at the same time.
* Add the ability to zoom in and out with the mouse wheel.
* Add the ability to move faster by holding left CTRL and slower by holding left shift.
* Change the interval for GameWindow.timerTick from 10ms to 1ms. In testing, this did not have a significant impact on framerate.
* Rename Zone.Changed to Zone.IsSaved.
* Rename WorldView.GetImage() to Draw().

### 5.0 (Part 5 Implementation Submission)
* Define the MovedEventHandler delegate in MovedEventHandler.cs.
* Use a delegate of type MovedEventHandler as the backend for the Player.Moved event.
	* Redo the parameters of WorldView.Player_Moved() to match MovedEventHandler.

### 5.1
* Add a player inventory and let the player gather materials from and place them into the world.
	* Draw the inventory in UILayer.Draw(), and reposition the hunger bar from where it was drawn before.
	* Pass click events from GameWindow to WorldView, where WorldView passes them to each of its Layers, in reverse order, until the event is handled.
	* In UILayer.HandleClick(), change the selected inventory slot if it was clicked on.
	* In ForegroundLayer.HandleClick(), get or set the player-level Material.
	* In BackgroundLayer.HandleClick(), get or set the ground Material.
	* Add CanBeGround and CanBePlayerLevel properties to Material to restrict where certain Materials can be placed.
	* The player's inventory is stored in the Inventories table, but is saved and loaded at the same time as the player.
* Save and load Player.Heading from the database.
* Add an image for the Unknown Material.
* Remove the Images.Total constant because it can be calculated from Successes and Errors.
* Fix Zones and Materials not being saved.
