using System;
using System.Collections.Generic;
using System.Linq;

namespace CoLib.DeCo;

[Serializable]
public class DeCoNet : Network
{
  #region Properties
  public List<DeCoUnit> DeCoUnits { get; set; }
  public List<DeCoUnit> Goals
  {
    get
    {
      List<DeCoUnit> goals = new List<DeCoUnit>();
      foreach (DeCoUnit e in DeCoUnits)
        if (e.Type == DeCoUnitType.GOAL)
          goals.Add(e);
      return goals;
    }
  }
  public List<DeCoUnit> Actions
  {
    get
    {
      List<DeCoUnit> actions = new List<DeCoUnit>();
      foreach (DeCoUnit e in DeCoUnits)
        if (e.Type == DeCoUnitType.ACTION)
          actions.Add(e);
      return actions;
    }
  }
  #endregion

  #region Ctor
  public DeCoNet() : base()
  {
    DeCoUnits = new List<DeCoUnit>();
  }
  #endregion

  #region Methods
  public void AddUnit(DeCoUnit unit)
  {
    base.AddUnit(unit);
    DeCoUnits.Add(unit);
  }

  public void facilitate(Unit a, Unit b)
  {
    base.cohere(a, b);
  }
  public void facilitate(Unit a, Unit b, double degree)
  {
    base.cohere(a, b, degree);
  }
  public void facilitate(List<Unit> a, Unit b)
  {
    base.cohere(a, b);
  }
  public void facilitate(List<Unit> a, Unit b, double degree)
  {
    base.cohere(a, b, degree);
  }
  public void incompatible(Unit a, Unit b)
  {
    base.incohere(a, b);
  }
  public void incompatible(Unit a, Unit b, double degree)
  {
    base.incohere(a, b, degree);
  }
  public void incompatible(List<Unit> a, List<Unit> b, double degree)
  {
    base.incohere(a, b, degree);
  }
  #endregion

  #region Helpers
  #endregion

}
