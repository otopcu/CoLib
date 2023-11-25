using System;
using System.Text;
using System.Collections.Generic;

namespace CoLib;

public class Interpreter
{
  protected double systemCoherence = 0;

  #region Properties
  public Network Model;
  /// <summary>
  /// Accepted units - units which have higher activations than desirability 
  /// </summary>
  public List<Unit> Acceptance
  {
    get
    {
      List<Unit> units = new List<Unit>();
      foreach (var unit in Model.GetComputationalUnits())
      {
        if (unit.Activation >= Parameters.DESIRABILITY)
          units.Add(unit);
      }
      return units;
    }
  }
  /// <summary>
  /// Rejected units - units which have lower activations than desirability 
  /// </summary>
  public List<Unit> Rejection
  {
    get
    {
      List<Unit> units = new List<Unit>();
      foreach (var unit in Model.GetComputationalUnits())
      {
        if (unit.Activation < Parameters.DESIRABILITY)
          units.Add(unit);
      }
      return units;
    }
  }
  #endregion

  #region Constructor
  public Interpreter()
  {
  }
  public Interpreter(Network model)
  {
    Model = model;
  }
  #endregion

  #region Methods
  /// <summary>
  /// Initializes the coherence network - inital values are set for activations and weights.
  /// </summary>
  /// <returns></returns>
  public string Initialize()
  {
    StringBuilder log = new StringBuilder();
    #region Initialization
    log.Append("***********************************************************\n");
    log.Append("NETWORK INITIALIZATION\n");
    log.Append("***********************************************************\n");
    // Set initial values
    log.Append("Running simulation with parameters:\n");
    log.Append("Start Value (default activation potential value) = ");
    log.Append(Parameters.START_VALUE);
    log.Append("\nStop Value (asymptote criteria) = ");
    log.Append(Parameters.STOP_VALUE);
    log.Append("\nDesirability = ");
    log.Append(Parameters.DESIRABILITY);
    log.Append("\nExcitation = ");
    log.Append(Parameters.EXCITATION);
    log.Append("\nInhibition = ");
    log.Append(Parameters.INHIBITION);
    log.Append("\nSpecial Unit Excitation = ");
    log.Append(Parameters.SU_EXCITATION);
    log.Append("\nDecay = ");
    log.Append(Parameters.THETA);
    log.Append("\n\n");
    // Report Units
    log.Append(printUnits());
    // Report Special Units
    log.Append(Model.PrintSpecialUnits() + "\n");

    // Set Default Activations
    log.Append(InitializeActivations());
    // Set Initial Weights
    log.Append(InitializeWeights());
    
    // Report Computational Units
    log.Append(Model.PrintComputationalUnits() + "\n");

    // Calculate Upper Level Incoherence
    //Model.generateIncoherence(Parameters.INHIBITION);// Generates upper level incoherences

    #endregion
    return log.ToString();
  }
  public string Run()
  {
    StringBuilder log = new StringBuilder();

    //// Initialization
    //log.Append(initialize(excitation, inhibition, goalExcitation, decay));

    //// Set test Input
    //foreach (var unit in model.Units)
    //  if (unit.Type == UnitType.SU)
    //    unit.Activation = unit.TestValue;
    
    // Update Activation Potentials
    log.Append(execute());
    // Compute Systems Coherence
    log.Append(ComputeSystemCoherence());
    return log.ToString();
  }
  protected string InitializeActivations()
  {
    StringBuilder log = new StringBuilder();

    // Set Default Activations
    Model.setActivations(Parameters.START_VALUE); //Set default Activations Except Special Units
    log.Append(printAllActivations("Initial Activation Potentials:"));

    return log.ToString();
  }
  protected string InitializeWeights()
  {
    StringBuilder log = new StringBuilder();

    // Set Initial Weights
    //Model.ResetWeights();
    Model.SetExcitatoryLinkWeights(Parameters.EXCITATION, true);
    //Model.SetInhibitoryLinkWeights(Parameters.INHIBITION, true);
    log.Append(printWeights("Initial Weights:"));

    return log.ToString();
  }    
  
