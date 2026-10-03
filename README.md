# CISC 486 Project- "Aye Aye, Captain!"

## Group Members:
- Kashan Rauf
- Tracy Chan
- Ikra Mortoza
- 

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
