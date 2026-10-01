==============================================================
DRAGON FIGHT - Junior Unity Developer Technical Assessment
Dexhigh Services Pvt Ltd
==============================================================

Author:  [Ashutosh Ganesh Nilpatrewar]
Email:   [agn7079@gmail.com]

--------------------------------------------------------------
1. OVERVIEW
--------------------------------------------------------------
A small 2.5D top-down battle between two dragons: one controlled
by the player and one controlled by AI. The camera is an angled
overhead view that keeps both dragons in frame.

--------------------------------------------------------------
2. UNITY VERSION
--------------------------------------------------------------
Unity [YOUR VERSION, e.g. 6000.0.84f1]  (Help > About Unity)
Render pipeline: Universal Render Pipeline (URP)
Platform: Windows (x86_64)

--------------------------------------------------------------
3. CONTROLS
--------------------------------------------------------------
Movement:   WASD (keyboard movement)

Abilities:
  1  -  Fire Breath   (ranged cone, damage over time)
  2  -  Claw Attack   (close-range melee with knockback)
  3  -  Fly Attack    (take off, fly over the enemy, slam down)

UI button:
  Restart  -  on the Victory / Defeat screen

--------------------------------------------------------------
4. ABILITIES
--------------------------------------------------------------
Ability        Damage   Cooldown   Range
Fire Breath    15       4 s        10   (cone in front, ticks over ~1.2 s)
Tail Attack    20       2 s        3.5  (plus knockback)
Fly Attack     30       8 s        8    (area damage on landing)

Every ability has its own animation, visual effect and sound, and
its cooldown is shown on its icon. Values live in ScriptableObject
assets (Assets/ScriptableObjects) and can be tuned without
touching code.

--------------------------------------------------------------
5. ENEMY AI
--------------------------------------------------------------
A simple state machine (Idle -> Chase -> Attack) in EnemyAI.cs.
The AI waits briefly, chases the player, and picks an ability by
distance:
  - close range : Claw Attack
  - mid range   : Fire Breath
  - far away    : Fly Attack
It uses the same AbilityRunner component as the player, so it
obeys exactly the same cooldowns.

--------------------------------------------------------------
6. PROJECT STRUCTURE
--------------------------------------------------------------
Assets/
  Scenes/            Battle.unity (main scene)
  Scripts/           all C# scripts
  Prefabs/           dragons, VFX, damage popup
  Abilities/ 	     ability data assets(Scriptable Objects)
  Souns/             sound effects
  VFX/               particle effects
  UI/                icons and UI sprites
  Materials/	     particle materials
  Animation Controller/ animation controller for the dragon's movement

Key scripts:
  Health.cs          HP, damage, events (OnChanged / OnDamaged / OnDied)
  AbilityData.cs     ScriptableObject: damage, cooldown, range, icon, VFX, SFX
  AbilityRunner.cs   cooldowns + the three abilities (shared by player and AI)
  PlayerController.cs  WASD movement and ability keys
  EnemyAI.cs         Idle / Chase / Attack state machine
  Knockback.cs       knockback impulse after tail hits
  CameraRig.cs       keeps both dragons in view, zooms, camera shake
  HealthBarUI.cs     health bars
  AbilityIconUI.cs   ability icons with radial cooldown + timer
  DamagePopup.cs     floating damage numbers
  DragonFeedback.cs  hit flash, damage popup, hit sound, hit/death animation
  GameManager.cs     win/lose detection, result screen, restart

--------------------------------------------------------------
7. HOW TO RUN
--------------------------------------------------------------
Windows build:
  1. Download and unzip the build from the Google Drive link.
  2. Run [GAME NAME].exe
  3. Play with WASD and 1 / 2 / 3.

From source:
  1. Install the Unity version listed above.
  2. Open the project folder in Unity Hub.
  3. Open Assets/Scenes/Battle.unity and press Play.

--------------------------------------------------------------
8. ASSET SOURCES (all free)
--------------------------------------------------------------
No paid or ripped assets were used.

Dragon model and animations:
  - Dragon Firyx by TajSensei - https://tajsensei.itch.io/dragon-firyx
    (Free; includes Idle, Walk, Claw Attack, Flame Attack,
     Fly Glide, Get Hit, Die)

Arena / environment:
  - Low Poly Gladiators Arena by Leonardo Olivieri Carvalho - https://assetstore.unity.com/publishers/38147
  - Low Poly Environment - Nature Free - LOWPOLY MEDIEVAL FANTASY SERIES by Polytope Studios - https://assetstore.unity.com/packages/3d/environments/low-poly-environment-nature-free-lowpoly-medieval-fantasy-series-187052

SFX:
  - Fireball Whoosh 5 by Floraphonic - https://pixabay.com/sound-effects/search/breath%20fire/
  - Scorpion Claw Attack 4 by Yodguard - https://pixabay.com/sound-effects/search/claw%20slash/
  - WindWhoosh by Universfield - https://pixabay.com/sound-effects/search/wind%20fast/

UI icons:
  - 16x16 Free Skill Icon Pack by [Corwin] - https://lmaomonkey.itch.io/free-skill-icon-pack

Visual effects:
  - Unity Particle System

Skybox (if used):
  - Low Poly Environment - Nature Free - LOWPOLY MEDIEVAL FANTASY SERIES by Polytope Studios

--------------------------------------------------------------
9. AI USAGE NOTE
--------------------------------------------------------------
Tools used:
  Claude (Anthropic) 
What I used them for:
  - Planning the scene and script structure, including a shared
    AbilityRunner so the enemy AI follows the same cooldown rules
    as the player.
  - Drafting the ability, AI, UI and camera scripts.
  - Debugging Unity setup problems (Animator transitions, audio,
    UI wiring).

Example where the AI got something wrong:
  Claude advised me moving the Animator component onto the Dragon
  mesh child. After doing that, no animation played at all, not
  even Idle. I checked the hierarchy and found that the skeleton & flesh
  (Root) is a sibling of the mesh, and animation clips store bone
  paths relative to the object that holds the Animator. I moved the
  Animator back to the root object together with its avatar and the
  animations worked again.


--------------------------------------------------------------
10. KNOWN LIMITATIONS
--------------------------------------------------------------
  - Fly Attack uses a code-driven position lerp over the
     imported glide animation.
  - Only one enemy; no difficulty settings.
  - Enemy attacks aggressively with little delay.

--------------------------------------------------------------
11. LINKS
--------------------------------------------------------------
GitHub repository:  [LINK]
Windows build:      [GOOGLE DRIVE LINK]
Gameplay video:     [YOUTUBE UNLISTED LINK]
