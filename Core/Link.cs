using System;
using System.Collections.Generic;
using System.Linq;

namespace CoLib;

public enum LinkType
{
  COHERE, INCOHERE
}

public class Link
{
  public LinkType Type { get; set; }
  public double Degree { get; set; }

  public List<Unit> Units { get; set; }

  private Link()
  {
    Units = new List<Unit>();
    Type = LinkType.COHERE;
    Degree = 1;
  }

  public Link(LinkType type)
    : this()
  {
    Type = type;
  }

  public Link(List<Unit> units, LinkType type, double degree)
    : this(type)
  {
    Units = units;
    Degree = degree;
  }

  public Link(List<Unit> units, LinkType type, double degree, double weight)
    : this(units, type, degree)
  {
    setWeights(weight, true);
  }

  public bool haveUnits(List<Unit> units)
  {
    bool ret = true;
    List<Unit> us = new List<Unit>();
    us = Units;

    for (int i = 0; i < units.Count(); i++)
    {
      bool hasUnit = false;
      for (int j = 0; j < us.Count(); j++)
      {
        if (units.ElementAt(i).Name.Equals(us.ElementAt(j).Name))
        {
          hasUnit = true;
        }
      }
      if (!hasUnit)
      {
        ret = false;
        break;
      }
    }
    return ret;
  }

  public bool hasSpecialUnit()
  {
    foreach (var unit in Units)
    {
      if (unit.Type == UnitType.SU)
        return true;
    }
    return false;
  }

  public void setWeights(double w, bool divWeight)
  {
    var last = Units.Last();

    // divide value among Links?
    if (divWeight)
    {
      w = (double) (w / (double)(Units.Count() - 1));
    }
    for (int i = 0; i < Units.Count() - 1; i++)
    {
      // add symmetric weights
      Units.ElementAt(i).AddWeight(last, w * Degree);
      last.AddWeight(Units.ElementAt(i), w * Degree);

      // if there are multiple Units in the link, establish Links between pairs
      for (int j = i + 1; j < Units.Count() - 1; j++)
      {
        Units.ElementAt(i).AddWeight(Units.ElementAt(j), w * Degree);
        Units.ElementAt(j).AddWeight(Units.ElementAt(i), w * Degree);
      }
    }
  }

  public bool IsFacilitate()
  {
    return (Type == LinkType.COHERE);
  }

  public bool IsIncompatible()
  {
    return (Type == LinkType.INCOHERE);
  }

  override public string ToString()
  {
    string tmp = "";
    foreach (var item in Units)
    {
      tmp += item.Name + "-";
    }
    tmp = tmp.Remove(tmp.Length - 1); // remove '-' at the end.
    return tmp;
  }
}
