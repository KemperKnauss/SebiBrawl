using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.VisualBasic;
using SDL2;


class Game
{
    public static readonly string Title = "Sebi Brawl";
    public static readonly Vector2 Resolution = new Vector2(1280, 720);

    String[] mapData = System.IO.File.ReadAllLines("./Assets/mapData.txt");
    String[] characterData = System.IO.File.ReadAllLines("./Assets/characterData.txt");
    Texture controllerBinds = Engine.LoadTexture("ControllerBinds.jpg");
    Texture keyboardBinds = Engine.LoadTexture("KeyboardBinds.jpg");
    Texture controllerIcon = Engine.LoadTexture("controller-icon.png");
    Texture startScreenTexture = Engine.LoadTexture("introscreen.png");
    Texture endScreen = Engine.LoadTexture("endscreen.jpg");

    Texture explosion = Engine.LoadTexture("explosion.png");
    Sound exsound = Engine.LoadSound("explosion.ogg");
    
    public static List<Player> players = new List<Player>();
    public static List<Projectile> projectiles = new List<Projectile>();
    List<Player> dead = new List<Player>();
    List<Map> availableMaps;
    List<PowerUp> powerUps = new List<PowerUp>();
    public static List<Attack> attacks = new List<Attack>();
    Character[] availableCharacters;
    Player player1;
    Bounds2[] characterSelectionBounds = new Bounds2[8];
    Bounds2 controlsBounds = new Bounds2(new Vector2(926, 570), new Vector2(100, 100));
    public Map game;
    List<Controller> connectedControllers = new List<Controller>();
    Controller setUp;

    static int screen = 0;
    public bool gameStarted = false;
    int numCharacters;
    int numMaps;
    int c = 0;
    int r = 0;
    int tiles;
    int pages;
    int currentPage = 1;
    int startScreenButtonIndex = 1;
    bool switchImage = false;
    float preMatchTimer = 3f;
    string availableC = "";
    float powerClock = 0;

    public static int gamesPlayed;

    MapMaker mapmaker;

    Bounds2 mapMakerButton = new Bounds2(new Vector2(180, 570), new Vector2(150, 100));
    Bounds2 next = new Bounds2(new Vector2(1120, 410), new Vector2(100, 100));
    Bounds2 back = new Bounds2(new Vector2(1010, 410), new Vector2(100, 100));
    Bounds2 exit = new Bounds2(new Vector2(1140, 600), new Vector2(100, 50));
    Bounds2 exit2 = new Bounds2(new Vector2(1190, 650), new Vector2(80, 50));
    Bounds2 newMap = new Bounds2(new Vector2(990, 600), new Vector2(110, 50));
    Bounds2 startButtonBounds = new Bounds2(new Vector2(362, 572), new Vector2(536, 94));
    Bounds2 replayButton = new Bounds2(new Vector2(100, 240), new Vector2(305, 305));

    Vector2 winStatement = new Vector2(800, 45);

    Font font = Engine.LoadFont("font.ttf", 50);
    Font sfont = Engine.LoadFont("font.ttf", 30);
    Font lfont = Engine.LoadFont("font.ttf", 90);
    
    Color[] colors = { Color.White, Color.Red, Color.Blue, Color.Lime, Color.Yellow, Color.Orange, Color.Purple, Color.LightPink };
    Color[] characterColors = { Color.White, Color.Red, Color.Purple, Color.Lime, Color.DarkGreen, Color.Orange, Color.Blue, Color.LightPink };
    
    public Game()
    {
        
        for (int i = 0; i < 4; i++)
        {
            characterSelectionBounds[i] = new Bounds2(new Vector2(i * 320 + 40, 5), new Vector2(230, 230));
            characterSelectionBounds[i + 4] = new Bounds2(new Vector2(i * 320 + 40, 245), new Vector2(230, 230));
        }
        String[] temp = System.IO.File.ReadAllLines("./Assets/gameData.txt");
        gamesPlayed = int.Parse(temp[0]);

        temp = System.IO.File.ReadAllLines("./Assets/characterData.txt");
        if (gamesPlayed <= 12)
        {
            temp[0] = "" + (gamesPlayed / 3 + 4);
        }
        else
        {
            temp[0] = "" + 8;
        }
        
        System.IO.File.WriteAllLines("./Assets/characterData.txt",temp);

        initControllers();
        loadMapsCharacters();

        numMaps = availableMaps.Count;
        pages = (numMaps - 1) / 15 + 1;



    }



