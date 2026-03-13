# 01_Bootstrap — Scene Setup Guide

---

## Overview

Bootstrap is a persistent scene loaded once at game start and never unloaded.
It holds all core managers that must survive across scene transitions.

> No Camera. No Canvas. No EventSystem.

---

## Part 1 — Hierarchy
```
GameBootstrap          ← GameBootstrap.cs
├── SaveManager        ← SaveManager.cs
├── SceneFlowManager   ← SceneFlowManager.cs
└── ConversationManager ← ConversationManager.cs
```

---

## Part 2 — Script Attachment

| GameObject | Script |
|---|---|
| `GameBootstrap` | `GameBootstrap.cs` |
| `SaveManager` | `SaveManager.cs` |
| `SceneFlowManager` | `SceneFlowManager.cs` |
| `ConversationManager` | `ConversationManager.cs` |

---

## Part 3 — Inspector Wiring

### GameBootstrap.cs
```
[Core Managers]
saveManager          → SaveManager (child GameObject)
sceneFlowManager     → SceneFlowManager (child GameObject)

[Game Systems]
conversationManager  → ConversationManager (child GameObject)

[Debug]
enableDebugLogs      → ☑ true
```

### SaveManager.cs
```
[Debug]
enableDebugLogs  → ☑ true

[Save Settings]
prettyPrintJson  → ☑ true  (disable for release builds)
```

### SceneFlowManager.cs
No serialized fields.

### ConversationManager.cs
No serialized fields.

---

## Part 4 — Checklist
```
GameBootstrap
☐ saveManager assigned
☐ sceneFlowManager assigned
☐ conversationManager assigned

SaveManager
☐ prettyPrintJson set appropriately for build type
```