using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BinhlEmul
{
    
    class Render
    {
        public World world;
        public Render(World world)
        {
            this.world = world;
        }
        public Image GetSingeLayeImage()
        {
            Image i = new Bitmap(world.worldSizeX * 10, world.worldSizeY*10);
            var gr = System.Drawing.Graphics.FromImage(i);
            gr.Clear(Color.Black);
            for (int x = 0; x < world.worldSizeX; x++)
            {
                for (int y = 0; y < world.worldSizeY; y++)
                {
                    for (int z = 0; z < world.worldSizeZ; z++)
                    {
                        if (world.GetObject(x,world.worldSizeY - y -1 , z).GetType() == typeof(WorldObjects.RedstoneWire))
                        {
                            Color c = Color.FromArgb(50 + 12 * world.GetObject(x, world.worldSizeY - y - 1, z).RedValue, 0, 0);
                            Brush b = new SolidBrush(c);
                            gr.FillRectangle(b, x * 10, y * 10, 10, 10);
                        }

                        if (world.GetObject(x, world.worldSizeY - y - 1, z).GetType() == typeof(WorldObjects.RedstoneRepiter))
                        {
                            Color c = Color.FromArgb(0, 50, 0);
                            if (world.GetObject(x, world.worldSizeY - y - 1, z).IsActivated)
                            {
                                c = Color.FromArgb(0, 250, 0);
                            }
                            Brush b = new SolidBrush(c);
                            gr.FillRectangle(b, x * 10, y * 10, 10, 10);
                        }
                    }   
                }
            }
            for (int x = 0; x < world.worldSizeX; x++)
            {
                for (int y = 0; y < world.worldSizeY; y++)
                {
                    for (int z = 0; z < world.worldSizeZ; z++)
                    {
                        if (world.GetObject(x, world.worldSizeY - y - 1, z).GetType() == typeof(WorldObjects.RedstoneTorch))
                        {
                            Color c = Color.FromArgb(0, 0, 50);
                            if (world.GetObject(x, world.worldSizeY - y - 1, z).IsActivated)
                            {
                                c = Color.FromArgb(0, 0, 250);
                            }
                            Brush b = new SolidBrush(c);
                            gr.FillEllipse(b, x * 10+2, y * 10+2, 6, 6);
                        }
                    }
                }
            }

            return i;
        }
    }
}
