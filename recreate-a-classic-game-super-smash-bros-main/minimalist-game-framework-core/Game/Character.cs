using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Character
{

    public List<List<Texture>> all = new List<List<Texture>>();
    public List<Texture> idle = new List<Texture>();
    public List<Texture> run = new List<Texture>();
    public List<Texture> jump = new List<Texture>();
    public List<Texture> attackup = new List<Texture>();
    public List<Texture> attack = new List<Texture>();
    public List<Texture> attackdown = new List<Texture>();
    public List<Texture> current;
    public float timer = 0;
    public int currentFrame;
    
    public Color color;
    public int attackDamage;
    public int shield;
    

    public Character(string path,Color color)
    {
        this.color = color;
        currentFrame = 0;

        foreach (string filename in Directory.GetFiles(path + "/idle/"))
        {

            idle.Add(Engine.LoadTexture(filename.Substring(9)));
        }
        all.Add(idle);
        foreach (string filename in Directory.GetFiles(path + "/run/"))
        {
            run.Add(Engine.LoadTexture(filename.Substring(9)));
        }
        all.Add(run);

        foreach (string filename in Directory.GetFiles(path + "/jump/"))
        {
            jump.Add(Engine.LoadTexture(filename.Substring(9)));
        }
        all.Add(jump);
        foreach (string filename in Directory.GetFiles(path + "/attackup/"))
        {
            attackup.Add(Engine.LoadTexture(filename.Substring(9)));
        }
        all.Add(attackup);
        foreach (string filename in Directory.GetFiles(path + "/attack/"))
        {
            attack.Add(Engine.LoadTexture(filename.Substring(9)));
        }
        all.Add(attack);
        foreach (string filename in Directory.GetFiles(path + "/attackdown/"))
        {
            attackdown.Add(Engine.LoadTexture(filename.Substring(9)));
        }
        all.Add(attackdown);







    }




    public Texture render(int state)
    {


        if (timer > .100)
        {
            Console.WriteLine(all[state].Count - 1);
            currentFrame++;
            timer = 0;
            if (currentFrame > all[state].Count - 1)
            {
                currentFrame = 0;
            }

        }
        //Console.WriteLine(currentFrame);
        timer += Engine.TimeDelta;

        return all[state][currentFrame];
    }





}