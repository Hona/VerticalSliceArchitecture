namespace VerticalSliceArchitectureTemplate.Domain;

[ValueObject(toPrimitiveCasting: CastOperator.Implicit)]
public readonly partial struct BoardSize
{
    public static readonly BoardSize DefaultBoardSize = From(3);

    private static Validation Validate(int input) =>
        input >= 2 ? Validation.Ok : Validation.Invalid("A board must be at least 2x2");
}