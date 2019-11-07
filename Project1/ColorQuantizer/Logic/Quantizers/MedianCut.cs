using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageProcessing.Logic.Quantizers
{
    public class MedianCut : Quantizer
    {
        private static readonly int PALETTE_MAX_COLORS = 256;
        private readonly ConcurrentDictionary<Color,int> colorList;
        private List<List<Color>> cubes;
        private int highestColorCount = 0;

        public MedianCut() : base()
        {
            colorList = new ConcurrentDictionary<Color, int>();
            cubes = new List<List<Color>>();
        }

        public override void AddColor(Color c)
        {
            var i = colorList.AddOrUpdate(c, 1, (color, count) => count + 1);
            highestColorCount = (highestColorCount<i)? i: highestColorCount;
        }

        protected override void PopulatePalette()
        {
            System.Diagnostics.Debug.WriteLine("Starting populate palette");
            int iterationCount = 1;
            int colorCount = PALETTE_MAX_COLORS;
            System.Diagnostics.Debug.WriteLine("Starting iterationCount init");
            while ((1 << iterationCount) < colorCount) { iterationCount++; }
            System.Diagnostics.Debug.WriteLine("deepcopy cubes");
            var cubes = Util.Cloner.DeepClone(this.cubes);
            System.Diagnostics.Debug.WriteLine("Convert colorList to list");
            cubes.Add(colorList.Select(keypair => keypair.Key).ToList());
            this.cubes = null;
            for (int iteration = 0; iteration < iterationCount; iteration++)
            {
                System.Diagnostics.Debug.WriteLine("Splitting " + iteration + "/" + iterationCount);

                SplitCubes(cubes);
            }
            System.Diagnostics.Debug.WriteLine(cubes.Count);
            palette.Clear();
            foreach (var cube in cubes)
            {
                long[] colors = new long[3];
                long total = 0;
                foreach (var color in cube)
                {
                    var factor = colorList[color];
                    colors[0] += color.R * factor;
                    colors[1] += color.G * factor;
                    colors[2] += color.B * factor;
                    total += factor;
                }
                colors[0] /= total;
                colors[1] /= total;
                colors[2] /= total;
                palette.Add(new Models.Color(colors[0], colors[1], colors[2]));
            }
            cubes = null;
            GC.WaitForPendingFinalizers();
        }

        public void SplitCubes(List<List<Color>> cubes)
        {
            List<List<Color>> newCubes = new List<List<Color>>();
            foreach (List<Color> cube in cubes)
            {
                if (cube.Count==1)
                {
                    newCubes.Add(cube);
                    continue;
                }
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
