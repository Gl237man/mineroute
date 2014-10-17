using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnetsynt3
{
    class WireGroup
    {
        public List<Wire> WList;
        public string GroupName;
        public bool Placed;
        public int weight;
        public bool CanPlace;

        internal void PlaceRepitors()
        {
            WList = WList.OrderBy(t => t.WirePoints.Count).ToList();
            WList.Reverse();
            foreach (var wire in WList)
            {
                int tPointIndex = 0;
                int lastRepLen = 0;
                while (tPointIndex < wire.WirePoints.Count)
                {
                    if(wire.WirePoints[tPointIndex].Repiter)
                    {
                        lastRepLen = 0;
                    }
                    if (lastRepLen < 15)
                    {
                        lastRepLen++;
                        tPointIndex++;
                    }
                    else
                    {
                        bool canplace = CanRepPlace(wire.WirePoints[tPointIndex].x, wire.WirePoints[tPointIndex].y);
                        if (canplace)
                        {
                            RepPlace(wire.WirePoints[tPointIndex].x, wire.WirePoints[tPointIndex].y);
                            lastRepLen = 0;
                        }
                        else
                        {
                            tPointIndex = RepPlaceBack(tPointIndex,wire);
                        }
                    }
                }
                //Заполнение направлений репитеров
                for (int i = 1; i < wire.WirePoints.Count; i++)
                {
                    if (wire.WirePoints[i].Repiter)
                    {
                        if (wire.WirePoints[i].x > wire.WirePoints[i - 1].x) wire.WirePoints[i].RepVapl = ">";
                        if (wire.WirePoints[i].x < wire.WirePoints[i - 1].x) wire.WirePoints[i].RepVapl = "<";
                        if (wire.WirePoints[i].y > wire.WirePoints[i - 1].y) wire.WirePoints[i].RepVapl = "^";
                        if (wire.WirePoints[i].y < wire.WirePoints[i - 1].y) wire.WirePoints[i].RepVapl = "v";
                    }
                }
            }

        }

        private int RepPlaceBack(int tPointIndex, Wire wire)
        {
            bool placed = false;
            while (!placed)
            {
                tPointIndex--;
                bool canplace = CanRepPlace(wire.WirePoints[tPointIndex].x, wire.WirePoints[tPointIndex].y);
                if (canplace)
                {
                    placed = true;
                    RepPlace(wire.WirePoints[tPointIndex].x, wire.WirePoints[tPointIndex].y);
                }
            }
            return tPointIndex;
        }

        private void RepPlace(int x, int y)
        {
            var totalwirepoints = new List<WirePoint>();
            foreach (var wire in WList)
            {
                totalwirepoints.AddRange(wire.WirePoints);
            }
            var points = totalwirepoints.Where(t => t.x == x && t.y == y).ToList();
            foreach (var point in points)
            {
                point.Repiter = true;
            }
        }

        private bool CanRepPlace(int x, int y)
        {
            var totalwirepoints = new List<WirePoint>();
            foreach (var wire in WList)
            {
                totalwirepoints.AddRange(wire.WirePoints);
            }
            var upPoint = totalwirepoints.Any(t => t.x == x && t.y == y + 1);
            var downPoint = totalwirepoints.Any(t => t.x == x && t.y == y - 1);
            var leftPoint = totalwirepoints.Any(t => t.x == x + 1 && t.y == y);
            var rightPoint = totalwirepoints.Any(t => t.x == x - 1 && t.y == y);
            if (upPoint && downPoint && !leftPoint && !rightPoint) return true;
            if (!upPoint && !downPoint && leftPoint && rightPoint) return true;
            return false;
        }
    }
}
