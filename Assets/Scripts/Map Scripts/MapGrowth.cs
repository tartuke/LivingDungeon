using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Map
{
    public class TileGrowth
    {
        public List<Displacement> Equations;

        public TileGrowth()
        {
            Equations = new List<Displacement>();
        }

        public double2 GetGrowthChance(int x, int y)
        {
            double res = 0;
            double pV = 0;
            double p;
            double sumP = 0;
            Equations.RemoveAll(item => item.transform == null);
            foreach (Displacement D in Equations)
            {
                p = D.calc(x, y);
                sumP += p;
                res += p * p * D.target;
                pV += (1 - p) * p;
            }
            return new double2(pV / sumP, res / sumP);
        }
    }

    public abstract class Displacement
    {
        public Transform transform;
        public double target;
        public abstract double calc(int x, int y);
    }


    public class PointDisplacement : Displacement
    {
        double height;
        double smoothing;

        public PointDisplacement(Transform _transform, double _height = 1, double _smoothing = 1, double _target = 1)
        {
            transform = _transform;
            height = _height;
            smoothing = _smoothing * _smoothing;
            target = _target;

        }

        public override double calc(int x, int y)
        {
            double dx = x - (int)math.floor(transform.position.x);
            double dy = y - (int)math.floor(transform.position.y);

            return Mathf.Clamp((float)height * (float)(1 / (((dx * dx) + (dy * dy)) / smoothing + 1)), -1, 1);
        }
    }

    public class RaidialDisplacement : Displacement
    {
        double height;
        double smoothing;
        double sqrRaduis;

        public RaidialDisplacement(Transform _transform, double _radius, double _height = 1, double _smoothing = 1, double _target = 1)
        {
            transform = _transform;
            sqrRaduis = _radius * _radius;
            height = _height;
            smoothing = _smoothing * _smoothing;
            target = _target;
        }

        public override double calc(int x, int y)
        {
            double dx = x - (int)math.floor(transform.position.x);
            double dy = y - (int)math.floor(transform.position.y);

            double dist = (dx * dx) + (dy * dy) - sqrRaduis;

            return Mathf.Clamp((float)height * (float)(1 / ((dist * dist / smoothing) + 1)), -1, 1);
        }
    }


    public class RayDisplacement : Displacement
    {
        double height;
        double smoothing;
        double sqrRaduis;

        public RayDisplacement(Transform _transform, double _radius, double _height = 1, double _smoothing = 5, double _target = 1)
        {
            transform = _transform;
            sqrRaduis = _radius * _radius;
            height = _height;
            smoothing = _smoothing * _smoothing;
            target = _target;
        }

        public override double calc(int x, int y)
        {
            double dx = x - (int)math.floor(transform.position.x);
            double dy = y - (int)math.floor(transform.position.y);

            double dist = (dx * dx) / smoothing + (dy * dy) / 1;

            return Mathf.Clamp((float)height * (float)(1 / (dist + 1)), -1, 1);
        }
    }
}