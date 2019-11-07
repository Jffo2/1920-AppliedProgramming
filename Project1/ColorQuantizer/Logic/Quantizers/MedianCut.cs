using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageProcessing.Logic.Quantizers
{
    public class MedianCut : Quantizer
    {
        private static int PALETTE_MAX_COLORS = 256;
        private ConcurrentBag<Color> colorList;
        private List<List<Color>> cubes;

        public MedianCut() : base()
        {
            colorList = new ConcurrentBag<Color>();
            cubes = new List<List<Color>>();
        }

        public override void AddColor(Color c)
        {
            colorList.Add(c);
        }

        protected override void PopulatePalette()
        {
            // finds the minimum iterations needed to achieve the cube count (color count) we need
            int iterationCount = 1;
            int colorCount = colorList.Count;
            while ((1 << iterationCount) < colorCount) { iterationCount++; }
            this.cubes.Add(colorList.ToList());
            var cubes = Util.Cloner.DeepClone(this.cubes);
            for (int iteration = 0; iteration < iterationCount; iteration++)
            {
                System.Diagnostics.Debug.WriteLine("Splitting " + iteration + "/" + iterationCount);
                SplitCubes(cubes);
            }
            System.Diagnostics.Debug.WriteLine(cubes.Count);
            palette.Add(new Models.Color(0, 0, 0));
            palette.Add(new Models.Color(255, 255, 255));

        }

        public void SplitCubes(List<List<Color>> cubes)
        {
            List<List<Color>> newCubes = new List<List<Color>>();
            foreach (List<Color> cube in cubes)
            {
                if (newCubes.Count == PALETTE_MAX_COLORS) break;
                var orderedRed = cube.OrderBy(color => color.R);
                var orderedGreen = cube.OrderBy(color => color.G);
                var orderedBlue = cube.OrderBy(color => color.B);

                int rdist = orderedRed.Last().R - orderedRed.First().R;
                int gdist = orderedGreen.Last().G - orderedGreen.First().G;
                int bdist = orderedBlue.Last().B - orderedBlue.First().B;

                int cubeCount = cube.Count();
                int center = cubeCount >> 1;

                List<Color> newCube1;
                List<Color> newCube2;

                if (rdist > gdist && rdist > bdist)
                {
                    newCube1 = new List<Color>(orderedRed.Take(center));
                    newCube2 = new List<Color>(orderedRed.Skip(center).Take(cubeCount - center));

                }
                else if (gdist > rdist && gdist > bdist)
                {
                    newCube1 = new List<Color>(orderedGreen.Take(center));
                    newCube2 = new List<Color>(orderedGreen.Skip(center).Take(cubeCount - center));
                }
                else
                {
                    newCube1 = new List<Color>(orderedBlue.Take(center));
                    newCube2 = new List<Color>(orderedBlue.Skip(center).Take(cubeCount - center));
                }
                if (newCubes.Count == PALETTE_MAX_COLORS) break;
                newCubes.Add(newCube1);
                if (newCubes.Count == PALETTE_MAX_COLORS) break;
                newCubes.Add(newCube2);
            }
            cubes.Clear();
            cubes.AddRange(newCubes);

        }
    }
}
