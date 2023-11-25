using System.Runtime.CompilerServices; // [CallerMemberName]

namespace CoLib.DeCo;

public class DeCoUnit : Unit
{
  #region Properties
  /// <summary>
  /// DeCo Unit's type overrides the base type.
  /// </summary>
  private DeCoUnitType _Type = DeCoUnitType.GOAL;
  public new DeCoUnitType Type
  {
    get
    {
      return _Type;
    }
    set
    {
      if (value != _Type)
      {
        _Type = value;
        switch (_Type)
        {
          case DeCoUnitType.SU:
            base.Type = UnitType.SU;
            break;
          default:
            base.Type = UnitType.UNIT;
            break;
        }
      }
    }
  }
  #endregion

  #region Ctor
  public DeCoUnit() : base()
  {
  }

  public DeCoUnit(DeCoUnitType type, [CallerMemberName] string name = null)
    : base(name: name)
  {
    Name = name;
    Type = type;
  }

  //  public DeCoUnit(DeCoUnitType type, string name)
  //: this()
  //  {
  //      Name = name;
  //      Type = type;
  //  }

  public DeCoUnit(DeCoUnitType type, string name, double priority)
    : this(type, name)
  {
    Priority = priority;
  }

  public DeCoUnit(DeCoUnit unit)
    : this(unit.Type, unit.Name, unit.Priority)
  {
    Activation = unit.Activation;
  }
  #endregion
}
