public partial class Level1 : Node2D
{
    [Export]
    SpecialArea leftArea;
    [Export]
    Camera2D cam;
    [Export]
    InteractiveMessage rest;
    [Export]
    PlatformerPlayer player;
    [Export]
    PlatformerPlayer npc2;
    [Export]
    Area2D npc2_Dialogue;
    [Export]
    PlatformerPlayer npc3;
    [Export]
    Area2D npc3_Dialogue;
    [Export]
    Area2D nextLevelPortal;
    [Export]
    HUD hud;
    [Export]
    CanvasLayer pauseMenu;

    public int maxHearts = 3;
    public int currentHearts = 3;

    public int secreteAreaFoundCounter = 0;
    
    public int coins = 0;
    public int timeRemain = 289;

    public override void _Ready()
    {
        if (GameWorld.Instance.HasMeta("max_hearts"))
        {
            maxHearts = (int)GameWorld.Instance.GetMeta("max_hearts");
            currentHearts = maxHearts;
        }

        GameWorld.Instance.PlayBGM("res://sounds/bgm_farm_town.mp3");

        GameWorld.Instance.Connect(
            nameof(GameWorld.SignalName.CoinCollected),
            Callable.From(OnCoinCollected)
        );

        GameWorld.Instance.Connect(
            nameof(GameWorld.SignalName.SecreteAreaFound), 
            Callable.From(onSecreteAreaFound)
        );

        // Bug: https://github.com/godotengine/godot/issues/70414
        // Use Connect instead of += for now, do not use lambda also
        // GameWorld.Instance.PlayerHurt += OnPlayerHurt;
        GameWorld.Instance.Connect(
            nameof(GameWorld.SignalName.PlayerHurt), 
            Callable.From(OnPlayerHurt)
        );

        bool leftAreaFirstTime = false;

        leftArea.BodyEntered += (Node2D body) => {
            cam.LimitLeft -= 1000;
            if (!leftAreaFirstTime)
            {
                leftAreaFirstTime = true;
                player.chatBubble.PlayText("Did I forget something\nat home?");
            }
        };

        leftArea.BodyExited += (Node2D body) => {
            cam.LimitLeft += 1000;
        };

        // One-shot event handler
        // if observer and listener share the same lifespan, += is ok
        InteractiveMessage.MessageActivatedEventHandler onRest = null;
        onRest = () => {
            GD.Print("[INFO] REST");
            GameWorld.Instance.PlayBGM("", 0);
            GameWorld.Instance.ChangeScene("res://scenes/rest_scene.tscn");
            rest.MessageActivated -= onRest;
        };
        rest.MessageActivated += onRest;

        CreateTween().TweenCallback(Callable.From(() => {
            hud.SetHearts(currentHearts, maxHearts);
            hud.collectedCoins.SetNumber(coins);
            hud.remainTime.SetNumber(timeRemain);
        })).SetDelay(0.1f);

        CreateTween().SetLoops().TweenCallback(Callable.From(() => {
            timeRemain -= 1;
            hud.remainTime.SetNumber(timeRemain);
            if (timeRemain <= 0)
            {
                // What will happen if time is running out? Here's the answer...
                GameWorld.Instance.ChangeScene("res://scenes/rest_scene.tscn");
            }
        })).SetDelay(1.0f);

        nextLevelPortal.BodyEntered += (Node2D body) => {
            GameWorld.Instance.ChangeScene("res://scenes/level_2_intro.tscn");
        };

        CreateTween().TweenCallback(Callable.From(() => {
            player.chatBubble.PlayText("I have to hurry up!\nTwiggy is waiting for me.");
        })).SetDelay(2.0f);

        SetupNPC2();
        SetupNPC3();
    }

    private void OnPlayerHurt()
    {
        currentHearts -= 1;
        hud.SetHearts(currentHearts, maxHearts);
        if (currentHearts <= 0)
        {
            GameWorld.Instance.PlaySFX("res://sounds/sfx_sounds_damage3.wav");
            player.collisionShape2D.SetDeferred("disabled", true);
            CreateTween().TweenCallback(Callable.From(()=>{
                GameWorld.Instance.ChangeScene("res://scenes/game_over_scene.tscn");
            })).SetDelay(1.0f);
        }
    }

