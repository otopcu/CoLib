using System;
using System.Collections.Generic;
using System.Linq;

namespace CoLib;

// Type aliases for Environment
public class InputPatterns : Dictionary<string, double> { };
public class OutputPatterns : Dictionary<string, double> { };

public enum ActivationUpdateMethods
{
  McClelland,
  Grossberg
}

[Serializable]
public abstract class Network
{
  #region Fields
  // Connectionist Network Run Parameters
  internal int Iteration = 0; // Computation iteration
  #endregion

  #region Properties
  /// <summary>
  /// Network's name
  /// </summary>
  public string Name { get; set; }
  /// <summary>
  /// All units including the special units
  /// </summary>
  public List<Unit> Units { get; set; }
  /// <summary>
  /// Special units - special units (inputs)
  /// </summary>
  public List<Unit> SpecialUnits
  {
    get
    {
      List<Unit> units = new List<Unit>();
      foreach (Unit unit in Units)
        if (unit.Type == UnitType.SU)
          units.Add(unit);
      return units;
    }
  }
  /// <summary>
  /// Computational units - other units than special units 
  /// </summary>
  public List<Unit> ComputationalUnits
  {
    get
    {
      List<Unit> units = new List<Unit>();
      foreach (Unit unit in Units)
        if (unit.Type != UnitType.SU)
          units.Add(unit);
      return units;
    }
  }
  /// <summary>
  /// All links
  /// </summary>
  public List<Link> Links { get; set; }
  public List<Link> InhibitoryLinks
  {
    get
    {
      List<Link> inhibitoryLinks = new List<Link>();

      foreach (Link link in Links)
        if (link.Type == LinkType.INCOHERE)
          inhibitoryLinks.Add(link);
      return inhibitoryLinks;
    }
  }
  public List<Link> ExcitatoryLinks
  {
    get
    {
      List<Link> excitatoryLinks = new List<Link>();

      foreach (Link link in Links)
      {
        if (link.Type == LinkType.COHERE)
        {
          excitatoryLinks.Add(link);
        }
      }
      return excitatoryLinks;
    }
  }
  //public ActivationUpdateMethods SelectedActivationUpdateRule { get; set; }
  // Environment is a collection of pattern pairs
  public Dictionary<InputPatterns, OutputPatterns> PatternEnvironment { get; set; }
  #endregion

  #region Ctor
  public Network()
  {
    Units = new List<Unit>();
    PatternEnvironment = new Dictionary<InputPatterns, OutputPatterns>();
    Links = new List<Link>();
    Name = "";
  }
  #endregion

