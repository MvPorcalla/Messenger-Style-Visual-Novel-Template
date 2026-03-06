Build Setting:

00_Disclaimer
01_Bootstrap
02_Custscene
03_Phonescreen
04_ChatApp

---

IBubbleSpinnerCallbacks — lets BubbleSpinner talk to whatever game wraps it without knowing anything about that game. This is the correct abstraction and must exist.

BubbleSpinnerBridge — implements that interface for ChatSim specifically. Handles save/load against SaveManager, and translates engine events into GameEvents calls. This is also correct — it's the adapter layer.

GameEvents — static event hub for decoupled communication across ChatSim systems (UI, phone OS, notifications, etc.). Also correct in purpose.

---

# AI Project Instructions — Phone Chat Simulation Game (Unity)

## Important Rule

Before refactoring, reviewing, or suggesting changes to any code:

* Always ask if the **full script or relevant context** is required.
* Do **not assume, infer, or invent missing code**.
* Only review or refactor **code explicitly provided**.

---

# BubbleSpinner Dialogue Engine

**BubbleSpinner** is a standalone, UI-agnostic dialogue engine responsible for parsing and executing branching chat conversations.

Supported dialogue elements may include:

* Text messages
* Dialogue choices
* Media attachments
* Conditional logic
* Node jumps
* State-driven branching

BubbleSpinner is designed to be **fully independent of Unity UI and scene logic**.

The engine must remain **data-driven and reusable across multiple projects**.

---

# Engine Responsibilities

BubbleSpinner is responsible for:

* Parsing dialogue data
* Executing dialogue nodes
* Handling branching logic
* Managing message flow
* Triggering dialogue events

BubbleSpinner is **not responsible for**:

* Rendering UI
* Message layout
* Scroll behavior
* Phone app navigation
* Asset loading for UI presentation

---

# UI Communication Layer

All communication between the dialogue engine and the UI layer occurs through:

**Interface**

```
IBubbleSpinnerCallbacks
```

**Bridge**

```
BubbleSpinnerBridge
```

The bridge translates engine events into UI actions.

This ensures the dialogue engine remains **fully decoupled from UI implementation**.

---

# Game Architecture Overview

The project follows a **UI-first, system-driven architecture** designed for narrative-driven chat gameplay.

The core template must remain **modular and reusable**, supporting multiple story projects built on the same system.

Placeholder sections below will define project structure and implementation rules.


[Folder Structure]

```
Assets/Scripts/
├── BubbleSpinner/                           # PURE STANDALONE MODULE
│   ├── Core/
│   │   ├── IBubbleSpinnerCallbacks.cs       # 
│   │   ├── BubbleSpinnerParser.cs           # 
│   │   ├── DialogueExecutor.cs              # 
│   │   └── ConversationManager.cs           #
│   │
│   └── Data/
│       ├── MessageData.cs                   #
│       ├── ConversationAsset.cs             #
│       └── CharacterDatabase.cs             #
│
├── ChatSim/ 
    ├── Core/
    │   ├── AddressablesImageLoader.cs          # NEW - Async image loader
    │   ├── BubbleSpinnerBridge.cs        
    │   ├── GameBootstrap.cs              
    │   ├── GameEvents.cs                 
    │   ├── SaveManager.cs                 
    │   ├── SceneFlowManager.cs
    │   └── Scenename.cs
    │
    ├── Data/
    │   ├── Config/
    │   │   └── 
    │   └── SaveData.cs
    │   
    └── UI/
        ├── ChatApp/                        # Chat application
        │   ├── Components/
        │   │   ├── MessageBubble.cs        # Individual bubble
        │   │   ├── ImageMessageBubble.cs   # NEW - CG bubble with click
        │   │   └── ChoiceButton.cs         # Individual choice button
        │   │
        │   ├── Controllers/
        │   │   ├── ChatAppController.cs    # ⭐ MAIN CONTROLLER (absorbs UIManager)
        │   │   ├── ChatAutoScroller.cs     # Auto-scroll logic
        │   │   ├── ChatChoiceSpawner.cs    # Spawns choice buttons
        │   │   ├── ChatMessageSpawner.cs   # Spawns message bubbles
        │   │   └── ChatTimingController.cs # Timing & animations
        │   │
        │   ├── Panels/
        │   │   ├── ContactListPanel.cs     # Contact list
        │   │   └── ContactListItem.cs      # Individual contact button
        │   │
        │   ├── ChatAppNavButtons.cs        # Contact list
        │   └── FullscreenCGViewer.cs       # 
        │
        ├── Common/                         # Shared UI utilities
        │   ├── Components/
        │   │   ├── AutoResizeText.cs       # Used by MessageBubble
        │   │   ├── PooledObject.cs         # Pooling system
        │   │   └── PoolingManager.cs       # Pooling system
        │   │
        │   └── Screens/                    # Scene Specific Controller
        │       ├── DisclaimerScreen.cs     # First-time disclaimer
        │       └── LockScreen.cs           # Lock screen
        │
        └── HomeScreen/                        # Phone operating system
            ├── Contacts/                       # 
            │   ├── ContactsAppDetails.cs
            │   ├── ContactsAppItems.cs
            │   ├── ContactsAppPanels.cs
            │   └── ResetConfirmationDialog.cs
            │
            ├── Gallery/                       # 
            │   ├── GalleryController.cs
            │   ├── GalleryFullscreenVeiwer.cs
            │   └── GalleryThumbnailItems.cs
            │
            ├── HomeScreenController.cs          # Home screen & app launcher
            └── HomeScreenNavButtons.cs          # 
```

---

# Game Type Constraints

Platform: **Mobile**

Engine: **Unity**

Gameplay is **entirely UI-driven**.

The game does **not include**:

* Character movement
* Physics gameplay
* World exploration

Player interaction occurs through:

* Chat messages
* Dialogue choices
* Phone app navigation
* State-based story progression

---

# Dialogue Data

Dialogue content must be **data-driven**.

The dialogue engine reads structured dialogue files which define:

* Messages
* Choices
* Branching paths
* Conditional events

```
[Dialogue Data Format]

contact: Sofia

title: Start
---
System: "9:42 AM"

Sofia: "1"
Sofia: "2"
Sofia: "3"

Sofia: "Batch 1 - Message A"
Sofia: "Batch 1 - Message B"

-> ...

Sofia: "Batch 2 - Message C"
Sofia: "Batch 2 - Message D"

>> media npc type:image unlock:true path:Sofia/CG1

-> ...

Player: "..."

Sofia: "4"
Sofia: "5"
Sofia: "6"

-> ...

Player: "Yes"

Sofia: "7"
Sofia: "8"

>> choice
    -> "Ask how she's feeling"
        # Player: "You sound troubled. What's on your mind?"
        <<jump Node_Concern>>

    -> "Keep it casual"
        # Player: "Morning. Just having coffee. What's up?"
        <<jump Node_Casual>>

===

//=====================================
// CONCERN NODE
//=====================================

title: Node_Concern
---
Sofia: "Node_Concern 9"
Sofia: "10"
Sofia: "11"

>> media npc type:image unlock:true path:Sofia/CG2

-> ...

Player: "..."

Sofia: "12"
Sofia: "13"

-> ...

Player: "I'm here. Take your time."

Sofia: "14"
Sofia: "15"

>> media npc type:image unlock:true path:Sofia/CG3

-> ...

<<jump EndNode>>

===

//=====================================
// CASUAL NODE
//=====================================

title: Node_Casual
---
Sofia: "Node_Casual 9"
Sofia: "10"
Sofia: "11"

>> media npc type:image unlock:true path:Sofia/CG2

-> ...

Player: "..."

Sofia: "12"
Sofia: "13"

Player: "Fair enough. One step at a time."

Sofia: "14"
Sofia: "15"

>> media npc type:image unlock:true path:Sofia/CG3

-> ...
Player: "..."

Sofia: "16"

-> ...

<<jump EndNode>>

===

//=====================================
// END NODE
//=====================================

title: EndNode
---
Sofia: "Node_EndNode 16"
Sofia: "17"

>> media npc type:image unlock:true path:Sofia/CG4

-> ...
Player: "..."

Sofia: "18"
Sofia: "19"

System: "Later that Day."

<<jump Start_Ch2>>

```

---

# Unity Packages

The project depends on the following Unity packages:

* **TextMeshPro (TMP)** — text rendering
* **Newtonsoft.Json** — dialogue parsing
* **Addressables** — asset loading and content management

---

# Design Principles

The system must follow these core principles:

* **Engine and UI must remain decoupled**
* **Dialogue must remain data-driven**
* **Systems must remain modular and reusable**
* **Chat UI must support scalable conversation sizes**