    // It's awesome that you can write the entire npc logic in one function in c#.
    private void SetupNPC2()
    {
        int npc2Moves = 0;
        string npc2State = "idle";

        npc2_Dialogue.BodyEntered += (Node2D body) => {
            npc2State = "talking";
            Tween tween = CreateTween();
            tween.TweenCallback(Callable.From(() => {
                npc2.chatBubble.PlayText("Good moring, Leia! ^_^");
            })).SetDelay(0.5f);
            tween.TweenCallback(Callable.From(() => {
                npc2.chatBubble.PlayText("Are you going on your adventure again?");
            })).SetDelay(2.0f);
            tween.TweenCallback(Callable.From(() => {
                npc2.chatBubble.PlayText("I hope to go with you.");
            })).SetDelay(2.0f);
            tween.TweenCallback(Callable.From(() => {
                npc2.chatBubble.PlayText("but I'm a little scared of the grumpy\ncreatures ahead... :(");
            })).SetDelay(2.0f);
            tween.TweenCallback(Callable.From(() => {
                npc2.chatBubble.PlayText("Wish you good luck! ^ ^");
            })).SetDelay(2.8f);
            tween.TweenCallback(Callable.From(() => {
                player.chatBubble.PlayText("Thank you, Mr. Mochi.\nHave a nice day!");
            })).SetDelay(2.0f);
            tween.TweenCallback(Callable.From(() => {
                npc2State = "idle";
            })).SetDelay(2.0f);
            // destroy the observer so it becomes a one-shot event handler
            npc2_Dialogue.QueueFree();
            npc2_Dialogue = null;
        };

        CreateTween().SetLoops().TweenCallback(Callable.From(() => {
            if (npc2State == "idle")
            {
                float rd = GD.Randf();
                if (rd < 0.1 && npc2Moves > -3)
                {
                    npc2.x_input = -1;
                    npc2Moves -= 1;
                }
                else if (rd < 0.2 && npc2Moves < 2)
                {
                    npc2.x_input = 1;
                    npc2Moves += 1;
                }
                else
                {
                    npc2.x_input = 0;
                }
            }
            else if (npc2State == "talking")
            {
                // Flip to face player
                npc2.x_input = 0;
                bool flip = (player.Position.X - npc2.Position.X) > 0 ? false : true;
                npc2.animatedSprite.FlipH = flip;
            }

        })).SetDelay(0.5f);
    }

    private void SetupNPC3()
    {
        string npc3State = "";
        npc3_Dialogue.BodyEntered += (Node2D body) => {
            npc3_Dialogue.QueueFree();
            npc3_Dialogue = null;
            Tween tween = CreateTween();
            tween.TweenCallback(Callable.From(() => {
                npc3.chatBubble.PlayText("Hi, Leia! ^_^");
            })).SetDelay(0.5f);
            tween.TweenCallback(Callable.From(() => {
                npc3.chatBubble.PlayText("I've been waiting here for so long.");
            })).SetDelay(2.0f);
            tween.TweenCallback(Callable.From(() => {
                npc3.chatBubble.PlayText("Finally you're here! Let's go!");
            })).SetDelay(2.0f);
            if (secreteAreaFoundCounter == 6)
            {
                tween.TweenCallback(Callable.From(() => {
                    npc3.chatBubble.PlayText("Wow, you found all the secret areas!");
                })).SetDelay(2.0f);
                tween.TweenCallback(Callable.From(() => {
                    npc3.emoBubble.PlayEmo(2);
                })).SetDelay(2.0f);
                tween.TweenCallback(Callable.From(() => {
                    npc3.chatBubble.PlayText("I'm so proud of you! ^_^");
                })).SetDelay(2.0f);
                tween.TweenCallback(Callable.From(() => {
                    player.chatBubble.PlayText("Well, I'm just lucky.");
                })).SetDelay(2.0f);
            }
            tween.TweenCallback(Callable.From(() => {
                // walk right
                npc3State = "walking";
                npc3.x_input = 1;
                npc3.emoBubble.PlayEmo(3);
            })).SetDelay(1.0f);
        };
        CreateTween().SetLoops().TweenCallback(Callable.From(() => {
            if (npc3State != "walking")
            {
                npc3.x_input = 0;
                bool flip = (player.Position.X - npc3.Position.X) > 0 ? false : true;
                npc3.animatedSprite.FlipH = flip;
            }
        })).SetDelay(0.5f);
    }

    private void OnCoinCollected()
    {
        coins += 1;
        hud.collectedCoins.SetNumber(coins);
    }

    private void onSecreteAreaFound()
    {
        GD.Print("Secret area found!");
        GameWorld.Instance.PlaySFX("res://sounds/sfx_sounds_fanfare1.wav");
        player.emoBubble.PlayEmo(2);
        secreteAreaFoundCounter += 1;
    }
}
