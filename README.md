![title_card](Assets/IconArt/SplashScreen.PNG) <!-- markdownlint-disable-line -->

https://github.com/gloliva/Shrouded-by-Darkness/assets/16783600/ac10a1aa-99e3-4416-a74c-cdbcf8829c5c <!-- markdownlint-disable-line -->

"Shrouded by Darkness" is a spine-tingling top-down 2D horror game revolves around a single, precarious element - light. Delve into the depths of fear and uncertainty as you find yourself trapped in an eerie, enigmatic house cloaked in darkness. You start with nothing and must explore carefully, collecting any weapons and light sources along the way. Do you run or fight back? Do you explore every room for useful resources or do you rush the exit? Do you use your precious sources of light to illuminate the path, drawing enemies towards you, or do you embrace the darkness? The choice is yours, if you can survive...

## How to Install and Run

### *Download Instructions*

Download the `ShroudedByDarkness` application from either:

1. [Shrouded by Darkness Itch.io](https://gloliva.itch.io/shrouded-by-darkness) page
2. [Shrouded by Darkness Github Releases](https://github.com/gloliva/Shrouded-by-Darkness/releases/tag/release-v1.0.0) page

### *Install Instructions*

#### Windows

Extract the zip file contents to a location of your choice. Run the game by double-clicking the **Shrouded by Darkness.exe** program.

#### Mac OS

Extract the zip file contents to a location of your choice. Due to MacOS security restrictions, you must open the `Terminal`  application and run the following commands:

```bash
sudo chmod -R 755 <path-to-app>/ShroudedByDarkness.app 
sudo xattr -dr com.apple.quarantine <path-to-app>/ShroudedByDarkness.app
```

You should replace `<path-to-app>` with the correct path to ShroudedByDarkness.app. For example, if you extracted the ShroudedByDarkness application to your Downloads folder, you would run:

```bash
sudo chmod -R 755 ~/Downloads/ShroudedByDarkness.app
sudo xattr -dr com.apple.quarantine ~/Downloads/ShroudedByDarkness.app
```

You can now double-click on the **ShroudedByDarkness** app to run the game.

## How to Play

To survive the horrors that await, you must:

1. **Employ Your Light Sources**: Finding and using sources of light is paramount to your ability to survive and find your escape. But be warned, the larger a light source, the further an enemy is able to detect you.
2. **Explore Diligently**: Search every corner of the mansion for doors, keys, weapons, and health items that will aid in your escape.
3. **Manage Your Resources**: Use lights and ammo strategically; if you run out of items you will have to brave the perils of the mansion without them.
4. **Unlock Blocked Paths**: Many of the doors you encounter will be locked, you must search for keys in order to progress. Some doors will contain valuable resources, whereas other doors are required for your escape.

## Controls

### Movement

`Move Mouse` - Change Player Direction  
`W`, `A`, `S`, `D` - Move Player Forward, Left, Back, Right  
`Shift` - Sprint

### Items

`Q` - Switch Light Source  
`F` - Use Light Source  
`E` - Interact (Open Door / Pickup Item)

### Weapons

`R` - Switch Weapon  
`Left Mouse Click` - Attack (with weapon equipped)

## Game Details

For an in-depth description of the game mechanics, visual style, and features, the following design document:  
**[Design Document](https://docs.google.com/document/d/1GWseQPh5oIwnZvmWEkxnCnkYbbK7BjnUx0QiIuBbIlY/edit?usp=sharing)**

For a complete overview of the game map, view the following document (*Warning!* Contains game spoilers):  
**[Map Overview](https://docs.google.com/drawings/d/1H5m1a3PK2eKtwl85023IiG9xRDuMnJB72C35zGUVmoM/edit?usp=sharing)**

### Game Files

Game programming scripts can be found in the [Assets/Scripts](https://github.com/gloliva/Shrouded-by-Darkness/tree/master/Assets/Scripts) directory. These scripts contain logic for certain game mechanics, such as healing, enemy AI, and how items work.

Art assets can be found in the [Assets/Sprites](https://github.com/gloliva/Shrouded-by-Darkness/tree/master/Assets/Sprites), [Assets/Animations](https://github.com/gloliva/Shrouded-by-Darkness/tree/master/Assets/Animations), [Assets/UI](https://github.com/gloliva/Shrouded-by-Darkness/tree/master/Assets/UI), and [Assets/IconArt](https://github.com/gloliva/Shrouded-by-Darkness/tree/master/Assets/IconArt) directories. These folders contain the images and animation files used for the game's visual style.

Music and Sound FX files can be found in the [Assets/Audio](https://github.com/gloliva/Shrouded-by-Darkness/tree/master/Assets/Audio) directory.

Additional builds can be found in the [Builds](https://github.com/gloliva/Shrouded-by-Darkness/tree/master/Builds) directory. If you have a Windows machine, you can play these builds by double-clicking the `Shrouded by Darkness.exe` executable (you can view them on Mac OS by loading this project into Unity). The `MVP`, `Alpha`, and `Beta` builds represent early stages of the game and are **NOT** the final product. The final game (and the one that is downloaded from itch.io and the releases page) is the `Release` build.

## Credits

Shrouded By Darkness was the Final project for the Game Development Decal at UC Berkeley.  
Made in Unity.

**Raymond Ly** - Art and Animation  
**Will He** - Programming  
**Gregg Oliva** - Programming, Music, Level Design
