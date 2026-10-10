# CISC 486 Project- "Aye Aye, Captain!"

## Group Members:
- Kashan Rauf
- Tracy Chan
- Ikra Mortoza
- 

## Generative AI Disclaimer
- Generative AI tools (LLMs, image generators, etc.) have not been used in any part of this game's development
- All code, dialogue, 2D assets, and 3D assets are made by humans and are either made by us or come from a licensed asset pack

## Setup Instructions 
Unity project files
1. Install Unity Hub or update an existing installation, then open it
2. If it automatically starts installing anything then pause it
3. Go to Hub Settings > Project > AI Assistant and disable it
4. Also in settings: change the paths of your downloads/installs/etc. if you want to
5. Go to Installs > Install Editor > Official Releases > Unity 6.6 (6000.6.2f1) > Install (If 6000.6.2f1 isn't there then you will have to download from Archive to install it)
6. Modules: Microsoft Visual Studio Community 2026, Windows/Mac Build Support (not the dedicated server), Documentation
7. After completing the installation go to Projects > New Project > Universal 3D > Project Name > Make sure it's local and name it CISC-486-11 > Uncheck "Use Unity CLI" > Make sure source control provider is blank and then create

Git repo check
1. Wait for it to build and then open the project's root directory in a terminal/IDE/etc.
2. Delete Assets, Packages, Project Settings, .gitignore, .vsconfig from the project if they exist
3. In the terminal at root directory, do the following commands in order
4. git init
5. git branch -M main
6. git remote add origin https://github.com/KashanRauf/CISC-486-11
7. git pull origin main (At this stage you should be pulling the files that exist on the repo)
8. Now open the README file and add your name to the group members list
9. Stage/commit/push that change (pulling/pushing is to ensure the repo is working properly)

## Licensed Assets
- (Nothing for now)

## Tutorials Used
- [Dialogue (Brackeys on YT)](https://www.youtube.com/watch?v=_nRzoTzeyxU)
- [JSON (Boxhead Productions via Wayback Machine)](https://web.archive.org/web/20200227163137/https://www.boxheadproductions.com.au/deserializing-top-level-arrays-in-json-with-unity/)
- [UI Management (Winterbolt Games on YT)](https://www.youtube.com/watch?v=rdXC2om16lo)

## Documentation

(Most information is in script comments, examples can be found in scenes)

### Scripts by Category
- Utilities: JsonHelper
- UI Management and Menus: UIManager, ManagedUI, MainMenu, SettingsMenu, PauseMenu, ConfirmationPopup, Dialogue Stuff
- Dialogue: Dialogue, DialogueTrigger, DialogueManager, ChangingPortrait, all JSONs in .\Assets\Dialogue\
- Settings: SettingsMenu
- Movement:

### Prefabs
- None yet

### Using Scripts/Prefabs
Creating dialogue:
- 

Creating ManagedUI:

Using changing portraits:
