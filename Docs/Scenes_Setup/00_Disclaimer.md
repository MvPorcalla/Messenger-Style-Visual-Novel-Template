# 00_Disclaimer — Scene Setup Guide

---

## Overview

First scene the player sees. Shows a disclaimer and Terms of Service before
allowing progression. On agree, navigates to Bootstrap.

---

## Part 1 — Hierarchy
```
Canvas                 ← DisclaimerScreen.cs
├── DisclaimerPanel
│   ├── Title          (TMP)
│   ├── Content        (TMP)
│   ├── AgreeToggle    (Toggle)   ← unchecked by default
│   ├── ContinueButton (Button)   ← non-interactable by default
│   ├── ExitButton     (Button)
│   └── TOSButton      (Button)
└── TOSPanel           ← INACTIVE by default
    ├── TOSContent     (TMP)
    └── BackButton     (Button)
```

---

## Part 2 — Script Attachment

| GameObject | Script |
|---|---|
| `Canvas` | `DisclaimerScreen.cs` |

---

## Part 3 — Inspector Wiring

### DisclaimerScreen.cs
```
[Panels]
disclaimerPanel  → DisclaimerPanel (GameObject)
tosPanel         → TOSPanel (GameObject)

[UI References]
checkBoxToggle   → AgreeToggle (Toggle)
agreeButton      → ContinueButton (Button)
exitButton       → ExitButton (Button)
tosButton        → TOSButton (Button)
tosBackButton    → BackButton (inside TOSPanel)

[Debug]
skipForTesting   → ☐ false  ← NEVER true in release builds
enableDebugLogs  → ☑ true
```

---

## Part 4 — Checklist
```
☐ DisclaimerScreen.cs attached to Canvas
☐ disclaimerPanel assigned
☐ tosPanel assigned
☐ All buttons assigned
☐ TOSPanel inactive by default in scene
☐ ContinueButton → Interactable unchecked in Inspector
☐ AgreeToggle → Is On unchecked in Inspector
☐ skipForTesting = ☐ false before any build
```

---

## Common Mistakes

**Continue button stays greyed out**
`AgreeToggle` is not assigned — the script can't listen for toggle changes
to enable the button.

**TOS panel visible on start**
`TOSPanel` was left active in the scene. Set it inactive before saving.

**Game skips disclaimer in release build**
`skipForTesting` was left checked. Always verify it is false before building.