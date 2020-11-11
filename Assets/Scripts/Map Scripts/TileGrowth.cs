using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TileGrowth
{
    public List<Displacement> Equations;

    public TileGrowth()
    {
        Equations = new List<Displacement>();
    }

    public double GetGrowthChance(int x, int y)
    {
        double res = 0;
        Equations.RemoveAll(item => item.transform == null);
        foreach (Displacement D in Equations)
            res += D.calc(x, y);

        return res;
    }
}

public abstract class Displacement
{
    public Transform transform;

    public abstract double calc(int x, int y);
}


public class PointDisplacement: Displacement
{
    double height;
    double smoothing;
    int sign;

    public PointDisplacement(Transform _transform, double _height = 1, double _smoothing = 1 , bool positive = true)
    {
        transform = _transform;
        height = _height;
        smoothing = _smoothing * _smoothing;
        sign = positive ? 1 : -1;
    }

    public override double calc(int x, int y)
    {
        double dx = x - (int)math.floor(transform.position.x);
        double dy = y - (int)math.floor(transform.position.y);

        return height * Mathf.Clamp((float)(sign / (((dx * dx) + (dy * dy)) / smoothing + 1)), -1, 1);
    }
}

public class RaidialDisplacement : Displacement
{
    double height;
    double smoothing;
    int sign;
    double sqrRaduis;

    public RaidialDisplacement(Transform _transform, double _radius, double _height = 1, double _smoothing = 1, bool positive = true)
    {
        transform = _transform;
        sqrRaduis = _radius * _radius;
        height = _height;
        smoothing = _smoothing * _smoothing;
        sign = positive ? 1 : -1;
    }

    public override double calc(int x, int y)
    {
        double dx = x - (int)math.floor(transform.position.x);
        double dy = y - (int)math.floor(transform.position.y);

        double dist = (dx * dx) + (dy * dy) - sqrRaduis;

        return height * Mathf.Clamp((float)(sign / ((dist*dist / smoothing) + 1)), -1, 1);
    }
}


public class RayDisplacement : Displacement
{
    double height;
    double smoothing;
    int sign;
    double sqrRaduis;

    public RayDisplacement(Transform _transform, double _radius, double _height = 1, double _smoothing = 5, bool positive = true)
    {
        transform = _transform;
        sqrRaduis = _radius * _radius;
        height = _height;
        smoothing = _smoothing * _smoothing;
        sign = positive ? 1 : -1;
    }

    public override double calc(int x, int y)
    {
        double dx = x - (int)math.floor(transform.position.x);
        double dy = y - (int)math.floor(transform.position.y);

        double dist = (dx * dx)/ smoothing + (dy * dy)/ 1;

        return height * Mathf.Clamp((float)(sign / (dist + 1)), -1, 1);
    }
}