  // Sync Update: nothing is done with the new activation of any of the units until all have been updated.
  //public String UpdateActivationsUsingMcClellandRule()
  protected string execute()
  {
    StringBuilder log = new StringBuilder();
    log.Append("***********************************************************\n");
    log.Append("NETWORK RUN using " + ToString() + "\n");
    log.Append("***********************************************************\n");
    log.Append("Updating activations using "+ Parameters.ActivationUpdateRule.ToString() + " rule:\n\n");
    //log.Append(printAllActivations("Initial Activation Potentials:"));

    Model.Compute();

    #region Report Simulation Results
    // Report Simulation Results
    log.Append("Simulation finished. Iterations = ");
    log.Append(Model.Iteration);
    log.Append(" (max iterations = ");
    log.Append(Parameters.MAX_ITERATIONS + ")");
    log.Append("\n\n");
    log.Append(printActivations());
    // Report accepted set
    string accepted = "Acceptance = { ";
    foreach (var unit in Acceptance)
        accepted += unit.Name + " ";
    log.Append(accepted + "}\n");
    // Report rejected set
    string rejected = "Rejection  = { ";
    foreach (var unit in Rejection)
      rejected += unit.Name + " ";
    log.Append(rejected + "}\n\n");
    // Report descriptions for accepted set
    log.Append(printDescriptions(Acceptance) + "\n");
    #endregion

    return log.ToString();
  }

  protected string ComputeSystemCoherence()
  {
    StringBuilder log = new StringBuilder();
    #region System Coherence
    systemCoherence = 0.0;
    // System Coherence: Global coherence of a whole system of units at time t
    // aka Goodness of Fit
    // (energy'nin tersi), or harmony of the network
    // Increases w/ number of Units, number of Links, number of cycles
    log.Append("***********************************************************\n");
    log.Append("SYSTEM COHERENCE (Goodness of Fit)\n");
    log.Append("***********************************************************\n");
    //  Hopfield Energy Function: This corresponds to the intuitive idea that stable states have a low energy. 
    // Divide system coherence by 2 as ALL links (SUs are included) are computed twice. 
    foreach (var unit in Model.Units)// Get all units. Special units are also included.
    {
      //log.Append("Unit " + unit.Name + " = ");
      foreach (var weight in unit.Weights)
      {
        systemCoherence += (weight.Value * unit.Activation * weight.Unit.Activation) / 2;
        //log.Append(weight.Value + " * " + unit.Activation + " * " + weight.Unit.Activation + " = " + (weight.Value * unit.Activation * weight.Unit.Activation) + "\n");
      }
    }
    log.Append("System Coherence (H) = " + systemCoherence + "\n\n");

    // Node Number
    log.Append("1. H / # of Units = " + systemCoherence / Model.Units.Count + "\n");
    log.Append("# of Units = " + Model.Units.Count + "\n");
    log.Append("Units : ");
    foreach (var item in Model.Units)
      log.Append(item.Name + ", ");
    log.Append("\n\n");

    // Link Number
    log.Append("2. H / # of Links = " + systemCoherence / Model.Links.Count + "\n");
    log.Append("# of Links = " + Model.Links.Count + "\n");
    log.Append("All Links :\n");
    foreach (var item in Model.Links)
      log.Append(item.ToString() + "\n");
    log.Append("\nExcitatory Links :\n");
    foreach (var item in Model.ExcitatoryLinks)
      log.Append(item.ToString() + "\n");
    log.Append("\nInhibitory Links :\n");
    foreach (var item in Model.InhibitoryLinks)
      log.Append(item.ToString() + "\n");
    log.Append("\n\n");

    // W/W*
    double W_Star = 0; // Sum of the weights of all constraints
    double W = 0; // Sum of the weights of all *satisfied* contraints
    foreach (var unit in Model.Units)
    {
      foreach (var weight in unit.Weights)
      {
        // All constraints
        W_Star += Math.Abs(weight.Value);
        // Satisfied Constraints
        // (1) constraint (ai, aj) is in C+ (excitatory link) set and both units are accepted 
        if ((weight.Value >= 0) && (unit.Activation >= Parameters.DESIRABILITY) && (weight.Unit.Activation >= Parameters.DESIRABILITY))
          W += weight.Value;
        // (2) constraint (ai, aj) is in C+ set and both units are rejected 
        else if ((weight.Value >= 0) && (unit.Activation < Parameters.DESIRABILITY) && (weight.Unit.Activation < Parameters.DESIRABILITY))
          W += weight.Value;
        // (3) constraint (ai, aj) is in C- (inhibitory link) set and both units are rejected 
        else if ((weight.Value < 0) && (((unit.Activation >= Parameters.DESIRABILITY) && (weight.Unit.Activation < Parameters.DESIRABILITY)) || ((unit.Activation < Parameters.DESIRABILITY) && (weight.Unit.Activation >= Parameters.DESIRABILITY))))
          W += Math.Abs(weight.Value);
      }
    }
    // Divide the weights by 2 as they are counted twice
    W_Star /= 2;
    W /= 2;
    log.Append("3. W / W* = " + W / W_Star + "\n");
    log.Append("W* = " + W_Star + ", W = " + W + "\n");
    log.Append("\n\n");
    #endregion
    return log.ToString();
  }
  #endregion