    void initController(Controller temp)
    {


        temp.color = players.Count%8;
        temp.taken = true;
        temp.pNum = players.Count + 1;
        Player player = new Player(1, temp, temp.pNum);
        players.Add(player);
        temp.cursor.Position.X = 1260 - (640 / players.Count);
        temp.cursor.Position.Y = 580;
        temp.player = player;

    }
    
    public void startScreen()
    {
        
        Controller temp = connectedControllers.Find(a => a.getSelect());
        Engine.DrawTexture(startScreenTexture, Vector2.Zero);
        Engine.DrawRectEmpty(mapMakerButton, Color.Black);
        Engine.DrawString("Map Maker", mapMakerButton.Position + new Vector2(10, 20), Color.White, font);
        Engine.DrawTexture(controllerIcon, new Vector2(926, 572), size: new Vector2(100, 100));
        Engine.DrawRectEmpty(controlsBounds, Color.Black);
        if (startScreenButtonIndex == 0)
        {
            Engine.DrawRectEmpty(mapMakerButton, Color.Lime);
            if (temp != null)
            {
                
                initController(temp);
                screen = 2;
            }
        }

        else if (startScreenButtonIndex == 1)
        {
            Engine.DrawRectEmpty(startButtonBounds, Color.Lime);
            if (temp != null)
            {

                players.Clear();
                dead.Clear();
                initControllers();
                gameStarted = true;
                screen++;

            }
        }
        else
        {
            if (temp != null)
            {
                screen = 7;
            }
            Engine.DrawRectEmpty(controlsBounds, Color.Lime);
        }
        temp = connectedControllers.Find(a => a.getMovement() > 0 && !a.movementFlag);
        if (temp != null && startScreenButtonIndex < 2)
        {
            temp.movementFlag = true;
            startScreenButtonIndex++;
        }
        temp = connectedControllers.Find(a => a.getMovement() < 0 && !a.movementFlag);
        if (temp != null && startScreenButtonIndex > 0)
        {
            temp.movementFlag = true;
            startScreenButtonIndex--;
        }
        if (startButtonBounds.Contains(Engine.MousePosition))
        {
            startScreenButtonIndex = 1;
        }
        else if (controlsBounds.Contains(Engine.MousePosition))
        {
            startScreenButtonIndex = 2;
        }
        else if (mapMakerButton.Contains(Engine.MousePosition))
        {
            startScreenButtonIndex = 0;
        }
        //printMousePos();

        





    }

