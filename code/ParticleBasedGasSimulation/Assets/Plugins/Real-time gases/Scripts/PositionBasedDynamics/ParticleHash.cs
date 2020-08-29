using System.Collections.Generic;
using Common.LinearAlgebra;

namespace Plugins.Scripts.PositionBasedDynamics
{
    /// <summary>
    /// Calculates hash values for all particles based on their position, for fast finding of neighbors.
    /// </summary>
    public class ParticleHash
    {
        private class HashEntry
        {
            public int TimeStamp;

            public readonly List<int> Indices;

            public HashEntry(int capacity, int timeStamp)
            {
                Indices = new List<int>(capacity);
                TimeStamp = timeStamp;
            }
        }

        public int[,] Neighbors { get; private set; }

        public List<int> NumNeighbors { get; private set; }

        public int NumParticles { get; set; }

        private int MaxNeighbors { get; set; }

        private int MaxParticles { get; set; }

        private int MaxParticlesPerCell { get; set; }

        private int[,] Indexes { get; set; }

        private double CellSize { get; set; }

        private double InvCellSize { get; set; }

        private int CurrentTimeStamp { get; set; }

        private Dictionary<int, HashEntry> GridMap { get; set; }

        public ParticleHash(double cellSize, int maxParticles, int maxNeighbors = 60,
            int maxParticlesPerCell = 50)
        {
            NumParticles = 0;
            MaxParticlesPerCell = maxParticlesPerCell;
            MaxNeighbors = maxNeighbors;
            MaxParticles = maxParticles;
            CurrentTimeStamp = 0;

            CellSize = cellSize;
            InvCellSize = 1.0 / CellSize;

            GridMap = new Dictionary<int, HashEntry>();

            NumNeighbors = new List<int>();
            Neighbors = new int[MaxParticles, MaxNeighbors];

            Indexes = new int[27, 3];

            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    for (var z = 0; z < 3; z++)
                    {
                        var i = x + y * 3 + z * 9;

                        Indexes[i, 0] = x;
                        Indexes[i, 1] = y;
                        Indexes[i, 2] = z;
                    }
                }
            }
        }

        public void IncrementTimeStamp()
        {
            CurrentTimeStamp++;
        }

        private int Floor(double v)
        {
            return (int) (v * InvCellSize + 32768.1) - 32768;
        }

        private void Floor(Vector3D v, out int pos1, out int pos2, out int pos3)
        {
            pos1 = (int) (v.x * InvCellSize + 32768.1) - 32768;
            pos2 = (int) (v.y * InvCellSize + 32768.1) - 32768;
            pos3 = (int) (v.z * InvCellSize + 32768.1) - 32768;
        }

        private static int Hash(int x, int y, int z)
        {
            var p1 = 73856093 * x;
            var p2 = 19349663 * y;
            var p3 = 83492791 * z;
            return p1 + p2 + p3;
        }

        private int Hash(Vector3D particle)
        {
            var x = (int) (particle.x * InvCellSize + 32768.1) - 32768 + 1;
            var y = (int) (particle.y * InvCellSize + 32768.1) - 32768 + 1;
            var z = (int) (particle.z * InvCellSize + 32768.1) - 32768 + 1;

            var p1 = 73856093 * x;
            var p2 = 19349663 * y;
            var p3 = 83492791 * z;
            return p1 + p2 + p3;
        }

        private void AddToGrid(int i, Vector3D particle)
        {
            var cellPos = Hash(particle);
            HashEntry entry = null;

            if (GridMap.TryGetValue(cellPos, out entry))
            {
                if (entry.TimeStamp != CurrentTimeStamp)
                {
                    entry.TimeStamp = CurrentTimeStamp;
                    entry.Indices.Clear();
                }
            }
            else
            {
                entry = new HashEntry(MaxParticlesPerCell, CurrentTimeStamp);
                GridMap.Add(cellPos, entry);
            }

            entry.Indices.Add(i);
        }

        /// <summary>
        /// Finds all neighbors for all particles.
        /// </summary>
        public void NeighborhoodSearch(Vector3D[] particles)
        {
            var r2 = CellSize * CellSize;

            for (var i = 0; i < NumParticles; i++)
            {
                AddToGrid(i, particles[i]);
            }

            NumNeighbors.Clear();
            for (var i = 0; i < NumParticles; i++)
            {
                NumNeighbors.Add(0);

                var p0 = particles[i];

                int cellPos1, cellPos2, cellPos3;
                Floor(p0, out cellPos1, out cellPos2, out cellPos3);

                for (var j = 0; j < 27; j++)
                {
                    var cellPos = Hash(cellPos1 + Indexes[j, 0], cellPos2 + Indexes[j, 1], cellPos3 + Indexes[j, 2]);

                    HashEntry entry = null;
                    GridMap.TryGetValue(cellPos, out entry);

                    if (entry == null || entry.TimeStamp != CurrentTimeStamp) continue;
                    var count = entry.Indices.Count;
                    for (var m = 0; m < count; m++)
                    {
                        var pi = entry.Indices[m];
                        if (pi == i) continue;

                        Vector3D p;
                        p.x = p0.x - particles[pi].x;
                        p.y = p0.y - particles[pi].y;
                        p.z = p0.z - particles[pi].z;

                        var dist2 = p.x * p.x + p.y * p.y + p.z * p.z;

                        if (!(dist2 < r2)) continue;
                        if (NumNeighbors[i] < MaxNeighbors)
                            Neighbors[i, NumNeighbors[i]++] = pi;
                    }
                }
            }
        }
    }
}