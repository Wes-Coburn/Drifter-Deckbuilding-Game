@ECHO PUSH THE CURRENT BUILD?
@ECHO OFF
PAUSE
butler push "C:\Users\wgCob\OneDrive\Documents\Programming\Unity\Drifter (Unity 2D)\Build" weslex555/drifter-deckbuilding-game:windows-universal --userversion BETA_1.0
@ECHO BUILD PUSHED SUCCESSFULLY!
PAUSE
butler status weslex555/drifter-deckbuilding-game:windows-universal
PAUSE