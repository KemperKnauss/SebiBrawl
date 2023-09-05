using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


public class MapMaker
{

    public Vector2 cursorPosition = new Vector2(640, 360);
    Texture right = Engine.LoadTexture("rarrow.png");
    Texture left = Engine.LoadTexture("larrow.png");
    Bounds2 rightArrow = new Bounds2(new Vector2(1200, 22 * 20), new Vector2(20, 20));
    Bounds2 leftArrow = new Bounds2(new Vector2(1200, 23 * 20), new Vector2(20, 20));
    Texture soil = Engine.LoadTexture("Soil.png");
    Texture stone = Engine.LoadTexture("Stone.png");
    Texture grass = Engine.LoadTexture("Grass.png");
    Texture magma = Engine.LoadTexture("magmablock.png");
    Texture wood = Engine.LoadTexture("woodblock.png");
    Texture spawn = Engine.LoadTexture("brick.png");
    Texture dirtgrass = Engine.LoadTexture("dirtandgrassblock.png");
    Texture line = Engine.LoadTexture("line.png");
    List<Texture> availableBackgrounds = new List<Texture>();
    List<String> availableBackgroundsPaths = new List<String>();
    int currentBackgroundIndex;
    float czTimer = 0;
    private Map map;
    bool grid;
    int x = 0;
    int y = 0;
    Bounds2 boundsHighlight;
    String[] lines;
    String txtPath;
    char current = ' ';
    Dictionary<int, char> pairs = new Dictionary<int, char>();
    Stack<String[]> undo = new Stack<String[]>();
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


    public void loadBackgrounds()
    {
        foreach (string filename in Directory.GetFiles("./Assets/backgrounds/"))
        {
            availableBackgrounds.Add(Engine.LoadTexture(filename.Substring(9)));
            availableBackgroundsPaths.Add(filename.Substring(9));
        }
        for (int i = 0; i < availableBackgroundsPaths.Count; i++)
        {
            if (availableBackgroundsPaths[i].Equals(map.backGroundPath))
            {
                currentBackgroundIndex = i;
                return;
            }
        }
    }
    public void changeBackground()
    {

        map.backGroundPath = availableBackgroundsPaths[currentBackgroundIndex];
        map.reloadBackground();
    }




    public void init(Controller controller)
    {
        
        this.cursorPosition = controller.cursor.Position;
        

        x = ((int)(cursorPosition.X - 100) / 20);
        y = ((int)(cursorPosition.Y - 80) / 20);
        map.draw();

        Engine.DrawRectSolid(new Bounds2(new Vector2(1195, 4 * 20 - 9), new Vector2(29, 410)), Color.White);

        Engine.DrawTexture(soil, new Vector2(60 * 20, 4 * 20));
        Engine.DrawRectEmpty(new Bounds2(new Vector2(1199, 4 * 20 - 1), new Vector2(21, 21)), Color.Lime);

        Engine.DrawTexture(grass, new Vector2(60 * 20, 6 * 20));
        Engine.DrawRectEmpty(new Bounds2(new Vector2(1199, 6 * 20 - 1), new Vector2(21, 21)), Color.Lime);

        Engine.DrawTexture(stone, new Vector2(60 * 20, 8 * 20));
        Engine.DrawRectEmpty(new Bounds2(new Vector2(1199, 8 * 20 - 1), new Vector2(21, 21)), Color.Lime);

        Engine.DrawTexture(magma, new Vector2(60 * 20, 10 * 20));
        Engine.DrawRectEmpty(new Bounds2(new Vector2(1199, 10 * 20 - 1), new Vector2(21, 21)), Color.Lime);

        Engine.DrawTexture(wood, new Vector2(60 * 20, 12 * 20));
        Engine.DrawRectEmpty(new Bounds2(new Vector2(1199, 12 * 20 - 1), new Vector2(21, 21)), Color.Lime);

        Engine.DrawTexture(dirtgrass, new Vector2(60 * 20, 14 * 20));
        Engine.DrawRectEmpty(new Bounds2(new Vector2(1199, 14 * 20 - 1), new Vector2(21, 21)), Color.Lime);

        Engine.DrawTexture(line, new Vector2(60 * 20, 16 * 20));
        Engine.DrawRectEmpty(new Bounds2(new Vector2(1199, 16 * 20 - 1), new Vector2(21, 21)), Color.Lime);

        Engine.DrawTexture(spawn, new Vector2(60 * 20, 18 * 20));
        Engine.DrawRectEmpty(new Bounds2(new Vector2(1199, 18 * 20 - 1), new Vector2(21, 21)), Color.Lime);

        Engine.DrawRectEmpty(new Bounds2(new Vector2(1199, 20 * 20 - 1), new Vector2(21, 21)), Color.Lime);



        boundsHighlight.Position.X = x * 20 + 100;
        boundsHighlight.Position.Y = y * 20 + 80;
        Engine.DrawRectSolid(boundsHighlight, Color.Lime);


        if (cursorPosition.X < 1180 && cursorPosition.X > 100 && cursorPosition.Y > 80)
        {




            if (controller.getSelectMapMaker())
            {

                char[] temp = lines[y].ToCharArray();
                if (temp[x] != current)
                {

                    String[] temp2 = new String[lines.Length];
                    Array.Copy(lines, 0, temp2, 0, lines.Length);
                    undo.Push(temp2);
                    temp[x] = current;

                    lines[y] = new String(temp);
                    System.IO.File.WriteAllLines(txtPath, lines);


                    map.reload();
                }





            }
        }
        
        
        
       
        
        drawGrid();
        
        if (Engine.GetKeyHeld(Key.LeftControl) && Engine.GetKeyHeld(Key.Z) && undo.Count > 0 && czTimer > 0.18)
        {

            lines = undo.Pop();
            System.IO.File.WriteAllLines(txtPath, lines);
            map.reload();
            czTimer = 0;
        }

        czTimer += Engine.TimeDelta;

        Engine.DrawTexture(right, rightArrow.Position, size: rightArrow.Size);
        Engine.DrawTexture(left, leftArrow.Position, size: leftArrow.Size);

        if (rightArrow.Contains(cursorPosition) && currentBackgroundIndex < availableBackgrounds.Count - 1 && controller.getSelect())
        {
            currentBackgroundIndex++;
            changeBackground();


        }
        else if (leftArrow.Contains(cursorPosition) && currentBackgroundIndex > 0 && controller.getSelect())
        {
            currentBackgroundIndex--;
            changeBackground();
        }
        if ((int)(cursorPosition.X) / 20 == 60 && controller.getSelect())
        {



            current = pairs.GetValueOrDefault((int)(cursorPosition.Y) / 20);



        }

    }

    void drawGrid()
    {
        for (int x = 100; x <= 1180; x += 20)
        {
            Engine.DrawLine(new Vector2(x, 80), new Vector2(x, 720), Color.White);
        }
        for (int y = 80; y <= 720; y += 20)
        {
            Engine.DrawLine(new Vector2(100, y), new Vector2(1180, y), Color.White);
        }
    }


    public MapMaker(Map map)
    {

        txtPath = map.txtPath;
        pairs.Add(4, 's');
        pairs.Add(6, 't');
        pairs.Add(8, 'p');
        pairs.Add(10, 'm');
        pairs.Add(12, 'w');
        pairs.Add(14, 'd');
        pairs.Add(16, 'l');
        pairs.Add(18, '1');
        pairs.Add(20, ' ');
        boundsHighlight = new Bounds2(new Vector2(x * 20, y * 20), new Vector2(20, 20));
        this.map = map;

        lines = map.lines;
        loadBackgrounds();

    }




}