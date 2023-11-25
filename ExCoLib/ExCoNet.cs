using System;
using System.Collections.Generic;
using System.Linq;

namespace CoLib.ExCo;

[Serializable]
public class ExCoNet : Network
{
  #region Properties
  public List<ExCoUnit> ExCoUnits { get; set; }
  /// <summary>
  /// Hypotheses and beliefs
  /// </summary>
  public List<ExCoUnit> Propositions
  {
    get
    {
      List<ExCoUnit> goals = new List<ExCoUnit>();
      foreach (ExCoUnit e in ExCoUnits)
        if (e.Type == ExCoUnitType.PROPOSITION)
          goals.Add(e);
      return goals;
    }
  }
  /// <summary>
  /// Evidence
  /// </summary>
  public List<ExCoUnit> Data
  {
    get
    {
      List<ExCoUnit> actions = new List<ExCoUnit>();
      foreach (ExCoUnit e in ExCoUnits)
        if (e.Type == ExCoUnitType.DATA)
          actions.Add(e);
      return actions;
    }
  }
  #endregion

  #region Ctor
  public ExCoNet() : base()
  {
    ExCoUnits = new List<ExCoUnit>();
  }
  #endregion

  #region Methods
  public void AddUnit(ExCoUnit unit)
  {
    base.AddUnit(unit);
    ExCoUnits.Add(unit);
  }
  public void explain(Unit a, Unit b)
  {
    cohere(a, b);
  }
  public void explain(Unit a, Unit b, double degree)
  {
    cohere(a, b, degree);
  }
  public void explain(List<Unit> a, Unit b)
  {
    cohere(a, b);
  }
  public void explain(List<Unit> a, Unit b, double degree)
  {
    cohere(a, b, degree);
  }
  public void contradict(Unit a, Unit b)
  {
    incohere(a, b);
  }
  public void contradict(Unit a, Unit b, double degree)
  {
    incohere(a, b, degree);
  }
  public string contradict(List<Unit> a, List<Unit> b, double degree)
  {
    return incohere(a, b, degree);
  }
  #endregion

  #region Helpers
  #endregion

}
