# .bub Format Design Notes — Syntax Updates

---

## 1. Choice Player Messages

Remove `# Player:` from choice blocks. Player message moves to the top of the target node.

```
// OLD
>> choice
    -> "Lonely?"
        # Player: "Lonely? Even when you're traveling together?"
        <<jump Node_Loneliness>>

// NEW
>> choice
    -> "Lonely?"
        <<jump Node_Loneliness>>
>> endchoice

title: Node_Loneliness
---
Player: "Lonely? Even when you're traveling together?"
Fern: "...Yes."
===
```

**Tradeoff:** If two choices jump to the same node but need different player messages, you'll need separate routing nodes. Rare enough to not complicate the format for.

---

## 2. Pause Point Syntax — `-> ...` replaced with `...`

Standalone `...` on its own line replaces `-> ...` as the pause point syntax.

**Two distinct pause cases:**

```
...                                // pure pacing pause — shows continue button, nothing sent on tap
Player: "I'm sure she notices"     // named message — shows continue button, sends message on tap
```

Both show the same continue button. The difference is what happens when the player taps:
- `...` — NPC continues, nothing sent
- `Player: "text"` — message sends first, then NPC continues

`Player:` lines are implicitly pause points — no separate `...` needed before them.

**Before:**
```
Fern: "Something heavy."
-> ...
Player: "I'm sure she notices"
Fern: "Maybe you're right."
```

**After:**
```
Fern: "Something heavy."
Player: "I'm sure she notices"
Fern: "Maybe you're right."
```

**Pure pacing pause:**
```
Fern: "Something heavy."
...
Fern: "She continues anyway."
```

### Conflict Check
- `-> ...` — being removed, no conflict
- `<<jump ...>>` — never uses `...`
- `>> media ...` — never uses `...`

Standalone `...` never appeared anywhere in the old format. Safe to use.

---

## 3. `---` and `===` — distinct meanings

Both were previously treated identically. They now have explicit, separate roles:

- `---` — opens node content. Only valid directly after `title:`. Parser warning if found outside that context.
- `===` — closes a node. Only valid when a node is open. Parser warning if found with no open node.

```
title: EndNode
---
Fern: "I should go now."
...
<<jump Start_Ch2>>
===
```

This makes the format self-documenting and uniform — every node has a clear open and close.

---

## 4. `>> choice` always closed with `>> endchoice`

`>> endchoice` is required to close a choice block. `===` closes the node, not the choice.

```
>> choice
    -> "Lonely?"
        <<jump Node_Loneliness>>

    -> "Unappreciated?"
        <<jump Node_Appreciation>>
>> endchoice

===
```

Parser warning if a choice block is still open when `===` is reached.

---

## 5. `>> media` — left as is

`npc` speaker and `type:` fields are kept for future-proofing. `type:audio` and player-sent media may be added later.

```
>> media npc type:image unlock:true path:Fern/CG1
```

---

## Full Node Example (updated syntax)

```
title: Start
---
System: "7:15 AM"

Fern: "Good morning."
Fern: "I hope I'm not disturbing you this early."

>> media npc type:image unlock:true path:Fern/CG1

...

Fern: "Sometimes I wonder if she'd even notice if I wasn't here."

Player: "I'm sure she notices"

Fern: "...Perhaps you're right."
Fern: "Still, there are moments when I feel..."

>> choice
    -> "Lonely?"
        <<jump Node_Loneliness>>

    -> "Unappreciated?"
        <<jump Node_Appreciation>>
>> endchoice

===

---

## Parser Impact

**`BubbleSpinnerParser.cs`**

Pause point:
- `TryParsePauseButton` — change trigger from `line == "-> ..."` to `line == "..."`
- `TryParsePauseButton` lookahead — remove entirely, `Player:` lines are implicitly pause points now
- `-> ...` inside choice block warning — remove, `-> ...` no longer exists

`---` and `===`:
- `---` — add validation: warn if found without a preceding `title:`
- `===` — add validation: warn if a choice block is still open when `===` is reached
- Both still continue parsing after warning

`>> endchoice`:
- `TryParseChoiceBlockEnd` — now required, not optional. Warn if choice block is still open at `===`

Choice player messages:
- `TryParseDialogueLine` — remove `#` speaker handling block
- `TryParseMediaCommand` — remove branch that adds to `ctx.currentChoice.playerMessages`
- `ChoiceData.playerMessages` — keep field for now, will always be empty

**`DialogueExecutor.cs`**
- `OnChoiceSelected` — remove `choice.playerMessages` emission block, simplify to call `JumpToNode` directly
- `pendingJumpNode` — can be removed, no longer needed

**Format reference doc**
- Update `-> ...` entry to `...`
- Remove `-> ...` from syntax table
- Remove `# [Speaker]: "Text"` entry
- Give `---` and `===` distinct documented roles
- Update `>> endchoice` as required, not optional
- Update all examples

**`.bub` files**
- Replace all `-> ...` with `...`
- Remove `-> ...` lines before `Player:` lines — `Player:` handles the pause itself
- Remove all `# Player:` lines from choice blocks, move player message to top of target node
- Ensure all `>> choice` blocks are closed with `>> endchoice`
- Ensure all nodes are closed with `===`