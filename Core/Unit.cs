using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices; // [CallerMemberName]

namespace CoLib;

public class Unit
{
  #region Fields
  // Special Unit connected to this unit - It is created when AddUnit() is called in Network
  public Unit SpecialUnit = null;
  #endregion

  #region Properties
  /// <summary>
  /// Unit name
  /// </summary>
  public string Name { get; set; }
  /// <summary>
  /// Description of the unit.
  /// </summary>
  public string Description { get; set; }
  /// <summary>
  /// Activation potential of the unit
  /// </summary>
  public double Activation { get; set; } // Actual Net Effect
  /// <summary>
  /// Next activation potential of the unit
  /// </summary>
  public double NextActivation { get; set; }
  /// <summary>
  /// Type of the unit
  /// </summary>
  public UnitType Type { get; set; }
  /// <summary>
  /// Unit's weights of all inputs provided as a List
  /// </summary>
  public List<Weight> Weights { get; set; }
  /// <summary>
  /// Priority
  /// </summary>
  private double _Priority = 0.0;
  public double Priority
  {
    get
    {
      if (SpecialUnit != null)
        return SpecialUnit.Activation;
      else
        return _Priority;
    }
    set
    {
      if (value != _Priority)
      {
        _Priority = value;
        if (SpecialUnit != null)
          SpecialUnit.Activation = _Priority;
      }
    }
  }
  #endregion

  #region Ctor
  public Unit(UnitType type, string name, string description, double priority)
  {
    Type = type;
    Name = name;
    Description = description; 
    Activation = 0.0;
    NextActivation = 0.0;
    _Priority = priority;
    // Initalize
    Weights = new List<Weight>();
  }
  public Unit(UnitType type, string name, string description) : this(type, name, description, 0.0)
  {
  }
  public Unit(UnitType type, [CallerMemberName] string name = "") : this(type, name, "", 0.0)
  {
  }
  public Unit([CallerMemberName] string name = "", string description = "", double priority = 0.0) : this(UnitType.UNIT, name, description, priority)
  {
  }

  public Unit(Unit unit)
    : this(unit.Type, unit.Name, unit.Description, unit.Priority)
  {
    Activation = unit.Activation;
    NextActivation = unit.NextActivation;
  }
  #endregion

  #region Methods
  public void AddWeight(Unit unit, double value)
  {
    bool haveWeight = false;

    foreach (var weight in Weights)
    {
      if (unit.Name.Equals(weight.Unit.Name))
      {
        weight.Value = weight.Value + value;
        haveWeight = true;
      }
    }
    if (!haveWeight)
    {
      Weight weight = new Weight(unit, value);
      if (!Weights.Contains(weight))
        Weights.Add(weight);
    }
  }
  public void InitializeWeight()
  {
    Weights.Clear();
  }
  #endregion

  #region Extended Methods
  /// <summary>
  /// Computes output value of a unit using McClelland Activation Update Rule.
  /// </summary>
  public double Compute()
  {
    double net = 0.0;
    double dAct = 0.0;

    // Compute Net Input
    net = ComputeNetInput();

    if (net > 0)
    {
      dAct = net * (Parameters.MAXACT - Activation) - (Parameters.THETA * Activation); // Change in activation
    }
    else
    {
      dAct = net * (Activation - Parameters.MINACT) - (Parameters.THETA * Activation); // Change in activation
    }
    NextActivation = Activation + dAct;// Next activation: ai(t+1)

    // Guards
    if (NextActivation > Parameters.MAXACT) NextActivation = Parameters.MAXACT;
    else if (NextActivation < Parameters.MINACT) NextActivation = Parameters.MINACT;

    return NextActivation;
  }

  /// <summary>
  /// Computes the weighted sum of inputs.
  /// </summary>
  public double ComputeNetInput()
  {
    double net = 0.0;
    // Compute Net Input

    // Weighted sum of inputs: activation = Sum[weight*input]
    foreach (Weight weight in Weights)
      net += weight.Value * (weight.Unit.Activation);

    //// Weighted production of inputs: net = Product[weight*input]
    //foreach (Weight weight in Weights)
    //  net *= weight.Value * (weight.Unit.Activation);

    return net;
  }

  // Get weight value between this neuron and target neuron.
  // It returns 0, if there is no link among them
  public double GetWeightValueBetween(Unit target)
  {
    double value = 0.0;
    foreach (var weight in Weights)
    {
      // Assumes unique neuron names
      if (weight.Unit.Name == target.Name)
      {
        value = weight.Value;
        break;
      }
    }

    return value;
  }
  #endregion

  #region Helper Methods
  public string PrintWeights(String header = "Weights for ")
  {
    StringBuilder log = new StringBuilder();
    log.Append(header + Name + ":" + Environment.NewLine);
    foreach (Weight weight in Weights)
    {
      log.Append("From " + weight.Unit.Name + " = " + weight.Value + "\n");
    }
    //log.Append("\n");
    return log.ToString();
  }

  public override bool Equals(object obj)
  {
    var item = obj as Unit;

    if (item == null)
    {
      return false;
    }
    return Name.Equals(item.Name) && Description.Equals(item.Description) && (Activation == item.Activation);
  }

  public override int GetHashCode()
  {
    return Name.GetHashCode();
  }

  /// <summary>
  /// Returns the name, description, and activation of the unit.
  /// </summary>
  /// <returns></returns>
  override public string ToString()
  {
    return $"Name_{Name}, Description_{Description}, Activation_{Activation}";
  }

  #endregion


}
