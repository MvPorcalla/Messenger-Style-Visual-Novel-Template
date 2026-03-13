# Scene Overview

## Build Settings

File → Build Settings — add scenes in this exact order:

| Index | Scene |
|---|---|
| 0 | `00_Disclaimer` |
| 1 | `01_Bootstrap` |
| 2 | `02_Lockscreen` |
| 3 | `03_PhoneScreen` |
| 4 | `04_ChatApp` |

---

## Per-Scene Summary

### 00_Disclaimer
Attach `DisclaimerScreen.cs` to Canvas.
Shows disclaimer + TOS before first boot. Navigates to Bootstrap on agree.

### 01_Bootstrap
No Camera, Canvas, or EventSystem.
Attach `GameBootstrap.cs`, `SaveManager.cs`, `SceneFlowManager.cs`, `ConversationManager.cs`.
Persistent scene — loaded once, never unloaded.

### 02_Lockscreen
Attach `LockScreen.cs` to LockScreen.
Shows time, date, and unlock button. Navigates to PhoneScreen on unlock.

### 03_PhoneScreen
Home screen and in-scene apps (Gallery, Contacts).
→ See `03_PhoneScreen_Setup.md` for full hierarchy, prefabs, and wiring.

### 04_ChatApp
Chat messaging interface and contact list.
→ See `04_ChatApp_Setup.md` for full hierarchy, prefabs, and wiring.