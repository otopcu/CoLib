namespace CoLib;

public enum UnitType
{
  UNIT, SU
}
public class Unittype
{
  public const int SU = 0;
}
public class DecoUnitType : Unittype
{
  public const int GOAL = 1;
  public const int ACTION = 2;
}
public class ExcoUnitType : Unittype
{
  public const int DATA = 3;
  public const int PROPOSITION = 4;
}

/// <summary>
/// Connectionist Network Runtime parameters
/// Defaults from Thagard's Conceptual Revolution book pg. 81
/// EXCITATION = 0.04;
/// INHIBITION = -0.06;
/// SU_EXCITATION = 0.05;
/// THETA = 0.05;
/// ANALOGY_IMPACT = 1.0;
/// </summary>
public static class Parameters
{
  public static double EXCITATION = 0.04; // 0.04 0.05
  public static double INHIBITION = -0.06; // -0.08 -0.2
  public static double SU_EXCITATION = 0.05; // 0.05 // From special unit to goal
  public static double THETA = 0.05; // Decay Rate
  public static int MAX_ITERATIONS = 200;
  public static double START_VALUE = 0.01; // Default activation value
  public static double STOP_VALUE = 0.001;  // 0.001 // Asymptote criteria
  // MINACT <= activation <= MAXACT
  public static double MAXACT = 0.99; // Max
  public static double MINACT = -0.99; // Min

  public static double ANALOGY_IMPACT = 1.0;
  public static double SIMPLICITY_IMPACT = 1.0;
  public static double VALERR = -999.0; //? 
  //public static double WEAK_INCOHERENCE_IMPACT=0.2;

  public static ActivationUpdateMethods ActivationUpdateRule = ActivationUpdateMethods.McClelland;
  public static double DESIRABILITY = 0;
}
