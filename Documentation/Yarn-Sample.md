title: Start
---
<<set $name = "Player">>
<<set $trust = 0>>

System: Booting connection...

Aiko: Oh? You're finally awake.

Aiko: Can you hear me, <<$name>>?

-> Yes... where am I?
    Aiko: Good. That means the link is stable.
    <<set $trust += 1>>

    Aiko: You're inside the Relay Network. A broken one, at that.

    -> What is this place?
        Aiko: A world stitched together by failing data streams.
        Aiko: People like us... get trapped between them.
        -> continue_1

    -> Who are you?
        Aiko: Someone trying to get you out.
        Aiko: That's all you need to know for now.
        -> continue_1

-> No... I can't.
    Aiko: Hm. Weak signal then.
    Aiko: We'll stabilize it.

    <<command PlaySound("signal_distort")>>
    <<set $trust -= 1>>

    -> Try again
        Aiko: There you are.
        -> continue_1

=== continue_1 ===

Aiko: Listen carefully.

Aiko: Your choices here matter more than you think.

-> I understand.
    <<set $trust += 1>>
    Aiko: Good.

-> I don’t trust you.
    Aiko: Fair. You shouldn't.

Aiko: We don't have much time.

<<command ShowSystemMessage("Incoming signal detected")>>

Aiko: They're scanning this channel.

-> Hide me.
    Aiko: Already doing it.
    <<command FadeScreen("black")>>
    -> chapter1_mid

-> Let them find me.
    Aiko: That would be... unwise.
    <<set $trust -= 1>>
    -> chapter1_mid

=== chapter1_mid ===

System: Signal interference increasing...

Aiko: They noticed us.

Aiko: If they break in, I can only save one connection.

-> Save yourself.
    Aiko: ...Noted.
    <<set $trust -= 2>>
    -> chapter1_end

-> Save me.
    Aiko: Bold choice.
    <<set $trust += 2>>
    -> chapter1_end

-> Try to save both.
    Aiko: That's not how this works.
    Aiko: But I'll try anyway.
    <<set $trust += 1>>
    <<command PlaySound("alarm_loop")>>
    -> chapter1_end

=== chapter1_end ===

Aiko: Connection stabilizing...

Aiko: For now, you're safe.

<<if $trust >= 2>>
Aiko: I think I can trust you.
<<else>>
Aiko: I'm still watching you.
<<endif>>

Aiko: We'll talk again soon.

System: Disconnecting...

-> END