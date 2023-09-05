using SDL2;
using System;
using System.Collections.Generic;
using System.Numerics;
using static SDL2.SDL;

public class Player
{
    public List<Texture> test;

    public Vector2 Position;
    public Vector2 Velocity;
    public Vector2 Acceleration;
    public Bounds2 hitbox;
    public List<Character> characters = new List<Character>();
    
    public int CurrentCharacter = 0;
    public float health = 0;
    public float attackDamage = 10;
    public float defense = 5;
    public Controller controller;
    public float aniTimer = 0;
    public int jumpState1 = 0;
    public bool doubleJump = false;
    public float jumpTimer = 0;
    public int currentFrame = 0;
    public bool playerLaunched = false;
    public bool powerUpH = false;
    public bool powerUpD = false;
    public bool powerUpS = false;
    public float numOfHits = 0;
    public float launchTimer = 0;
    public float attackTimer = .400f;
    public PowerUp power;
    public int state = 1;
    public bool grounded = true;
    public bool attacking = false;
    TextureMirror Tmirror = TextureMirror.None;
    //idle 0
    //run 1
    //jump 2
    //attackup 3
    //attack 4
    //attackdown 5

    //player number
    public int pNum;

    //did player select characters
    public bool ready = false;

    Dictionary<int, float> timeDict = new Dictionary<int, float>();

    public Player(int mass, Controller c, int h)
    {
        test = new List<Texture>();
        test.Add(Engine.LoadTexture("grenade-right.png"));
        test.Add(Engine.LoadTexture("grenade-left.png"));
        test.Add(Engine.LoadTexture("grenade-up.png"));

        this.Position = new Vector2(540, 320);
        hitbox = new Bounds2(Position,new Vector2(40,40));
        pNum = h;
        this.controller = c;

        timeDict.Add(0, .090f);

        timeDict.Add(1, .100f);
        timeDict.Add(2, .350f);
        timeDict.Add(3, .100f);
        timeDict.Add(4, .025f);
        timeDict.Add(5, .02f);
        this.CurrentCharacter = 0;

    }

    public void render()
    {
        attackDamage = characters[CurrentCharacter].attackDamage;
        defense = characters[CurrentCharacter].shield;
        if (power != null)
        {
            if (power.powerUpType == 0)
            {
                if (health >= 50)
                {
                    health -= 50;
                }
                else
                {
                    health = 0;
                }
                power = null;

            }
            else if (power.powerUpType == 1)
            {
                attackDamage = 40;

            }
            else
            {
                defense = 100;
            }
        }
        
        
        hitbox.Position = Position;
        
        
        if (Velocity.Equals(Vector2.Zero) && !attacking)
        {
            state = 0;
        }
        if (Velocity.X < 0 && !attacking)
        {
            Tmirror = TextureMirror.Horizontal;
        }
        if (Velocity.X > 0 && !attacking)
        {
            Tmirror = TextureMirror.None;
        }
        this.Velocity += Acceleration * Engine.TimeDelta;
        this.Position += this.Velocity * Engine.TimeDelta;

        if (currentFrame >= characters[CurrentCharacter].all[state].Count)
        {
            currentFrame = 0;
            attacking = false;
        }
        float scale = characters[CurrentCharacter].all[state][currentFrame].Height / 40f;
        Console.WriteLine(scale);
        Engine.DrawTexture(characters[CurrentCharacter].all[state][currentFrame], Position, mirror: Tmirror, size: new Vector2(characters[CurrentCharacter].all[state][currentFrame].Width / scale, characters[CurrentCharacter].all[state][currentFrame].Height / scale), color: characters[CurrentCharacter].color);

        if (aniTimer > timeDict.GetValueOrDefault(state))
        {
            currentFrame++;
            aniTimer = 0;
        }
        //stop player movement if attacked and launch them
        if (playerLaunched)
        {

            if (launchTimer > .600)
            {
                playerLaunched = false;
                launchTimer = 0;
            }
            launchTimer += Engine.TimeDelta;
            SDL.SDL_GameControllerRumble(controller.connectedController, 65535, 65535, 100);
        }
        aniTimer += Engine.TimeDelta;



    }