  #region Methods
  /// <summary>
  /// Adds a unit to the coherence network. If unit priority is not 0, then create a special unit (i.e. input unit).
  /// </summary>
  /// <param name="unit"></param>
  public void AddUnit(Unit unit)
  {
    if (!Units.Contains(unit))
    {
      Units.Add(unit);

      // Add Special Unit
      if (unit.Priority != 0)
      {
        Unit su = new Unit(UnitType.SU, "SU_" + unit.Name);
        su.Activation = unit.Priority;
        unit.SpecialUnit = su;
        Units.Add(su);
        cohere(su, unit);
      }
    }
  }
  public void RemoveUnit(Unit unit)
  {
    if (Units.Contains(unit))
      Units.Remove(unit);
  }
  public void SetInhibitoryLinkWeights(double inhibition, bool divWeight)
  {
    foreach (Link link in InhibitoryLinks)
    {
      link.setWeights(inhibition, divWeight);
    }
  }
  public void SetExcitatoryLinkWeights(double excitation, bool divWeight)
  {
    foreach (Link link in ExcitatoryLinks)
    {
      if (link.hasSpecialUnit())
      {
        link.setWeights(Parameters.SU_EXCITATION, divWeight);
      }
      else
      {
        link.setWeights(excitation, divWeight);
      }
    }
  }
  public void AddLink(Link link)
  {
    if (!Links.Contains(link))
      Links.Add(link);
  }
  public void RemoveLink(Link link)
  {
    if (Links.Contains(link))
      Links.Remove(link);
  }
  public bool hasLink(List<Unit> units)
  {
    bool ret = false;
    List<Link> links = Links;
    for (int i = 0; i < links.Count(); i++)
    {
      if (links.ElementAt(i).haveUnits(units))
      {
        ret = true;
      }
    }
    return ret;
  }
  protected void UpdateActivationsUsingMcClellandRule(List<Unit> units)
  {
    double change = 99;
    // PHASE-I: Calculate nets for each unit
    for (Iteration = 1; ((Iteration < Parameters.MAX_ITERATIONS) && (change > Parameters.STOP_VALUE)); Iteration++)
    {
      change = 0.0;
      foreach (var unit in units)
      {
        unit.NextActivation = unit.Compute();
        change = (Math.Max(Math.Abs(unit.Activation - unit.NextActivation), change));
      }
      // PHASE-II: Set Activations
      foreach (var unit in units)
        unit.Activation = unit.NextActivation;
    }
  }
  protected void UpdateActivationsUsingGrossbergRule(List<Unit> units)
  {
    double change = 99, excit = 0, inhib = 0, dAct = 0;
    // Start iteration
    for (Iteration = 1; ((Iteration < Parameters.MAX_ITERATIONS) && (change > Parameters.STOP_VALUE)); Iteration++)
    {
      change = 0.0;
      var desirability = 0.0;
      foreach (var unit in units)
      {
        excit = 0.0; inhib = 0.0;
        foreach (Weight wij in unit.Weights)
        {
          if (wij.Value > desirability) excit += wij.Value * (wij.Unit.Activation);// Net unit of Excitatory links
          else inhib += wij.Value * (wij.Unit.Activation);// Net unit of inhib links
        }
        dAct = (Parameters.MAXACT - unit.Activation) * excit - (unit.Activation - Parameters.MINACT) * inhib - (Parameters.THETA * unit.Activation); // Change in activation
        unit.NextActivation = unit.Activation + dAct;// Next activation: ai(t+1)
        // Guards
        if (unit.NextActivation > Parameters.MAXACT) unit.NextActivation = Parameters.MAXACT;
        else if (unit.NextActivation < Parameters.MINACT) unit.NextActivation = Parameters.MINACT;

        change = (Math.Max(Math.Abs(unit.Activation - unit.NextActivation), change));
      }
      // Set Activations
      foreach (var unit in units)
        unit.Activation = unit.NextActivation;
    }
  }
  public void setActivations(double defActPot)
  {
    foreach (var unit in Units)
    {
      if (unit.Type != UnitType.SU)
      {
        unit.Activation = defActPot;
      }
    }
  }
  public void ResetWeights()
  {
    foreach (var unit in Units)
    {
      unit.InitializeWeight();
    }
  }
  internal void cohere(Unit a, Unit b)
  {
    cohere(a, b, 1);
  }
  internal void cohere(Unit a, Unit b, double degree)
  {
    if (Units.Contains(a) && Units.Contains(b))
    {
      List<Unit> us = new List<Unit>();
      us.Add(a);
      us.Add(b);
      AddLink(new Link(us, LinkType.COHERE, degree));
    }
  }
  internal void cohere(List<Unit> a, Unit b)
  {
    cohere(a, b, 1);
  }
  internal void cohere(List<Unit> a, Unit b, double degree)
  {
    List<Unit> us = new List<Unit>();

    for (int i = 0; i < a.Count(); i++)
    {
      if (Units.Contains(a.ElementAt(i)))
      {
        us.Add(a.ElementAt(i));
      }
      else
        return;
    }

    if (Units.Contains(b))
    {
      us.Add(b);
    }
    else
      return;

    AddLink(new Link(us, LinkType.COHERE, degree));
  }
  internal void incohere(Unit a, Unit b)
  {
    incohere(a, b, 1);
  }
  internal void incohere(Unit a, Unit b, double degree)
  {
    if (Units.Contains(a) && Units.Contains(b))
    {
      List<Unit> us = new List<Unit>();
      us.Add(a);
      us.Add(b);
      AddLink(new Link(us, LinkType.INCOHERE, degree, Parameters.INHIBITION));
    }
  }
  internal string incohere(List<Unit> a, List<Unit> b, double degree)
  {
    string result = "";
    List<Unit> us = new List<Unit>();
    List<Unit> bs = new List<Unit>();
    for (int i = 0; i < a.Count(); i++)
    {
      if (Units.Contains(a.ElementAt(i)))
      {
        us.Add(a.ElementAt(i));
      }
    }
    for (int i = 0; i < b.Count(); i++)
    {
      if (Units.Contains(b.ElementAt(i)))
      {
        bs.Add(b.ElementAt(i));
      }
    }

    int nodeNumber = us.Count() + bs.Count();
    //result += nodeNumber;
    for (int i = 0; i < us.Count(); i++)
    {
      for (int j = 0; j < bs.Count(); j++)
      {
        List<Unit> uns = new List<Unit>();
        uns.Add(us.ElementAt(i));
        uns.Add(bs.ElementAt(j));
        
        if (!hasLink(uns) && !(us.ElementAt(i).Name.Equals(bs.ElementAt(j).Name)))
        {
          //AddLink(new Link(uns, LinkType.INCOHERE, 1, degree / (nodeNumber / 2)));
          AddLink(new Link(uns, LinkType.INCOHERE, degree, Parameters.INHIBITION / (nodeNumber / 2)));
          result += uns.ElementAt(0).Name + " to " + uns.ElementAt(1).Name + ". ";
        }
      }
    }
    return result;
  }
  public void generateIncoherence(double impact)
  {
    foreach (var link in InhibitoryLinks)
    {
      generateIncoherence(link, impact);
    }
  }
  protected void generateIncoherence(Link link, double impact)
  {
    Unit a;
    List<Unit> us;
    Unit b;
    List<Unit> bs;
    if (link.Units.Count() == 2)
    {
      a = link.Units.ElementAt(0);
      b = link.Units.ElementAt(1);
      us = getCoheredUnits(a);
      bs = getCoheredUnits(b);

      if (us.Count() >= 1 && bs.Count() >= 1)
      {
        incohere(us, bs, impact);
      }
    }
  }
  private List<Unit> getCoheredUnits(Unit unit)
  {
    List<Unit> coheredUnits = new List<Unit>();
    for (int i = 0; i < ExcitatoryLinks.Count(); i++)
    {
      List<Unit> units = (ExcitatoryLinks.ElementAt(i)).Units;
      bool addResult = false;
      for (int j = 0; j < units.Count(); j++)
      {
        if (unit.Name.Equals(units.ElementAt(j).Name))
        {
          addResult = true;
        }
        else if (addResult)
        {
          coheredUnits.Add(units.ElementAt(j));
        }
      }
    }
    return coheredUnits;
  }
  public Unit FindUnitByName(string name)
  {
    Unit neuron = new Unit();
    foreach (var unit in Units)
    {
      if (name == unit.Name)
      {
        return unit;
      }
    }
    return null; // Not found
  }

