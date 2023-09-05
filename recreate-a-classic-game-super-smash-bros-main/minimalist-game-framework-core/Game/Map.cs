using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Map
{
    int mapIndex;
    public bool gameStarted = false;
    public List<Vector2> spawnPoints = new List<Vector2>();
    Texture soil = Engine.LoadTexture("Soil.png");
    Texture stone = Engine.LoadTexture("Stone.png");
    Texture grass = Engine.LoadTexture("Grass.png");
    Texture magma = Engine.LoadTexture("magmablock.png");
    Texture wood = Engine.LoadTexture("woodblock.png");
    Texture spawn = Engine.LoadTexture("brick.png");
    Texture dirtgrass = Engine.LoadTexture("dirtandgrassblock.png");
    Texture line = Engine.LoadTexture("line.png");
    public Texture background;
    public String backGroundPath;
    public List<Vector2> spawns = new List<Vector2>();
    TileType[][] bitmap;
    public String txtPath;
    public String[] lines;
    private enum TileType
    {
        Empty,
        Soil,
        Top,
        Platform,
        Spawn,
        Magma,
        DirtGrass,
        Wood,
        Line



    }

    public Map(String backgroundPath, String txtPath, int index)
    {
        this.backGroundPath = backgroundPath;
        background = Engine.LoadTexture(backgroundPath);
        mapIndex = index;

        bitmap = new TileType[32][];
        this.txtPath = txtPath;
        reload();
        getSpawnPoints();
    }

    public void reloadBackground()
    {
        background = Engine.LoadTexture(this.backGroundPath);
        String[] temp = File.ReadAllLines("./Assets/mapData.txt");
        temp[2 * mapIndex - 1] = this.backGroundPath.Substring(12);


        System.IO.File.WriteAllLines("./Assets/mapData.txt", temp);
    }
    public void getSpawnPoints()
    {

        for (int i = 0; i < lines.Length; i++)
        {

            for (int j = 0; j < lines[i].Length; j++)
            {
                switch (lines[i][j])
                {

                    case '1':
                        spawnPoints.Add(new Vector2(j * 20 + 100, i * 20 + 59));

                        break;


                }
            }
        }
    }
    public void reload()
    {
        lines = System.IO.File.ReadAllLines(txtPath);

        for (int i = 0; i < lines.Length; i++)
        {
            bitmap[i] = new TileType[54];
            for (int j = 0; j < lines[i].Length; j++)
            {
                switch (lines[i][j])
                {
                    case ' ':
                        bitmap[i][j] = TileType.Empty;
                        break;
                    case 's':
                        bitmap[i][j] = TileType.Soil;
                        break;
                    case 't':
                        bitmap[i][j] = TileType.Top;
                        break;
                    case 'p':
                        bitmap[i][j] = TileType.Platform;
                        break;
                    case 'd':
                        bitmap[i][j] = TileType.DirtGrass;
                        break;
                    case '1':
                        if (gameStarted)
                        {
                            bitmap[i][j] = TileType.Empty;
                        }
                        else
                        {
                            bitmap[i][j] = TileType.Spawn;
                        }

                        break;
                    case 'm':
                        bitmap[i][j] = TileType.Magma;
                        break;
                    case 'w':
                        bitmap[i][j] = TileType.Wood;
                        break;
                    case 'l':
                        bitmap[i][j] = TileType.Line;
                        break;

                }
            }
        }
    }
    public void draw()
    {
        Engine.DrawTexture(background, Vector2.Zero);
        for (int i = 0; i < bitmap.Length; i++)
        {
            for (int j = 0; j < bitmap[i].Length; j++)
            {
                switch (bitmap[i][j])
                {
                    case TileType.Empty:
                        break;
                    case TileType.Soil:
                        Engine.DrawTexture(soil, new Vector2(j * 20 + 100, i * 20 + 80));
                        break;
                    case TileType.Top:
                        Engine.DrawTexture(grass, new Vector2(j * 20 + 100, i * 20 + 80));
                        break;
                    case TileType.Platform:
                        Engine.DrawTexture(stone, new Vector2(j * 20 + 100, i * 20 + 80));
                        break;
                    case TileType.DirtGrass:
                        Engine.DrawTexture(dirtgrass, new Vector2(j * 20 + 100, i * 20 + 80));
                        break;
                    case TileType.Spawn:
                        Engine.DrawTexture(spawn, new Vector2(j * 20 + 100, i * 20 + 80));
                        break;
                    case TileType.Magma:
                        Engine.DrawTexture(magma, new Vector2(j * 20 + 100, i * 20 + 80));
                        break;
                    case TileType.Wood:
                        Engine.DrawTexture(wood, new Vector2(j * 20 + 100, i * 20 + 80));
                        break;
                    case TileType.Line:
                        Engine.DrawTexture(line, new Vector2(j * 20 + 100, i * 20 + 80));
                        break;

                }
            }
        }
    }

    public void drawPreview(Vector2 start)
    {
        Engine.DrawTexture(background, start, size: new Vector2(216, 128));
        for (int i = 0; i < bitmap.Length; i++)
        {
            for (int j = 0; j < bitmap[i].Length; j++)
            {
                switch (bitmap[i][j])
                {
                    case TileType.Empty:
                        break;
                    case TileType.Soil:
                        Engine.DrawTexture(soil, start + new Vector2(j * 4, i * 4), size: (soil.Size / 5));
                        break;
                    case TileType.Top:
                        Engine.DrawTexture(grass, start + new Vector2(j * 4, i * 4), size: (grass.Size / 5));
                        break;
                    case TileType.Platform:
                        Engine.DrawTexture(stone, start + new Vector2(j * 4, i * 4), size: (stone.Size / 5));
                        break;
                    case TileType.DirtGrass:
                        Engine.DrawTexture(dirtgrass, start + new Vector2(j * 4, i * 4), size: (soil.Size / 5));
                        break;
                    case TileType.Spawn:
                        Engine.DrawTexture(spawn, start + new Vector2(j * 4, i * 4), size: (soil.Size / 5));
                        break;
                    case TileType.Magma:
                        Engine.DrawTexture(magma, start + new Vector2(j * 4, i * 4), size: (soil.Size / 5));
                        break;
                    case TileType.Wood:
                        Engine.DrawTexture(wood, start + new Vector2(j * 4, i * 4), size: (soil.Size / 5));
                        break;
                    case TileType.Line:
                        Engine.DrawTexture(line, start + new Vector2(j * 4, i * 4), size: (soil.Size / 5));
                        break;
                }
            }
        }
    }

    public void drawHover()
    {
        Engine.DrawTexture(background, new Vector2(393.6035f, 392.7559f), size: new Vector2(552, 310.5f));
        for (int i = 0; i < bitmap.Length; i++)
        {
            for (int j = 0; j < bitmap[i].Length; j++)
            {
                switch (bitmap[i][j])
                {
                    case TileType.Empty:
                        break;
                    case TileType.Soil:
                        Engine.DrawTexture(soil, new Vector2(393.6035f + (100 / 2.31884057971f) + j * (20 / 2.31884057971f), 392.7559f + (40 / 2.31884057971f) + i * (20 / 2.31884057971f)), size: (soil.Size / 2.221884057971f));
                        break;
                    case TileType.Top:
                        Engine.DrawTexture(grass, new Vector2(393.6035f + (100 / 2.31884057971f) + j * (20 / 2.31884057971f), 392.7559f + (40 / 2.31884057971f) + i * (20 / 2.31884057971f)), size: (grass.Size / 2.221884057971f));
                        break;
                    case TileType.Platform:
                        Engine.DrawTexture(stone, new Vector2(393.6035f + (100 / 2.31884057971f) + j * (20 / 2.31884057971f), 392.7559f + (40 / 2.31884057971f) + i * (20 / 2.31884057971f)), size: (stone.Size / 2.221884057971f));
                        break;
                    case TileType.DirtGrass:
                        Engine.DrawTexture(dirtgrass, new Vector2(393.6035f + (100 / 2.31884057971f) + j * (20 / 2.31884057971f), 392.7559f + (40 / 2.31884057971f) + i * (20 / 2.31884057971f)), size: (stone.Size / 2.221884057971f));
                        break;
                    case TileType.Spawn:
                        Engine.DrawTexture(spawn, new Vector2(393.6035f + (100 / 2.31884057971f) + j * (20 / 2.31884057971f), 392.7559f + (40 / 2.31884057971f) + i * (20 / 2.31884057971f)), size: (stone.Size / 2.221884057971f));
                        break;
                    case TileType.Magma:
                        Engine.DrawTexture(magma, new Vector2(393.6035f + (100 / 2.31884057971f) + j * (20 / 2.31884057971f), 392.7559f + (40 / 2.31884057971f) + i * (20 / 2.31884057971f)), size: (stone.Size / 2.221884057971f));
                        break;
                    case TileType.Wood:
                        Engine.DrawTexture(wood, new Vector2(393.6035f + (100 / 2.31884057971f) + j * (20 / 2.31884057971f), 392.7559f + (40 / 2.31884057971f) + i * (20 / 2.31884057971f)), size: (stone.Size / 2.221884057971f));
                        break;
                    case TileType.Line:
                        Engine.DrawTexture(line, new Vector2(393.6035f + (100 / 2.31884057971f) + j * (20 / 2.31884057971f), 392.7559f + (40 / 2.31884057971f) + i * (20 / 2.31884057971f)), size: (stone.Size / 2.221884057971f));
                        break;
                }
            }
        }
    }

    //collisions use the four corners of the 40 by 40 hitbox to check for tiles
    //upward collision is ignored to jump through platforms
    public bool grounded(Player player)
    {
        float projectY = player.Position.Y - 80 + Engine.TimeDelta * player.Velocity.Y;
        float projectX = player.Position.X - 100 + Engine.TimeDelta * player.Velocity.X;

        if (((int)projectY) / 20 + 2 >= 32 || ((int)projectX) / 20 >= 54 || projectX < 0 || projectY < 0 || ((int)projectX + 40) / 20 >= 54)
        {
            return false;
        }
        return bitmap[(int)(((projectY) + 40) / 20)][(int)((projectX + 3) / 20)] != TileType.Empty || bitmap[(int)(((projectY) + 40) / 20)][(int)((projectX + 37) / 20)] != TileType.Empty;

    }

    public void handleCollision(Player player)
    {


        //LEFT 0 RIGHT 2 UP 1
        //Console.WriteLine((int)(((player.Position.X + 40)) / 20));
        //Console.WriteLine((int)Engine.MousePosition.X / 20);
        float projectY = player.Position.Y - 80 + Engine.TimeDelta * player.Velocity.Y;
        float projectX = player.Position.X - 100 + Engine.TimeDelta * player.Velocity.X;

        if (((int)projectY) / 20 + 2 >= 32 || ((int)projectX) / 20 >= 54 || projectX < 0 || projectY < 0 || ((int)projectY) <= 80 || ((int)projectX + 40) / 20 >= 54)
        {
            return;
        }



        if (bitmap[(int)((projectY) / 20)][(int)((projectX) / 20)] != TileType.Empty && player.Velocity.Y < 0)
        {
            if (player.playerLaunched)
            {
                player.Velocity.Y *= -1;
            }
            else if (bitmap[(int)((projectY - 20) / 20)][(int)((projectX) / 20)] != TileType.Empty)
            {
                player.Velocity.Y = 0;
            }

        }

        if ((bitmap[(int)(projectY / 20)][(int)((projectX) / 20)] != TileType.Empty || bitmap[(int)((projectY + 39) / 20)][(int)(projectX / 20)] != TileType.Empty) && player.Velocity.X < 0)
        {
            if (player.playerLaunched && bitmap[(int)((projectY + 20) / 20)][(int)((projectX) / 20)] != TileType.Empty)
            {
                player.Velocity.X *= -1;
            }
            else
            {
                player.Velocity.X = 0;
            }
        }
        else if ((bitmap[(int)(projectY / 20)][(int)((projectX + 40) / 20)] != TileType.Empty || bitmap[(int)((projectY + 39) / 20)][(int)((projectX + 40) / 20)] != TileType.Empty) && player.Velocity.X > 0)
        {
            if (player.playerLaunched && bitmap[(int)((projectY + 20) / 20)][(int)((projectX + 40) / 20)] != TileType.Empty)
            {
                player.Velocity.X *= -1;
            }
            else
            {
                player.Velocity.X = 0;
            }
        }





    }

    public void gravity(Player player)
    {
        if (!grounded(player))
        {
            if (!player.attacking)
            {
                player.state = 2;
            }
            
            player.Acceleration.Y = 900f;
            if (player.jumpState1 == 1)
            {
                player.jumpTimer += Engine.TimeDelta;
            }
            if (player.jumpTimer > .350)
            {
                player.doubleJump = true;
            }
        }
        else
        {
            if (player.Velocity.Y > 0)
            {
                player.Acceleration.Y = 0f;
                player.Velocity.Y = 0f;
                player.jumpState1 = 0;
                player.jumpTimer = 0;
                player.doubleJump = false;
            }
        }
    }


}