  #region Helper Methods
  protected string printUnits(string header = "Units:")
  {
    StringBuilder log = new StringBuilder();
    log.Append(header + "\n");
    foreach (var unit in Model.GetComputationalUnits())
    {
      string priority = (unit.SpecialUnit != null) ? " (priority: " + unit.SpecialUnit.Activation + ")" : "";
      log.Append(unit.Name + priority + Environment.NewLine);
    }
    log.Append("\n");
    return log.ToString();
  }
  protected string printWeights(string header = "Weights:")
  {
    StringBuilder log = new StringBuilder();
    log.Append(header + "\n");
    foreach (var unit in Model.GetComputationalUnits())
    {
      foreach (Weight weight in unit.Weights)
      {
        log.Append(unit.Name + " from " + weight.Unit.Name + " = " + weight.Value + "\n");
      }
    }
    log.Append("\n");
    return log.ToString();
  }
  protected string printAllActivations(string header = "Activation Potentials:")
  {
    StringBuilder log = new StringBuilder();
    log.Append(header + "\n");
    foreach (var unit in Model.Units)
      log.Append(unit.Name + " = " + unit.Activation + "\n");
    log.Append("\n");
    return log.ToString();
  }
  /// <summary>
  /// Activation potentials (special units are excluded)
  /// </summary>
  /// <param name="header"></param>
  /// <returns></returns>
  protected string printActivations(string header = "Activation Potentials:")
  {
    StringBuilder log = new StringBuilder();
    log.Append(header + "\n");
    foreach (var unit in Model.GetComputationalUnits())
      log.Append(unit.Name + " = " + unit.Activation + "\n");
    log.Append("\n");
    return log.ToString();
  }
  protected string printNextActivations(string header = "Next Activation Potentials:")
  {
    StringBuilder log = new StringBuilder();
    log.Append(header + "\n");
    foreach (var unit in Model.GetComputationalUnits())
    {
      log.Append(unit.Name + " = " + unit.NextActivation + "\n");
    }
    log.Append("\n");
    return log.ToString();
  }
  protected string printDescriptions(List<Unit> units, string header = "Unit Descriptions:")
  {
    StringBuilder log = new StringBuilder();
    log.Append(header + "\n");
    foreach (var unit in units)
      log.Append(unit.Name + "( " + unit.Description + ")\n");
    log.Append("\n");
    return log.ToString();
  }
  public override string ToString()
  {
    return "Coherence Computation Interpreter";
  }
  #endregion
}