    public void startGame()
    {

        if (connectedControllers.Exists(a => a.getButton("pause")))
        {
            screen = 6;
        }
        game.draw();
        
        if (powerClock > 10)
        {
            powerUps.Add(new PowerUp());
            powerClock = 0;
        }
        foreach(PowerUp power in powerUps)
        {
            power.render();
        }
        foreach(Projectile projectile in projectiles)
        {
            if (projectile.exploded)
            {
                Random rand = new Random();

                Engine.DrawTexture(explosion, projectile.Position - new Vector2(300,300), size: new Vector2(700, 700));
                projectile.Enemy.Position += new Vector2(0, -10);
                projectile.Enemy.setVelocity(new Vector2(rand.Next(300) - 150, -50));
                projectile.Enemy.playerLaunched = true;
                projectile.Enemy.launchTimer = 0;
                projectile.deathTimer++;
                projectile.Enemy.health += 40;
                Engine.StopSound(projectile.sound);
                continue;
            }
            projectile.recalculate();
            projectile.render();
            projectile.updateEnemy();
        }
        foreach (Player player in players)
        {


            foreach (PowerUp power in powerUps)
            {
                if (player.hitbox.Overlaps(power.hitbox))
                {
                    player.powerUp(power);
                    power.disabled = true;
                }
            }
            if (!player.playerLaunched)
            {
                //Console.WriteLine(1);
                if (!player.attacking)
                {
                    player.state = 1;

                }

                player.setVelocity(new Vector2(350 * player.controller.getMovement(), player.Velocity.Y));
            }
            
            if ((((int)player.Position.Y) > 720 || ((int)player.Position.X) > 1280 || player.Position.X < -40 || player.Position.Y < -40))
            {
                if (players.Count < 3)
                {
                    String[] temp = { "" + (gamesPlayed + 1) };
                    
                    System.IO.File.WriteAllLines("./Assets/gameData.txt",temp);
                    temp = System.IO.File.ReadAllLines("./Assets/gameData.txt");
                    gamesPlayed = int.Parse(temp[0]);

                    temp = System.IO.File.ReadAllLines("./Assets/characterData.txt");
                    if (gamesPlayed <= 12)
                    {
                        temp[0] = "" + (gamesPlayed / 3 + 4);
                    }
                    else
                    {
                        temp[0] = "" + 8;
                    }

                    System.IO.File.WriteAllLines("./Assets/characterData.txt", temp);
                    screen++;
                    Engine.Fullscreen = false;
                    SDL.SDL_SetWindowFullscreen(Engine.Window, Engine.Fullscreen ? (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP : 0);
                }
                dead.Add(player);
            }


            player.handleJump();
            game.handleCollision(player);
            game.gravity(player);
            player.attack();
            player.render();
            Engine.DrawString("p" + player.pNum + " " + player.health + "%", player.Position + new Vector2(0, -23), colors[player.pNum], sfont);
            if (player.controller.getButton("switch"))
            {
                player.nextCharacter();
            }
            foreach (Attack a in attacks)
            {

                if (!a.player.Equals(player) && !(a.bounds.Position.X > player.Position.X + 40 || player.Position.X > a.bounds.Position.X + 40) && !(a.bounds.Position.Y > player.Position.Y + 40 || player.Position.Y > a.bounds.Position.Y + 40))
                {
                    if (a.damage - player.defense > 0)
                    {
                        player.health += a.damage - player.defense;
                    }
                    
                    player.Velocity = Vector2.Zero;


                    player.Velocity = new Vector2(3.2f * player.health * a.direction.X, 3.2f * player.health * a.direction.Y);
                    player.playerLaunched = true;
                    player.launchTimer = 0;


                    a.active = false;
                    if (player.power != null && player.power.powerUpType == 2)
                    {
                        player.power = null;
                    }
                }
                a.expire();
                //Engine.DrawRectSolid(a.bounds, Color.Black);


            }

            attacks.RemoveAll(a => !a.active);
            powerUps.RemoveAll(b => b.disabled);
        }

        foreach (Player d in dead)
        {
            players.Remove(d);
        }
        powerClock += Engine.TimeDelta;

        for (int i = projectiles.Count - 1; i >= 0; i--)
        {
            if (projectiles[i].exploded && projectiles[i].deathTimer > 30)
            {
                projectiles.RemoveAt(i);
            }
        }
    }

    public void playerSelection()
    {
        Console.WriteLine(players.Count);
        for (int j = 0; j < availableCharacters.Length; j++)
        {
            Engine.DrawRectEmpty(characterSelectionBounds[j], characterColors[j]);
            
            Engine.DrawTexture(availableCharacters[j].idle[0], characterSelectionBounds[j].Position + new Vector2(30, 25), size: new Vector2(availableCharacters[j].idle[0].Width / 2.5f, availableCharacters[j].idle[0].Height / 2.5f),color: characterColors[j]);
        }

        if (Engine.GetKeyDown(Key.R))
        {

            initControllers();

            players.Clear();
        }
        Engine.DrawLine(new Vector2(0, 480), new Vector2(1280, 480), Color.White);


        for (int i = 1; i < players.Count; i++)
        {
            Engine.DrawLine(new Vector2(1280 / players.Count * i, 480), new Vector2(1280 / players.Count * i, 720), Color.White);
            Engine.DrawString("p" + (i + 1), new Vector2(1280 / players.Count * i + 10, 480), colors[i%8], font, alignment: TextAlignment.Left);

        }
        if (players.Count > 0)
        {

            Engine.DrawString("p1", new Vector2(10, 480), colors[0], font, alignment: TextAlignment.Left);
        }
        else
        {
            Engine.DrawString("available controllers:" + availableC, new Vector2(540, 580), Color.GhostWhite, font, TextAlignment.Center);
        }


        foreach (Controller c in connectedControllers)
        {
            if (c.getButton("connect") && !c.taken)
            {
                c.color = players.Count%8;
                c.taken = true;

                if (c.keyboard == 0)
                {


                    SDL.SDL_GameControllerRumble(c.connectedController, 65535, 65535, 400);


                }
                c.pNum = players.Count + 1;
                Player temp = new Player(1, c, c.pNum);
                players.Add(temp);

                c.cursor.Position.X = 1260 - (640 / players.Count);
                c.cursor.Position.Y = 580;



                c.player = temp;

            }
            if (c.taken)
            {


                c.getCursor(8);

                Engine.DrawString("p" + c.pNum, new Vector2(c.cursor.Position.X + 10, c.cursor.Position.Y - 10), colors[c.color], font, alignment: TextAlignment.Left);
                for (int h = 0; h < c.player.characters.Count; h++)
                {
                    float scale = c.player.characters[h].idle[0].Height / 40f;
                    float location = (((1280 / players.Count)) * c.pNum- 640 / players.Count);
                    Engine.DrawTexture(c.player.characters[h].idle[0],new Vector2(location,540+40*h),size: new Vector2(c.player.characters[h].idle[0].Width/scale, c.player.characters[h].idle[0].Height/scale),color: c.player.characters[h].color);
                }
                if (c.getSelect() && !c.player.ready)
                {


                    for (int j = 0; j < 8; j++)
                    {


                        if (characterSelectionBounds[j].Overlaps(c.cursor))
                        {
                            
                            c.player.characters.Add(availableCharacters[j]);
                            if (c.player.characters.Count > 1)
                            {
                                c.player.ready = true;
                            }
                        }
                    }
                }

            }

        }
        if (!players.Exists(a => !a.ready) && players.Count > 1)
        {
            
            player1 = players[0];
            screen++;
        }

        IntPtr o = SDL.SDL_GameControllerOpen(Controller.controllers);
        if (SDL.SDL_GameControllerGetAttached(o) > 0)
        {
            Controller temp;
            temp = new Controller(o, 0);
            connectedControllers.Add(temp);
            availableC += " " + temp.getType();
            Controller.controllers++;
        }
        Engine.DrawString("choose 2", new Vector2(640, 0), Color.WhiteSmoke, sfont, alignment: TextAlignment.Center);
    
}
    public void endGame()
    {

        foreach (Projectile projectile in projectiles)
        {
            Engine.StopSound(projectile.sound);
        }
        
        if (connectedControllers.Exists(a => a.getButton("connect")) || (Engine.GetMouseButtonDown(MouseButton.Left) && replayButton.Contains(Engine.MousePosition)))
        {

            
            

            players.Clear();
                
                
            dead.Clear();
            game.gameStarted = false;
            gameStarted = false;
            screen = 0;
            availableMaps.Clear();
            initControllers();
            loadMaps();
            gamesPlayed++;
            String[] temp = System.IO.File.ReadAllLines("./Assets/characterData.txt");
            if (gamesPlayed <= 12)
            {
                temp[0] = "" + (gamesPlayed / 3 + 4);
            }
            else
            {
                temp[0] = "" + 8;
            }

            System.IO.File.WriteAllLines("./Assets/characterData.txt", temp);


        }
        if (Engine.GetKeyHeld(Key.E))
        {
            Environment.Exit(1);
        }
        Engine.DrawTexture(endScreen, new Vector2(0,-50));
        if (players.Count > 0)
        {
            Engine.DrawString("player " + players[0].pNum + " wins", winStatement, Color.Black, font);
        }

    }
    void mapMakerScreen()
    {
        players[0].controller.getCursor(2);

        mapmaker.init(players[0].controller);
        
        Engine.DrawRectEmpty(exit2, Color.Red);
        if (exit2.Overlaps(players[0].controller.cursor))
        {
            Engine.DrawRectSolid(exit2, Color.Red);
            if (players[0].controller.getSelect())
            {
                screen = 2;
                availableMaps.Clear();
                loadMaps();
            }
            
        }
        
        else if (players[0].controller.getBack())
        {
            screen = 2;
            availableMaps.Clear();
            loadMaps();
        }
        Engine.DrawString("exit",exit2.Position + new Vector2(15,0),Color.Black,font);
        

    }

    void preMatch()
    {
        game.draw();
        foreach (Player player in players)
        {
            player.render();
        }
        Engine.DrawString("" + (((int)preMatchTimer) + 1), new Vector2(640, 360), Color.Black, font, TextAlignment.Center);
        preMatchTimer -= Engine.TimeDelta;
        if (preMatchTimer <= 0)
        {
            screen = 3;
            preMatchTimer = 3f;
        }

    }

    void mapsScreen()
    {

        if (!gameStarted)
        {
            Engine.DrawRectEmpty(exit, Color.White);
            Engine.DrawRectEmpty(newMap, Color.White);
            if (exit.Overlaps(players[0].controller.cursor))
            {
                Engine.DrawRectSolid(exit, Color.Lime);
                if (players[0].controller.getSelect())
                {
                    players.Clear();
                    
                    screen = 0;
                    return;
                }
            }
            if (players[0].controller.getBack())
            {
                players.Clear();

                screen = 0;
                return;
            }
            if (newMap.Overlaps(players[0].controller.cursor))
            {
                Engine.DrawRectSolid(newMap, Color.Lime);
                if (players[0].controller.getSelect())
                {
                    String[] lines = new String[32];
                    for (int i = 0; i < lines.Length; i++)
                    {
                        lines[i] = "                                                      ";
                    }

                    System.IO.File.WriteAllLines("./maps/level" + (numMaps + 1) + ".txt", lines);
                    String[] lines2 = { "sebi.jpg", "level" + (numMaps + 1) + ".txt" };
                    availableMaps.Add(new Map("backgrounds/sebi.jpg", "./maps/level" + (numMaps + 1) + ".txt", availableMaps.Count));
                    numMaps++;
                    string[] temp = System.IO.File.ReadAllLines("./Assets/mapData.txt");
                    temp[0] = "" + (int.Parse(temp[0]) + 1);
                    System.IO.File.WriteAllLines("./Assets/mapData.txt", temp);

                    System.IO.File.AppendAllLines("./Assets/mapData.txt",lines2);

                    Console.WriteLine("new file");
                    numMaps = availableMaps.Count;
                    pages = (numMaps - 1) / 15 + 1;
                }
            }
            Engine.DrawString("exit",exit.Position + new Vector2(28,-4),Color.White,font);
            Engine.DrawString("new map", newMap.Position + new Vector2(2, -4), Color.White, font);
            

        }
        Console.WriteLine(players.Count);

        if (players[0].controller.cursor.Position.X + 20 < 1180 && players[0].controller.cursor.Position.X + 20 > 100 && players[0].controller.cursor.Position.Y + 20 < 389 && players[0].controller.cursor.Position.Y + 20 > 5)
        {
            int index = ((((int)players[0].controller.cursor.Position.Y - 5) / 128) * 5 + (((int)players[0].controller.cursor.Position.X - 100) / 216)) + 15 * (currentPage - 1);
            if (index < availableMaps.Count)
            {
                availableMaps[index].drawHover();
                if (players[0].controller.getSelect())
                {
                    if (!gameStarted)
                    {
                        mapmaker = new MapMaker(availableMaps[index]);

                        screen = 5;
                    }
                    else
                    {
                        game = availableMaps[index];
                        game.gameStarted = true;
                        game.reload();
                        for (int i = 0; i < players.Count; i++)
                        {
                            players[i].Position = game.spawnPoints[i];
                        }
                        Engine.Fullscreen = true;
                        SDL.SDL_SetWindowFullscreen(Engine.Window, Engine.Fullscreen ? (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP : 0);
                        screen = 8;
                    }


                }
            }


        }


        

        tiles = 15;
        if (currentPage == pages)
        {

            tiles = numMaps % 15;
        }
        players[0].controller.getCursor(8);

        Engine.DrawRectEmpty(new Bounds2(new Vector2(393.6035f, 392.7559f), new Vector2(552, 310.5f)), Color.LimeGreen);
        for (int k = 0; k < tiles; k++)
        {
            availableMaps[k].drawPreview(new Vector2(216 * c + 100, 128 * r + 5));
            Engine.DrawRectEmpty(new Bounds2(new Vector2(216 * c + 100, 128 * r + 5), new Vector2(216f, 128f)), Color.AntiqueWhite);

            c++;
            if (c > 4)
            {
                c = 0;
                r++;
            }
        }
        c = 0;
        r = 0;
        if (numMaps > 15)
        {
            if (currentPage < pages)
            {
                Engine.DrawRectSolid(next, Color.Green);
                
                
                if (next.Contains(players[0].controller.cursor.Position) && players[0].controller.getSelect())
                {
                    currentPage++;
                }
            }
            if (currentPage > 1)
            {
                Engine.DrawRectSolid(back, Color.Red);
                if (back.Contains(players[0].controller.cursor.Position) && players[0].controller.getSelect())
                {
                    currentPage--;
                }
            }


        }


        Engine.DrawRectSolid(players[0].controller.cursor, colors[0]);
    }


    
    void controlsScreen()
    {

        if (Engine.GetKeyDown(Key.T))
        {
            switchImage = !switchImage;
        }
        if (switchImage)
        {
            Engine.DrawTexture(keyboardBinds, Vector2.Zero);

        }
        else
        {
            Engine.DrawTexture(controllerBinds, Vector2.Zero);

        }
        if (connectedControllers.Exists(a => a.getBack()))
        {
            screen = 0;
        }
        Engine.DrawString("press T to see keyboard controls", new Vector2(10, 0), Color.Black, sfont);
    }

    public void pauseScreen()
    {
        if (connectedControllers.Exists(a => a.getButton("pause")))
        {
            screen = 3;

        }
        Engine.DrawString("paused", new Vector2(640, 360), Color.White, font, alignment: TextAlignment.Center);
    }
    public void Update()
    {
        switch (screen)
        {

           
            // Start Screen

            case 0:
                startScreen();
                break;
            // How many players
            case 1:
                playerSelection();
                break;
            case 2:
                mapsScreen();
                break;
            case 3:
                startGame();
                break;
            case 4:
                endGame();
                break;
            case 5:
                mapMakerScreen();
                break;
            case 6:
                pauseScreen();
                break;
            case 7:
                controlsScreen();
                break;
            case 8:
                preMatch();
                break;


            default:
                throw new Exception("Invalid screen");
        }

    }

    void printFps()
    {
        Console.WriteLine(1 / Engine.TimeDelta);
    }

    void printMousePos()
    {
        Console.WriteLine(Engine.MousePosition);
    }

    void initControllers()
    {
        connectedControllers.Clear();
        availableC = "";
        Controller temp;
        for (int i = 0; i < 14; i++)
        {

            IntPtr c = SDL.SDL_GameControllerOpen(i);
            if (SDL.SDL_GameControllerGetAttached(c) > 0)
            {
                temp = new Controller(c, 0);
                connectedControllers.Add(temp);
                availableC += " " + temp.getType();
                Controller.controllers++;
            }
            else
            {
                break;
            }
        }
        temp = new Controller(new IntPtr(), 1);
        connectedControllers.Add(temp);
        availableC += " " + temp.getType();
        temp = new Controller(new IntPtr(), 2);
        connectedControllers.Add(temp);
        availableC += " " + temp.getType();



    }

    public void loadMapsCharacters()
    {
         
        numCharacters = int.Parse(characterData[0]);
        availableCharacters = new Character[numCharacters];
        for (int k = 0; k < numCharacters; k++)
        {
            availableCharacters[k] = new Character("./Assets/characters/" + characterData[k + 1], characterColors[k]);
            availableCharacters[k].attackDamage = 17-k;
            availableCharacters[k].shield = k;
        }

        loadMaps();

    } 

    public void loadMaps()
    {

        mapData = System.IO.File.ReadAllLines("./Assets/mapData.txt");
        numMaps = int.Parse(mapData[0]);
        availableMaps = new List<Map>();
        for (int i = 1; i < numMaps + 1; i++)
        {
            availableMaps.Add(new Map("backgrounds/" + mapData[2 * i - 1], "./Assets/maps/" + mapData[2 * i], i));
        }
    }





}