    public void setVelocity(Vector2 v)
    {
        this.Velocity = v;
    }
    public void setAcceleration(Vector2 Acc)
    {
        this.Acceleration = Acc;
    }
    public void setPosition(Vector2 p)
    {
        this.Position = p;
    }

    public void handleJump()
    {
        if (controller.getButton("jump"))
        {
            if (jumpState1 == 0)
            {

                //makes sure jump velocity does not get too high
                if (Velocity.Y > 0)
                {


                    Velocity.Y += (Math.Abs(Velocity.Y) + 450f) * -1;

                }
                else
                {
                    Velocity.Y += -450;
                }
                jumpState1++;
            }
            else if (jumpState1 == 1 && doubleJump)
            {
                if (state == 2)
                {
                    currentFrame = 0;
                }

                if (Velocity.Y > 0)
                {


                    Velocity.Y += (Math.Abs(Velocity.Y) + 450f) * -1;

                }
                else
                {
                    Velocity.Y += -450;
                }
                jumpState1++;
            }


        }

    }

    public void nextCharacter()
    {
        this.CurrentCharacter = (this.CurrentCharacter + 1) % (characters.Count);
    }

    public void powerUp(PowerUp powerUp)
    {
        power = powerUp;
        
    }

    public void attack()
    {


        


        if (attackTimer > .400f)
        {

            //directional attacks 0 left, 1 up, 2 right, 3 down
            int k = controller.getAttack();
            if (k == 0)
            {
                Game.attacks.Add(new Attack(attackDamage, this, 0.2f, new Bounds2(new Vector2(this.Position.X - 40, this.Position.Y), new Vector2(30, 40)), new Vector2(-1, -1)));
                attackTimer = 0;
                state = 4;
                attacking = true;
                Tmirror = TextureMirror.Horizontal;
                if (power != null && power.powerUpType == 1)
                {
                    power = null;
                }
            }
            if (k == 1)
            {
                Game.attacks.Add(new Attack(attackDamage, this, 0.2f, new Bounds2(new Vector2(this.Position.X, this.Position.X - 40), new Vector2(40, 30)), new Vector2(0, -1)));
                attackTimer = 0;
                attacking = true;
                state = 3;
                if (power != null && power.powerUpType == 1)
                {
                    power = null;
                }

                if (Game.gamesPlayed > 12)
                {
                    int index = this.pNum % Game.players.Count;
                    Player target = Game.players[index];

                    Projectile nade = new Projectile(this.Position, target.Position, new Vector2(0, 0), 10, test, enemy: target, selfGuided: true);
                    Game.projectiles.Add(nade);
                    SoundInstance MSLOCK = Engine.PlaySound(Engine.LoadSound("lockon.mp3"), true, 4.8f);
                    nade.sound = MSLOCK;
                }

                if (Game.gamesPlayed > 14)
                {
                    int index = this.pNum % Game.players.Count;
                    Player target = Game.players[index];

                    Projectile ultralauncher = new Projectile(this.Position, target.Position, new Vector2(0, 0), 30, new List<Texture>() { Engine.LoadTexture("ultralauncher.png") }, enemy: target, selfGuided: false);
                    Game.projectiles.Add(ultralauncher);
                    SoundInstance MSLOCK = Engine.PlaySound(Engine.LoadSound("OMGITSTHEULTRALAUNCHER.ogg"), true, 4.8f);
                    ultralauncher.sound = MSLOCK;
                }


            }
            if (k == 2)
            {
                Game.attacks.Add(new Attack(attackDamage, this, 0.2f, new Bounds2(new Vector2(this.Position.X + 40, this.Position.Y), new Vector2(30, 40)), new Vector2(1, -1)));
                attackTimer = 0;
                state = 4;
                attacking = true;
                Tmirror = TextureMirror.None;
                if (power != null && power.powerUpType == 1)
                {
                    power = null;
                }
            }
            if (k == 3)
            {
                Game.attacks.Add(new Attack(attackDamage, this, 0.2f, new Bounds2(new Vector2(this.Position.X, this.Position.Y + 40), new Vector2(40, 30)), new Vector2(0, 1)));
                attackTimer = 0;
                attacking = true;
                state=5;
                if (power != null && power.powerUpType == 1)
                {
                    power = null;
                }
            }

        }
        attackTimer += Engine.TimeDelta;
    }






}