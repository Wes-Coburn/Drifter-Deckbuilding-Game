<h1 align="center">
  💎Drifter Deckbuilding Game💎
</h1>

<p align="center">
  <a href="https://drifterthegame.com/" target="_blank">💠Play Now!💠</a>
  <br><br>
  A deckbuilding adventure game in corporate dystopia.
  <br>
  Created with <a href="https://www.unity.com">Unity</a>.
</p>

<br>

<p align="center">
  <img src="https://i.imgur.com/v7UQh3km.jpg" alt="Drifter Deckbuilding Game">
  <img src="https://i.imgur.com/KBnHxI0m.jpg" alt="Drifter Deckbuilding Game">
  <img src="https://i.imgur.com/7wDsw9Vm.png" alt="Drifter Deckbuilding Game">
  <img src="https://i.imgur.com/YwqUa7zm.jpg" alt="Drifter Deckbuilding Game">
  <img src="https://i.imgur.com/onqGLa4m.jpg" alt="Drifter Deckbuilding Game">
</p>

<h2>
  👯Collaborators👯
</h2>
<p>
  Wesley Coburn (Concept and Programming) --> <a href="https://github.com/weslex555" target="_blank">GitHub</a>
  <br><br>
  Joe Rouverol (Art and Design) --> <a href="https://www.instagram.com/dragonswordart/" target="_blank">Instagram</a>
  <br><br>
  Alden Muller (Soundtrack) --> <a href="https://soundcloud.com/little_fields" target="_blank">SoundCloud</a>
</p>
                                                                                                     
<h2>
  🚩Noteable Files🚩
</h2>

<h3>
  Managers
</h3>
  
<p>  
  🔴
  <a href="Assets/Scripts/Managers/CardManager.cs">CardManager</a>
  🔴
  <a href="Assets/Scripts/Managers/CombatManager.cs">CombatManager</a>
  🔴
  <a href="Assets/Scripts/Managers/EffectManager.cs">EffectManager</a>
  🔴
</p>

> Manager classes are attached to gameObjects in *ManagerScene* and follow the **singleton** pattern:
```c#
// Singleton Pattern
  
public static GameManager Instance { get; private set; }
private void Awake()
{
  if (Instance == null)
  {
    Instance = this;
    DontDestroyOnLoad(gameObject);
  }
  else Destroy(gameObject);
}
  ```
    
<h3>
  Components
</h3>

> Display/component classes are attached to prefabs and loaded from a manager class.

<p>
  🔴
  <a href="Assets/Scripts/Displays/Card Displays/CardPageDisplay.cs">CardPageDisplay</a>
  🔴
  <a href="Assets/Scripts/Cards/Card Displays/Card Displays/UnitCardDisplay.cs">UnitCardDisplay</a>
  🔴
  <a href="Assets/Scripts/Cards/Card Classes/Card Components/CardZoom.cs">CardZoom</a>
  🔴
  <a href="Assets/Scripts/Cards/Card Classes/Card Components/DragDrop.cs">DragDrop</a>
  🔴
</p>

<h2>
📋License📋
</h2>
<p>
  <a href="LICENSE.md">ALL RIGHTS RESERVED</a>
</p>

<p align="center">
  <img src="https://i.imgur.com/IHxpjkKm.png" alt="Secret Passage Games">
</p>
