abstract class Operand
{
    public abstract Operand Add(Operand b);

    public abstract Operand Sub(Operand b);

    public abstract Operand Mult(Operand b);

    public abstract Operand Div(Operand b);

    public abstract Operand Rem(Operand b);
}