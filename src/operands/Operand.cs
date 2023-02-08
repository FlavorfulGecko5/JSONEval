interface Operand
{
    public Operand Add(Operand b);

    public Operand UnaryAdd();

    public Operand Sub(Operand b);

    public Operand UnarySub();

    public Operand Mult(Operand b);

    public Operand Div(Operand b);

    public Operand Rem(Operand b);
}