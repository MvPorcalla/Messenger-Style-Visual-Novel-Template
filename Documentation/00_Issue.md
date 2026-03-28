

Cross chapter jump issue

Cross-chapter jump does not correctly resolve the target node in the destination chapter.
Instead of jumping to the title: entry point of the target chapter file, the system attempts to resolve the node using the current chapter context, causing node lookup failure.

---

[DialogueExecutor] Node 'Start' not found in chapter 'Start_Ch2'

---

Steps to Reproduce

Run Chapter 1 dialogue
Reach final node containing:
<<jump Start_Ch2>>
System attempts to transition to Chapter 2
Chapter 2 loads, but node resolution fails
Error appears and dialogue stops

---

**Suggestion for `.bub` syntax**

* make a new chapter `jump title` syntax for **chapter IDs**.
* For cross-chapter jumping, introduce a new syntax instead of `<jump Node_name>`.
* Keep `<jump Node_name>` strictly for **node jumps within the same chapter**.

e.g:

# chapter 1 - file 1 .bub

contact: Sofia

**chapter IDs** <- where chapter jump e.g. (chapter: Ch1)

// after that it reads the chapter downward starting to the first title node

title: Start
---
System: "9:42 AM"
Sofia: "Batch 1 - Message A"
Sofia: "Batch 1 - Message B"

<<new syntax for corss jumping chapter **chapter IDs**>> e.g. ( <<jump chapter:Ch2>> )

---

# chapter 2 - file 2 .bub

**chapter IDs** <- where chapter jump e.g. (chapter: Ch2)

title: Start
---
System: "9:42 AM"
Sofia: "Batch 2 - Message C"
Sofia: "Batch 2 - Message D"


what do you think of this?

reasoning:

Node jumps are restricted to the current file to keep control flow local and predictable.
Cross-file jumps are handled separately and are only used to move between chapters/files.
This separation enforces clear structural boundaries in the story flow.


// ===========================================================

What limit makes sense for your format:

Based on your planned structure:

indent 0 — node level
indent 1 — choice option (->)
indent 2 — jump / <<if>>
indent 3 — <<else>> body
indent 4 — nested <<if>> inside <<else>>
4 is the right limit. It covers everything you've planned including deeply nested conditionals, and anything beyond 4 is almost certainly an author mistake.

Do you want to add the limit now?

--- TODO: Delete soon

contact: Fern

title: Start
---
System: "7:15 AM"

Fern: "Good morning."
Fern: "I hope I'm not disturbing you this early."
Fern: "Frieren-sama is still asleep, and I found myself with some time before I need to prepare breakfast."

>> media npc type:image unlock:true path:Fern/CG1

...

Fern: "I've been traveling with Frieren-sama for years now."
Fern: "Watching her sleep in until noon, forgetting to eat, losing track of time..."
Fern: "Sometimes I wonder if she'd even notice if I wasn't here."

Player: "I'm sure she notices"

Fern: "...Perhaps you're right."
Fern: "She just has her own way of showing it."
Fern: "Still, there are moments when I feel..."

>> choice
	-> "Lonely?"                                                // shown on choice button
        Player: "Lonely? Even when you're traveling together?"  // shown on ChatBubbles base on who is the speaker
		<<jump Node_Loneliness>>                                // what node to jump on

	-> "Unappreciated?"
		<<jump Node_Appreciation>>
>> endchoice

---

	-> "Lonely?"                                                // shown on choice button
        Player: "Lonely? Even when you're traveling together?"  // shown on ChatBubbles base on who is the speaker
        Npc: blah blah                                          // can also support npc chatbubble
		<<jump Node_Loneliness>>                                // what node to jump on

> choice option can have or dont have a dialogue still works fine

ask me clarifying question