  /// <summary>
  /// Compute the activation potential of each unit in DeCoNet according to the activation update method: McClelland or Grossberg.
  /// </summary>
  public void Compute()
  {
    switch (Parameters.ActivationUpdateRule)
    {
      case ActivationUpdateMethods.McClelland:
        UpdateActivationsUsingMcClellandRule(GetComputationalUnits());
        break;
      case ActivationUpdateMethods.Grossberg:
        UpdateActivationsUsingGrossbergRule(GetComputationalUnits());
        break;
      default:
        break;
    }
  }
  #endregion

  #region Helpers
  /// <summary>
  /// Get all units except special units
  /// </summary>
  public List<Unit> GetComputationalUnits()
  {
    List<Unit> us = new List<Unit>();
    foreach (var unit in Units)
    {
      if (unit.Type != UnitType.SU)
        us.Add(unit);
    }
    return us;
  }
  /// <summary>
  /// Prints inputs (special units - SUs)
  /// </summary>
  /// <returns>Returns the list of input units as string</returns>
  public string PrintSpecialUnits()
  {
    string str = "Special Units:\n";
    foreach (var input in SpecialUnits)
    {
      str += input.Name + "= " + input.Activation + Environment.NewLine;
    }
    return str;
  }
  /// <summary>
  /// Prints outputs (units other than SUs)
  /// </summary>
  /// <returns>Returns the list of output units as string</returns>
  public string PrintComputationalUnits()
  {
    string str = "Computational Units:\n";
    foreach (var output in ComputationalUnits)
    {
      str += output.Name + "= " + output.Activation + Environment.NewLine;
    }
    return str;
  }
  public string PrintTrainingData()
  {
    string output = "";
    int i = 0;
    foreach (var data in PatternEnvironment)
    {
      // Print inputList value
      foreach (var item in data.Key.ToList())
      {
        output += ++i + " " + item.ToString() + " ";
      }
      output += ":\t";
      // Print Data value
      foreach (var item in data.Value.ToList())
      {
        output += item.ToString() + " ";
      }
      output += Environment.NewLine;
    }
    return output;
  }
  public void ShuffleTestData()
  {
    Random rand = new Random();
    PatternEnvironment = PatternEnvironment.OrderBy(x => rand.Next())
      .ToDictionary(item => item.Key, item => item.Value);
  }
  public virtual void GenerateTrainData(int total) { }
  #endregion
}
