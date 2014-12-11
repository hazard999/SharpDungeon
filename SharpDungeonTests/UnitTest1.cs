using Microsoft.VisualStudio.TestTools.UnitTesting;
using sharpdungeon;
using sharpdungeon.levels;
using System.Drawing;
using Rectangle = System.Drawing.Rectangle;

namespace SharpDungeonTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Dungeon.Depth = 1;
            var level = new SewerLevel();
            level.feeling = Level.Feeling.WATER;
            
            level.Create();

            //level.DebugBuild();
            var quadLevel = new int[Level.Width, Level.Width];
            for (var i = 0; i < level.map.Length; i++)
            {
                var x = i / Level.Width;
                var y = i % Level.Width;
                quadLevel[x, y] = level.map[i];
            }

            var levelBitmap = DrawLevel(quadLevel);
            levelBitmap.Save(@"c:\temp\lbl.bmp");
        }

        private static Bitmap DrawLevel(int[,] quadLevel)
        {
            var tiles = System.Drawing.Image.FromFile(@"C:\Users\admin\Source\Repos\pdsharp\SharpDungeon\Assets\tiles0.png") as Bitmap;

            var tileImages = new Bitmap[64];
            var index = 0;
            for (var j = 0; j < 4; j++)
                for (var i = 0; i < 16; i++)
                {
                    tileImages[index] = new Bitmap(16, 16);
                    using (var g = Graphics.FromImage(tileImages[index]))
                    {
                        var destRect = new System.Drawing.Rectangle(0, 0, 16, 16);
                        var srcRect = new System.Drawing.Rectangle(i * 16, j * 16, 16, 16);
                        g.DrawImage(tiles, destRect, srcRect, GraphicsUnit.Pixel);
                    }
                    index++;
                }

            var levelBitmap = new Bitmap(512, 512);
            var g2 = Graphics.FromImage(levelBitmap);
            for (int i = 0; i < 32; i++)
                for (int j = 0; j < 32; j++)
                {
                    var b = (byte)quadLevel[i, j];
                    g2.DrawImage(tileImages[b], new Rectangle(i * 16, j * 16, 16, 16), new Rectangle(0, 0, 16, 16),
                        GraphicsUnit.Pixel);
                }
            return levelBitmap;
        }
    }
}
